﻿using System;
using ZeldaOracle.Common.Content;

namespace ZeldaWpf.WinForms {
	/// <summary>A dummy graphics device control used to initialize resources.</summary>
	internal class GraphicsDeviceInitializer : GraphicsDeviceControl {

		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the resources.</summary>
		protected override void Initialize() {
			Resources.Initialize(GraphicsDevice, Services);
			GraphicsInitialized?.Invoke(this, EventArgs.Empty);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		/// <summary>Occurs after the dummy graphics control initializes the resources.</summary>
		public event EventHandler GraphicsInitialized;
	}
}
