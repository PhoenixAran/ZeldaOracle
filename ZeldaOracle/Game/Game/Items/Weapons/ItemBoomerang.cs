﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBoomerang : ItemWeapon {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBoomerang() {
			this.id				= "item_boomerang";
			this.name			= new string[] { "Boomerang", "Magic Boomerang" };
			this.description	= new string[] { "Always comes back to you.", "A remote-control weapon." };
			this.level			= 0;
			this.maxLevel		= Item.Level2;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWithSword | ItemFlags.UsableWhileInHole;

			sprite = new Sprite[] {
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(4, 1)),
				new Sprite(GameData.SHEET_ITEMS_SMALL, new Point2I(5, 1))
			};
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnButtonPress() {
			Player.Direction = Player.UseDirection;

			// Spawn the boomerang.
			// TODO: keep track of boomerang entity.
			Boomerang boomerang = new Boomerang(level);
			boomerang.Owner		= Player;
			boomerang.Angle		= Player.UseAngle;
			RoomControl.SpawnEntity(boomerang, Player.Center, Player.ZPosition);

			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			Player.BeginBusyState(10);
		}
	}
}
