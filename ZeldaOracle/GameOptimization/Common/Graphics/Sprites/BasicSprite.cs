﻿using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The base sprite that all sprites are composed of.</summary>
	public class BasicSprite : ISprite {

		/// <summary>The image used by the sprite.</summary>
		private Image image;
		/// <summary>The source rectangle of the sprite.</summary>
		private Rectangle2I sourceRect;
		/// <summary>The draw offset of the sprite.</summary>
		private Point2I drawOffset;
		/// <summary>The flipping applied to the sprite.</summary>
		private Flip flipEffects;
		/// <summary>The number of 90-degree rotations for the sprite.</summary>
		private Rotation rotation;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an unassigned basic sprite.</summary>
		public BasicSprite() {
			this.image			= null;
			this.sourceRect		= Rectangle2I.Zero;
			this.drawOffset		= Point2I.Zero;
			this.flipEffects	= Flip.None;
			this.rotation		= Rotation.None;
		}

		/// <summary>Constructs a basic sprite from a sprite sheet.</summary>
		public BasicSprite(SpriteSheet sheet, Point2I index, Flip flip = Flip.None,
			Rotation rotation = Rotation.None) :
			this(sheet, index, Point2I.Zero, flip)
		{
		}

		/// <summary>Constructs a basic sprite from a sprite sheet.</summary>
		public BasicSprite(SpriteSheet sheet, Point2I index, Point2I drawOffset,
			Flip flip = Flip.None, Rotation rotation = Rotation.None)
		{
			this.image			= sheet.Image;
			this.sourceRect		= sheet.GetSourceRect(index);
			this.drawOffset		= drawOffset;
			this.flipEffects	= flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs a basic sprite from an image and source rect.</summary>
		public BasicSprite(Image image, Rectangle2I sourceRect, Flip flip = Flip.None,
			Rotation rotation = Rotation.None) :
			this(image, sourceRect, Point2I.Zero, flip)
		{
		}

		/// <summary>Constructs a basic sprite from an image and source rect.</summary>
		public BasicSprite(Image image, Rectangle2I sourceRect, Point2I drawOffset,
			Flip flip = Flip.None, Rotation rotation = Rotation.None)
		{
			this.image			= image;
			this.sourceRect		= sourceRect;
			this.drawOffset		= drawOffset;
			this.flipEffects	= flip;
			this.rotation		= rotation;
		}

		/// <summary>Constructs a copy of the specified basic sprite.</summary>
		public BasicSprite(BasicSprite copy) {
			this.image			= copy.image;
			this.sourceRect		= copy.sourceRect;
			this.drawOffset		= copy.drawOffset;
			this.flipEffects	= copy.flipEffects;
			this.rotation		= copy.rotation;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			return new SpritePart(image, sourceRect, drawOffset/*, flipEffects, rotation*/);
		}

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		public IEnumerable<ISprite> GetAllSprites() {
			yield return this;
		}

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		public bool ContainsSubSprite(ISprite sprite) {
			return false;
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new BasicSprite(this);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
			return new Rectangle2I(drawOffset, sourceRect.Size);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return new Rectangle2I(drawOffset, sourceRect.Size); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the image used by the sprite.</summary>
		public Image Image {
			get { return image; }
			set { image = value; }
		}

		/// <summary>Gets or sets the source rectangle of the sprite.</summary>
		public Rectangle2I SourceRect {
			get { return sourceRect; }
			set { sourceRect = value; }
		}

		/// <summary>Gets or sets the draw offset of the sprite.</summary>
		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}

		/// <summary>Gets or sets the flipping applied to the sprite.</summary>
		public Flip FlipEffects {
			get { return flipEffects; }
			set { flipEffects = value; }
		}

		/// <summary>Gets or sets the number of 90-degree rotations for the sprite.</summary>
		public Rotation Rotation {
			get { return rotation; }
			set { rotation = value; }
		}
	}
}
