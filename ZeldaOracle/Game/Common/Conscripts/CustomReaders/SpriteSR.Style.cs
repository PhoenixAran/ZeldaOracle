﻿using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public partial class SpriteSR : ConscriptRunner {

		/// <summary>Adds StyleSprite commands to the script reader.</summary>
		public void AddStyleCommands() {

			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("STYLE", (int) Modes.Root,
				"string name, string styleGroup",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(1);

				spriteName = parameters.GetString(0);
				sprite = new StyleSprite(styleGroup);
				AddSprite(spriteName, sprite);
				Resources.RegisterStyleGroup(styleGroup, StyleSprite);

				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE STYLE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				spriteName = parameters.GetString(0);
				ContinueSprite<StyleSprite>(spriteName);
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// SETUP SpriteSet
			//=====================================================================================
			AddCommand("MULTIPLE STYLE", (int) Modes.SpriteSet,
				"string styleGroup, Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = parameters.GetPoint(2);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						StyleSprite styleSprite = new StyleSprite(styleGroup);
						editingSpriteSet.SetSprite(editingSetStart + point, styleSprite);
						if (point.IsZero)
							Resources.RegisterStyleGroup(styleGroup, styleSprite);
					}
				}
				
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE MULTIPLE STYLE", (int) Modes.SpriteSet,
				"Point start = (0, 0), Point span = (0, 0)",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = parameters.GetPoint(1);
				if (editingSetDimensions.X == 0) editingSetDimensions.X = editingSpriteSet.Width;
				if (editingSetDimensions.Y == 0) editingSetDimensions.Y = editingSpriteSet.Height;

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						GetSprite<StyleSprite>(editingSpriteSet, editingSetStart + new Point2I(x, y));
					}
				}
				
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("SINGLE STYLE", (int) Modes.SpriteSet,
				"string styleGroup, Point setIndex",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				editingSetStart = parameters.GetPoint(1);
				editingSetDimensions = Point2I.One;

				sprite = new StyleSprite(styleGroup);
				editingSpriteSet.SetSprite(editingSetStart, sprite);
				Resources.RegisterStyleGroup(styleGroup, StyleSprite);
				
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			AddCommand("CONTINUE SINGLE STYLE", (int) Modes.SpriteSet,
				"Point setIndex",
			delegate (CommandParam parameters) {
				editingSetStart = parameters.GetPoint(0);
				editingSetDimensions = Point2I.One;
				
				sprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart);
				
				Mode |= Modes.StyleSprite;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("ADD", (int) Modes.StyleSprite,
				"string style, Sprite sprite",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				ISprite styledSprite = GetSpriteFromParams(parameters, 1);
				StyleSprite.Add(style, styledSprite);
				Resources.RegisterStyle(StyleSprite.Group, style);
			});
			//=====================================================================================
			AddCommand("REPLACE", (int) Modes.StyleSprite,
				"string style, Sprite sprite",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				StyleSprite.Set(style, GetSpriteFromParams(parameters, 1));
			});
			//=====================================================================================
			/*AddCommand("REMOVE", (int) Modes.StyleSprite,
				"string style",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				StyleSprite.Remove(style);
			});*/
			//=====================================================================================
			// BUILDING SpriteSet
			//=====================================================================================
			AddCommand("ADD", (int) (Modes.SpriteSet |  Modes.StyleSprite),
				"string style, Point sourceIndex",
			delegate (CommandParam parameters) {
				string style = parameters.GetString(0);
				Point2I sourceIndex = parameters.GetPoint(1);

				for (int x = 0; x < editingSetDimensions.X; x++) {
					for (int y = 0; y < editingSetDimensions.Y; y++) {
						Point2I point = new Point2I(x, y);
						StyleSprite styleSprite = GetSprite<StyleSprite>(editingSpriteSet, editingSetStart + point);
						ISprite styledSprite = GetSprite(source, sourceIndex + point);
						styleSprite.Add(style, styledSprite);
						if (point.IsZero)
							Resources.RegisterStyle(styleSprite.Group, style);
					}
				}
			});
			//=====================================================================================
			// Style Preview
			//=====================================================================================
			AddCommand("STYLEPREVIEW",
				"string styleGroup, Sprite sprite",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				ISprite preview = GetSpriteFromParams(parameters, 1);
				Resources.SetStylePreview(styleGroup, preview);
			});
			//=====================================================================================
		}
	}
}
