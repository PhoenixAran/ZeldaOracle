﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class CarriedTile : Entity {
		private Tile tile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CarriedTile(Tile tile) {
			this.tile = tile;

			EnablePhysics(PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.DestroyedOutsideRoom |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.DestroyedInHoles);
			
			//OriginOffset				= new Point2I(8, 14);
			Physics.CollisionBox		= new Rectangle2F(-3, -5, 6, 1);
			Physics.SoftCollisionBox	= new Rectangle2F(-3, -5, 6, 1);
			graphics.DrawOffset			= new Point2I(-8, -14);
			centerOffset				= new Point2I(0, -6);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void Break() {
			if (tile.BreakAnimation != null) {
				RoomControl.SpawnEntity(new Effect(tile.BreakAnimation), Center);
			}
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			if (!tile.SpriteAsObject.IsNull)
				Graphics.PlayAnimation(tile.SpriteAsObject);
			else
				Graphics.PlayAnimation(tile.CurrentSprite);
		}

		public override void OnLand() {
			Break();
			base.OnLand();
		}

		public override void Update() {
			base.Update();

			if (Physics.IsColliding)
				Break();
		}
	}
}
