﻿using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ZeldaWpf.Util;

namespace ZeldaWpf.Controls.TextEditing {
	/// <summary>Highlights the current line even when the text editor doesn't have
	/// focus.
	/// https://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused </summary>
	public class CurrentLineHighlighter : IBackgroundRenderer {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The pen used for the rectangle.</summary>
		private static Pen pen;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The reference to the owning text editor.</summary>
		private TextEditor editor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the line highlighter.</summary>
		static CurrentLineHighlighter() {
			pen = WpfHelper.ColorPen(40, 40, 42, 20, 2.0).AsFrozen();
		}

		/// <summary>Constructs the line highlighter.</summary>
		public CurrentLineHighlighter(TextEditor editor) {
			this.editor = editor;
			this.editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			this.editor.TextArea.TextView.SizeChanged += OnTextViewSizeChanged;
		}


		//-----------------------------------------------------------------------------
		// IBackgroundRenderer Overrides
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the layer on which this background renderer should draw.</summary>
		public KnownLayer Layer {
			get { return KnownLayer.Background; }
		}

		/// <summary>Causes the background renderer to draw.</summary>
		public void Draw(TextView textView, DrawingContext drawingContext) {
			if (editor.Document == null)
				return;
			
			textView.EnsureVisualLines();
			// Don't highlight the line when a selection exists
			if (editor.TextArea.TextView.ActualWidth > 0 && editor.TextArea.Selection.IsEmpty) {
				var currentLine = editor.Document.GetLineByOffset(editor.CaretOffset);
				foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine)) {
					Point point = new Point(Math.Round(rect.X + 2), Math.Round(rect.Y));
					Size size = new Size(
						Math.Round(Math.Max(0, editor.TextArea.TextView.ActualWidth - 5)),
						Math.Round(rect.Height));
					drawingContext.DrawRectangle(null, pen, new Rect(point, size));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called when the caret position is changed to highlight the current line.</summary>
		private void OnCaretPositionChanged(object sender, EventArgs e) {
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
		}

		/// <summary>Called when the text view changes size to update the current line rect width.</summary>
		private void OnTextViewSizeChanged(object sender, SizeChangedEventArgs e) {
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
		}
	}
}
