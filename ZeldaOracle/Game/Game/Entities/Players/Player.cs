﻿using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Entities.Players.States.SwingStates;
using ZeldaOracle.Game.Entities.Players.Tools;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities.Players {
	
	public class Player : Unit, ZeldaAPI.Player {

		/// <summary>The current direction that the player wants to face to use items.</summary>
		private Direction useDirection;
		/// <summary>The current angle that the player wants face to use items.</summary>
		private Angle useAngle;
		/// <summary>The position the player was at when he entered the room.</summary>
		private Vector2F respawnPosition;
		private int respawnDirection;
		/// <summary>The movement component for the player.</summary>
		private PlayerMoveComponent movement;

		private PlayerStateMachine environmentStateMachine;
		private PlayerStateMachine controlStateMachine;
		private PlayerStateMachine weaponStateMachine;
		private List<PlayerStateMachine> conditionStateMachines;
		private PlayerStateParameters stateParameters;

		private bool isFrozen;

		private Animation moveAnimation;

		private Vector2F viewFocusOffset;

		public delegate void PlayerDelegate(Player player);

		private event PlayerDelegate eventJump;
		private event PlayerDelegate eventLand;

		private PlayerSwimmingSkills	swimmingSkills;
		private PlayerTunics			tunic;

		// Action buttons -------------------------------------------------------------

		public delegate bool ActionButtonCallback(ActionButtons button);
		private List<ActionButtonCallback>[] buttonCallbacks;
		private bool[] pressedActionButtons;

		// Player Tools ---------------------------------------------------------------

		private PlayerToolShield	toolShield;
		private PlayerToolSword		toolSword;
		private PlayerToolVisual	toolVisual;

		// Weapon States --------------------------------------------------------------

		private PlayerPushState				statePush;
		private PlayerSwingSwordState		stateSwingSword;
		private PlayerSwingBigSwordState	stateSwingBigSword;
		private PlayerSwingCaneState		stateSwingCane;
		private PlayerSwingMagicRodState	stateSwingMagicRod;
		private PlayerHoldSwordState		stateHoldSword;
		private PlayerSwordStabState		stateSwordStab;
		private PlayerSpinSwordState		stateSpinSword;
		private PlayerSeedShooterState		stateSeedShooter;
		private PlayerSwitchHookState		stateSwitchHook;
		private PlayerSwitchHookSwitchState	stateSwitchHookSwitch;
		private PlayerMagicBoomerangState	stateMagicBoomerang;
		private PlayerGrabState				stateGrab;
		private PlayerCarryState			stateCarry;
		private PlayerPullHandleState		statePullHandle;
		private PlayerMagnetGlovesState		stateMagnetGloves;

		// Control States -------------------------------------------------------------

		private PlayerLedgeJumpState		stateLedgeJump;
		private PlayerLeapLedgeJumpState	stateLeapLedgeJump;
		private PlayerRespawnDeathState		stateRespawnDeath;
		private PlayerMinecartState			stateMinecart;
		private PlayerJumpToState			stateJumpTo;

		// Environment States ---------------------------------------------------------

		private PlayerGrassEnvironmentState					environmentStateGrass;
		private PlayerStairsEnvironmentState				environmentStateStairs;
		private PlayerJumpEnvironmentState					environmentStateJump;
		private PlayerIceEnvironmentState					environmentStateIce; 
		private PlayerLadderEnvironmentState				environmentStateLadder;
		private PlayerSideScrollLadderEnvironmentState		environmentStateSideScrollLadder;
		private PlayerSwimEnvironmentState					environmentStateSwim;
		private PlayerUnderwaterEnvironmentState			environmentStateUnderwater;
		private PlayerSideScrollSwimEnvironmentState		environmentStateSideScrollSwim;

		// Condition States -----------------------------------------------------------

		private PlayerShieldState stateShield;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int	InvincibleDuration					= 25;
		private const int	InvincibleControlRestoreDuration	= 8;
		private const int	KnockbackSnapCount					= 16;
		private const float	KnockbackSpeed						= 1.3f;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Player() {
			IsPersistentBetweenRooms = true;

			pressedActionButtons = new bool[(int) ActionButtons.Count];
			buttonCallbacks = new List<ActionButtonCallback>[
				(int) ActionButtons.Count];
			for (int i = 0; i < (int) ActionButtons.Count; i++)
				buttonCallbacks[(int) i] = new List<ActionButtonCallback>();

			// Register A-button callbacks
			RegisterActionButtonCallback(ActionButtons.A, ActionThrowCarriedObject);
			RegisterActionButtonCallback(ActionButtons.A, ActionObjectInteractions);
			RegisterActionButtonCallback(ActionButtons.A, ActionUseWeapon);
			RegisterActionButtonCallback(ActionButtons.A, ActionStroke);
			// Register B-button callbacks
			RegisterActionButtonCallback(ActionButtons.B, ActionThrowCarriedObject);
			RegisterActionButtonCallback(ActionButtons.B, ActionUnderwaterResurface);
			RegisterActionButtonCallback(ActionButtons.B, ActionSwimSubmerge);
			RegisterActionButtonCallback(ActionButtons.B, ActionUseWeapon);
			RegisterActionButtonCallback(ActionButtons.B, ActionStroke);

			// Unit properties
			centerOffset			= new Point2I(0, -5);
			MaxHealth               = 4 * 3;
			Health					= 4 * 3;
			swimmingSkills			= PlayerSwimmingSkills.CannotSwim;
			tunic					= PlayerTunics.GreenTunic;
			moveAnimation			= GameData.ANIM_PLAYER_DEFAULT;
			knockbackSpeed			= GameSettings.PLAYER_KNOCKBACK_SPEED;
			hurtKnockbackDuration	= GameSettings.PLAYER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration	= GameSettings.PLAYER_BUMP_KNOCKBACK_DURATION;

			// Physics
			Physics.CollisionBox			= new Rectangle2F(-4, -10 + 3, 8, 9);
			Physics.CollideWithWorld		= true;
			Physics.CollideWithRoomEdge		= true;
			Physics.CheckRadialCollisions	= true;
			Physics.RoomEdgeCollisionBox	= new Rectangle2F(-6, -14 + 3, 12, 13);
			Physics.HasGravity				= true;
			Physics.AutoDodges				= true;
			Physics.MovesWithConveyors		= true;
			Physics.MovesWithPlatforms		= true;
			Physics.AllowEdgeClipping		= true;
			Physics.IsCrushable				= true;
			Physics.ResolveClippingCollisions= true;
			Physics.EdgeClipAmount			= 1;
			Physics.CrushMaxGapSize			= 4;
			Physics.CustomTileIsNotSolidCondition = delegate(Tile tile) {
				if (movement.IsOnColorBarrier && (tile is TileColorBarrier)) {
					return false;
				}
				return true;
			};

			// Graphics
			Graphics.DepthLayer			= DepthLayer.PlayerAndNPCs;
			Graphics.DepthLayerInAir	= DepthLayer.InAirPlayer;
			Graphics.DrawOffset			= new Point2I(-8, -13);

			// Tools
			toolShield	= new PlayerToolShield();
			toolSword	= new PlayerToolSword();
			toolVisual	= new PlayerToolVisual();
			
			// Movement
			movement = new PlayerMoveComponent(this);

			// State machines
			stateParameters			= new PlayerStateParameters();
			environmentStateMachine	= new PlayerStateMachine(this);
			controlStateMachine		= new PlayerStateMachine(this);
			weaponStateMachine		= new PlayerStateMachine(this);
			conditionStateMachines	= new List<PlayerStateMachine>();
			
			// Construct the basic player states
			stateMagicBoomerang	= new PlayerMagicBoomerangState();

			// Weapon states
			statePush			= new PlayerPushState();
			stateGrab			= new PlayerGrabState();
			stateSwingSword		= new PlayerSwingSwordState();
			stateSwingBigSword	= new PlayerSwingBigSwordState();
			stateSwingMagicRod	= new PlayerSwingMagicRodState();
			stateSwingCane		= new PlayerSwingCaneState();
			stateHoldSword		= new PlayerHoldSwordState();
			stateSwordStab		= new PlayerSwordStabState();
			stateSpinSword		= new PlayerSpinSwordState();
			stateSeedShooter	= new PlayerSeedShooterState();
			stateSwitchHook		= new PlayerSwitchHookState();
			stateCarry			= new PlayerCarryState();
			stateMagnetGloves	= new PlayerMagnetGlovesState();

			statePush.Player = this; // Necessary to call statePush.GetPushTile()
			stateGrab.Player = this; // Necessary to call stateGrab.GetGrabTile()

			// Control States
			stateLedgeJump			= new PlayerLedgeJumpState();
			stateLeapLedgeJump		= new PlayerLeapLedgeJumpState();
			statePullHandle			= new PlayerPullHandleState();
			stateRespawnDeath		= new PlayerRespawnDeathState();
			stateMinecart			= new PlayerMinecartState();
			stateJumpTo				= new PlayerJumpToState();
			stateSwitchHookSwitch	= new PlayerSwitchHookSwitchState();

			// Environment States
			environmentStateGrass				= new PlayerGrassEnvironmentState();
			environmentStateStairs				= new PlayerStairsEnvironmentState();
			environmentStateJump				= new PlayerJumpEnvironmentState();
			environmentStateIce					= new PlayerIceEnvironmentState();
			environmentStateSwim				= new PlayerSwimEnvironmentState();
			environmentStateUnderwater			= new PlayerUnderwaterEnvironmentState();
			environmentStateSideScrollSwim		= new PlayerSideScrollSwimEnvironmentState();
			environmentStateLadder				= new PlayerLadderEnvironmentState();
			environmentStateSideScrollLadder	= new PlayerSideScrollLadderEnvironmentState();
			
			// Condition States
			stateShield = new PlayerShieldState();

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-6, -14 + 3, 12, 13);
			Interactions.InteractionZRange = new RangeF(-1, 4);
			Interactions.InteractionType = InteractionType.PlayerContact;
			Interactions.SetReaction(InteractionType.BombExplosion,
				delegate(Entity sender, EventArgs args)
			{
				RoomControl.Player.Hurt(new DamageInfo(
					GameSettings.BOMB_EXPLOSION_PLAYER_DAMAGE, sender.Center));
			});
		}


		//-----------------------------------------------------------------------------
		// Player states
		//-----------------------------------------------------------------------------

		/// <summary>Begin a new condition state. There can be any number of condition
		/// states occurring at once.</summary>
		public void BeginConditionState(PlayerState state) {
			// Prune any completed condition states
			for (int i = 0; i < conditionStateMachines.Count; i++) {
				if (!conditionStateMachines[i].IsActive)
					conditionStateMachines.RemoveAt(i--);
			}

			// Begin the new condition state
			PlayerStateMachine stateMachine = new PlayerStateMachine(this);
			conditionStateMachines.Add(stateMachine);
			stateMachine.BeginState(state);
		}

		/// <summary>Returns true if the player currently has a condition state of the
		/// given type.</summary>
		public bool HasCondition<T>() where T : PlayerState {
			foreach (PlayerState state in ConditionStates) {
				if (state is T)
					return true;
			}
			return false;
		}

		public bool EndCondition<T>() where T : PlayerState {
			foreach (PlayerState state in ConditionStates) {
				if (state is T)
					state.End();
			}
			return false;
		}

		/// <summary>Begin a new weapon state, replacing the previous weapon state.
		/// </summary>
		public void BeginWeaponState(PlayerState weaponState) {
			weaponStateMachine.BeginState(weaponState);
		}
		
		/// <summary>Begin a new control state, replacing the previous control state.
		/// </summary>
		public void BeginControlState(PlayerState state) {
			controlStateMachine.BeginState(state);
		}
		public void EndControlState() {
			if (controlStateMachine.IsActive)
				controlStateMachine.CurrentState.End();
		}
		
		
		/// <summary>Begin a new environment state, replacing the previous environment
		/// state.</summary>
		public void BeginEnvironmentState(PlayerState state) {
			environmentStateMachine.BeginState(state);
		}

		/// <summary>Try to switch to a natural state.</summary>
		public void RequestNaturalState() {
			PlayerState desiredNaturalState = GetDesiredNaturalState();
			environmentStateMachine.BeginState(desiredNaturalState);
			IntegrateStateParameters();
		}

		/// <summary>Try to switch to a natural state based on the current
		/// player position. This includes side-scrolling ladder checks.</summary>
		public void RequestSpawnNaturalState(bool silent = false) {
			RequestSpawnNaturalState(Position, silent);
		}

		/// <summary>Try to switch to a natural state based on the desired
		/// destination position. This includes side-scrolling ladder checks.</summary>
		public void RequestSpawnNaturalState(Vector2F destination, bool silent = false) {
			Vector2F oldPosition = position;
			position = destination;
			Physics.TopTile = RoomControl.TileManager
				.GetSurfaceTileAtPosition(position, Physics.MovesWithPlatforms);
			if (!RoomControl.RoomPhysics.CheckPlayerLadderClimbing(this, true)) {
				PlayerState desiredNaturalState = GetDesiredNaturalState(silent);
				environmentStateMachine.BeginState(desiredNaturalState);
				IntegrateStateParameters();
			}
			movement.UpdateMoveMode();
			position = oldPosition;
		}

		/// <summary>Forces the player to attach to the current ladder.</summary>
		public void ForceSideScrollingLadder() {
			RoomControl.RoomPhysics.CheckPlayerLadderClimbing(this, true);
		}

		/// <summary>Return the player environment state that the player wants to be
		/// in based on his current surface and jumping state.</summary>
		public PlayerState GetDesiredNaturalState(bool silent = false) {
			
			// Find the surface tile underneath the entity
			Tile feetTile = RoomControl.TileManager.GetSurfaceTileAtPosition(
				position, true);

			if (RoomControl.IsSideScrolling) {
				if (physics.IsInWater || RoomControl.IsUnderwater)
					return environmentStateSideScrollSwim;
				else if (physics.IsInLava)
					return environmentStateSwim; // TODO: this is here to trigger player death
				else if (physics.IsInAir)
					return environmentStateJump;
				else if (physics.IsInGrass)
					return environmentStateGrass;
				else if (physics.IsOnStairs)
					return environmentStateStairs;
				else if (physics.IsOnSideScrollingIce)
					return environmentStateIce;
				else 
					return null;
			}
			else {
				if (RoomControl.IsUnderwater) // TODO: underwater environment state.
					return environmentStateUnderwater;
				if (physics.IsInAir)
					return environmentStateJump;
				else if (physics.IsInWater || physics.IsInLava) {
					environmentStateSwim.SilentBeginning = silent;
					return environmentStateSwim;
				}
				else if (physics.IsInGrass)
					return environmentStateGrass;
				else if (physics.IsOnStairs)
					return environmentStateStairs;
				else if (physics.IsOnLadder)
					return environmentStateLadder;
				else if (physics.IsOnIce)
					return environmentStateIce;
				else
					return null;
			}
		}

		/// <summary>Begin a new busy condition state with the specified duration.
		/// </summary>
		public PlayerBusyState BeginBusyState(int duration) {
			return BeginBusyState(duration, graphics.Animation);
		}
		
		/// <summary>Begin a new busy condition state with the specified duration and
		/// animation(s).</summary>
		public PlayerBusyState BeginBusyState(int duration, Animation animation) {
			if (WeaponState == null)
				Graphics.PlayAnimation(animation);
			PlayerBusyState state = new PlayerBusyState(duration, animation);
			BeginConditionState(state);
			return state;
		}

		/// <summary>Jump to the given position using the special jump state.</summary>
		public void JumpToPosition(Vector2F destinationPosition,
			float destinationZPosition, int duration,
			Action<PlayerJumpToState> endAction)
		{
			stateJumpTo.JumpDuration			= duration;
			stateJumpTo.DestinationPosition		= destinationPosition;
			stateJumpTo.DestinationZPosition	= destinationZPosition;
			stateJumpTo.EndAction				= endAction;
			BeginControlState(stateJumpTo);
		}

		/// <summary>Hop into a minecart.</summary>
		public void JumpIntoMinecart(Minecart minecart) {
			JumpToPosition(minecart.Center, 4.0f, 26,
				delegate(PlayerJumpToState state)
			{
				ZPosition = 0.0f;
				stateMinecart.Minecart = minecart;
				BeginControlState(stateMinecart);
			});
		}

		/// <summary>Hop out of a minecart and land at the given tile location.
		/// </summary>
		public void JumpOutOfMinecart(Point2I landingTileLocation) {
			Vector2F landingPoint = (landingTileLocation +
				Vector2F.Half) * GameSettings.TILE_SIZE;
			landingPoint -= centerOffset + new Vector2F(0, 2.0f);
			JumpToPosition(landingPoint, 0.0f, 26, null);
		}

		public void OnJump() {
			eventJump?.Invoke(this);
		}

		/// <summary>Pickup the given entity.</summary>
		public void PickupEntity(Entity entity) {
			stateCarry.SetCarriedObject(entity);
			BeginWeaponState(stateCarry);
		}

		/// <summary>Pickup the given tile, removing it from the tile grid.</summary>
		public void PickupTile(Tile tile) {
			if (tile.IsAlive) {
				tile.SpawnDrop();
				tile.Destroy();
			}
			stateCarry.SetCarriedObject(tile);
			BeginWeaponState(stateCarry);
		}


		//-----------------------------------------------------------------------------
		// Interactions
		//-----------------------------------------------------------------------------
		
		/// <summary>Land on the given surface, breaking any obstructions. This is used
		/// for when the player begins colliding with the world again in a new
		/// position.</summary>
		public void LandOnSurface() {
			// Break tiles in the way, not if on a color barrier.
			foreach (Tile tile in Physics.GetTilesMeeting(position)) {
				if (tile.IsSolid) {
					if (tile.IsBreakable)
						tile.Break(false);
					else if (tile is TileColorBarrier)
						movement.IsOnColorBarrier = true;
				}
			}
		}

		/// <summary>For when the player needs to stop pushing, such as when reading
		/// text or opening a chest.</summary>
		public void StopPushing() {
			if (WeaponState == statePush)
				statePush.End();
			IntegrateStateParameters();
			movement.IsMoving = false;
			movement.StopMotion();
			movement.ChooseAnimation();
			if (IsOnGround && !stateParameters.ProhibitMovementControlOnGround)
				Graphics.SetAnimation(Animations.Default);
		}

		public void Freeze() {
			isFrozen = true;
			Movement.StopMotion();
			Graphics.PauseAnimation();
		}

		public void Unfreeze() {
			isFrozen = false;
			Graphics.ResumeAnimation();
		}

		public void RespawnDeath() {
			stateRespawnDeath.WaitForAnimation = true;
			BeginControlState(stateRespawnDeath);
		}

		public void RespawnDeathInstantaneous() {
			stateRespawnDeath.WaitForAnimation = false;
			BeginControlState(stateRespawnDeath);
		}

		///<summary>Mark the player's current position/direction as where he should
		///respawn.</summary>
		public void MarkRespawn() {
			respawnPosition		= position;
			respawnDirection	= direction;
		}

		/// <summary>Move the player to his marked spawn position in the room.</summary>
		public void Respawn() {
			position	= respawnPosition;
			direction	= respawnDirection;

			// Break any breakable blocks the player respawns and collides with
			LandOnSurface();

			// If colliding with a door, then move forward one tile
			foreach (Tile tile in Physics.GetTilesMeeting(position)) {
				if ((tile is TileDoor) && tile.IsSolid) {
					SetPositionByCenter(tile.Center +
						((TileDoor) tile).Direction.ToVector(GameSettings.TILE_SIZE));
					break;
				}
			}

			// Force the player to attach to ladders on spawn.
			ForceSideScrollingLadder();

			RoomControl.OnPlayerRespawn();
		}


		//-----------------------------------------------------------------------------
		// Items
		//-----------------------------------------------------------------------------

		public void InterruptWeapons() {
			for (int i = 0; i < EquippedWeapons.Length; i++) {
				ItemWeapon item = EquippedWeapons[i];
				if (item != null) {
					item.Interrupt();
					if (item.IsTwoHanded)
						break;
				}
			}
			foreach (PlayerState state in ActiveStates)
				state.OnInterruptWeapons();
			if (weaponStateMachine.IsActive)
				weaponStateMachine.CurrentState.End();
			IntegrateStateParameters();
		}

		/// <summary>Update items by checking if their buttons are pressed.<summary>
		private void UpdateEquippedItems() {
			for (int i = 0; i < EquippedWeapons.Length; i++) {
				ItemWeapon item = EquippedWeapons[i];
				if (item != null && item.IsUsable()) {
					if (Inventory.GetSlotButton(i).IsDown())
						item.OnButtonDown();
					if (!item.IsTwoHanded || i == 1)
						item.Update();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		private void UpdateUseDirections() {
			Direction controlDirection = Direction.Invalid;
			Angle controlAngle = Angle.Invalid;
			
			// Find control direction
			// Arrow priority: Left > Up > Right > Down
			if (Controls.Left.IsDown())
				controlDirection = Direction.Left;
			else if (Controls.Up.IsDown())
				controlDirection = Direction.Up;
			else if (Controls.Right.IsDown())
				controlDirection = Direction.Right;
			else if (Controls.Down.IsDown())
				controlDirection = Direction.Down;

			// Find control angle
			// Arrow priorities: Left > Right, Up > Down
			if (Controls.Up.IsDown()) {
				if (Controls.Left.IsDown())
					controlAngle = Angle.UpLeft;
				else if (Controls.Right.IsDown())
					controlAngle = Angle.UpRight;
				else
					controlAngle = Angle.Up;
			}
			else if (Controls.Down.IsDown()) {
				if (Controls.Left.IsDown())
					controlAngle = Angle.DownLeft;
				else if (Controls.Right.IsDown())
					controlAngle = Angle.DownRight;
				else
					controlAngle = Angle.Down;
			}
			else if (Controls.Left.IsDown())
				controlAngle = Angle.Left;
			else if (Controls.Right.IsDown())
				controlAngle = Angle.Right;

			// Determine use angle/direction
			if (movement.IsMoving && !StateParameters.EnableStrafing) {
				useAngle		= movement.MoveAngle;
				useDirection	= movement.MoveDirection;
			}
			else if (controlAngle.IsValid) {
				useAngle		= controlAngle;
				useDirection	= controlDirection;
			}
			else {
				useAngle		= direction.ToAngle();
				useDirection	= direction;
			}
		}

		/// <summary>Check if the player can room-transition in the given direction.
		/// </summary>
		private bool CanRoomTransition(int transitionDirection) {
			return (movement.IsMovingInDirection(transitionDirection) ||
				stateParameters.EnableAutomaticRoomTransitions);
		}
		
		private bool IsOnHazardTile() {
			return physics.IsInHole ||
				(physics.IsInWater &&
					!swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInWater)) ||
				(physics.IsInOcean &&
					!swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInOcean)) ||
				(physics.IsInLava  &&
					!swimmingSkills.HasFlag(PlayerSwimmingSkills.CanSwimInLava));
		}

		/// <summary>Custom collision function for colliding with room edges.</summary>
		public void CheckRoomTransitions() {
			if (StateParameters.ProhibitRoomTransitions || IsOnHazardTile())
				return;

			// Check for room edge collisions
			foreach (Collision collision in Physics.Collisions) {
				if (collision.IsRoomEdge && CanRoomTransition(collision.Direction)) {
					physics.Velocity = Vector2F.Zero;
					RoomControl.RequestRoomTransition(collision.Direction);
					break;
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Button Actions
		//-----------------------------------------------------------------------------

		/// <summary>Check for tile & entity press interactions.</summary>
		private bool CheckPressInteractions(ActionButtons button) {
			List<ActionButtonCallback> callbacks = buttonCallbacks[(int) button];
			for (int i = 0; i < callbacks.Count; i++) {
				if (callbacks[i].Invoke(button))
					return true;
			}
			return false;
		}

		/// <summary>Register a callback which will be invoked when the given action
		/// button is pressed. The callback should return true if an action was
		/// performed, so that other button callbacks do not also trigger.</summary>
		public void RegisterActionButtonCallback(ActionButtons button,
			ActionButtonCallback callback)
		{
			buttonCallbacks[(int) button].Add(callback);
		}
		
		/// <summary>Use equipped weapons.</summary>
		private bool ActionUseWeapon(ActionButtons button) {
			int slot = (button == ActionButtons.A ?
				Inventory.SLOT_A : Inventory.SLOT_B);
			ItemWeapon weapon = Inventory.EquippedWeapons[slot];
			if (weapon != null && weapon.IsUsable()) {
				if (weapon.OnButtonPress())
					return true;
			}
			return false;
		}
		
		/// <summary>Throw or drop any carried objects</summary>
		private bool ActionThrowCarriedObject(ActionButtons button) {
			if (WeaponState == stateCarry) {
				stateCarry.ReleaseObject();
				return true;
			}
			return false;
		}

		public bool TriggeredButtonReaction { get; set; }
		
		/// <summary>Interact with entities and tiles in front of the player.</summary>
		private bool ActionObjectInteractions(ActionButtons button) {
			if (IsBeingKnockedBack)
				return false;

			// First check entity button-press reactions
			TriggeredButtonReaction = false;
			RoomControl.InteractionManager.TriggerInstantReaction(
				this, InteractionType.ButtonAction, delegate(Entity other)
			{
				return (Direction.FromVector(other.Center - Center) == direction);
			});
			// This variable will be set to true by reaction callbacks if the reaction
			// triggered a button action
			if (TriggeredButtonReaction) {
				StopPushing();
				return true;
			}

			// Then check tile interactions
			Tile actionTile = stateGrab.GetGrabTile();
			if (actionTile != null) {
				if (actionTile.OnAction(direction)) {
					StopPushing();
					return true;
				}
			}

			return false;
		}

		/// <summary>Submerge/resurface while swimming in top-down mode.</summary>
		private bool ActionSwimSubmerge(ActionButtons button) {
			if (EnvironmentState == environmentStateSwim) {
				if (environmentStateSwim.IsSubmerged)
					environmentStateSwim.Resurface();
				else
					environmentStateSwim.Submerge();
				return true;
			}
			return false;
		}

		/// <summary>Resurface when swimming underwater.</summary>
		private bool ActionUnderwaterResurface(ActionButtons button) {
			if (EnvironmentState == environmentStateUnderwater &&
				environmentStateUnderwater.CanResurface())
			{
				environmentStateUnderwater.Resurface();
				return true;
			}
			return false;
		}
		
		/// <summary>Perform a stroke motion when swimming.</summary>
		private bool ActionStroke(ActionButtons button) {
			if (IsSwimming && movement.CanStroke()) {
				movement.Stroke();
				return true;
			}
			return false;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			
			viewFocusOffset		= Vector2F.Zero;
			direction			= Direction.Down;
			useDirection		= 0;
			useAngle			= 0;
			syncAnimationWithDirection = true;
			isFrozen			= false;

			stateParameters = new PlayerStateParameters();
		}

		public override void OnEnterRoom() {
			// Update information about the tile we are standing on
			Physics.TopTile = RoomControl.TileManager
				.GetSurfaceTileAtPosition(position, Physics.MovesWithPlatforms);

			// Notify the state
			foreach (PlayerState state in ActiveStates)
				state.OnEnterRoom();

			// In side-scroll mode, the player samples the top-tile from his center
			// postion opposed to his origin position
			if (RoomControl.IsSideScrolling)
				Physics.TopTilePointOffset = CenterOffset;
			else
				Physics.TopTilePointOffset = Point2I.Zero;
		}

		public override void OnLeaveRoom() {
			// Notify the state
			foreach (PlayerState state in ActiveStates)
				state.OnLeaveRoom();

			// Clear events
			eventJump = null;
			eventLand = null;
		}

		public override void Knockback(int duration, float speed, Vector2F sourcePosition) {
			// Don't apply knockback when doomed to fall in a hole
			if (!movement.IsDoomedToFallInHole)
				base.Knockback(duration, speed, sourcePosition);
		}

		public override void OnHurt(DamageInfo damage) {
			if (damage.PlaySound)
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_HURT);
			foreach (PlayerState state in ActiveStates)
				state.OnHurt(damage);
		}

		public override void OnCrush(Collision rock, Collision hardPlace) {
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
			RespawnDeath();
			if (rock.IsHorizontal)
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_HORIZONTAL);
			else
				Graphics.PlayAnimation(GameData.ANIM_PLAYER_CRUSH_VERTICAL);
		}

		public override void Die() {
			// Don't actually die.
		}

		public override void OnBeginFalling() {
			base.OnBeginFalling();

			if (RoomControl.IsSideScrolling) {
				// Play the jump animation
				if (WeaponState == null)
					Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
				else if (Graphics.Animation == Animations.Default)
					Graphics.PlayAnimation(Animations.Default);
			}
		}

		public override void OnLand() {
			base.OnLand();

			// Notify the tile we landed on
			Tile tile = RoomControl.GetTopTile(RoomControl.GetTileLocation(Position));
			if (tile != null)
				tile.OnLand(movement.JumpStartTile);
			movement.JumpStartTile = -Point2I.One;

			eventLand?.Invoke(this);

			Physics.Gravity = GameSettings.DEFAULT_GRAVITY;

			if (RoomControl.DeathOutOfBounds &&
				Position.Y + 3 >= RoomControl.RoomBounds.Bottom)
			{
				RespawnDeathInstantaneous();
			}
			else {
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_LAND);
			}
		}

		public override void UpdateGraphics() {
			base.UpdateGraphics();
			
			// Sync the graphics color with the current tunic.
			switch (tunic) {
			case PlayerTunics.GreenTunic:
				Graphics.ColorDefinitions.SetAll("green");
				break;
			case PlayerTunics.RedTunic:
				Graphics.ColorDefinitions.SetAll("red");
				break;
			case PlayerTunics.BlueTunic:
				Graphics.ColorDefinitions.SetAll("blue");
				break;
			}
		}

		public void IntegrateStateParameters() {
			// Combine all state parameters from each active state
			stateParameters = new PlayerStateParameters();
			stateParameters.PlayerAnimations.Default		= GameData.ANIM_PLAYER_DEFAULT;
			stateParameters.PlayerAnimations.Aim			= GameData.ANIM_PLAYER_AIM;
			stateParameters.PlayerAnimations.Throw			= GameData.ANIM_PLAYER_THROW;
			stateParameters.PlayerAnimations.Swing			= GameData.ANIM_PLAYER_SWING;
			stateParameters.PlayerAnimations.SwingNoLunge	= GameData.ANIM_PLAYER_SWING_NOLUNGE;
			stateParameters.PlayerAnimations.SwingBig		= GameData.ANIM_PLAYER_SWING_BIG;
			stateParameters.PlayerAnimations.Spin			= GameData.ANIM_PLAYER_SPIN;
			stateParameters.PlayerAnimations.Stab			= GameData.ANIM_PLAYER_STAB;
			stateParameters.PlayerAnimations.Carry			= GameData.ANIM_PLAYER_CARRY;
			foreach (PlayerState state in ConditionStates)
				stateParameters |= state.StateParameters;
			stateParameters |= environmentStateMachine.StateParameters;
			stateParameters |= controlStateMachine.StateParameters;
			stateParameters |= weaponStateMachine.StateParameters;

			// Integrate the combined state parameters
			
			physics.HasGravity				= !stateParameters.DisableGravity;
			physics.OnGroundOverride		= stateParameters.EnableGroundOverride;
			physics.CollideWithWorld		= !stateParameters.DisableSolidCollisions;
			physics.MovesWithPlatforms		= !StateParameters.DisablePlatformMovement;
			physics.MovesWithConveyors		= !StateParameters.DisablePlatformMovement;
			physics.DisableSurfaceContact	= StateParameters.DisableSurfaceContact;

			IsPassable = stateParameters.DisableInteractionCollisions;
		}

		private void UpdateStates() {
			IntegrateStateParameters();

			// Check for beginning pushing
			if (WeaponState == null && statePush.GetPushTile() != null) {
				BeginWeaponState(statePush);
				IntegrateStateParameters();
			}

			// Update the weapon state
			weaponStateMachine.Update();

			// Update the environment state
			environmentStateMachine.Update();
			RequestNaturalState();

			// Update the control state
			controlStateMachine.Update();

			// Update the condition states
			for (int i = 0; i < conditionStateMachines.Count; i++) {
				conditionStateMachines[i].Update();
				if (!conditionStateMachines[i].IsActive)
					conditionStateMachines.RemoveAt(i--);
			}

			IntegrateStateParameters();
			
			// Play the default animation
			if (IsOnGround && !stateParameters.ProhibitMovementControlOnGround)
				Graphics.SetAnimation(Animations.Default);
		}

		public override void Update() {
			if (!isFrozen) {

				// Pre-state update
				RequestNaturalState();
				movement.Update();
				UpdateUseDirections();
				pressedActionButtons[(int) ActionButtons.A] = false;
				pressedActionButtons[(int) ActionButtons.B] = false;
				if (Controls.A.IsPressed()) {
					pressedActionButtons[(int) ActionButtons.A] =
						CheckPressInteractions(ActionButtons.A);
				}
				IntegrateStateParameters();
				if (Controls.B.IsPressed()) {
					pressedActionButtons[(int) ActionButtons.B] =
						CheckPressInteractions(ActionButtons.B);
				}
				IntegrateStateParameters();
				RequestNaturalState();
			
				// Update the current player states
				UpdateStates();

				// Post-state update
				UpdateEquippedItems();

				// Update superclass
				base.Update();
			}
		}

		public override void Draw(RoomGraphics g) {
			foreach (PlayerState state in ActiveStates)
				state.DrawUnder(g);

			base.Draw(g);

			foreach (PlayerState state in ActiveStates)
				state.DrawOver(g);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns the inventory of the player.</summary>
		public Inventory Inventory {
			get { return GameControl.Inventory; }
		}

		public ItemWeapon[] EquippedWeapons {
			get { return Inventory.EquippedWeapons; }
		}

		public Angle MoveAngle {
			get { return movement.MoveAngle; }
		}

		public Direction MoveDirection {
			get { return movement.MoveDirection; }
		}

		public Angle UseAngle {
			get { return useAngle; }
		}
		
		public Direction UseDirection {
			get { return useDirection; }
		}
		
		public PlayerMoveComponent Movement {
			get { return movement; }
			set { movement = value; }
		}

		public PlayerSwimmingSkills SwimmingSkills {
			get { return swimmingSkills; }
			set { swimmingSkills = value; }
		}

		public PlayerTunics Tunic {
			get { return tunic; }
			set { tunic = value; }
		}

		public PlayerStateAnimations Animations {
			get { return stateParameters.PlayerAnimations; }
		}

		public Animation MoveAnimation {
			get { return stateParameters.PlayerAnimations.Default; }
		}

		public float PushSpeed {
			get {
				if (Inventory.IsItemObtained("bracelet")) {
					int braceletLevel = Inventory.GetItem("bracelet").Level;
					return GameSettings.BRACELET_PUSH_SPEEDS[braceletLevel];
				}
				return GameSettings.PLAYER_DEFAULT_PUSH_SPEED;
			}
		}

		public Vector2F ViewFocusOffset {
			get { return viewFocusOffset; }
			set { viewFocusOffset = value; }
		}

		public bool IsInMinecart {
			get { return (stateMinecart.IsActive); }
		}

		public bool IsUnderwater {
			get { return RoomControl.IsUnderwater; }
		}

		// Events.

		public event PlayerDelegate EventJump {
			add { eventJump += value; }
			remove { eventJump -= value; }
		}

		public event PlayerDelegate EventLand {
			add { eventLand += value; }
			remove { eventLand -= value; }
		}
		
		// Player states

		public PlayerStateParameters StateParameters {
			get { return stateParameters; }
		}

		public IEnumerable<PlayerState> ActiveStates {
			get {
				if (controlStateMachine.IsActive)
					yield return controlStateMachine.CurrentState;
				if (weaponStateMachine.IsActive)
					yield return weaponStateMachine.CurrentState;
				if (environmentStateMachine.IsActive)
					yield return environmentStateMachine.CurrentState;
				foreach (PlayerStateMachine stateMachine in conditionStateMachines) {
					if (stateMachine.IsActive)
						yield return stateMachine.CurrentState;
				}
			}
		}

		public PlayerEnvironmentState EnvironmentState {
			get { return environmentStateMachine.CurrentState as PlayerEnvironmentState; }
		}

		public PlayerState ControlState {
			get { return controlStateMachine.CurrentState; }
		}

		public PlayerState WeaponState {
			get { return weaponStateMachine.CurrentState; }
		}

		public IEnumerable<PlayerState> ConditionStates {
			get {
				foreach (PlayerStateMachine stateMachine in conditionStateMachines) {
					if (stateMachine.IsActive)
						yield return stateMachine.CurrentState;
				}
			}
		}

		public PlayerSwimEnvironmentState SwimState {
			get { return environmentStateSwim; }
		}

		public PlayerSideScrollSwimEnvironmentState SideScrollSwimState {
			get { return environmentStateSideScrollSwim; }
		}

		public PlayerUnderwaterEnvironmentState UnderwaterState {
			get { return environmentStateUnderwater; }
		}

		public PlayerLedgeJumpState LedgeJumpState {
			get { return stateLedgeJump; }
		}

		public PlayerLeapLedgeJumpState LeapLedgeJumpState {
			get { return stateLeapLedgeJump; }
		}

		public PlayerLadderEnvironmentState LadderState {
			get { return environmentStateLadder; }
		}

		public PlayerSideScrollLadderEnvironmentState SideScrollLadderState {
			get { return environmentStateSideScrollLadder; }
		}

		public bool IsOnSideScrollLadder {
			get { return (EnvironmentState == environmentStateSideScrollLadder); }
		}

		public bool IsOnAnyLadder {
			get {
				return (EnvironmentState == environmentStateLadder ||
						EnvironmentState == environmentStateSideScrollLadder);
			}
		}

		public PlayerSwingSwordState SwingSwordState {
			get { return stateSwingSword; }
		}

		public PlayerSwingBigSwordState SwingBigSwordState {
			get { return stateSwingBigSword; }
		}

		public PlayerSwingCaneState SwingCaneState {
			get { return stateSwingCane; }
		}

		public PlayerSwingMagicRodState SwingMagicRodState {
			get { return stateSwingMagicRod; }
		}

		public PlayerHoldSwordState HoldSwordState {
			get { return stateHoldSword; }
		}

		public PlayerSwordStabState SwordStabState {
			get { return stateSwordStab; }
		}

		public PlayerSpinSwordState SpinSwordState {
			get { return stateSpinSword; }
		}

		public PlayerSeedShooterState SeedShooterState {
			get { return stateSeedShooter; }
		}

		public PlayerSwitchHookState SwitchHookState {
			get { return stateSwitchHook; }
		}

		public PlayerSwitchHookSwitchState SwitchHookSwitchState {
			get { return stateSwitchHookSwitch; }
		}

		public PlayerMagicBoomerangState MagicBoomerangState {
			get { return stateMagicBoomerang; }
		}

		public PlayerCarryState CarryState {
			get { return stateCarry; }
		}

		public PlayerGrabState GrabState {
			get { return stateGrab; }
		}

		public PlayerPushState PushState {
			get { return statePush; }
		}

		public PlayerMagnetGlovesState MagnetGlovesState {
			get { return stateMagnetGloves; }
		}

		public PlayerShieldState ShieldState {
			get { return stateShield; }
		}

		public PlayerPullHandleState PullHandleState {
			get { return statePullHandle; }
		}

		public PlayerRespawnDeathState RespawnDeathState {
			get { return stateRespawnDeath; }
		}

		public PlayerMinecartState MinecartState {
			get { return stateMinecart; }
		}

		public PlayerJumpToState JumpToState {
			get { return stateJumpTo; }
		}

		public PlayerJumpEnvironmentState JumpState {
			get { return environmentStateJump; }
		}

		public bool IsSwimmingUnderwater {
			get {
				return (EnvironmentState == environmentStateUnderwater |
					EnvironmentState == environmentStateSideScrollSwim);
			}
		}

		public bool IsSwimming {
			get {
				return (EnvironmentState == environmentStateSwim ||
					EnvironmentState == environmentStateUnderwater ||
					EnvironmentState == environmentStateSideScrollSwim);
			}
		}

		// Tools

		public PlayerToolSword ToolSword {
			get { return toolSword; }
		}
		
		public PlayerToolShield ToolShield {
			get { return toolShield; }
		}
		
		public PlayerToolVisual ToolVisual {
			get { return toolVisual; }
		}

		public bool IsFrozen {
			get { return isFrozen; }
			set { isFrozen = value; }
		}

		/// <summary>Used to check if the player is currently swimming and submerged
		/// in order to collect divable rewards.</summary>
		public bool IsSubmerged {
			get {
				return (EnvironmentState == environmentStateSwim &&
					environmentStateSwim.IsSubmerged);
			}
		}

		public bool[] PressedActionButtons {
			get { return pressedActionButtons; }
		}
	}
}
