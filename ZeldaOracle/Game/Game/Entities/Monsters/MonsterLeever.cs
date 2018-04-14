﻿using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterLeever : Monster {
		
		private enum BurrowState {
			Burrowed,
			Burrowing,
			Unburrowing,
			Unburrowed,
		}
		
		private float moveSpeed;
		private int timer;
		private int duration;
		private BurrowState burrowState;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterLeever() {
			// General
			healthMax		= 2;
			ContactDamage	= 2;

			// Graphics
			syncAnimationWithDirection	= false;
			Color						= MonsterColor.Red;

			// Movement
			moveSpeed = 0.5f;
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private bool CanUnburrowAtLocation(Point2I location) {
			Tile tile = RoomControl.GetTopTile(location);
			return (tile == null || !tile.IsSolid);
		}

		private bool GetRandomLocation(out Point2I location) {
			List<Point2I> possibleLocations = new List<Point2I>();

			// Get a list of all non-solid tile locations
			for (int x = 0; x < RoomControl.Room.Width; x++) {
				for (int y = 0; y < RoomControl.Room.Height; y++) {
					Point2I loc = new Point2I(x, y);
					if (CanUnburrowAtLocation(loc))
						possibleLocations.Add(loc);
				}
			}

			if (possibleLocations.Count == 0) {
				location = Point2I.Zero;
				return false;
			}
			
			// Pick a random location
			int index = GRandom.NextInt(possibleLocations.Count);
			location = possibleLocations[index];
			return true;
		}

		private void Burrow() {
			burrowState = BurrowState.Burrowing;
			duration	= 100;
			timer		= 0;
			IsPassable	= true;
			Physics.Velocity = Vector2F.Zero;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER_BURROW);
		}

		private bool Unburrow() {
			if (Color == MonsterColor.Red) {
				// Unborrow between 3 to 5 tiles in front of the player
				// First create a list of the possibly locations to unburrow
				List<Point2I> possibleLocations = new List<Point2I>();
				Point2I dirPoint = RoomControl.Player.Direction.ToPoint();
				Point2I loc = RoomControl.GetTileLocation(RoomControl.Player.Position);

				for (int i = 0; i <= 5 && RoomControl.IsTileInBounds(loc); i++) {
					Tile t = RoomControl.GetTopTile(loc);
					if (i >= 3 && CanUnburrowAtLocation(loc))
						possibleLocations.Add(loc);
					loc += dirPoint;
				}

				if (possibleLocations.Count > 0) {
					// Randomly pick one of the possible unburrow locations
					int index = GRandom.NextInt(possibleLocations.Count);
					SetPositionByCenter(possibleLocations[index] *
						GameSettings.TILE_SIZE + new Vector2F(8, 8));
				}
				else {
					// No possible unburrow locations.
					return false;
				}
			}
			else if (Color == MonsterColor.Blue) {
				// Unburrow at a random location in the room
				Point2I location;
				if (GetRandomLocation(out location)) {
					SetPositionByCenter(location *
						GameSettings.TILE_SIZE + new Vector2F(8, 8));
				}
				else {
					// No possible unburrow locations.
					return false;
				}
			}

			duration = 180;
			timer = 0;
			Direction = Direction.FromVector(RoomControl.Player.Center - Center);
			Graphics.IsVisible = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER_UNBURROW);
			burrowState = BurrowState.Unburrowing;
			return true;
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER);
						
			burrowState = BurrowState.Burrowed;
			timer = 0;
			duration = 100;
			IsPassable = true;
			Graphics.IsVisible = false;
		}

		public override void UpdateAI() {
			timer++;

			if (burrowState == BurrowState.Burrowing) {
				if (Graphics.IsAnimationDone) {
					Graphics.IsVisible = false;
					burrowState = BurrowState.Burrowed;
				}
			}
			else if (burrowState == BurrowState.Unburrowing) {
				if (Graphics.IsAnimationDone) {
					IsPassable = false;
					Graphics.PlayAnimation(GameData.ANIM_MONSTER_LEEVER);
					burrowState = BurrowState.Unburrowed;
				}
			}
			else if (burrowState == BurrowState.Burrowed) {
				if (timer > duration) {
					Unburrow();
				}
			}
			else {
				if (timer > duration || (Physics.IsColliding &&
					Color != MonsterColor.Orange))
				{
					// Burrow after a delay or upon hitting a wall
					Burrow();
				}
				else if (Color == MonsterColor.Red) {
					physics.Velocity = Direction.ToVector(moveSpeed);
				}
				else if (Color == MonsterColor.Blue) {
					// Re-face player regularly.
					if (timer % 30 == 0) {
						Direction = Direction.FromVector(
							RoomControl.Player.Center - Center);
					}
					physics.Velocity = Direction.ToVector(moveSpeed);
				}
				else if (Color == MonsterColor.Orange) {
					// Chase player
					Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
					physics.Velocity = vectorToPlayer.Normalized * moveSpeed;
				}
			}
		}
	}
}
