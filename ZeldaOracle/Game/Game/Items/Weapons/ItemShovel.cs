﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Players.States;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemShovel : ItemWeapon {
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemShovel() {
			this.id				= "item_shovel";
			this.name			= new string[] { "Shovel" };
			this.description	= new string[] { "A handy tool." };
			this.maxLevel		= Item.Level1;
			this.flags			= ItemFlags.UsableWhileInHole;
			this.sprite			= new Sprite[] { GameData.SPR_ITEM_ICON_SHOVEL };
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void PerformDig(PlayerState state) {
			// Look for tiles to dig up.
			float distance = 6.5f;
			Vector2F center = Player.Center;
			if (Directions.IsVertical(Player.Direction))
				distance = 7.5f;
			else
				center.Y += 4.0f;
			Vector2F hotspot = GMath.Round(center) + (Directions.ToVector(Player.Direction) * distance);
			Point2I tileLoc = RoomControl.GetTileLocation(hotspot);
			Tile tile = RoomControl.GetTopTile(tileLoc);

			if (tile != null && tile.OnDig(Player.Direction)) {
				// Create dirt effect.
				Effect effect = new Effect();
				effect.CreateDestroyTimer(15);
				effect.EnablePhysics(PhysicsFlags.HasGravity);
				effect.Physics.Velocity = Directions.ToVector(Player.Direction) * 0.5f;
				effect.Graphics.IsShadowVisible = false;
				effect.Graphics.PlayAnimation(GameData.ANIM_EFFECT_DIRT);
				effect.Graphics.SubStripIndex = Player.Direction;
					
				if (Directions.IsHorizontal(Player.Direction)) {
					effect.Physics.ZVelocity	= 3.0f;
					effect.Physics.Gravity		= 0.5f;
				}
				else {
					effect.Physics.ZVelocity	= 2.5f;
					effect.Physics.Gravity		= 0.4f;
				}

				RoomControl.SpawnEntity(effect, tile.Center);
			}
			else {
				// TODO: Play cling sound.
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DIG);
			Player.BusyState.AddDelayedAction(4, PerformDig);
			Player.BeginBusyState(GameData.ANIM_PLAYER_DIG.Duration);
		}

		// Update the item.
		public override void Update() { }

		// Draws under link's sprite.
		public override void DrawUnder() { }

		// Draws over link's sprite.
		public override void DrawOver() { }
	}
}