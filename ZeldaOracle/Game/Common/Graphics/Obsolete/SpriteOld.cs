﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {

	// A sprite representing a portion of an image
	public class SpriteOld {

		// The image used by the sprite.
		private Image image;
		// The source rectangle of the sprite.
		private Rectangle2I sourceRect;
		// The draw offset of the sprite.
		private Point2I drawOffset;

		// For compound sprites (made up of multiple sub-sprites).
		private SpriteOld nextPart;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public SpriteOld() {
			this.image			= null;
			this.sourceRect		= Rectangle2I.Zero;
			this.drawOffset		= Point2I.Zero;
			this.nextPart		= null;
		}

		public SpriteOld(SpriteSheet sheet, int indexX, int indexY) :
			this(sheet, new Point2I(indexX, indexY), Point2I.Zero)
		{
		}

		public SpriteOld(SpriteSheet sheet, int indexX, int indexY, int drawOffsetX, int drawOffsetY) :
			this(sheet, new Point2I(indexX, indexY), new Point2I(drawOffsetX, drawOffsetY))
		{
		}

		public SpriteOld(SpriteSheet sheet, Point2I index) :
			this(sheet, index, Point2I.Zero)
		{
		}

		public SpriteOld(SpriteSheet sheet, Point2I index, Point2I drawOffset) {
			this.image			= sheet.Image;
			this.sourceRect		= new Rectangle2I(
				sheet.Offset.X + (index.X * (sheet.CellSize.X + sheet.Spacing.X)),
				sheet.Offset.Y + (index.Y * (sheet.CellSize.Y + sheet.Spacing.Y)),
				sheet.CellSize.X,
				sheet.CellSize.Y
			);
			this.drawOffset		= new Point2I(drawOffset.X, drawOffset.Y);
			this.image			= sheet.Image;
			this.nextPart		= null;
		}



		public SpriteOld(Image image, int sourceX, int sourceY, int sourceWidth, int sourceHeight) :
			this(image, new Rectangle2I(sourceX, sourceY, sourceWidth, sourceHeight), Point2I.Zero)
		{
		}

		public SpriteOld(Image image, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int drawOffsetX, int drawOffsetY) :
			this(image, new Rectangle2I(sourceX, sourceY, sourceWidth, sourceHeight), Point2I.Zero)
		{
		}

		public SpriteOld(Image image, Rectangle2I sourceRect) :
			this(image, sourceRect, Point2I.Zero)
		{
		}

		public SpriteOld(Image image, Rectangle2I sourceRect, Point2I drawOffset) {
			this.image			= image;
			this.sourceRect		= sourceRect;
			this.drawOffset		= drawOffset;
			this.nextPart		= null;
		}

		public SpriteOld(SpriteOld copy) {
			this.image			= copy.image;
			this.sourceRect		= copy.sourceRect;
			this.drawOffset		= copy.drawOffset;
			this.nextPart		= null;
			if (copy.nextPart != null)
				this.nextPart	= new SpriteOld(copy.nextPart); // This is recursive.
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		public void Set(SpriteOld copy) {
			this.image		= copy.image;
			this.sourceRect	= copy.sourceRect;
			this.drawOffset	= copy.drawOffset;
			this.nextPart	= null;
			if (copy.nextPart != null) {
				// This is recursive.
				if (this.nextPart == null)
					this.nextPart = new SpriteOld(copy.nextPart); 
				else
					this.nextPart.Set(copy.nextPart);
			}
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets the image used by the sprite.
		public Image Image {
			get { return image; }
			set { image = value; }
		}
		
		// Gets the source rectangle of the sprite.
		public Rectangle2I SourceRect {
			get { return sourceRect; }
			set { sourceRect = value; }
		}
		
		// Gets the draw offset of the sprite.
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}
		
		// Gets the next part of the sprite.
		public SpriteOld NextPart {
			get { return nextPart; }
			set { nextPart = value; }
		}
	}
}