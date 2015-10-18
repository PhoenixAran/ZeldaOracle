﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.GameStates;
using ZeldaOracle.Game.GameStates.Transitions;

namespace ZeldaOracle.Game.Tiles.EventTiles {

	public enum WarpType {
		Tunnel		= 0,
		Entrance	= 1,
		Stairs		= 2,

		Count		= 3
	}

	public class WarpEvent : EventTile {

		public WarpType warpType;
		public bool warpEnabled;
		public int edgeDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WarpEvent() {
		}
		
		
		//-----------------------------------------------------------------------------
		// Warp Accessors
		//-----------------------------------------------------------------------------

		// Find the event tile for the this warp point's destination.
		public EventTileDataInstance FindDestinationPoint() {
			string warpID = properties.GetString("destination_warp_point", "?");
			string warpLevelID = Properties.GetString("destination_level", RoomControl.Level.Properties.GetString("id"));
			if (warpID.Length == 0 || warpLevelID.Length == 0)
				return null;
			Level warpLevel = RoomControl.GameControl.World.GetLevel(warpLevelID);
			if (warpLevel == null)
				return null;
			return warpLevel.FindEventTileByID(warpID);
		}


		//-----------------------------------------------------------------------------
		// Warping
		//-----------------------------------------------------------------------------

		// This method is called when a room is entered through this warp point.
		public void SetupRoomOnEnter() {
			Player player = RoomControl.Player;
			warpEnabled = false;
			
			// Position the player.
			if (warpType == WarpType.Entrance) {
				player.Position = position + new Point2I(8, 16);
				if (edgeDirection == Directions.Down)
					player.Position += Directions.ToVector(edgeDirection) * 8.0f;
				else
					player.Position += Directions.ToVector(edgeDirection) * 16.0f;
				player.Direction = Directions.Reverse(edgeDirection);
			}
			else {
				int faceDirection = Properties.GetInteger("face_direction", Directions.Down);
				player.Position = position + new Point2I(8, 16);
				player.Direction = faceDirection;
			}

			// Setup the player's state.
			player.InterruptItems();
			player.BeginNormalState();
			player.StopPushing();
		}

		// Create the game-state when exiting a room through this warp point.
		public GameState CreateExitState() {
			if (warpType == WarpType.Entrance)
				return RoomEnterExitState.CreateExit(edgeDirection, 25);
			return null;
		}
		
		// Create the game-state when entering a room through this warp point.
		public GameState CreateEnterState() {
			int distance = 19;
			if (edgeDirection == Directions.Down)
				distance += 8;
			if (warpType == WarpType.Entrance)
				return RoomEnterExitState.CreateEnter(Directions.Reverse(edgeDirection), distance, null);
			return null;
		}

		// Create the room transition state for this warp point.
		public RoomTransition CreateTransition(EventTileDataInstance destination) {
			int dir = destination.Properties.GetInteger("face_direction", Directions.Down);
			if (warpType == WarpType.Stairs)
				return new RoomTransitionFade();
			return new RoomTransitionSplit();
		}

		// Warp to the destination point.
		public void Warp(int warpDirection) {
			EventTileDataInstance destination = FindDestinationPoint();

			if (destination != null) {
				RoomControl.Warp(this, destination);
				RoomControl.Player.BeginNormalState();
			}
			else {
				Console.WriteLine("Invalid warp destination!");
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		protected override void Initialize() {
			base.Initialize();

			string typeName = Properties.GetString("warp_type", "Tunnel");
			warpType		= (WarpType) Enum.Parse(typeof(WarpType), typeName, true);
			collisionBox	= new Rectangle2I(2, 6, 12, 12);
			warpEnabled		= !IsTouchingPlayer();
			
			// Find the closest room edge.
			edgeDirection = -1;
			Rectangle2I roomBounds = RoomControl.RoomBounds;
			Rectangle2I myBox = new Rectangle2I((int) position.X, (int) position.Y, 16, 16);
			int minDist = -1;
			for (int dir = 0; dir < 4; dir++) {
				int dist = Math.Abs(myBox.GetEdge(dir) - roomBounds.GetEdge(dir));
				if (dist < minDist || minDist < 0) {
					edgeDirection = dir;
					minDist = dist;
				}
			}

			// Make sure we know if the player respawns on top of this warp point.
			RoomControl.PlayerRespawn += delegate(Player player) {
				warpEnabled = !IsTouchingPlayer();
			};

			// For entrance warp points, intercept room transitions in order to warp.
			RoomControl.RoomTransitioning += delegate(int direction) {
				if (warpType == WarpType.Entrance && direction == edgeDirection && IsTouchingPlayer()) {
					RoomControl.CancelRoomTransition();
					Warp(direction);
				}
			};
		}

		public override void Update() {
			base.Update();

			if (warpType == WarpType.Entrance) {
				warpEnabled = true;
			}
			else if (IsTouchingPlayer()) {
				if (RoomControl.Player.CanUseWarpPoint && warpEnabled && RoomControl.Player.IsOnGround) {
					Warp(RoomControl.Player.Direction);
					warpEnabled = false;
				}
			}
			else {
				warpEnabled = true;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
