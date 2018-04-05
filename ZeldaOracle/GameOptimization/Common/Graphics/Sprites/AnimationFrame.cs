﻿using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>A single frame with a sprite and time for an animation.</summary>
	public class AnimationFrame {
		/// <summary>Start time in ticks.</summary>
		private int startTime;
		/// <summary>Duration in ticks.</summary>
		private int duration;
		/// <summary>The offset sprite containing the sprite used in the frame.</summary>
		private OffsetSprite sprite;
		/// <summary>The source of the sprite.</summary>
		private ISpriteSource source;
		/// <summary>The source index of the sprite.</summary>
		private Point2I sourceIndex;
		/// <summary>The definition used for the source.</summary>
		private string sourceDefinition;
		/// <summary>The depth of the animation frame.</summary>
		private int depth;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs an empty animation frame.</summary>
		public AnimationFrame() {
			startTime			= 0;
			duration			= 0;
			sprite				= new OffsetSprite();
			source				= null;
			sourceIndex			= Point2I.Zero;
			sourceDefinition	= null;
			depth				= 0;
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISprite sprite,
			Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None, int depth = 0)
			: this(startTime, duration, sprite, Point2I.Zero, clipping, flip,
				  rotation, depth)
		{
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISprite sprite,
			Point2I drawOffset, Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None, int depth = 0)
		{
			this.startTime		= startTime;
			this.duration		= duration;
			this.sprite			= new OffsetSprite(sprite, drawOffset, clipping, flip, rotation);
			source				= null;
			sourceIndex			= Point2I.Zero;
			sourceDefinition	= null;
			this.depth			= depth;
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISpriteSource source,
			Point2I sourceIndex, Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None, int depth = 0)
			: this(startTime, duration, source, sourceIndex, null, Point2I.Zero,
				  clipping, flip, rotation, depth)
		{
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISpriteSource source,
			Point2I sourceIndex, string sourceDefinition, Rectangle2I? clipping = null,
			Flip flip = Flip.None, Rotation rotation = Rotation.None, int depth = 0)
			: this(startTime, duration, source, sourceIndex, sourceDefinition,
				  Point2I.Zero, clipping, flip, rotation, depth)
		{
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISpriteSource source,
			Point2I sourceIndex, Point2I drawOffset, Rectangle2I? clipping = null,
			Flip flip = Flip.None, Rotation rotation = Rotation.None, int depth = 0)
			: this(startTime, duration, source, sourceIndex, null, drawOffset,
				  clipping, flip, rotation, depth)
		{
		}

		/// <summary>Constructs an animation frame with the specified settings.</summary>
		public AnimationFrame(int startTime, int duration, ISpriteSource source,
			Point2I sourceIndex, string sourceDefinition, Point2I drawOffset,
			Rectangle2I? clipping = null, Flip flip = Flip.None,
			Rotation rotation = Rotation.None, int depth = 0)
		{
			this.startTime		= startTime;
			this.duration		= duration;
			sprite				= new OffsetSprite(null, drawOffset, clipping,
										flip, rotation);
			this.source			= source;
			this.sourceIndex	= sourceIndex;
			this.sourceDefinition = sourceDefinition;
			this.depth			= depth;
			UpdateSource();
		}

		/// <summary>Constructs a copy of the specified animation frame.</summary>
		public AnimationFrame(AnimationFrame copy) {
			startTime			= copy.startTime;
			duration			= copy.duration;
			sprite				= new OffsetSprite(copy.sprite);
			source				= copy.source;
			sourceIndex			= copy.sourceIndex;
			sourceDefinition	= copy.sourceDefinition;
			depth				= copy.depth;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		/// <summary>Adds the clipping to the sprite and insersects with the previous
		/// clipping.</summary>
		public void Clip(Rectangle2I clipping) {
			sprite.Clip(clipping);
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		/// <summary>Updates the sprite from the source members.</summary>
		private void UpdateSource() {
			sprite.Sprite = source.GetSprite(sourceIndex);
			if (sourceDefinition != null)
				sprite.Sprite =
					((DefinitionSprite) sprite.Sprite).Get(sourceDefinition);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the start time in ticks.</summary>
		public int StartTime {
			get { return startTime; }
			set { startTime = value; }
		}

		/// <summary>Gets the end time in ticks.</summary>
		public int EndTime {
			get { return (startTime + duration); }
		}

		/// <summary>Gets or sets the duration time in ticks.</summary>
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		/// <summary>Gets the offset sprite containing this frame.</summary>
		public OffsetSprite OffsetSprite {
			get { return sprite; }
		}

		/// <summary>Gets or sets the sprite of the frame.</summary>
		public ISprite Sprite {
			get { return sprite.Sprite; }
			set {
				sprite.Sprite = value;
				source = null;
				sourceIndex = Point2I.Zero;
				sourceDefinition = null;
			}
		}

		/// <summary>Gets or sets the draw offset of the frame.</summary>
		public Point2I DrawOffset {
			get { return sprite.DrawOffset; }
			set { sprite.DrawOffset = value; }
		}

		/// <summary>Gets or sets the clipping of the sprite.</summary>
		public Rectangle2I? Clipping {
			get { return sprite.Clipping; }
			set { sprite.Clipping = value; }
		}

		/// <summary>Gets or sets the flipping applied to the frame.</summary>
		public Flip FlipEffects {
			get { return sprite.FlipEffects; }
			set { sprite.FlipEffects = value; }
		}

		/// <summary>Gets or sets the number of 90-degree rotations for the frame.</summary>
		public Rotation Rotation {
			get { return sprite.Rotation; }
			set { sprite.Rotation = value; }
		}

		/// <summary>Gets or sets the draw depth of the frame.</summary>
		public int Depth {
			get { return depth; }
			set { depth = value; }
		}

		/// <summary>Returns true if the animation frame has a sprite sheet source.</summary>
		public bool HasSource {
			get { return source != null; }
		}

		/// <summary>Gets or sets the source of the sprite.</summary>
		public ISpriteSource Source {
			get { return source; }
			set {
				source = value;
				if (source != null)
					UpdateSource();
			}
		}

		/// <summary>Gets or sets the source index of the sprite.</summary>
		public Point2I SourceIndex {
			get { return sourceIndex; }
			set {
				sourceIndex = value;
				if (source != null)
					UpdateSource();
			}
		}

		/// <summary>Gets or sets the source definition of the sprite.</summary>
		public string SourceDefinition {
			get { return sourceDefinition; }
			set {
				sourceDefinition = value;
				if (source != null)
					UpdateSource();
			}
		}

		/// <summary>Gets the sprite acting as the source that can be
		/// shifted or have definitions changed.</summary>
		public ISprite SourceSprite {
			get {
				if (source != null)
					return source.GetSprite(sourceIndex);
				return null;
			}
		}
	}
}
