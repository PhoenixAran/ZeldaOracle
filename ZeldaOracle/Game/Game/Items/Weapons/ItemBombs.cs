﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Items {
	public abstract class ItemBombs : Item {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		protected ItemBombs() : base() {
			this.id = "item_bombs";
			this.name = new string[] { "Bombs" };
			this.description = new string[] { "Very explosive." };
			this.maxLevel = 0;
			this.currentAmmo = 0;
		}

		//-----------------------------------------------------------------------------
		// Virtual
		//-----------------------------------------------------------------------------

		// Called when the item is switched to.
		public override void OnStart() { }
		// Called when the item is put away.
		public override void OnEnd() { }
		// Immediately interrupt this item (ex: if link falls in a hole).
		public override void Interrupt() { }
		// Draws under link's sprite.
		public override void DrawUnder() { }
		// Draws over link's sprite.
		public override void DrawOver() { }
		// Called when the item is added to the inventory list
		public override void OnAdded(Inventory inventory) {
			base.OnAdded(inventory);

			this.ammo = new Ammo[] { new Ammo("ammo_bombs", "Bombs", 10, 10) };
			if (inventory.AmmoExists("ammo_bombs"))
				this.ammo[0] = inventory.GetAmmo("ammo_bombs");
			else
				inventory.AddAmmo(this.ammo[0]);
		}
		// Called when the item's level is changed.
		public override void OnLevelUp() { }
		// Called when the item has been obtained.
		public override void OnObtained() {
			inventory.GetItem("item_bomb_bag").Obtained = true;
		}
		// Called when the item has been unobtained.
		public override void OnUnobtained() {
			inventory.GetItem("item_bomb_bag").Obtained = false;
		}
		// Called when the item has been stolen.
		public override void OnStolen() {
			inventory.GetItem("item_bomb_bag").Stolen = true;
		}
		// Called when the stolen item has been returned.
		public override void OnReturned() {
			inventory.GetItem("item_bomb_bag").Stolen = false;
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
