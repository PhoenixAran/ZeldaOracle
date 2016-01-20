﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts {

	public class SoundsSR : NewScriptReader {

		private TemporaryResources resources;
		private bool useTemporary;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public SoundsSR(TemporaryResources resources = null) {

			this.resources		= resources;
			this.useTemporary	= resources != null;

			// Sound <name> <path> <volume=1> <pitch> <pan=0> <muted=false>
			AddCommand("Sound", delegate(CommandParam parameters) {
				string name = parameters.GetString(0);
				string path = parameters.GetString(1);
				Sound sound	= Resources.LoadSound(name, Resources.SoundDirectory + path);
				sound.name	= name;
				if (parameters.Count > 2)
					sound.Volume = parameters.GetFloat(2);
				if (parameters.Count > 3)
					sound.Pitch = parameters.GetFloat(3);
				if (parameters.Count > 4)
					sound.Pan = parameters.GetFloat(4);
				if (parameters.Count > 5)
					sound.IsMuted = parameters.GetBool(5);
			});
		}

		// Begins reading the script.
		protected override void BeginReading() {
		}

		// Ends reading the script.
		protected override void EndReading() {
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool UseTemporaryResources {
			get { return useTemporary; }
			set { useTemporary = value; }
		}
	}
} // end namespace
