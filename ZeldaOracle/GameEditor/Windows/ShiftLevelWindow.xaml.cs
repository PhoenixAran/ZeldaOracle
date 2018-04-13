﻿using System.Windows;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ShiftLevelWindow.xaml
	/// </summary>
	public partial class ShiftLevelWindow : Window {
		private Point2I distance;

		public ShiftLevelWindow() {
			InitializeComponent();

			distance = Point2I.Zero;

			spinnerWidth.Value = distance.X;
			spinnerHeight.Value = distance.Y;
			spinnerWidth.Focus();
		}

		private void OnShift(object sender, RoutedEventArgs e) {
			distance = new Point2I(spinnerWidth.Value.Value, spinnerHeight.Value.Value);
			DialogResult = true;
			Close();
		}


		public static bool Show(Window owner, out Point2I distance) {
			ShiftLevelWindow window = new ShiftLevelWindow();
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				distance = window.distance;
				return true;
			}
			distance = Point2I.Zero;
			return false;
		}
	}
}
