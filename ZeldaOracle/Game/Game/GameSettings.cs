﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game {

	public class GameSettings {
		
		// World
		public const int					TILE_SIZE					= 16;	// Tile size in texels.
		public static readonly Point2I		ROOM_SIZE_SMALL				= new Point2I(10, 8);
		public static readonly Point2I		ROOM_SIZE_LARGE				= new Point2I(15, 11);
		public const int					ROOM_RESPAWN_VISIT_COUNT	= 8;
		public const int					DEFAULT_TILE_LAYER_COUNT	= 3;

		// Display
		public const int					HUD_HEIGHT				= 16;
		public static readonly Point2I		HUD_OFFSET				= new Point2I(0, HUD_HEIGHT);
		public const int					SCREEN_WIDTH			= 160;
		public const int					SCREEN_HEIGHT			= 144;
		public const int					VIEW_WIDTH				= 160;
		public const int					VIEW_HEIGHT				= 128;
		public static readonly Point2I		SCREEN_SIZE				= new Point2I(SCREEN_WIDTH, SCREEN_HEIGHT);
		public static readonly Point2I		VIEW_SIZE				= new Point2I(VIEW_WIDTH, VIEW_HEIGHT);
		public static readonly Rectangle2I	SCREEN_BOUNDS			= new Rectangle2I(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
		public const int					VIEW_PAN_SPEED			= 1;

		/// <summary>The bias used to make sure 0.5 always rounds in the same direction when drawing.</summary>
		public const float					BIAS					= 0.001f;

		/// <summary>The epsilon value for velocity used in collision dodging.</summary>
		public const float					EPSILON					= 0.001f;

		// Properties
		public const string				TEXT_UNDEFINED				= "<red>undefined<red>";

		public const string				HEART_PIECE_TEXT			= "You got a <n> <red>Piece of " +
																		"<n>Heart<red>!<heart-piece>" +
																		"Collect four in all to get an " +
																		"extra <red>Heart Container<red>! Check " +
																		"them on the Item Screen.";

		public const string				HEART_CONTAINER_TEXT		= "You got four <red>Pieces of Heart<red>! " +
																		"That makes one <red>Heart Container<red>!";

		// Physics
		public const float				DEFAULT_GRAVITY				= 0.125f;	// Default gravity acceleration in pixels per frame^2
		public const float				DEFAULT_MAX_FALL_SPEED		= 4.0f;
		
		// Drops & Collectibles
		public const float				DROP_ENTITY_SPAWN_ZVELOCITY				= 1.5f;
		public const float				DROP_ENTITY_DIG_VELOCITY				= 0.75f;
		public const int				COLLECTIBLE_ALIVE_DURATION				= 513;
		public const int				COLLECTIBLE_FADE_DELAY					= 400;
		public const int				COLLECTIBLE_PICKUPABLE_DELAY			= 12;
		public const int				COLLECTIBLE_DIG_PICKUPABLE_DELAY		= 20;
		public const int				COLLECTIBLE_FAIRY_ALIVE_DURATION		= 513;
		public const int				COLLECTIBLE_FAIRY_FADE_DELAY			= 400;
		public const int				COLLECTIBLE_FAIRY_HOVER_HEIGHT			= 8;

		// Projectiles and Items
		public static readonly float[]	BRACELET_PUSH_SPEEDS					= { 0.5f, 1.0f };
		public const float				PROJECTILE_ARROW_SPEED					= 3.0f;
		public const int				PROJECTILE_ARROW_DAMAGE					= 1;
		public const float				PROJECTILE_SWORD_BEAM_SPEED				= 3.0f;
		public const float				PROJECTILE_MAGIC_ROD_FIRE_SPEED			= 2.0f;
		public static readonly float[]	PROJECTILE_BOOMERANG_SPEEDS				= { 1.5f, 3.0f };
		public static readonly int[]	PROJECTILE_BOOMERANG_RETURN_DELAYS		= { 41, 100 };
		public static readonly float[]	PROJECTILE_SWITCH_HOOK_SPEEDS			= { 2.0f, 3.0f };
		public static readonly int[]	PROJECTILE_SWITCH_HOOK_LENGTHS			= { 82, 112 };
		public static readonly int[]    PROJECTILE_FIRE_SHOOTER_PHASE_DURATIONS	= new int[] { 10, 14 };
		public const int				SWITCH_HOOK_LATCH_DURATION				= 20;
		public const float				SWITCH_HOOK_LIFT_SPEED					= 1.0f;
		public const int				SWITCH_HOOK_LIFT_HEIGHT					= 16;
		public const float				SLINGSHOT_SEED_SPEED					= 3.0f;
		public const float				SLINGSHOT_SEED_RADIAN_OFFSET			= GMath.FullAngle / 18.0f; // For the 2 extra seed from the hyper slingshot.
		public const float				SEED_SHOOTER_SHOOT_SPEED				= 3.0f;
		public const int				SEED_SHOOTER_SHOOT_PAUSE_DURATION		= 12;
		public const int				SEED_SHOOTER_AUTO_ROTATE_DELAY			= 16; // Auto-rotate every 16 frames
		public const int				SEED_PROJECTILE_REBOUND_COUNT			= 3;
		public const int				SCENT_POD_DURATION						= 240;
		public const int				SCENT_POD_FADE_DELAY					= 60;
		public const int				BOMB_FUSE_TIME							= 108;
		public const int				BOMB_FLICKER_DELAY						= 72;
		public const int				BOMB_EXPLOSION_PLAYER_DAMAGE			= 2;
		public const int				BOMB_EXPLOSION_DAMAGE_DELAY				= 10; // delay before bomb explosion
		public const float				MAGNET_BALL_LATERAL_MOVE_SPEED			= 1.0f;
		public const float				MAGNET_BALL_MAX_MOVE_SPEED				= 3.0f;
		public const float				MAGNET_BALL_LEDGE_JUMP_MOVE_SPEED		= 1.0f;
		public const float				MAGNET_BALL_LEDGE_JUMP_JUMP_SPEED		= 1.8f;
		public const float				MAGNET_BALL_ACCELERATION				= 0.02f;
		public const float				MAGNET_BALL_DECELERATION				= 0.04f;
		public const float				MAGNET_BALL_MIN_DISTANCE				= 21;
		// Units
		public const float				UNIT_KNOCKBACK_SPEED			= 1.0f; // 1.3 ??
		public const int				UNIT_KNOCKBACK_DURATION			= 16;
		public const int				UNIT_HURT_INVINCIBLE_DURATION	= 32;
		public const int				UNIT_HURT_FLICKER_DURATION		= 32;
		public const int				UNIT_KNOCKBACK_ANGLE_SNAP_COUNT	= 16;

		public const int				MONSTER_SPAWN_STATE_DURATION		= 31;

		public const float				MONSTER_KNOCKBACK_SPEED				= 2.0f; // 1.3 ??
		public const int				MONSTER_HURT_KNOCKBACK_DURATION		= 11;
		public const int				MONSTER_BUMP_KNOCKBACK_DURATION		= 8;

		public const int				MONSTER_HURT_INVINCIBLE_DURATION	= 16;
		public const int				MONSTER_HURT_FLICKER_DURATION		= 16;
		public const int				MONSTER_BURN_DURATION				= 59;
		
		public const float				PLAYER_KNOCKBACK_SPEED				= 1.25f; // 1.3 ??
		public const int				PLAYER_HURT_KNOCKBACK_DURATION		= 15;
		public const int				PLAYER_BUMP_KNOCKBACK_DURATION		= 11;
		//public const int				MONSTER_HURT_INVINCIBLE_DURATION	= 16;
		//public const int				MONSTER_HURT_FLICKER_DURATION		= 16;
		//public const int				MONSTER_BURN_DURATION				= 59;
		
		//public const int				InvincibleDuration					= 25;
		//public const int				InvincibleControlRestoreDuration	= 8;
		//public const int				KnockbackSnapCount					= 16;
		//public const float			KnockbackSpeed						= 1.3f;


		// Player
		public const float				PLAYER_MOVE_SPEED					= 1.0f;		// Pixels per second.
		public const float				PLAYER_JUMP_SPEED					= 2.0f;
		public const float				PLAYER_CAPE_JUMP_SPEED				= 0.5f;
		public const float				PLAYER_SIDESCROLL_JUMP_SPEED		= 2.25f;
		public const float				PLAYER_SIDESCROLL_CAPE_JUMP_SPEED	= 0.5f;
		public const float				PLAYER_CAPE_REQUIRED_FALLSPEED		= 1.0f;		// Player must be falling this fast to be able to deploy cape.
		public const float				PLAYER_CAPE_GRAVITY					= 0.04f;	// 0.04 = 1/25
		public const int				PLAYER_SPRINT_DURATION				= 480;
		public const float				PLAYER_SPRINT_SPEED_SCALE			= 1.5f;
		public const int				PLAYER_SPRINT_EFFECT_INTERVAL		= 10;
		public const float				PLAYER_DEFAULT_PUSH_SPEED			= 0.5f;
		public const int				PLAYER_LEAP_LEDGE_JUMP_DURATION		= 30;
		public const float				PLAYER_LEAP_LEDGE_JUMP_DISTANCE		= 35;
		// TODO: Perfect this value, it's only close to accurate
		public const float              PLAYER_LEAP_LEDGE_JUMP_SPEED		= 1.75f;
		public const float				PLAYER_MAGNET_GLOVE_MOVE_SPEED		= 1.5f;

		public static readonly Rectangle2F[] PLAYER_BRACELET_BOXES = {
			new Rectangle2I(-2, -7, 13, 13),
			new Rectangle2I(-6, -13, 12, 13),
			new Rectangle2I(-11, -7, 13, 13),
			new Rectangle2I(-6, -2, 12, 13),
		};

		// Monsters
		public const int				MONSTER_STUN_DURATION					= 400;	// How long a monster gets stunned for (by boomerang/pegasus seeds).
		public const int				MONSTER_STUN_SHAKE_DURATION				= 60;	// How long the monster shakes at the end of being stunned.
		public const int				MONSTER_BOOMERANG_DAMAGE				= 1;
		public const int				MONSTER_PROJECTILE_BONE_DAMAGE			= 2;
		public const int				MONSTER_ELECTROCUTE_FREEZE_DURATION		= 45;
		public const int				MONSTER_ELECTROCUTE_ANIMATE_DURATION	= 60;

		public const int				MONSTER_THWOMP_CRUSH_MIN_ALIGNMENT				= 20; // TODO: needs confirmation
		public const float				MONSTER_THWOMP_CRUSH_ACCELERATION				= 0.2f;
		public const float				MONSTER_THWOMP_CRUSH_INITIAL_SPEED				= 0.5f;
		public const float				MONSTER_THWOMP_CRUSH_MAX_SPEED					= 2.5f;
		public const float				MONSTER_THWOMP_RAISE_SPEED						= 0.5f;
		public const int				MONSTER_THWOMP_HIT_SHAKE_DURATION				= 46;
		public const int				MONSTER_THWOMP_HIT_WAIT_DURATION				= 46 + 16;
		public const float				MONSTER_BEAMOS_SHOOT_SPEED						= 8.0f;
		public const int				MONSTER_BEAMOS_SHOOT_ANGLE_COUNT				= 24;
		public const WindingOrder		MONSTER_BEAMOS_ROTATE_DIRECTION					= WindingOrder.Clockwise;
		public const float				MONSTER_PINCER_STRIKE_SPEED						= 2.0f;
		public const float				MONSTER_PINCER_RETURN_SPEED						= 1.0f;
		public const float				MONSTER_PINCER_STRIKE_DISTANCE					= 32.0f;	// Distance to extend when striking
		public const float				MONSTER_PINCER_PEEK_RANGE						= 40.0f;	// Min distance to player to start peeking
		public const int				MONSTER_PINCER_PEEK_DURATION					= 35;		// Peek duration before striking
		public const int				MONSTER_PINCER_PEEK_DELAY						= 30;		// Delay before can peek again after returning
		public const int				MONSTER_PINCER_RETURN_DELAY						= 8;		// Delay before return starts after strike completes
		public const int				MONSTER_PINCER_BODY_SEGMENT_COUNT				= 4;
		public const float				MONSTER_BLADE_TRAP_INITIAL_SPEED				= 0.75f;
		public const float				MONSTER_BLADE_TRAP_MAX_SPEED					= 0.75f;
		public const float				MONSTER_BLADE_TRAP_ACCELERATION					= 0.2f;
		public const float				MONSTER_BLADE_TRAP_AGGRO_RANGE					= 14.0f;
		public const float				MONSTER_SPINNING_BLADE_TRAP_SLOW_SPEED			= 0.35f;
		public const float				MONSTER_SPINNING_BLADE_TRAP_FAST_SPEED			= 0.75f;
		public const int				MONSTER_SPINNING_BLADE_TRAP_SPEED_UP_DELAY		= 20;
		public const float				MONSTER_SPINNING_BLADE_TRAP_AGGRO_RANGE			= 14.0f;
		public const int				MONSTER_POLS_VOICE_SEEK_PLAYER_ODDS				= 10;	// Odds to jump toward player
		public const float				MONSTER_POLS_VOICE_SEEK_MOVE_SPEED_MULTIPLIER	= 1.7f;	// movement speed scale when jumping toward player
		public const float				MONSTER_POLS_VOICE_SEEK_JUMP_SPEED_MULTIPLIER	= 1.3f;	// jump speed scale when jumping toward player
		public const float				MONSTER_ZOL_GREEN_UNBURROW_RANGE				= 48;	// min distance to player to unburrow
		public const int				MONSTER_ZOL_GREEN_JUMP_COUNT					= 3;	// number of jumps before burrowing
		public const int				MONSTER_ZOL_GREEN_UNBURROW_DELAY				= 50;	// delay after burrowing before unburrowing
		public const int				MONSTER_GEL_ATTACH_TIME							= 120;
		public const float				MONSTER_KEESE_ACCELERATION						= 0.05f;
		public const float				MONSTER_KEESE_DECELERATION						= 0.015f;
		public const float				MONSTER_WIZZROBE_SHOOT_SPEED					= 2.0f;
		public const int				MONSTER_WIZZROBE_GREEN_PEEK_DURATION			= 75;
		public const int				MONSTER_WIZZROBE_GREEN_PEEK_FLICKER_DURATION	= 30;
		public const int				MONSTER_WIZZROBE_GREEN_APPEAR_DURATION			= 72;
		public const int				MONSTER_WIZZROBE_GREEN_SHOOT_DELAY				= 20;
		public const int				MONSTER_WIZZROBE_RED_PEEK_DURATION				= 58;
		public const int				MONSTER_WIZZROBE_RED_SHOOT_DELAY				= 20;
		public const int				MONSTER_WIZZROBE_RED_APPEAR_DURATION			= 30;
		public const int				MONSTER_WIZZROBE_BLUE_FLICKER_DURATION			= 238;
		public const int				MONSTER_WIZZROBE_BLUE_APPEAR_DURATION			= 36;
		public const int				MONSTER_WIZZROBE_BLUE_MOVE_DELAY				= 8;
		public const int				MONSTER_GOPONGA_FLOWER_CLOSED_DURATION			= 120;
		public const int				MONSTER_GOPONGA_FLOWER_OPEN_DURATION			= 60;
		public const int				MONSTER_GOPONGA_FLOWER_SHOOT_DELAY				= 20;
		public const int				MONSTER_GOPONGA_FLOWER_SHOOT_HOLD_DURATION		= 10;
		public const float				MONSTER_GOPONGA_FLOWER_SHOOT_SPEED				= 1.5f;
		public const int				MONSTER_GOPONGA_FLOWER_SHOOT_ODDS				= 6;
		public const float				MONSTER_ANTI_FAIRY_MOVE_SPEED					= 0.75f;
		public const float				MONSTER_WATER_TEKTIKE_MOVE_SPEED				= 1.5f;
		public const float				MONSTER_WATER_TEKTIKE_ACCELERATION				= 0.0375f;
		public const float				MONSTER_WATER_TEKTIKE_DECELERATION				= 0.075f;
		public const int				MONSTER_WATER_TEKTIKE_DECELERATION_TIME			= 20;
		public static RangeI			MONSTER_RIVER_ZORA_SUBMERGED_DURATION			= new RangeI(25, 60);
		public const int				MONSTER_RIVER_ZORA_RESURFACE_DURATION			= 48;
		public const int				MONSTER_RIVER_ZORA_SURFACED_DURATION			= 88;
		public const int				MONSTER_RIVER_ZORA_SHOOT_DELAY					= 9;
		public const int				MONSTER_RIVER_ZORA_SUBMERGE_DELAY				= 9;
		public const int				MONSTER_POKEY_BODY_SEPARATION					= 12;
		public static RangeI			MONSTER_PEAHAT_FLY_DURATION						= new RangeI(476);
		public static RangeI			MONSTER_PEAHAT_STOP_DURATION					= new RangeI(15, 150);
		public const float				MONSTER_PEAHAT_FLY_SPEED						= 0.75f;
		public const float				MONSTER_PEAHAT_ACCELERATION						= 0.0125f; // 60 frames from 0.00 to 0.75
		public const float				MONSTER_PEAHAT_DECELERATION						= 0.0125f;
		public const int				MONSTER_PEAHAT_DECELERATE_DURATION				= 60;
		public const float				MONSTER_PEAHAT_FLY_ALTITUDE						= 5;
		public const float				MONSTER_PEAHAT_RAISE_SPEED						= 0.1f;
		public const float				MONSTER_PEAHAT_LOWER_SPEED						= 0.1f;
		public static int[]				MONSTER_BARI_ELECTROCUTE_DELAYS					= { 90, 120, 150 };
		public const int				MONSTER_BARI_ELECTROCUTE_DURATION				= 60;
		//public const int				MONSTER_BARI_HOVER_DELAY						= 16; // 16, 16, 16: low, mid, high,
		public const float				MONSTER_STALFOS_ORANGE_JUMP_MOVE_SPEED			= 1.0f;
		public const float				MONSTER_STALFOS_ORANGE_JUMP_SPEED				= 2.25f;
		public const float				MONSTER_STALFOS_ORANGE_JUMP_RANGE				= 48;
		public const int				MONSTER_STALFOS_ORANGE_JUMP_RECHARGE_DELAY		= 0;
		public const int				MONSTER_FLYING_TILE_LAUNCH_ANGLE_COUNT			= 16;
		public const float				MONSTER_FLYING_TILE_LAUNCH_SPEED				= 1.75f;
		public const int				MONSTER_FLYING_TILE_HOVER_DURATION				= 24;
		public const float				MONSTER_FLYING_TILE_HOVER_ALTITUDE				= 4f;
		public const float				MONSTER_FLYING_TILE_RISE_SPEED					= 0.5f;
		public const int				MONSTER_FLYING_TILE_START_OFFSET				= 122;
		public const int				MONSTER_FLYING_TILE_NEXT_OFFSET					= 60;
		public const int				MONSTER_ARMOS_BREATH_LIFE_DURATION				= 60;
		public const float				MONSTER_SPIKE_ROLLER_MOVE_SPEED					= 0.75f;
		public const float				MONSTER_IRON_MASK_MAGNETIC_PULL_RANGE			= 60.0f;
		public const int				MONSTER_IRON_MASK_UNMASK_DELAY					= 60;
		public const int				MONSTER_IRON_MASK_MASK_DISSAPEAR_DELAY			= 28;
		public const float				MONSTER_IRON_MASK_MASK_MOVE_SPEED				= 2.0f;
		
		// Tiles
		public const int				TILE_BUTTON_TILE_RAISE_AMOUNT			= 2;	// Pixels to raise certain tiles when pushed over a button.
		public const int				TILE_BUTTON_UNCOVER_RELEASE_DELAY		= 27;	// Delay between being uncovered and becoming released.
		public static readonly Rectangle2F	TILE_BUTTON_PLAYER_PRESS_AREA		= new Rectangle2F(0, 5, 16, 16);
		public const int				TILE_CRACKED_FLOOR_CRUMBLE_DELAY		= 30;
		public const float				TILE_ROLLER_MOVE_SPEED					= 0.5f;
		public const float				TILE_PULL_HANDLE_EXTEND_SPEED			= 0.25f;
		public const float				TILE_PULL_HANDLE_RETRACT_SPEED			= 0.25f;
		public const int				TILE_PULL_HANDLE_PLAYER_PULL_DURATION	= 40;
		public const int				TILE_PULL_HANDLE_PLAYER_PAUSE_DURATION	= 20;
		public const int				TILE_PULL_HANDLE_EXTEND_LENGTH			= 64;
		public const int				TILE_PULL_HANDLE_WALL_OFFSET			= 8;

		public const int				TILE_DISAPPEARING_PLATFORM_APPEAR_DURATION	= 30;

		public static readonly int[]	TILE_ARROW_CANNON_SHOOT_STARTS			= new int[] { 16, 32 };
		public static readonly int[]	TILE_ARROW_CANNON_SHOOT_INTERVALS		= new int[] { 32, 64, 96, 128 };
		public const int				TILE_ARROW_SHOOTER_SHOOT_INTERVAL		= 33;
		public const int				TILE_FIRE_SHOOTER_SHOOT_INVERVAL		= 16;
		public const int				TILE_FIRE_SHOOTER_SHOOT_OFFSET			= 7;

		public const float				TILE_ICE_BLOCK_MOVEMENT_SPEED			= 1.75f;



		//-----------------------------------------------------------------------------
		// Draw modes
		//-----------------------------------------------------------------------------

		public static DrawMode DRAW_MODE_BASIC = new DrawMode() {
			BlendState      = BlendState.AlphaBlend,
			SortMode        = SpriteSortMode.Deferred,
			SamplerState    = SamplerState.PointClamp
		};

		// Effect is set to PALETTE_LERP_SHADER
		public static DrawMode DRAW_MODE_PALLETE = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.Deferred,
			SamplerState	= SamplerState.PointClamp
		};

		public static DrawMode DRAW_MODE_BACK_TO_FRONT = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.FrontToBack, // Use FrontToBack so our depth values mean 0 is below and 1 is above.
			SamplerState	= SamplerState.PointClamp
		};

		public static DrawMode DRAW_MODE_ROOM_GRAPHICS = new DrawMode() {
			BlendState		= BlendState.AlphaBlend,
			SortMode		= SpriteSortMode.Deferred,
			SamplerState	= SamplerState.PointClamp
		};

		//-----------------------------------------------------------------------------
		// Designer/Editor Settings
		//-----------------------------------------------------------------------------

		public static bool EditorMode { get; set; } = false;
		public static bool DesignerMode { get; set; } = false;
	}
}
