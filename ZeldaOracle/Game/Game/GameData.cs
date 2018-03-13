﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Scripts.CustomReaders;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles.ActionTiles;
using System.Diagnostics;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Common.Util;

namespace ZeldaOracle.Game {
	
	// A static class for storing links to all game content.
	public partial class GameData {
		
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initializes and loads the game content. NOTE: The order here is important.
		public static void Initialize(bool preloadSprites = true, RewardManager rewardManager = null) {
			/*
			CommandReferenceParam param = CommandParamParser.ParseReferenceParams(
				"(any objs...)...");
				//"any name, (int gridLocationX, int gridLocationY), (int drawOffsetX, any drawOffsetY) = (0, fuck)");
				//"float x, string y, bool width, (int a, int b) = (1, 2), float height, (int hoop) = (0), bool asd");
				//"float x, string y, bool width, (int a, int b, (string c)), float height");

			Console.WriteLine(CommandParamParser.ToString(param));
			throw new LoadContentException("END");
			*/

			Logs.InitializeLogs();

			Stopwatch watch = Stopwatch.StartNew();
			Stopwatch audioWatch = new Stopwatch();
			Stopwatch spriteWatch = new Stopwatch();
			ScriptReader.Watch.Restart();

			if (preloadSprites &&
				Resources.PalettedSpriteDatabase.DatabaseFileExists())
				Resources.PalettedSpriteDatabase.Load();

			Logs.Initialization.LogNotice("Loading Palette Dictionaries");
			LoadPaletteDictionaries();

			Logs.Initialization.LogNotice("Loading Palettes");
			LoadPalettes();

			Logs.Initialization.LogNotice("Loading Shaders");
			LoadShaders();

			Logs.Initialization.LogNotice("Pre-Loading Zones");
			LoadZonesPreTileData();

			Logs.Initialization.LogNotice("Loading Images");
			LoadImages();

			spriteWatch.Start();

			Logs.Initialization.LogNotice("Loading Sprites");
			LoadSprites();

			spriteWatch.Stop();

			Logs.Initialization.LogNotice("Loading Animations");
			LoadAnimations();

			Logs.Initialization.LogNotice("Loading Collision Models");
			LoadCollisionModels();
			
			Logs.Initialization.LogNotice("Loading Fonts");
			LoadFonts();

			audioWatch.Start();

			Logs.Initialization.LogNotice("Loading Sound Effects");
			LoadSounds();

			Logs.Initialization.LogNotice("Loading Music");
			LoadMusic();

			audioWatch.Stop();

			// CONSCRIPT DESIGNER ONLY
			if (rewardManager != null) {
				Logs.Initialization.LogNotice("Loading Rewards");
				LoadRewards(rewardManager);
			}

			Logs.Initialization.LogNotice("Loading Tiles");
			LoadTiles();

			Logs.Initialization.LogNotice("Loading Tilesets");
			LoadTilesets();

			Logs.Initialization.LogNotice("Loading Zones");
			LoadZonesPostTileData();

			//Logs.Initialization.LogNotice("Took " + spriteWatch.ElapsedMilliseconds + "ms to load sprites.");
			//Logs.Initialization.LogNotice("Took " + audioWatch.ElapsedMilliseconds + "ms to load audio.");
			if (rewardManager == null) {
				Logs.Initialization.LogInfo("Took {0} ms to parse conscripts.",
					ScriptReader.Watch.ElapsedMilliseconds);
				Logs.Initialization.LogInfo("Took {0} ms to load game data.",
					watch.ElapsedMilliseconds);
			}

			if (!Resources.PalettedSpriteDatabase.IsPreloaded) {
				watch.Restart();
				Resources.PalettedSpriteDatabase.Save();

				if (rewardManager == null)
					Logs.Initialization.LogInfo(
						"Took {0} ms to save sprite database.",
						watch.ElapsedMilliseconds);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		// Assign static fields from their corresponding loaded resources.
		private static void IntegrateResources<T>(string prefix) {
			IEnumerable<FieldInfo> fields = typeof(GameData).GetFields()
				.Where(field =>
					field.Name.StartsWith(prefix) &&
					field.FieldType == typeof(T) &&
					field.IsStatic);

			// Set the values of the static fields to their corresponding loaded resources.
			foreach (FieldInfo field in fields) {
				string name = field.Name.ToLower().Remove(0, prefix.Length);
				
				if (Resources.ContainsResource<T>(name)) {
					field.SetValue(null, Resources.GetResource<T>(name));
				}
				else if (field.GetValue(null) != null) {
					//Console.WriteLine("** WARNING: " + name + " is built programatically.");
				}
				else {
					//Console.WriteLine("** WARNING: " + name + " is never defined.");
				}
			}
			
			// Loop through resource dictionary.
			// Find any resources that don't have corresponding fields in GameData.
			Dictionary<string, T> dictionary = Resources.GetResourceDictionary<T>();
			foreach (KeyValuePair<string, T> entry in dictionary) {
				string name = prefix.ToLower() + entry.Key;
				FieldInfo matchingField = fields.FirstOrDefault(
					field => string.Compare(field.Name, name, true) == 0);
				
				if (matchingField == null) {
					//Console.WriteLine("** WARNING: Resource \"" + name + "\" does not have a corresponding field.");
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Image Loading
		//-----------------------------------------------------------------------------

		// Loads the images.
		private static void LoadImages() {
			Resources.LoadImagesFromScript("Images/images.conscript");
		}


		//-----------------------------------------------------------------------------
		// Collision Model Loading
		//-----------------------------------------------------------------------------

		private static void LoadCollisionModels() {
			Resources.LoadCollisionModels("Data/collision_models.conscript");
			IntegrateResources<CollisionModel>("MODEL_");

			// Leap ledges' models must be constant to prevent any bugs
			MODEL_LEAP_LEDGE_RIGHT	= new CollisionModel(new Rectangle2I(12, 0, 4, 16));
			MODEL_LEAP_LEDGE_UP		= new CollisionModel(new Rectangle2I(0, 0, 16, 4));
			MODEL_LEAP_LEDGE_LEFT	= new CollisionModel(new Rectangle2I(0, 0, 4, 16));
			MODEL_LEAP_LEDGE_DOWN	= new CollisionModel(new Rectangle2I(0, 12, 16, 4));
			MODEL_LEAP_LEDGES = new CollisionModel[Direction.Count] {
				MODEL_LEAP_LEDGE_RIGHT,
				MODEL_LEAP_LEDGE_UP,
				MODEL_LEAP_LEDGE_LEFT,
				MODEL_LEAP_LEDGE_DOWN
			};
		}


		//-----------------------------------------------------------------------------
		// Zone Loading
		//-----------------------------------------------------------------------------

		private static void LoadZonesPreTileData() {
			Resources.LoadZones("Zones/zones.conscript", false);
			IntegrateResources<Zone>("ZONE_");
		}

		private static void LoadZonesPostTileData() {
			Resources.LoadZones("Zones/zones.conscript", true);
			IntegrateResources<Zone>("ZONE_");
		}


		//-----------------------------------------------------------------------------
		// Tliesets Loading
		//-----------------------------------------------------------------------------

		private static void LoadTilesets() {
			// Load tilesets and tile data.
			Resources.LoadTilesets("Tilesets/tilesets.conscript");

			IntegrateResources<Tileset>("TILESET_");
		}

		private static void LoadTiles() {
			Resources.LoadTiles("Tiles/tiles.conscript");
		}


		//-----------------------------------------------------------------------------
		// Font Loading
		//-----------------------------------------------------------------------------

		// Loads the fonts.
		private static void LoadFonts() {
			Resources.LoadGameFonts("Fonts/fonts.conscript");

			IntegrateResources<GameFont>("FONT_");
		}

		//-----------------------------------------------------------------------------
		// Palette Loading
		//-----------------------------------------------------------------------------

		// Loads the palette dictionaries.
		private static void LoadPaletteDictionaries() {
			Resources.LoadPaletteDictionaries("Palettes/Dictionaries/palette_dictionaries.conscript");

			IntegrateResources<PaletteDictionary>("PAL_");
		}

		// Loads the palettes.
		private static void LoadPalettes() {
			Resources.LoadPalettes("Palettes/palettes.conscript");

			// Menu palette is made programatically as it's just a 16 unit offset (Maxes at 248)
			Palette entitiesMenu = new Palette(Resources.GetResource<Palette>("entities_default"));
			foreach (var pair in entitiesMenu.GetDefinedConsts()) {
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					if (pair.Value[i].IsUndefined)
						continue;
					Color color = pair.Value[i].Color;
					color.R = (byte) GMath.Min(248, color.R + 16);
					color.G = (byte) GMath.Min(248, color.G + 16);
					color.B = (byte) GMath.Min(248, color.B + 16);
					pair.Value[i].Color = color;
				}
			}
			foreach (var pair in entitiesMenu.GetDefinedColors()) {
				for (int i = 0; i < PaletteDictionary.ColorGroupSize; i++) {
					if (pair.Value[i].IsUndefined)
						continue;
					Color color = pair.Value[i].Color;
					color.R = (byte) GMath.Min(248, color.R + 16);
					color.G = (byte) GMath.Min(248, color.G + 16);
					color.B = (byte) GMath.Min(248, color.B + 16);
					pair.Value[i].Color = color;
				}
			}
			entitiesMenu.UpdatePalette();
			Resources.AddResource<Palette>("entities_menu", entitiesMenu);

			IntegrateResources<Palette>("PAL_");
			
			// Map monster colors to color definitions
			MONSTER_COLOR_DEFINITION_MAP = new string[(int) MonsterColor.Count];
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Red]			= "red";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Blue]			= "blue";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Orange]			= "orange";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Green]			= "green";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.DarkBlue]		= "shaded_blue";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.DarkRed]		= "shaded_red";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.Gold]			= "gold";
			MONSTER_COLOR_DEFINITION_MAP[(int) MonsterColor.InverseBlue]	= "inverse_blue";
		}

		//-----------------------------------------------------------------------------
		// Shader Loading
		//-----------------------------------------------------------------------------

		// Loads the shaders.
		private static void LoadShaders() {
			PALETTE_SHADER		= Resources.LoadShader("Shaders/palette_shader");
			PALETTE_LERP_SHADER	= Resources.LoadShader("Shaders/palette_lerp_shader");
			
			GameSettings.DRAW_MODE_DEFAULT.Effect = PALETTE_LERP_SHADER;

			PaletteShader = new PaletteShader(PALETTE_LERP_SHADER);
			PaletteShader.TilePalette = PAL_TILES_DEFAULT;
			PaletteShader.EntityPalette = PAL_ENTITIES_DEFAULT;
		}

		public static Effect PALETTE_SHADER;
		public static Effect PALETTE_LERP_SHADER;


		//-----------------------------------------------------------------------------
		// Collision Models
		//-----------------------------------------------------------------------------

		public static CollisionModel MODEL_EMPTY;
		public static CollisionModel MODEL_BLOCK;
		public static CollisionModel MODEL_EDGE_E;
		public static CollisionModel MODEL_EDGE_N;
		public static CollisionModel MODEL_EDGE_W;
		public static CollisionModel MODEL_EDGE_S;
		public static CollisionModel MODEL_DOORWAY;
		public static CollisionModel MODEL_DOORWAY_HALF_RIGHT;
		public static CollisionModel MODEL_DOORWAY_HALF_LEFT;
		public static CollisionModel MODEL_CORNER_NE;
		public static CollisionModel MODEL_CORNER_NW;
		public static CollisionModel MODEL_CORNER_SW;
		public static CollisionModel MODEL_CORNER_SE;
		public static CollisionModel MODEL_INSIDE_CORNER_NE;
		public static CollisionModel MODEL_INSIDE_CORNER_NW;
		public static CollisionModel MODEL_INSIDE_CORNER_SW;
		public static CollisionModel MODEL_INSIDE_CORNER_SE;
		public static CollisionModel MODEL_BRIDGE_H_TOP;
		public static CollisionModel MODEL_BRIDGE_H_BOTTOM;
		public static CollisionModel MODEL_BRIDGE_H;
		public static CollisionModel MODEL_BRIDGE_V_LEFT;
		public static CollisionModel MODEL_BRIDGE_V_RIGHT;
		public static CollisionModel MODEL_BRIDGE_V;
		public static CollisionModel MODEL_CENTER;

		public static CollisionModel MODEL_LEAP_LEDGE_RIGHT;
		public static CollisionModel MODEL_LEAP_LEDGE_UP;
		public static CollisionModel MODEL_LEAP_LEDGE_LEFT;
		public static CollisionModel MODEL_LEAP_LEDGE_DOWN;
		public static CollisionModel[] MODEL_LEAP_LEDGES;


		//-----------------------------------------------------------------------------
		// Zones
		//-----------------------------------------------------------------------------

		public static Zone ZONE_DEFAULT;


		//-----------------------------------------------------------------------------
		// Fonts
		//-----------------------------------------------------------------------------

		public static GameFont FONT_LARGE;
		public static GameFont FONT_SMALL;


		//-----------------------------------------------------------------------------
		// Palettes
		//-----------------------------------------------------------------------------
		
		public static PaletteDictionary PAL_TILE_DICTIONARY;
		public static PaletteDictionary PAL_ENTITY_DICTIONARY;
		
		public static Palette PAL_ENTITIES_DEFAULT;
		public static Palette PAL_ENTITIES_MENU;
		public static Palette PAL_ENTITIES_ELECTROCUTED;

		public static Palette PAL_TILES_DEFAULT;
		public static Palette PAL_TILES_ELECTROCUTED;

		public static Palette PAL_DUNGEON_MAP_DEFAULT;
		public static Palette PAL_MENU_DEFAULT;

		public static string[] MONSTER_COLOR_DEFINITION_MAP;


		//-----------------------------------------------------------------------------
		// Render Targets
		//-----------------------------------------------------------------------------

		public static PaletteShader PaletteShader;

		public static RenderTarget2D RenderTargetGame;
		public static RenderTarget2D RenderTargetGameTemp;
		public static RenderTarget2D RenderTargetDebug;
	}
}
