﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Effects {
	public class Fire : Effect {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			EnablePhysics(PhysicsFlags.HasGravity);
			Graphics.DrawOffset = new Point2I(0, -2);
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void OnDestroy() {
			// Burn tiles.
			Point2I location = RoomControl.GetTileLocation(position);
			
			if (RoomControl.IsTileInBounds(location)) {
				Tile tile = RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnBurn();
			}
		}

		public override void Update() {
			base.Update();
			// TODO: collide with monsters.
		}
	}
}
