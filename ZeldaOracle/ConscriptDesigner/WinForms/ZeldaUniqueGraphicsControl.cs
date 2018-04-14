﻿using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ConscriptDesigner.Control;
using ZeldaWpf.WinForms;

namespace ConscriptDesigner.WinForms {

	public class ZeldaUniqueGraphicsDeviceControl : TimersGraphicsDeviceControl {

		// Static
		private static RenderTarget renderTarget;
		private static Zone defaultZone;

		protected int columns;

		// Hover
		protected Point2I mouse;
		protected Point2I hoverPoint;
		private Point2I hoverSize;
		private Point2I trueHoverPoint;

		private bool isInitialized;

		private bool needsToInvalidate;

		private int scale;

		private Rectangle2I sourceRect;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaUniqueGraphicsDeviceControl() {
			this.isInitialized = false;
			this.columns = 1;
			this.mouse = -Point2I.One;
			this.hoverPoint = -Point2I.One;
			this.trueHoverPoint = -Point2I.One;
			this.needsToInvalidate = false;
			this.scale = 1;
			sourceRect = new Rectangle2I(1, 1);

			this.ResizeRedraw = true;

			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;
			MouseWheel += OnMouseWheel;
			PreviewReset += OnPreviewReset;
			PostReset += OnPostReset;
		}

		protected override void Initialize() {
			if (renderTarget == null)
				renderTarget = new RenderTarget(Point2I.One, false);
			if (defaultZone == null)
				defaultZone = new Zone();

			isInitialized = true;

			ContinuousEvents.StartRender(TimerUpdate);
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler HoverChanged;
		public event EventHandler ScaleChanged;


		//-----------------------------------------------------------------------------
		// WndProc Override
		//-----------------------------------------------------------------------------

		protected override void WndProc(ref Message m) {
			Point2I scrollPositionBefore = ScrollPosition;
			base.WndProc(ref m);
			// 0x115 and 0x20a both tell the control to scroll. If either one comes 
			// through, you can handle the scrolling before any repaints take place
			if (ModifierKeys.HasFlag(Keys.Control) && (m.Msg == 0x115 || m.Msg == 0x20a)) {
				ScrollPosition = scrollPositionBefore;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mouse = (ScrollPosition + new Point2I(e.X, e.Y)) / scale;
			UpdateHoverSprite();
		}

		private void OnMouseLeave(object sender, EventArgs e) {
			mouse = -Point2I.One;
			UpdateHoverSprite();
		}

		private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (ModifierKeys.HasFlag(Keys.Control)) {
				if (e.Delta > 0 && scale < 3)
					UpdateScale(scale + 1);
				else if (e.Delta < 0 && scale > 1)
					UpdateScale(scale - 1);
			}
		}

		private void OnPreviewReset(object sender, EventArgs e) {
			/*if (isInitialized) {

			}*/
		}

		private void OnPostReset(object sender, EventArgs e) {
			if (isInitialized) {
				Point2I newSize = GMath.Max(renderTarget.Size, ClientSize);
				renderTarget.Resize(newSize);
				ScrollPosition = Point2I.Zero;
			}
		}


		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------

		protected virtual void TimerUpdate() {
			if (DesignerControl.IsActive) {
				if (DesignerControl.PlayAnimations || needsToInvalidate) {
					needsToInvalidate = false;
					Invalidate();
				}
			}
		}

		protected virtual bool IsValidHoverPoint(ref Point2I point, out Point2I hoverSize) {
			hoverSize = Point2I.Zero;
			return false;
		}

		protected virtual void UpdateHoverSprite() {
			int column = mouse.X / (BaseSpriteSize.X + 1);
			int row = mouse.Y / (BaseSpriteSize.Y + 1);
			Point2I point = new Point2I(column, row);
			Point2I newHoverPoint = -Point2I.One;
			if (mouse >= Point2I.Zero && column < columns && IsValidHoverPoint(ref point, out hoverSize)) {
				newHoverPoint = point;
			}
			if (point != trueHoverPoint) {
				if (newHoverPoint != hoverPoint) {
					hoverPoint = newHoverPoint;
					if (HoverChanged != null)
						HoverChanged(this, EventArgs.Empty);
				}
				if (mouse >= Point2I.Zero)
					trueHoverPoint = point;
				else
					trueHoverPoint = -Point2I.One;
				if (!DesignerControl.PlayAnimations)
					Invalidate();
			}
		}

		protected virtual void UpdateHeight() {
			columns = 1;
		}

		protected virtual void Draw(Graphics2D g, SpriteSettings settings, Zone zone) { }

		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		protected void UpdateSize(Point2I newSize) {
			newSize *= scale;
			sourceRect.Size = newSize;
			ScrollSize = newSize;
			UpdateHoverSprite();
			needsToInvalidate = true;
		}

		public void UpdateScale(int scale) {
			this.scale = scale;
			UpdateHeight();
			if (ScaleChanged != null)
				ScaleChanged(this, EventArgs.Empty);
		}


		//-----------------------------------------------------------------------------
		// Drawing
		//-----------------------------------------------------------------------------

		protected sealed override void Draw() {
			if (!Resources.IsInitialized) return;
			SpriteSettings settings = new SpriteSettings(DesignerControl.PlaybackTime);
			Zone zone = DesignerControl.PreviewZone ?? defaultZone;
			Graphics2D g = new Graphics2D();

			if (GameData.SHADER_PALETTE != null && !GameData.SHADER_PALETTE.Effect.IsDisposed) {
				//GameData.PaletteShader.EntityPalette = GameData.PAL_ENTITIES_DEFAULT;
				//GameData.PaletteShader.TilePalette = GameData.PAL_TILES_DEFAULT;
				//if (zone.Palette != null)
				//	GameData.PaletteShader.TilePalette = zone.Palette;
				GameData.SHADER_PALETTE.TilePalette = DesignerControl.PreviewTilePalette;
				GameData.SHADER_PALETTE.EntityPalette = DesignerControl.PreviewEntityPalette;
				GameData.SHADER_PALETTE.ApplyParameters();
			}
			else {
				g.Clear(BackgroundColor);
				return;
			}

			if (zone != null) {
				settings.Styles = zone.StyleDefinitions;
			}

			g.Clear(BackgroundColor);
			//if (scale > 1) {
			g.SetRenderTarget(renderTarget);
			//}
			g.Clear(Color.White);
			g.PushTranslation(-ScrollPosition / scale);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);

			Draw(g, settings, zone);

			if (hoverPoint != -Point2I.One) {
				Rectangle2I selectRect = new Rectangle2I(
					hoverPoint * (BaseSpriteSize + 1),
					hoverSize * BaseSpriteSize + 2);
				g.DrawRectangle(selectRect, 1, Color.Black);
				g.DrawRectangle(selectRect.Inflated(1, 1), 1, Color.White);
				g.DrawRectangle(selectRect.Inflated(2, 2), 1, Color.Black);
			}

			g.End();
			g.PopTranslation();
			//if (scale > 1) {
				g.SetRenderTarget(null);
			g.Clear(BackgroundColor);
			g.Begin(GameSettings.DRAW_MODE_PALLETE);
				g.DrawImage(renderTarget, Vector2F.Zero, sourceRect, scale);
				g.End();
			//}
		}


		//-----------------------------------------------------------------------------
		// Virtual Properties
		//-----------------------------------------------------------------------------

		protected virtual Point2I BaseSpriteSize {
			get { return new Point2I(GameSettings.TILE_SIZE); }
		}

		protected virtual Color BackgroundColor {
			get { return Color.White; }
		}

		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		protected Point2I SpriteSize {
			get { return BaseSpriteSize * scale; }
		}

		protected Point2I SpriteSpacing {
			get { return (BaseSpriteSize + 1) * scale; }
		}

		protected Point2I SpriteOffset {
			get { return new Point2I(scale); }
		}

		/*public Point2I ScrollPosition {
			get { return new Point2I(HorizontalScroll.Value, VerticalScroll.Value); }
			set {
				AutoScrollPosition = new System.Drawing.Point(
					GMath.Clamp(value.X, HorizontalScroll.Minimum, HorizontalScroll.Maximum),
					GMath.Clamp(value.Y, VerticalScroll.Minimum, VerticalScroll.Maximum)
				);
			}
		}*/

		public Point2I UnscaledScrollPosition {
			get { return ScrollPosition / scale; }
		}

		public Point2I UnscaledClientSize {
			get { return ClientSize / scale; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I HoverPoint {
			get { return hoverPoint; }
		}

		public Point2I TrueHoverPoint {
			get { return trueHoverPoint; }
		}

		public new int Scale {
			get { return scale; }
		}
	}
}
