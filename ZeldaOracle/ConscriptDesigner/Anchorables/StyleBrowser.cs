﻿using System;
using System.Windows.Controls;
using System.Xml;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Anchorables {
	public class StyleBrowser : RequestCloseAnchorable {

		private StyleBrowserControl browser;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the style browser.</summary>
		public StyleBrowser() {
			Border border = CreateBorder();
			this.browser = new StyleBrowserControl();
			border.Child = this.browser;

			Closed += OnAnchorableClosed;

			Title = "Style Browser";
			Content = border;
		}


		//-----------------------------------------------------------------------------
		// Control
		//-----------------------------------------------------------------------------

		public void Reload() {
			browser.Reload();
		}

		public void Unload() {
			browser.Unload();
		}

		/// <summary>Focuses on the anchorable's content.</summary>
		public override void Focus() {
			browser.Focus();
		}


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			base.ReadXml(reader);
			DesignerControl.MainWindow.StyleBrowser = this;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to force cleanup during close.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			browser.Dispose();
		}
	}
}
