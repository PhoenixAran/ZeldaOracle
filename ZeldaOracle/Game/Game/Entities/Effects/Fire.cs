﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Effects {
	public class Fire : Effect {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			EnablePhysics(PhysicsFlags.HasGravity);

			Graphics.DrawOffset	= new Point2I(0, -2);
			Graphics.DepthLayer	= DepthLayer.EffectFire;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnDestroyTimerDone() {
			// Burn tiles.
			Point2I location = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(location)) {
				Tile tile = RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnBurn();
			}
			Destroy();
		}

		public override void Update() {
			
			// Collide with monsters.
			CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
			for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
				Monster monster = iterator.CollisionInfo.Entity as Monster;
				monster.TriggerInteraction(InteractionType.Fire, this);
				if (IsDestroyed)
					return;
			}

			base.Update();
		}
	}
}
