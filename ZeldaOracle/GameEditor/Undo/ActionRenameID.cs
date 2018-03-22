﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.Undo {
	public class ActionRenameID : EditorAction {

		private IIDObject idObject;
		private string oldID;
		private string newID;

		private List<Property> idProperties;

		public ActionRenameID(IIDObject idObject, string newID) {
			ActionName = "Rename " + idObject.GetType().Name;
			ActionIcon = EditorImages.Rename;
			this.idObject = idObject;
			this.oldID = idObject.ID;
			this.newID = newID;
			this.idProperties = new List<Property>();
		}

		public override void Execute(EditorControl editorControl) {
			string editorType = null;
			if (idObject is Level)
				editorType = "level";
			else if (idObject is Area)
				editorType = "area";
			if (editorType != null) {
				foreach (Property property in
					editorControl.World.GetModifiedProperties())
				{
					if (property.EditorType == editorType &&
						property.StringValue == oldID)
						idProperties.Add(property);
				}
			}
			Redo(editorControl);
		}

		public override void Undo(EditorControl editorControl) {
			if (idObject is Level) {
				editorControl.World.RenameLevel((Level)idObject, oldID);
				editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			}
			else if (idObject is Area) {
				editorControl.World.RenameArea((Area)idObject, oldID);
				editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			}
			else if (idObject is Script) {
				editorControl.World.RenameScript((Script)idObject, oldID);
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
				editorControl.ScriptRenamed(newID, oldID);
			}
			else if (idObject is World) {
				editorControl.World.ID = oldID;
				editorControl.EditorWindow.WorldTreeView.RefreshWorld();
			}
			bool propertyGridNeedsUpdating = false;
			IPropertyObject current = editorControl.PropertyGrid.PropertyObject;
			foreach (Property property in idProperties) {
				property.StringValue = oldID;
				if (!propertyGridNeedsUpdating &&
					current == property.Properties.PropertyObject)
				{
					propertyGridNeedsUpdating = true;
				}
			}
			if (propertyGridNeedsUpdating)
				editorControl.PropertyGrid.RefreshProperties();
		}

		public override void Redo(EditorControl editorControl) {
			if (idObject is Level) {
				editorControl.World.RenameLevel((Level) idObject, newID);
				editorControl.EditorWindow.WorldTreeView.RefreshLevels();
			}
			else if (idObject is Area) {
				editorControl.World.RenameArea((Area) idObject, newID);
				editorControl.EditorWindow.WorldTreeView.RefreshAreas();
			}
			else if (idObject is Script) {
				editorControl.World.RenameScript((Script) idObject, newID);
				editorControl.EditorWindow.WorldTreeView.RefreshScripts(true, false);
				editorControl.ScriptRenamed(oldID, newID);
			}
			else if (idObject is World) {
				editorControl.World.ID = newID;
				editorControl.EditorWindow.WorldTreeView.RefreshWorld();
			}
			bool propertyGridNeedsUpdating = false;
			IPropertyObject current = editorControl.PropertyGrid.PropertyObject;
			foreach (Property property in idProperties) {
				property.StringValue = newID;
				if (!propertyGridNeedsUpdating &&
					current == property.Properties.PropertyObject) {
					propertyGridNeedsUpdating = true;
				}
			}
			if (propertyGridNeedsUpdating)
				editorControl.PropertyGrid.RefreshProperties();
		}

		public override bool IgnoreAction { get { return newID == oldID; } }
	}
}
