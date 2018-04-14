﻿using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterThwomp : Monster {

		private enum CrushState {
			Idle,	// Waiting for player to come near
			Crush,	// Accelearting down to the ground
			Hit,	// Hit the ground, screen is shaking. TODO: screen shake
			Raise,	// Moving up back to idle position
		}

		private GenericStateMachine<CrushState> stateMachine;
		private Vector2F hoverPosition;
		private float crushSpeed;
		private Angle eyeAngle;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterThwomp() {
			// Graphics
			centerOffset		= new Point2I(0, 0);
			Graphics.DrawOffset	= new Point2I(-16, -16);

			// Physics
			Physics.CollisionBox		= new Rectangle2F(-15, -16, 30, 8);
			Physics.HasGravity			= false;
			Physics.IsSolid				= true;
			Physics.CollideWithWorld	= false;
			Physics.CollideWithRoomEdge	= false;

			// Interactions
			Interactions.InteractionBox = new Rectangle2F(-16, -12, 32, 28);//.Inflated(-2, -2);
			// Weapon Reactions
			Reactions[InteractionType.Sword]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordStrafe]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwordSpin]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.BiggoronSword]
				.SetBegin(MonsterReactions.ClingEffect);
			Reactions[InteractionType.Shovel]
				.SetBegin(MonsterReactions.ParryWithClingEffect);
			// Projectile Reactions
			Reactions[InteractionType.Arrow]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.SwordBeam]
				.Set(SenderReactions.Intercept);
			Reactions[InteractionType.Boomerang]
				.Set(MonsterReactions.ParryWithClingEffect);
			Reactions[InteractionType.SwitchHook]
				.Set(MonsterReactions.ParryWithClingEffect);
			
			// Monster
			ContactDamage	= 4;
			Color			= MonsterColor.DarkBlue;
			IsDamageable	= false;
			isBurnable		= false;
			isStunnable		= false;
			isGaleable		= false;
			IsKnockbackable	= false;

			// Behavior
			stateMachine = new GenericStateMachine<CrushState>();
			stateMachine.AddState(CrushState.Idle)
				.OnBegin(OnBeginIdleState)
				.OnUpdate(OnUpdateIdleState);
			stateMachine.AddState(CrushState.Crush)
				.OnBegin(OnBeginCrushState)
				.OnUpdate(OnUpdateCrushState);
			stateMachine.AddState(CrushState.Hit)
				.OnBegin(OnBeginHitState)
				.SetDuration(GameSettings.MONSTER_THWOMP_HIT_WAIT_DURATION);
			stateMachine.AddState(CrushState.Raise)
				.OnUpdate(OnUpdateRaiseState);
		}


		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------
		
		private void OnBeginIdleState() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);
			position = hoverPosition;
			physics.Velocity = Vector2F.Zero;
		}

		private void OnUpdateIdleState() {
			// Update eye angle
			Angle angleToPlayer = Angle.FromVector(RoomControl.Player.Center - Center);
			eyeAngle = angleToPlayer;
			Graphics.SubStripIndex = eyeAngle;

			// TODO: after raising, eye will not change back until NOT looking down.
			// TODO: slight delay after raising

			// Check for crushing
			if (Entity.AreEntitiesAligned(this, RoomControl.Player,
				Direction.Down, GameSettings.MONSTER_THWOMP_CRUSH_MIN_ALIGNMENT))
			{
				stateMachine.BeginState(CrushState.Crush);
			}
		}
		
		private void OnBeginCrushState() {
			crushSpeed = GameSettings.MONSTER_THWOMP_CRUSH_INITIAL_SPEED;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP_CRUSH);
		}

		private void OnUpdateCrushState() {
			// Move down
			Physics.VelocityY = crushSpeed;

			// Accelerate crush speed
			if (crushSpeed < GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED) {
				crushSpeed = GMath.Min(crushSpeed +
					GameSettings.MONSTER_THWOMP_CRUSH_ACCELERATION,
					GameSettings.MONSTER_THWOMP_CRUSH_MAX_SPEED);
			}

			// Stop crushing upon hitting a solid tile
			// TODO: Properly snap position to be flush with surface.
			// TODO: collide with room edge too
			Rectangle2F crushBox = new Rectangle2F(-16, -16, 32, 32);
			if (Physics.IsPlaceMeetingSolid(position, crushBox)) {
				stateMachine.BeginState(CrushState.Hit);
			}
		}

		private void OnBeginHitState() {
			AudioSystem.PlaySound(GameData.SOUND_BARRIER);
			physics.Velocity = Vector2F.Zero;
		}
		
		private void OnUpdateRaiseState() {
			// Move up until we reach our wait position
			physics.VelocityY = -GameSettings.MONSTER_THWOMP_RAISE_SPEED;
			if (position.Y <= hoverPosition.Y)
				stateMachine.BeginState(CrushState.Idle);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();

			hoverPosition = position;
			eyeAngle = Angle.Down;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_THWOMP);

			stateMachine.BeginState(CrushState.Idle);
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
