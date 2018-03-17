﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Tools;
using ZeldaEditor.Scripting;
using ZeldaOracle.Common.Scripting;
using ZeldaEditor.PropertiesEditor;

namespace ZeldaEditor.Control {

	public class EditorControl {
		
		private bool isInitialized;

		// Control
		private EditorForm			editorForm;
		private string				worldFilePath;
		private string				worldFileName;
		private World				world;
		private Level				level;
		private ITileset			tileset;
		private Zone				zone;
		private RewardManager		rewardManager;
		private Inventory			inventory;

		private Stopwatch			timer;
		private int					ticks;
		private bool				hasMadeChanges;

		// Settings
		private bool				playAnimations;
		private bool				eventMode;
		
		// Tools
		private List<EditorTool>	tools;
		private ToolPointer			toolPointer;
		private ToolPlace			toolPlace;
		private	ToolSquare			toolSquare;
		private ToolFill			toolFill;
		private ToolSelection		toolSelection;
		private ToolEyedrop			toolEyedrop;

		// Editing
		private int				roomSpacing;
		private int				currentLayer;
		private int				currentToolIndex;
		private TileDrawModes	aboveTileDrawMode;
		private TileDrawModes	belowTileDrawMode;
		private bool			showRewards;
		private bool			showGrid;
		private bool			highlightMouseTile;
		private Point2I			selectedRoom;
		private Point2I			selectedTilesetTile;
		private BaseTileData	selectedTilesetTileData;
		private bool			playerPlaceMode;
		private bool			showEvents;
		private bool						needsRecompiling;
		private Task<ScriptCompileResult>	compileTask;
		private ScriptCompileCallback		compileCallback;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public EditorControl(EditorForm editorForm) {
			this.editorForm		= editorForm;

			//this.propertyGridControl	= null;
			this.worldFilePath	= String.Empty;
			this.worldFileName	= "untitled";
			this.world			= null;
			this.level			= null;
			this.tileset		= null;
			this.zone			= null;
			this.rewardManager	= null;
			this.inventory		= null;
			this.timer			= null;
			this.ticks			= 0;
			this.roomSpacing	= 1;
			this.playAnimations	= false;
			this.isInitialized	= false;
			this.hasMadeChanges	= false;
			this.needsRecompiling	= false;
			this.compileTask		= null;
			this.compileCallback	= null;

			this.currentLayer				= 0;
			this.currentToolIndex			= 0;
			this.aboveTileDrawMode			= TileDrawModes.Fade;
			this.belowTileDrawMode			= TileDrawModes.Fade;
			this.showRewards				= true;
			this.showGrid					= false;
			this.showEvents					= false;
			this.highlightMouseTile			= true;
			this.selectedRoom				= -Point2I.One;
			this.selectedTilesetTile		= Point2I.Zero;
			this.selectedTilesetTileData	= null;
			this.playerPlaceMode			= false;
		}

		public void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice) {
			if (!isInitialized) {
				Resources.Initialize(contentManager, graphicsDevice);
				GameData.Initialize();
				EditorResources.Initialize();

				this.inventory		= new Inventory(null);
				this.rewardManager	= new RewardManager(null);
				this.timer			= Stopwatch.StartNew();
				this.ticks			= 0;
				this.roomSpacing	= 1;
				this.playAnimations = false;
				this.tileset		= GameData.TILESET_CLIFFS;
				this.zone			= GameData.ZONE_PRESENT;
				this.selectedTilesetTileData = this.tileset.GetTileData(0, 0);
				this.eventMode		= false;

				GameData.LoadInventory(inventory);
				GameData.LoadRewards(rewardManager);

				// Create tileset combo box.
				editorForm.ComboBoxTilesets.Items.Clear();
				foreach (KeyValuePair<string, TilesetOld> entry in Resources.GetDictionary<TilesetOld>()) {
					editorForm.ComboBoxTilesets.Items.Add(entry.Key);
				}
				foreach (KeyValuePair<string, EventTileset> entry in Resources.GetDictionary<EventTileset>()) {
					editorForm.ComboBoxTilesets.Items.Add(entry.Key);
				}
				editorForm.ComboBoxTilesets.SelectedIndex = 0;
				
				// Create zone combo box.
				editorForm.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key))
						editorForm.ComboBoxZones.Items.Add(entry.Key);
				}
				editorForm.ComboBoxZones.SelectedIndex = 0;

				// Create tools.
				tools = new List<EditorTool>();
				AddTool(toolPointer		= new ToolPointer());
				AddTool(toolPlace		= new ToolPlace());
				AddTool(toolSquare		= new ToolSquare());
				AddTool(toolFill		= new ToolFill());
				AddTool(toolSelection	= new ToolSelection());
				AddTool(toolEyedrop		= new ToolEyedrop());
				currentToolIndex = 0;
				tools[currentToolIndex].OnBegin();

				this.isInitialized = true;

				Application.Idle += delegate {
					Update();
				};
			}
		}

		//-----------------------------------------------------------------------------
		// General
		//-----------------------------------------------------------------------------

		public void UpdateWindowTitle() {
			editorForm.Text = "Oracle Engine Editor - " + worldFileName;
			if (hasMadeChanges)
				editorForm.Text += "*";
			if (level != null)
				editorForm.Text += " [" + level.Properties.GetString("id") + "]";
		}

		// Called with Application.Idle.
		private void Update() {
			if (compileTask != null) {
				if (compileTask.IsCompleted) {
					compileCallback(compileTask.Result);
					compileTask = null;
				}
			}
			else if (needsRecompiling) {
				needsRecompiling = false;

				CompileAllScripts(OnCompileCompleted);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Script Compiling
		//-----------------------------------------------------------------------------

		public delegate void ScriptCompileCallback(ScriptCompileResult result);

		public void CompileScript(Script script, ScriptCompileCallback callback) {
			this.compileCallback = callback;
			compileTask = ScriptEditorCompiler.CompileScriptAsync(script);
		}
		
		private void CompileAllScripts(ScriptCompileCallback callback) {
			this.compileCallback = callback;
			string code = world.ScriptManager.CreateCode(world, false);
			compileTask = Task.Run(() => world.ScriptManager.Compile(code));
			editorForm.statusLabelTask.Text = "Compiling scripts.";
		}
				
		private void OnCompileCompleted(ScriptCompileResult result) {
			world.ScriptManager.RawAssembly = result.RawAssembly;
			Console.WriteLine("Compiled scripts with " + result.Errors.Count + " errors and " + result.Warnings.Count + " warnings.");
			editorForm.statusLabelTask.Text = "";
		}


		//-----------------------------------------------------------------------------
		// World
		//-----------------------------------------------------------------------------

		// Save the world file to the given filename.
		public void SaveFileAs(string fileName) {
			if (IsWorldOpen) {
				WorldFile saveFile = new WorldFile();
				saveFile.Save(fileName, world, true);
				hasMadeChanges	= false;
				worldFilePath	= fileName;
				worldFileName	= Path.GetFileName(fileName);
			}
		}

		// Open a world file with the given filename.
		public void OpenFile(string fileName) {
			// Load the world.
			WorldFile worldFile = new WorldFile();
			World loadedWorld = worldFile.Load(fileName, true);

			// Verify the world was loaded successfully.
			if (loadedWorld != null) {
				CloseFile();

				hasMadeChanges		= false;
				worldFilePath		= fileName;
				worldFileName		= Path.GetFileName(fileName);
				needsRecompiling	= true;

				world = loadedWorld;
				if (world.LevelCount > 0)
					OpenLevel(0);

				RefreshWorldTreeView();
				editorForm.worldTreeView.ExpandAll();
			}
			else {
				// Display the error.
				MessageBox.Show(editorForm, "Failed to open world file:\n" +
					worldFile.ErrorMessage, "Error Opening World",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		// Close the world file.
		public void CloseFile() {
			if (IsWorldOpen) {
				PropertyGrid.CloseProperties();
				world			= null;
				level			= null;
				hasMadeChanges	= false;
				worldFilePath	= "";
				RefreshWorldTreeView();
			}
		}

		// Open the given level.
		public void OpenLevel(Level level) {
			this.level = level;
			editorForm.LevelDisplay.UpdateLevel();
			UpdateWindowTitle();
			PropertyGrid.OpenProperties(level.Properties, level);
		}

		// Open the given level index in the level display.
		public void OpenLevel(int index) {
			level = world.GetLevelAt(index);
			editorForm.LevelDisplay.UpdateLevel();
			UpdateWindowTitle();
			PropertyGrid.OpenProperties(level.Properties, level);
		}

		public void CloseLevel() {
			level = null;
			editorForm.LevelDisplay.UpdateLevel();
			UpdateWindowTitle();
			PropertyGrid.CloseProperties();
		}

		// Add a new level the world, and open it if specified.
		public void AddLevel(Level level, bool openLevel) {
			world.AddLevel(level);
			RefreshWorldTreeView();
			if (openLevel)
				OpenLevel(world.LevelCount - 1);
		}

		public void AddScript(Script script) {
			world.AddScript(script);
			RefreshWorldTreeView();
		}

		public void ChangeTileset(string name) {
			if (Resources.Contains<TilesetOld>(name))
				tileset = Resources.Get<TilesetOld>(name);
			else if (Resources.Contains<EventTileset>(name))
				tileset = Resources.Get<EventTileset>(name);
			
			if (tileset.SpriteSheet != null) {
				// Determine which zone to begin using for this tileset.
				int index = 0;
				if (!tileset.SpriteSheet.Image.HasVariant(zone.ID)) {
					zone = Resources.GetResource<Zone>(tileset.SpriteSheet.Image.VariantName);
					if (zone == null)
						zone = GameData.ZONE_DEFAULT;
				}

				// Setup zone combo box for the new tileset.
				editorForm.ComboBoxZones.Items.Clear();
				foreach (KeyValuePair<string, Zone> entry in Resources.GetDictionary<Zone>()) {
					if (tileset.SpriteSheet.Image.HasVariant(entry.Key)) {
						editorForm.ComboBoxZones.Items.Add(entry.Key);
						if (entry.Key == zone.ID)
							editorForm.ComboBoxZones.SelectedIndex = index;
						index++;
					}
				}
			}

			editorForm.TileDisplay.UpdateTileset();
			editorForm.TileDisplay.UpdateZone();
		}

		public void ChangeZone(string name) {
			if (name != "(none)") {
				zone = Resources.Get<Zone>(name);
				editorForm.TileDisplay.UpdateZone();
			}
		}

		// Test/play the world.
		public void TestWorld() {
			if (IsWorldOpen) {
				string worldPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "ZeldaOracle.exe");
				Process.Start(exePath, "\"" + worldPath + "\"");
			}
		}
		
		// Test/play the world with the player placed at the given room and point.
		public void TestWorld(Point2I roomCoord, Point2I playerCoord) {
			if (IsWorldOpen) {
				playerPlaceMode = false;
				int levelIndex = world.IndexOfLevel(level);
				string worldPath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "testing.zwd");
				WorldFile worldFile = new WorldFile();
				worldFile.Save(worldPath, world, true);
				string exePath = Path.Combine(Directory.GetParent(Application.ExecutablePath).FullName, "ZeldaOracle.exe");
				Process.Start(exePath, "\"" + worldPath + "\" -test " + levelIndex + " " + roomCoord.X + " " + roomCoord.Y + " " + playerCoord.X + " " + playerCoord.Y);
				// TODO: editorForm.ButtonTestPlayerPlace.Checked = false;
			}
		}

		public void RefreshWorldTreeView() {
			editorForm.worldTreeView.RefreshTree();
		}


		//-----------------------------------------------------------------------------
		// Tiles
		//-----------------------------------------------------------------------------

		// Open the properties for the given tile in the property grid.
		public void OpenObjectProperties(IPropertyObject propertyObject) {
			PropertyGrid.OpenProperties(propertyObject.Properties, propertyObject);
		}

		public void OnDeleteObject(IPropertyObject propertyObject) {
			// Remove any hidden scripts referenced in the tile's propreties.
			foreach (Property property in propertyObject.Properties.GetProperties()) {
				PropertyDocumentation doc = property.Documentation;

				// Remove hidden scripts referenced by this property.
				if (doc.EditorType == "script") {
					string scriptID = property.StringValue;
					Script script = world.GetScript(scriptID);
					if (script != null && script.IsHidden) {
						world.RemoveScript(script);
						Console.WriteLine("Removed hidden script: " + scriptID);
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Tools
		//-----------------------------------------------------------------------------
		
		// Change the current tool to the tool of the given index.
		public void ChangeTool(int toolIndex) {
			if (toolIndex != currentToolIndex) {
				tools[currentToolIndex].OnEnd();

				currentToolIndex = toolIndex;
				if (currentToolIndex != 0) {
					selectedRoom = -Point2I.One;
				}

				editorForm.OnToolChange(toolIndex);
				tools[currentToolIndex].OnBegin();
			}
		}
		
		// Add a new tool to the list of tools and initialize it.
		private EditorTool AddTool(EditorTool tool) {
			tool.Initialize(this);
			tools.Add(tool);
			return tool;
		}


		//-----------------------------------------------------------------------------
		// Scripts
		//-----------------------------------------------------------------------------

		public Script GenerateInternalScript() {
			return GenerateInternalScript(new Script());
		}

		public Script GenerateInternalScript(Script script) {
			script.IsHidden = true;

			int i = 0;
			do {
				script.ID = ScriptManager.CreateInternalScriptName(i);
				i++;
			}
			while (world.GetScript(script.ID) != null);

			world.AddScript(script);
			return script;
		}


		//-----------------------------------------------------------------------------
		// Ticks
		//-----------------------------------------------------------------------------

		// Update the elapsed ticks based on the total elapsed seconds.
		public void UpdateTicks() {
			double time = timer.Elapsed.TotalSeconds;
			if (playAnimations)
				ticks = (int)(time * 60.0);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorForm EditorForm {
			get { return editorForm; }
			set { editorForm = value; }
		}
		
		public ZeldaPropertyGrid PropertyGrid {
			get { return editorForm.PropertyGrid; }
		}
		
		public LevelDisplay LevelDisplay {
			get { return editorForm.LevelDisplay; }
		}

		public bool IsWorldOpen {
			get { return (world != null); }
		}

		public bool IsLevelOpen {
			get { return (world != null && level != null); }
		}

		public World World {
			get { return world; }
		}
		public Level Level {
			get { return level; }
		}

		public int RoomSpacing {
			get { return roomSpacing; }
			set { roomSpacing = value; }
		}

		public int Ticks {
			get { return ticks; }
		}

		public bool PlayAnimations {
			get { return playAnimations; }
			set { playAnimations = value; }
		}

		public ITileset Tileset {
			get { return tileset; }
		}

		public Zone Zone {
			get { return zone; }
		}

		public Point2I SelectedRoom {
			get { return selectedRoom; }
			set { selectedRoom = value; }
		}

		public Point2I SelectedTilesetTile {
			get { return selectedTilesetTile; }
			set { selectedTilesetTile = value; }
		}

		public BaseTileData SelectedTilesetTileData {
			get { return selectedTilesetTileData; }
			set {
				selectedTilesetTileData = value;
				editorForm.TileDisplay.Invalidate();
			}
		}

		public RewardManager RewardManager {
			get { return rewardManager; }
		}

		public Inventory Inventory {
			get { return inventory; }
		}

		public int CurrentLayer {
			get { return currentLayer; }
			set { currentLayer = GMath.Clamp(value, 0, 3); }
		}

		public int CurrentToolIndex {
			get { return currentToolIndex; }
			set { currentToolIndex = value; }
		}

		public EditorTool CurrentTool {
			get {
				if (tools == null)
					return null;
				return tools[currentToolIndex];
			}
		}

		public TileDrawModes AboveTileDrawMode {
			get { return aboveTileDrawMode; }
			set { aboveTileDrawMode = value; }
		}

		public TileDrawModes BelowTileDrawMode {
			get { return belowTileDrawMode; }
			set { belowTileDrawMode = value; }
		}

		public bool ShowRewards {
			get { return showRewards; }
			set { showRewards = value; }
		}

		public bool ShowGrid {
			get { return showGrid; }
			set { showGrid = value; }
		}

		public bool ShowEvents {
			get { return showEvents; }
			set { showEvents = value; }
		}
		
		public bool ShouldDrawEvents {
			get { return (showEvents || eventMode || (selectedTilesetTileData is ActionTileData) || (tileset is EventTileset)); }
		}

		public bool EventMode {
			get { return (eventMode || (selectedTilesetTileData is ActionTileData)); }
			set { eventMode = value; }
		}

		public bool HighlightMouseTile {
			get { return highlightMouseTile; }
			set { highlightMouseTile = value; }
		}

		public bool PlayerPlaceMode {
			get { return playerPlaceMode; }
			set { playerPlaceMode = value; }
		}

		public ToolPointer ToolPointer {
			get { return toolPointer; }
		}

		public ToolPlace ToolPlace {
			get { return toolPlace; }
		}

		public ToolSelection ToolSelection {
			get { return toolSelection; }
		}

		public ToolEyedrop ToolEyedrop {
			get { return toolEyedrop; }
		}

		public bool IsWorldFromFile {
			get { return (worldFilePath != String.Empty); }
		}

		public string WorldFilePath {
			get { return worldFilePath; }
		}

		public string WorldFileName {
			get { return worldFileName; }
		}

		public bool HasMadeChanges {
			get { return hasMadeChanges; }
		}

		public bool IsSelectedTileAnEvent {
			get { return (selectedTilesetTileData is ActionTileData); }
		}

		public bool NeedsRecompiling {
			get { return needsRecompiling; }
			set { needsRecompiling = value; }
		}

		public bool IsBusyCompiling {
			get { return (compileTask != null); }
		}
	}
}
