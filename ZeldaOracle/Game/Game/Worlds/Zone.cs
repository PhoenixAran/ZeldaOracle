﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Worlds {
	public class Zone : IPropertyObject, IIDObject {
		
		private int			imageVariantID;
		private TileData	defaultTileData;
		private Properties  properties;

		private StyleDefinitions styles;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public Zone() {
			this.styles = new StyleDefinitions();

			properties = new Properties(this);

			properties.Set("id", "");
			properties.Set("name", "");
			properties.Set("palette", "");
			properties.Set("side_scrolling", false);
			properties.Set("underwater", false);
		}

		public Zone(string id, string name, int imageVariantID, TileData defaultTileData) :
			this()
		{
			this.imageVariantID		= imageVariantID;
			this.defaultTileData	= defaultTileData;

			properties.Set("id", id);
			properties.Set("name", name);
		}

		public Zone(Zone copy) :
			this()
		{
			this.imageVariantID		= copy.imageVariantID;
			this.defaultTileData    = copy.defaultTileData;
			this.properties.SetAll(copy.properties);
		}

		
		//-----------------------------------------------------------------------------
		// Stuff
		//-----------------------------------------------------------------------------

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string ID {
			get { return properties.GetString("id"); }
			set { properties.Set("id", value); }
		}
		
		public string Name {
			get { return properties.GetString("name"); }
			set { properties.Set("name", value); }
		}
		
		public int ImageVariantID {
			get { return imageVariantID; }
			set { imageVariantID = value; }
		}
		
		public TileData DefaultTileData {
			get { return defaultTileData; }
			set { defaultTileData = value; }
		}
		
		public bool IsSideScrolling {
			get { return properties.GetBoolean("side_scrolling"); }
			set { properties.Set("side_scrolling", value); }
		}
		
		public bool IsUnderwater {
			get { return properties.GetBoolean("underwater"); }
			set { properties.Set("underwater", value); }
		}

		public Properties Properties {
			get { return properties; }
		}

		public StyleDefinitions StyleDefinitions {
			get { return styles; }
			set { styles = value; }
		}

		public string PaletteID {
			get { return properties.GetString("palette"); }
			set { properties.Set("palette", value); }
		}

		public Palette Palette {
			get { return properties.GetResource<Palette>("palette"); }
		}
	}
}
