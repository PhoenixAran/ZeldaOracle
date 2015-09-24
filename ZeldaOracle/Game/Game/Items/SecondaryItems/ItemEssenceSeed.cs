﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Items.KeyItems {
	public class ItemEssenceSeed : ItemSecondary {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemEssenceSeed() {
			this.id = "item_essence_seed";
			this.name = new string[] { "Maku Seed" };
			this.description = new string[] { "Evil-cleansing sacred seed." };
			this.slot = new Point2I(3, 0);
			this.sprite = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE, new Point2I(0, 9)) };
			this.spriteLight = new Sprite[] { new Sprite(GameData.SHEET_ITEMS_LARGE_LIGHT, new Point2I(0, 9)) };
		}

	}
}