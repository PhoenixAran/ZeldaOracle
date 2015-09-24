﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerBusyState : PlayerState {
		private int timer;
		private int duration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerBusyState(int duration) {
			this.duration = duration;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			timer = duration;
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
		}

		public override void Update() {
			base.Update();

			timer--;
			if (timer <= 0) {
				player.BeginNormalState();
			}
		}
	}
}