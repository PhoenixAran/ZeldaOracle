﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Monsters {
	public class MonsterFlyingTile : Monster {

		private int launchTimer;
		private Vector2F launchVector;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterFlyingTile() {
			// General
			MaxHealth       = 1;
			ContactDamage   = 2;
			Color           = MonsterColor.DarkRed;

			// Graphics
			Graphics.DepthLayer         = DepthLayer.Monsters;
			Graphics.DepthLayerInAir    = DepthLayer.Monsters;

			// Physics
			Physics.CollisionBox			= new Rectangle2F(0, -6, 1, 1);
			Physics.AutoDodges				= false;
			Physics.HasGravity				= false;
			Physics.IsDestroyedInHoles		= false;
			physics.DisableSurfaceContact	= true;

			// Interactions
			Interactions.InteractionZRange = new RangeF(-100, 100);
			Interactions.InteractionBox = new Rectangle2F(-4, -10, 8, 8);
			
			// Monster & unit settings
			isKnockbackable			= false;
			isStunnable				= false;
			isGaleable				= false;
			isBurnable				= false;

			// Weapon interations
			Interactions.SetReaction(InteractionType.Sword,			SenderReactions.Intercept, MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordSpin,		MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.BiggoronSword,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.Shield,			SenderReactions.Bump,		MonsterReactions.Kill);
			// Seed interations
			Interactions.SetReaction(InteractionType.EmberSeed,		SenderReactions.Destroy,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.ScentSeed,		SenderReactions.Destroy,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.PegasusSeed,	SenderReactions.Destroy,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.MysterySeed,	SenderReactions.Destroy,	MonsterReactions.Kill);
			// Projectile interations
			Interactions.SetReaction(InteractionType.Arrow,			SenderReactions.Destroy,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.RodFire,		SenderReactions.Destroy,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept,	MonsterReactions.Kill);
			// Environment interations
			Interactions.SetReaction(InteractionType.Fire,			MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.Gale,			MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.BombExplosion,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.ThrownObject,	MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.MineCart,		MonsterReactions.Kill);
			Interactions.SetReaction(InteractionType.Block,			MonsterReactions.Kill);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			launchTimer = 0;

			// Begin rising
			Physics.ZVelocity = GameSettings.MONSTER_FLYING_TILE_RISE_SPEED;

			// Target the player
			Vector2F vectorToPlayer = RoomControl.Player.Position - Center;
			int launchAngleCount = GameSettings.MONSTER_FLYING_TILE_LAUNCH_ANGLE_COUNT;
			int launchAngle = Orientations.NearestFromVector(vectorToPlayer, launchAngleCount);
			launchVector = Orientations.ToVector(launchAngle, launchAngleCount);
			launchVector *= GameSettings.MONSTER_FLYING_TILE_LAUNCH_SPEED;
			
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_FLYING_TILE);
		}

		public override void UpdateAI() {
			if (ZPosition < GameSettings.MONSTER_FLYING_TILE_HOVER_ALTITUDE) {
				Physics.ZVelocity = GameSettings.MONSTER_FLYING_TILE_RISE_SPEED;
			}
			else {
				Physics.ZVelocity = 0f;
			}
			if (launchTimer >= GameSettings.MONSTER_FLYING_TILE_HOVER_DURATION) {
				Physics.Velocity = launchVector;

				// Kill when colliding with solid objects
				if (Physics.IsColliding) {
					Kill();
					return;
				}
			}
			launchTimer++;
		}

		public override void OnTouchPlayer(Entity sender, EventArgs args) {
			base.OnTouchPlayer(sender, args);
			Kill();
		}

		public override void OnHurt(DamageInfo damage) {
			Kill();
		}

		public override void CreateDeathEffect() {
			Effect effect = new Effect(
				GameData.ANIM_EFFECT_ROCK_BREAK,
				DepthLayer.EffectTileBreak);
			AudioSystem.PlaySound(GameData.SOUND_ROCK_SHATTER);
			RoomControl.SpawnEntity(effect, Center);
		}
	}
}
