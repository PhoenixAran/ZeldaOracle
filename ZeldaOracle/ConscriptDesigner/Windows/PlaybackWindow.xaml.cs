﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NAudio.Wave;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ZeldaWpf.Controls;
using ZeldaWpf.Util;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for PlaybackWindow.xaml
	/// </summary>
	public partial class PlaybackWindow : TimersWindow {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>
		/// Stream for looping playback
		/// http://mark-dot-net.blogspot.com/2009/10/looped-playback-in-net-with-naudio.html
		/// </summary>
		public class LoopStream : WaveStream {
			WaveStream sourceStream;

			/// <summary>Creates a new Loop stream</summary>
			/// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
			/// or else we will not loop to the start again.</param>
			public LoopStream(WaveStream sourceStream) {
				this.sourceStream = sourceStream;
				this.EnableLooping = true;
			}

			/// <summary>Use this to turn looping on or off</summary>
			public bool EnableLooping { get; set; }

			/// <summary>Return source stream's wave format</summary>
			public override WaveFormat WaveFormat {
				get { return sourceStream.WaveFormat; }
			}

			/// <summary>LoopStream simply returns</summary>
			public override long Length {
				get { return sourceStream.Length; }
			}

			/// <summary>LoopStream simply passes on positioning to source stream</summary>
			public override long Position {
				get { return sourceStream.Position; }
				set { sourceStream.Position = value; }
			}

			public override int Read(byte[] buffer, int offset, int count) {
				int totalBytesRead = 0;

				while (totalBytesRead < count) {
					int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
					if (bytesRead == 0) {
						if (sourceStream.Position == 0 || !EnableLooping) {
							// something wrong with the source stream
							break;
						}
						// loop
						sourceStream.Position = 0;
					}
					totalBytesRead += bytesRead;
				}
				return totalBytesRead;
			}
		}


		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		//private static double lastVolume = 1.0;
		//private static bool lastLooping = false;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private ContentSound sound;
		private WaveOut waveOut;
		private AudioFileReader audioReader;
		private LoopStream looper;
		private bool looping;
		private bool paused;

		private bool suppressEvents;
		
		private ScheduledEvent startTimer;
		private bool soundLoaded;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the playback window.</summary>
		public PlaybackWindow(ContentSound sound) {
			suppressEvents = true;
			InitializeComponent();
			waveOut = new WaveOut();
			waveOut.PlaybackStopped += OnPlaybackStopped;

			looping = ProjectUserSettings.Playback.Looping;
			toggleButtonLooping.IsChecked = ProjectUserSettings.Playback.Looping;
			soundLoaded = false;
			spinnerVolume.Value = ProjectUserSettings.Playback.Volume;
			waveOut.Volume = (float) ProjectUserSettings.Playback.Volume;
			ContinuousEvents.Start(0.01, TimerPriority.Low, Update);
			startTimer = ScheduledEvents.New(0.05, TimerPriority.Low,
				() => { OnPlay(); });
			suppressEvents = false;

			PlaySound(sound);
		}


		//-----------------------------------------------------------------------------
		// Playback
		//-----------------------------------------------------------------------------

		/// <summary>Plays the new sound.</summary>
		public void PlaySound(ContentSound sound) {
			this.sound = sound;
			Title = sound.Name;
			OnStop();
			soundLoaded = false;
			try {
				LoopStream oldLooper = looper;
				AudioFileReader oldAudioReader = audioReader;
				audioReader = new AudioFileReader(sound.FilePath);
				looper = new LoopStream(audioReader);
				looper.EnableLooping = looping;
				waveOut.Init(looper);
				soundLoaded = true;
				if (oldLooper != null) {
					oldLooper.EnableLooping = false;
					oldLooper.Dispose();
					oldAudioReader.Dispose();
				}
				// HACK: Fix playing directly after stopping causing
				// the previous sound to continue playing.
				startTimer.Restart();
			}
			catch (Exception ex) {
				soundLoaded = false;
				audioReader = null;
				looper = null;
				DesignerControl.ShowExceptionMessage(ex, "play", sound.Name);
			}
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows the playback window and plays the new sound.</summary>
		public static PlaybackWindow Show(Window owner, ContentSound sound, EventHandler onClose) {
			PlaybackWindow window = new PlaybackWindow(sound);
			window.Owner = owner;
			window.Show();
			window.Closed += onClose;
			return window;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Update the playback time and slider position.</summary>
		private void Update() {
			suppressEvents = true;

			if (soundLoaded) {
				sliderPosition.Value =
					(double) looper.Position / (double) looper.Length;
				labelPosition.Content = looper.CurrentTime.ToString(@"m\:ss\.ff") + "/" +
					looper.TotalTime.ToString(@"m\:ss\.ff");
			}
			else {
				sliderPosition.Value = 0.0;
				labelPosition.Content = "-:--.--/-:--.--";
			}

			suppressEvents = false;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnClosing(object sender, CancelEventArgs e) {
			waveOut.Stop();
			waveOut.Dispose();
			if (looper != null) {
				looper.Dispose();
				audioReader.Dispose();
			}
		}

		private void OnPlaybackStopped(object sender, StoppedEventArgs e) {
			Dispatcher.Invoke(() => {
				OnStop();
			});
		}
		
		private void OnStop(object sender = null, RoutedEventArgs e = null) {
			toggleButtonStop.IsChecked = true;
			toggleButtonPlay.IsChecked = false;
			toggleButtonPause.IsChecked = false;
			paused = false;
			if (soundLoaded) {
				waveOut.Stop();
				looper.Position = 0;
			}
			Update();
		}

		private void OnPlay(object sender = null, RoutedEventArgs e = null) {
			if (!soundLoaded) {
				OnStop();
				return;
			}
			toggleButtonStop.IsChecked = false;
			toggleButtonPlay.IsChecked = true;
			toggleButtonPause.IsChecked = false;
			paused = false;
			waveOut.Play();
		}

		private void OnPause(object sender = null, RoutedEventArgs e = null) {
			if (!soundLoaded) {
				OnStop();
				return;
			}
			if (paused) {
				toggleButtonStop.IsChecked = false;
				toggleButtonPlay.IsChecked = true;
				toggleButtonPause.IsChecked = false;
				paused = false;
				waveOut.Resume();
				OnPlay();
			}
			else {
				toggleButtonStop.IsChecked = false;
				toggleButtonPlay.IsChecked = false;
				toggleButtonPause.IsChecked = true;
				paused = true;
				waveOut.Pause();
			}
		}

		private void OnLoopingChanged(object sender, RoutedEventArgs e) {
			looping = toggleButtonLooping.IsChecked == true;
			ProjectUserSettings.Playback.Looping = looping;
			if (looper != null)
				looper.EnableLooping = looping;
		}

		private void OnPositionChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (suppressEvents) return;
			looper.Position =
				(long) (looper.Length * sliderPosition.Value);
			paused = (e.NewValue != 0);
			toggleButtonStop.IsChecked = (e.NewValue == 0 && waveOut.PlaybackState != PlaybackState.Playing);
			toggleButtonPlay.IsChecked = waveOut.PlaybackState == PlaybackState.Playing;
			toggleButtonPause.IsChecked = (e.NewValue == 1 && waveOut.PlaybackState != PlaybackState.Playing);
		}

		private void OnVolumeChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			if (suppressEvents) return;
			if (spinnerVolume.Value.HasValue) {
				ProjectUserSettings.Playback.Volume = spinnerVolume.Value.Value;
				waveOut.Volume = (float) spinnerVolume.Value.Value;
			}
		}


		//-----------------------------------------------------------------------------
		// Commands Execution
		//-----------------------------------------------------------------------------

		private void OnEscapeCloseCommand(object sender, ExecutedRoutedEventArgs e) {
			Close();
		}


		//-----------------------------------------------------------------------------
		// Commands Can Execute
		//-----------------------------------------------------------------------------

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
	}
}
