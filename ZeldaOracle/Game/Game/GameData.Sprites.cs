﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game {
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Sprites
		//-----------------------------------------------------------------------------

		// Effects
		public static ISprite SPR_SHADOW;

		// Special Background tiles
		public static ISprite SPR_TILE_DEFAULT;	// The default ground background tile.
		public static ISprite SPR_TILE_DUG;		// A hole in the ground created by a shovel.
	
		// Player sprites
		public static ISprite SPR_PLAYER_FORWARD;

		// Entities
		public static ISprite SPR_SWITCH_HOOK_LINK;
		public static ISprite SPR_MINECART_VERTICAL;
		public static ISprite SPR_MINECART_HORIZONTAL;
		public static ISprite SPR_MAGNET_BALL_NORTH;
		public static ISprite SPR_MAGNET_BALL_SOUTH;
		
		// Monsters
		public static ISprite SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_LARGE;
		public static ISprite SPR_MONSTER_MINI_MOLDORM_BODY_SEGMENT_SMALL;
		public static ISprite SPR_MONSTER_PINCER_BODY_SEGMENT;

		// Object tiles
		public static ISprite SPR_TILE_BUSH;
		public static ISprite SPR_TILE_BUSH_ASOBJECT;
		public static ISprite SPR_TILE_CRYSTAL;
		public static ISprite SPR_TILE_CRYSTAL_ASOBJECT;
		public static ISprite SPR_TILE_POT;
		public static ISprite SPR_TILE_POT_ASOBJECT;
		public static ISprite SPR_TILE_ROCK;
		public static ISprite SPR_TILE_ROCK_ASOBJECT;
		public static ISprite SPR_TILE_DIAMOND_ROCK;
		public static ISprite SPR_TILE_DIAMOND_ROCK_ASOBJECT;
		public static ISprite SPR_TILE_SIGN;
		public static ISprite SPR_TILE_SIGN_ASOBJECT;
		public static ISprite SPR_TILE_GRASS;
		public static ISprite SPR_TILE_MOVABLE_BLOCK;
		public static ISprite SPR_TILE_MOVABLE_BLOCK_ASOBJECT;
		public static ISprite SPR_TILE_BOMBABLE_BLOCK;
		public static ISprite SPR_TILE_LOCKED_BLOCK;
		public static ISprite SPR_TILE_CHEST;
		public static ISprite SPR_TILE_CHEST_OPEN;
		public static ISprite SPR_TILE_DIRT_PILE;
		public static ISprite SPR_TILE_BURNABLE_TREE;
		public static ISprite SPR_TILE_CACTUS;
		public static ISprite SPR_TILE_BUTTON_UP;
		public static ISprite SPR_TILE_BUTTON_DOWN;
		public static ISprite SPR_TILE_LEVER_LEFT;
		public static ISprite SPR_TILE_LEVER_RIGHT;
		public static ISprite SPR_TILE_LANTERN_UNLIT;
		public static ISprite SPR_TILE_EYE_STATUE;
		public static ISprite SPR_TILE_BRIDGE_H;
		public static ISprite SPR_TILE_BRIDGE_V;
		public static ISprite SPR_TILE_COLOR_CUBE_SLOT;
		public static ISprite SPR_TILE_CRACKED_FLOOR;
		public static ISprite SPR_TILE_PIT;
		public static ISprite SPR_TILE_ARMOS_STATUE;
		public static ISprite SPR_TILE_MOVING_PLATFORM;
		public static ISprite SPR_TILE_OWL;
		public static ISprite SPR_TILE_OWL_ACTIVATED;
		public static ISprite SPR_TILE_STATUE_EYE;
		public static ISprite SPR_TILE_SOMARIA_BLOCK;
		public static ISprite SPR_TILE_REGROWABLE_BUSH_CUT;
		public static ISprite SPR_TILE_BUSH_REGROW_GROWING;
		public static ISprite SPR_TILE_REGROWABLE_BUSH;
		public static ISprite SPR_TILE_COLOR_SWITCH_RED;
		public static ISprite SPR_TILE_COLOR_SWITCH_BLUE;
		public static ISprite SPR_TILE_COLOR_TILE_RED;
		public static ISprite SPR_TILE_COLOR_TILE_BLUE;
		public static ISprite SPR_TILE_COLOR_TILE_YELLOW;
		public static ISprite SPR_TILE_COLOR_JUMP_PAD_RED;
		public static ISprite SPR_TILE_COLOR_JUMP_PAD_YELLOW;
		public static ISprite SPR_TILE_COLOR_JUMP_PAD_BLUE;
		public static ISprite SPR_TILE_COLOR_STATUE_RED;
		public static ISprite SPR_TILE_COLOR_STATUE_YELLOW;
		public static ISprite SPR_TILE_COLOR_STATUE_BLUE;
		public static ISprite SPR_TILE_COLOR_BARRIER_RED_RAISED;
		public static ISprite SPR_TILE_COLOR_BARRIER_RED_LOWERED;
		public static ISprite SPR_TILE_COLOR_BARRIER_RED_HALFWAY;
		public static ISprite SPR_TILE_COLOR_BARRIER_BLUE_RAISED;
		public static ISprite SPR_TILE_COLOR_BARRIER_BLUE_LOWERED;
		public static ISprite SPR_TILE_COLOR_BARRIER_BLUE_HALFWAY;
		public static ISprite SPR_TILE_PULL_HANDLE_BAR_HORIZONTAL;
		public static ISprite SPR_TILE_PULL_HANDLE_UP;
		public static ISprite SPR_TILE_PULL_HANDLE_LEFT;
		public static ISprite SPR_TILE_PULL_HANDLE_BAR_VERTICAL;
		public static ISprite SPR_TILE_PULL_HANDLE_DOWN;
		public static ISprite SPR_TILE_PULL_HANDLE_RIGHT;
		public static ISprite SPR_TILE_MINECART_TRACK_HORIZONTAL;
		public static ISprite SPR_TILE_MINECART_TRACK_VERTICAL;
		public static ISprite SPR_TILE_MINECART_TRACK_UP_RIGHT;
		public static ISprite SPR_TILE_MINECART_TRACK_UP_LEFT;
		public static ISprite SPR_TILE_MINECART_TRACK_DOWN_LEFT;
		public static ISprite SPR_TILE_MINECART_TRACK_DOWN_RIGHT;
		public static ISprite SPR_TILE_DIGABLE_REWARD_DIRT;
		public static ISprite SPR_TILE_DIVABLE_REWARD_WATER;

		public static ISprite[] SPR_COLOR_CUBE_ORIENTATIONS;

		// Item Icons
		public static ISprite SPR_ITEM_SEED_EMBER;
		public static ISprite SPR_ITEM_SEED_SCENT;
		public static ISprite SPR_ITEM_SEED_PEGASUS;
		public static ISprite SPR_ITEM_SEED_GALE;
		public static ISprite SPR_ITEM_SEED_MYSTERY;
		public static ISprite[] SPR_ITEM_SEEDS;

		public static ISprite SPR_ITEM_AMMO_ARROW;
		public static ISprite SPR_ITEM_AMMO_BOMB;
		
		public static ISprite SPR_ITEM_ICON_BIGGORON_SWORD;
		public static ISprite SPR_ITEM_ICON_BIGGORON_SWORD_EQUIPPED;

		public static ISprite SPR_ITEM_ICON_SWORD_1;
		public static ISprite SPR_ITEM_ICON_SWORD_2;
		public static ISprite SPR_ITEM_ICON_SWORD_3;
		public static ISprite SPR_ITEM_ICON_SHIELD_1;
		public static ISprite SPR_ITEM_ICON_SHIELD_2;
		public static ISprite SPR_ITEM_ICON_SHIELD_3;
		public static ISprite SPR_ITEM_ICON_SATCHEL;
		public static ISprite SPR_ITEM_ICON_SATCHEL_EQUIPPED;
		public static ISprite SPR_ITEM_ICON_SEED_SHOOTER;
		public static ISprite SPR_ITEM_ICON_SEED_SHOOTER_EQUIPPED;
		public static ISprite SPR_ITEM_ICON_SLINGSHOT_1;
		public static ISprite SPR_ITEM_ICON_SLINGSHOT_2;
		public static ISprite SPR_ITEM_ICON_SLINGSHOT_2_EQUIPPED;
		public static ISprite SPR_ITEM_ICON_BOMB;
		public static ISprite SPR_ITEM_ICON_BOMBCHEW;
		public static ISprite SPR_ITEM_ICON_SHOVEL;
		public static ISprite SPR_ITEM_ICON_BRACELET;
		public static ISprite SPR_ITEM_ICON_POWER_GLOVES;
		public static ISprite SPR_ITEM_ICON_FEATHER;
		public static ISprite SPR_ITEM_ICON_CAPE;
		public static ISprite SPR_ITEM_ICON_BOOMERANG_1;
		public static ISprite SPR_ITEM_ICON_BOOMERANG_2;
		public static ISprite SPR_ITEM_ICON_SWITCH_HOOK;
		public static ISprite SPR_ITEM_ICON_HOOK_SHOT;
		public static ISprite SPR_ITEM_ICON_MAGNET_GLOVES_SOUTH;
		public static ISprite SPR_ITEM_ICON_MAGNET_GLOVES_NORTH;
		public static ISprite SPR_ITEM_ICON_CANE;
		public static ISprite SPR_ITEM_ICON_ROD_OF_ESSENCES;
		public static ISprite SPR_ITEM_ICON_MAGIC_ROD;
		public static ISprite SPR_ITEM_ICON_OCARINA;
		public static ISprite SPR_ITEM_ICON_BOW;
		public static ISprite SPR_ITEM_ICON_FLIPPERS_1;
		public static ISprite SPR_ITEM_ICON_FLIPPERS_2;
		public static ISprite SPR_ITEM_ICON_MAGIC_POTION;
		public static ISprite SPR_ITEM_ICON_MEMBERS_CARD;
		public static ISprite SPR_ITEM_ICON_ESSENCE_SEED;
		public static ISprite SPR_ITEM_ICON_ESSENCE_1;
		public static ISprite SPR_ITEM_ICON_ESSENCE_2;
		public static ISprite SPR_ITEM_ICON_ESSENCE_3;
		public static ISprite SPR_ITEM_ICON_ESSENCE_4;
		public static ISprite SPR_ITEM_ICON_ESSENCE_5;
		public static ISprite SPR_ITEM_ICON_ESSENCE_6;
		public static ISprite SPR_ITEM_ICON_ESSENCE_7;
		public static ISprite SPR_ITEM_ICON_ESSENCE_8;
		public static ISprite SPR_ITEM_ICON_WALLET_1;
		public static ISprite SPR_ITEM_ICON_WALLET_2;
		public static ISprite SPR_ITEM_ICON_WALLET_3;
		public static ISprite SPR_ITEM_FOOLS_ORE;

		// Reward Icons
		public static ISprite SPR_REWARD_RUPEE_SMALL_GREEN;
		public static ISprite SPR_REWARD_RUPEE_SMALL_RED;
		public static ISprite SPR_REWARD_RUPEE_SMALL_BLUE;
		public static ISprite SPR_REWARD_RUPEE_RED;
		public static ISprite SPR_REWARD_RUPEE_BLUE;
		public static ISprite SPR_REWARD_RUPEE_BIG_BLUE;
		public static ISprite SPR_REWARD_RUPEE_BIG_RED;
		public static ISprite SPR_REWARD_ORE_CHUNK_BLUE;
		public static ISprite SPR_REWARD_ORE_CHUNK_RED;
		public static ISprite SPR_REWARD_ORE_CHUNK_ORANGE;
		public static ISprite SPR_REWARD_HEART;
		public static ISprite SPR_REWARD_HEARTS_3;
		public static ISprite SPR_REWARD_SEED_EMBER;
		public static ISprite SPR_REWARD_SEED_SCENT;
		public static ISprite SPR_REWARD_SEED_PEGASUS;
		public static ISprite SPR_REWARD_SEED_GALE;
		public static ISprite SPR_REWARD_SEED_MYSTERY;
		public static ISprite SPR_REWARD_HEART_PIECE;
		public static ISprite SPR_REWARD_HEART_CONTAINER;
		public static ISprite SPR_REWARD_HARP;
		public static ISprite SPR_REWARD_SMALL_KEY;
		public static ISprite SPR_REWARD_BOSS_KEY;
		public static ISprite SPR_REWARD_MAP;
		public static ISprite SPR_REWARD_COMPASS;
	
		// HUD Sprites
		public static ISprite SPR_HUD_BACKGROUND;
		public static ISprite SPR_HUD_BRACKET_LEFT;
		public static ISprite SPR_HUD_BRACKET_LEFT_A;
		public static ISprite SPR_HUD_BRACKET_LEFT_B;
		public static ISprite SPR_HUD_BRACKET_RIGHT;
		public static ISprite SPR_HUD_BRACKET_RIGHT_A;
		public static ISprite SPR_HUD_BRACKET_RIGHT_B;
		public static ISprite SPR_HUD_BRACKET_LEFT_RIGHT;
		public static ISprite SPR_HUD_HEART_0;
		public static ISprite SPR_HUD_HEART_1;
		public static ISprite SPR_HUD_HEART_2;
		public static ISprite SPR_HUD_HEART_3;
		public static ISprite SPR_HUD_HEART_4;
		public static ISprite SPR_HUD_RUPEE;
		public static ISprite SPR_HUD_ORE_CHUNK;
		public static ISprite SPR_HUD_KEY;
		public static ISprite SPR_HUD_X;
		public static ISprite SPR_HUD_LEVEL;
		public static ISprite SPR_HUD_TEXT_NEXT_ARROW;
		public static ISprite SPR_HUD_AMMO_SELECT_ARROW;
		public static ISprite SPR_HUD_AMMO_SELECT_BACKGROUND;
		public static ISprite SPR_HUD_AMMO_SELECT_BACKGROUND_FANCY;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_TOP_LEFT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_TOP_RIGHT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_BOTTOM_LEFT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_FULL_TOP_LEFT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_FULL_TOP_RIGHT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_FULL_BOTTOM_LEFT;
		public static ISprite SPR_HUD_MESSAGE_HEART_PIECES_FULL_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_TOP_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_TOP_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_TOP_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_TOP_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_BOTTOM_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_FANCY_TOP_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_FANCY_TOP_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_FANCY_BOTTOM_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_EMPTY_FANCY_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_FANCY_TOP_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_FANCY_TOP_RIGHT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_FANCY_BOTTOM_LEFT;
		public static ISprite SPR_HUD_HEART_PIECES_FULL_FANCY_BOTTOM_RIGHT;
		public static ISprite SPR_HUD_SAVE_BUTTON;
		public static ISprite[] SPR_HUD_HEARTS;
		public static ISprite[] SPR_HUD_MESSAGE_HEART_PIECES_EMPTY;
		public static ISprite[] SPR_HUD_MESSAGE_HEART_PIECES_FULL;
		public static ISprite[] SPR_HUD_HEART_PIECES_EMPTY;
		public static ISprite[] SPR_HUD_HEART_PIECES_FULL;
		public static ISprite SPR_UI_MAP_FLOOR_BOX;
		public static ISprite SPR_UI_MAP_PLAYER;
		public static ISprite SPR_UI_MAP_BOSS_ROOM;
		public static ISprite SPR_UI_MAP_TREASURE_ROOM;
		public static ISprite SPR_UI_MAP_BOSS_FLOOR;
		public static ISprite SPR_UI_MAP_UNDISCOVERED_ROOM;
		public static ISprite SPR_UI_MAP_FLOOR_BACKGROUND;
		public static ISprite SPR_UI_MAP_FLOOR_INDICATOR;
		public static ISprite SPR_UI_MAP_ARROW_DOWN;
		public static ISprite SPR_UI_MAP_ARROW_UP;
		public static ISprite SPR_UI_MAP_CURSOR;
		public static ISprite SPR_UI_MAP_ROOM_NONE;
		public static ISprite SPR_UI_MAP_ROOM_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_UP;
		public static ISprite SPR_UI_MAP_ROOM_UP_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_LEFT;
		public static ISprite SPR_UI_MAP_ROOM_LEFT_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_LEFT_UP;
		public static ISprite SPR_UI_MAP_ROOM_LEFT_UP_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_DOWN;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_UP;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_UP_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_LEFT;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_LEFT_RIGHT;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_LEFT_UP;
		public static ISprite SPR_UI_MAP_ROOM_DOWN_LEFT_UP_RIGHT;

		public static ISprite SPR_BACKGROUND_MENU_WEAPONS;
		public static ISprite SPR_BACKGROUND_MENU_KEY_ITEMS;
		public static ISprite SPR_BACKGROUND_MENU_ESSENCES;
		public static ISprite SPR_BACKGROUND_SCREEN_DUNGEON_MAP;

		// Action tiles
		public static ISprite SPR_ACTION_TILE_WARP_STAIRS;
		public static ISprite SPR_ACTION_TILE_WARP_TUNNEL;
		public static ISprite SPR_ACTION_TILE_WARP_ENTRANCE;
		public static ISprite SPR_ACTION_TILE_TRACK_INTERSECTION;

		public static ISprite SPR_TURNSTILE_BARS_CLOCKWISE;
		public static ISprite SPR_TURNSTILE_BARS_COUNTERCLOCKWISE;

		//-----------------------------------------------------------------------------
		// Sprite Loading
		//-----------------------------------------------------------------------------

		// Loads the sprites and sprite-sheets.
		private static void LoadSprites() {
			Resources.LoadSpriteSheets("Sprites/sprites.conscript");
			IntegrateResources<ISprite>("SPR_");

			// TEMPORARY: Create sprite arrays here.
			SPR_ITEM_SEEDS = new ISprite[] {
				SPR_ITEM_SEED_EMBER,
				SPR_ITEM_SEED_SCENT,
				SPR_ITEM_SEED_PEGASUS,
				SPR_ITEM_SEED_GALE,
				SPR_ITEM_SEED_MYSTERY
			};
			SPR_HUD_HEARTS = new ISprite[] {
				SPR_HUD_HEART_0,
				SPR_HUD_HEART_1,
				SPR_HUD_HEART_2,
				SPR_HUD_HEART_3,
				SPR_HUD_HEART_4,
			};
			SPR_HUD_MESSAGE_HEART_PIECES_EMPTY = new ISprite[] {
				SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_TOP_LEFT,
				SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_BOTTOM_LEFT,
				SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_BOTTOM_RIGHT,
				SPR_HUD_MESSAGE_HEART_PIECES_EMPTY_TOP_RIGHT
			};
			SPR_HUD_MESSAGE_HEART_PIECES_FULL = new ISprite[] {
				SPR_HUD_MESSAGE_HEART_PIECES_FULL_TOP_LEFT,
				SPR_HUD_MESSAGE_HEART_PIECES_FULL_BOTTOM_LEFT,
				SPR_HUD_MESSAGE_HEART_PIECES_FULL_BOTTOM_RIGHT,
				SPR_HUD_MESSAGE_HEART_PIECES_FULL_TOP_RIGHT
			};
			SPR_HUD_HEART_PIECES_EMPTY = new ISprite[] {
				SPR_HUD_HEART_PIECES_EMPTY_TOP_LEFT,
				SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_LEFT,
				SPR_HUD_HEART_PIECES_EMPTY_BOTTOM_RIGHT,
				SPR_HUD_HEART_PIECES_EMPTY_TOP_RIGHT
			};
			SPR_HUD_HEART_PIECES_FULL = new ISprite[] {
				SPR_HUD_HEART_PIECES_FULL_TOP_LEFT,
				SPR_HUD_HEART_PIECES_FULL_BOTTOM_LEFT,
				SPR_HUD_HEART_PIECES_FULL_BOTTOM_RIGHT,
				SPR_HUD_HEART_PIECES_FULL_TOP_RIGHT
			};

			SPR_COLOR_CUBE_ORIENTATIONS = new ISprite[6];
			string[] orientations = { "blue_yellow", "blue_red", "yellow_red", "yellow_blue", "red_blue", "red_yellow" };

			for (int i = 0; i < 6; i++) {
				SPR_COLOR_CUBE_ORIENTATIONS[i] = Resources.GetResource<ISprite>("color_cube_" + orientations[i]);
			}
		}
	}
}
