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
using ZeldaOracle.Game.Items.Ammos;
using ZeldaOracle.Game.Entities.Projectiles.Seeds;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Items.Weapons {

	public class ItemSlingshot : SeedBasedItem {

		private EntityTracker<SeedProjectile> seedTracker;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemSlingshot() {
			this.id				= "item_slingshot";
			this.name			= new string[] { "Slingshot", "Hyper Slingshot" };
			this.description	= new string[] { "Used to shoot seeds.", "Shoots in 3 directions." };
			this.maxLevel		= Item.Level2;
			this.currentAmmo	= 0;
			this.flags			= ItemFlags.UsableInMinecart | ItemFlags.UsableWhileJumping | ItemFlags.UsableWhileInHole;
			this.sprite			= new ISprite[] { GameData.SPR_ITEM_ICON_SLINGSHOT_1, GameData.SPR_ITEM_ICON_SLINGSHOT_2 };
			this.spriteEquipped	= new ISprite[] { GameData.SPR_ITEM_ICON_SLINGSHOT_1, GameData.SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED };
			this.seedTracker	= new EntityTracker<SeedProjectile>(3);
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			if (!seedTracker.IsEmpty || !HasAmmo())
				return;

			UseAmmo();

			SeedType seedType = CurrentSeedType;

			int direction = Player.UseDirection;
			Player.Direction = direction;

			// Determine the seed spawn position based on player facing direction.
			Vector2F seedPos;
			if (direction == Directions.Up)
				seedPos = Directions.ToVector(direction) * 1;
			else if (direction == Directions.Down)
				seedPos = Directions.ToVector(direction) * 8;
			else
				seedPos = new Vector2F(0, 6) + (Directions.ToVector(direction) * 4);

			// Spawn the main seed projectile.
			SeedProjectile seed = new SeedProjectile(seedType, false);
			Player.ShootProjectile(seed,
				Directions.ToVector(direction) * GameSettings.SLINGSHOT_SEED_SPEED,
				seedPos, 5);
			seedTracker.TrackEntity(seed);

			// Spawn the extra 2 seeds for the Hyper Slingshot.
			if (level == Item.Level2) {
				for (int i = 0; i < 2; i++) {
					int sideDirection = direction + (i == 0 ? 1 : 3);
					
					// Calculate the velocity based on a degree offset.
					float degrees = direction * GMath.QuarterAngle;
					if (i == 0)
						degrees += GameSettings.SLINGSHOT_SEED_DEGREE_OFFSET;
					else
						degrees -= GameSettings.SLINGSHOT_SEED_DEGREE_OFFSET;
					Vector2F velocity = Vector2F.CreatePolar(GameSettings.SLINGSHOT_SEED_SPEED, degrees);
					velocity.Y = -velocity.Y;
					
					// Spawn the seed.
					seed = new SeedProjectile(seedType, false);
					Player.ShootProjectile(seed, velocity, seedPos, 5);
					seedTracker.TrackEntity(seed);
				}
			}

			// Set the tool animation.
			Player.EquipTool(Player.ToolVisual);
			if (level == Item.Level1)
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_1);
			else
				Player.ToolVisual.PlayAnimation(GameData.ANIM_SLINGSHOT_2);
			Player.ToolVisual.AnimationPlayer.SubStripIndex = direction;

			// Begin the player busy state.
			Player.BusyState.SetEndAction(delegate(PlayerState playerState) {
				playerState.Player.UnequipTool(playerState.Player.ToolVisual);
			});
			Player.BeginBusyState(10, GameData.ANIM_PLAYER_THROW, GameData.ANIM_PLAYER_MINECART_THROW);
		}

		// Draws the item inside the inventory.
		public override void DrawSlot(Graphics2D g, Point2I position, int lightOrDark) {
			DrawSprite(g, position, lightOrDark);
			DrawAmmo(g, position, lightOrDark);
			g.DrawSprite(ammo[currentAmmo].Sprite, lightOrDark, position + new Point2I(8, 0));
		}
	}
}
