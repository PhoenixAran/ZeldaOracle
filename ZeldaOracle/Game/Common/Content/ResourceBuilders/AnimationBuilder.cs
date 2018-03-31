﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Common.Content.ResourceBuilders {

	public class AnimationBuilder {
		
		private Animation			animation;
		private SpritePaletteArgs	paletteArgs;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public AnimationBuilder() {
			animation = null;
		}
		


		//-----------------------------------------------------------------------------
		// Begin/End
		//-----------------------------------------------------------------------------

		public AnimationBuilder Begin(Animation animation) {
			this.animation = animation;
			return this;
		}

		public AnimationBuilder Begin() {
			animation = new Animation();
			return this;
		}

		public AnimationBuilder BeginNull() {
			animation = null;
			return this;
		}

		public Animation End() {
			Animation temp = animation;
			animation = null;
			return temp;
		}

		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		public AnimationBuilder InsertFrameStrip(int time, int duration, ISpriteSource source,
			Point2I index, string definition, int length, Point2I drawOffset, Rectangle2I? clipping,
			Flip flip, Rotation rotation, int depth, Point2I relative)
		{
			//if (relative.IsZero)
			//	relative = new Point2I(1, 0);
			for (int i = 0; i < length; ++i) {
				InsertFrame(time + (duration * i), duration, source, index + (i * relative),
					definition, drawOffset, clipping, flip, rotation, depth);
			}
			return this;
		}

		public AnimationBuilder AddFrameStrip(int duration, ISpriteSource source, Point2I index,
			string definition, int length, Point2I drawOffset, Rectangle2I? clipping, Flip flip,
			Rotation rotation, int depth, Point2I relative)
		{
			return InsertFrameStrip(animation.Duration, duration, source, index, definition, length,
				drawOffset, clipping, flip, rotation, depth, relative);
		}

		public AnimationBuilder AddFrame(int duration, ISpriteSource source, Point2I index,
			string definition, Point2I drawOffset, Rectangle2I? clipping, Flip flip, Rotation rotation, int depth)
		{
			return InsertFrame(animation.Duration, duration, source, index, definition, drawOffset, clipping,
				flip, rotation, depth);
		}

		public AnimationBuilder AddFrame(int duration, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping,
			Flip flip, Rotation rotation, int depth)
		{
			return InsertFrame(animation.Duration, duration, sprite, drawOffset, clipping, flip, rotation, depth);
		}
		
		public AnimationBuilder AddPart(int duration, ISpriteSource source, Point2I index, string definition,
			Point2I drawOffset, Rectangle2I? clipping, Flip flip, Rotation rotation, int depth)
		{
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, source, index, definition, drawOffset, clipping, flip, rotation, depth);
		}

		public AnimationBuilder AddPart(int duration, ISprite sprite, Point2I drawOffset, Rectangle2I? clipping,
			Flip flip, Rotation rotation, int depth)
		{
			AnimationFrame prevFrame = animation.LastFrame();
			return InsertFrame(prevFrame.StartTime, duration, sprite, drawOffset, clipping, flip, rotation, depth);
		}
		
		public AnimationBuilder AddStatic(ISpriteSource source, Point2I index, string definition,
			Point2I drawOffset, Rectangle2I? clipping, Flip flip, Rotation rotation, int depth)
		{
			return InsertFrame(0, animation.Duration, source, index, definition, drawOffset, clipping, flip, rotation, depth);
		}

		public AnimationBuilder AddStatic(ISprite sprite, Point2I drawOffset, Rectangle2I? clipping, Flip flip,
			Rotation rotation, int depth)
		{
			return InsertFrame(0, animation.Duration, sprite, drawOffset, clipping, flip, rotation, depth);
		}

		public AnimationBuilder AddEmptyFrame(int duration) {
			return InsertFrame(animation.Duration, duration, new EmptySprite(), Point2I.Zero, null, Flip.None, Rotation.None, 0);
		}

		public AnimationBuilder InsertFrame(int time, int duration, ISpriteSource source, Point2I index,
			string definition, Point2I drawOffset, Rectangle2I? clipping, Flip flip, Rotation rotation, int depth)
		{
			if (paletteArgs.Dictionary != null && source is SpriteSheet) {
				SpriteSheet spriteSheet = (SpriteSheet) source;
				paletteArgs.Image = spriteSheet.Image;
				paletteArgs.SourceRect = spriteSheet.GetSourceRect(index);
				ISprite sprite = Resources.SpriteDatabase.AddSprite(paletteArgs);
				return InsertFrame(time, duration, sprite, drawOffset, clipping, flip, rotation, depth);
			}
			else {
				animation.AddFrame(time, duration, source, index, definition, drawOffset, clipping, flip, rotation, depth);
				return this;
			}
		}

		public AnimationBuilder InsertFrame(int time, int duration, ISprite sprite, Point2I drawOffset,
			Rectangle2I? clipping, Flip flip, Rotation rotation, int depth)
		{
			animation.AddFrame(time, duration, sprite, drawOffset, clipping, Flip.None, Rotation.None, depth);
			return this;
		}

		public AnimationBuilder AddFrame(AnimationFrame frame) {
			animation.AddFrame(frame);
			return this;
		}

		public AnimationBuilder AddDelay(int duration) {
			animation.Duration += duration;
			return this;
		}

		public AnimationBuilder CreateSubStrip() {
			if (animation == null) {
				animation = new Animation();
			}
			else if (animation.FrameCount > 0 || animation.Duration > 0) {
				animation.NextStrip = new Animation();
				animation = animation.NextStrip;
			}
			return this;
		}

		public AnimationBuilder Combine(Animation combineAnim, int substrip, int timeOffset, Point2I drawOffset,
			Rectangle2I? clipping, int depthOffset)
		{
			combineAnim = combineAnim.GetSubstrip(substrip);
			foreach (AnimationFrame frame in combineAnim.GetFrames()) {
				AnimationFrame newFrame = new AnimationFrame(frame);
				newFrame.Depth      += depthOffset;
				newFrame.DrawOffset += drawOffset;
				newFrame.StartTime  += timeOffset;
				if (clipping.HasValue)
					newFrame.Clip(clipping.Value);
				animation.AddFrame(newFrame);
			}
			return this;
		}

		
		//-----------------------------------------------------------------------------
		// Midifications
		//-----------------------------------------------------------------------------

		public AnimationBuilder RepeatPreviousFrames(int numFrames, int numRepeats) {
			int start = animation.FrameCount - numFrames;
			for (int i = 0; i < numRepeats; i++) {
				for (int j = 0; j < numFrames; j++) {
					AnimationFrame frame = new AnimationFrame(animation.GetFrameAt(start + j));
					frame.StartTime = animation.Duration;
					animation.AddFrame(frame);
				}
			}
			return this;
		}

		public AnimationBuilder SetDuration(int duration) {
			animation.Duration = duration;
			return this;
		}

		public AnimationBuilder SetLoopMode(LoopMode loopMode) {
			animation.LoopMode = loopMode;
			return this;
		}

		public AnimationBuilder Offset(Point2I offset) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++)
					anim.GetFrameAt(i).DrawOffset += offset;
			}
			return this;
		}

		public AnimationBuilder ShiftSourcePositions(Point2I offset) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++)
					anim.GetFrameAt(i).SourceIndex += offset;
			}
			return this;
		}

		public AnimationBuilder ChangeStyle(string oldStyle, string style, bool all) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++) {
					AnimationFrame frame = anim.GetFrameAt(i);
					if ((all || frame.SourceDefinition == oldStyle) && frame.SourceSprite is StyleSprite)
						frame.SourceDefinition = style;
				}
			}
			return this;
		}

		public AnimationBuilder ChangeColor(string oldColor, string color, bool all) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++) {
					AnimationFrame frame = anim.GetFrameAt(i);
					if ((all || frame.SourceDefinition == oldColor) && frame.SourceSprite is ColorSprite)
						frame.SourceDefinition = color;
				}
			}
			return this;
		}

		public AnimationBuilder MakeQuad() {
			int numFrames = animation.FrameCount;
			AnimationFrame[] frames = new AnimationFrame[numFrames];

			for (int i = 0; i < numFrames; ++i)
				frames[i] = new AnimationFrame(animation.GetFrameAt(i));

			for (int i = 0; i < numFrames; ++i) {
				for (int x = 0; x < 2; ++x) {
					for (int y = 0; y < 2; ++y) {
						if (x > 0 || y > 0) {
							frames[i].DrawOffset = new Point2I(8 * x, 8 * y);
							animation.AddFrame(new AnimationFrame(frames[i]));
						}
					}
				}
			}
			return this;
		}
		
		public AnimationBuilder MakeFlicker(int alternateDelayTicks) {

			Animation newAnimation = new Animation();

			for (int i = 0; i < animation.FrameCount; i++)  {
				AnimationFrame frame = animation.GetFrameAt(i);
				
				int beginSection	= frame.StartTime / (alternateDelayTicks * 2);
				int endSection		= frame.EndTime / (alternateDelayTicks * 2);
				if (frame.EndTime % (alternateDelayTicks * 2) == 0)
					endSection--;

				for (int section = beginSection; section <= endSection; section++) {
					int t = section * alternateDelayTicks * 2;

					if (frame.StartTime < t + alternateDelayTicks && frame.EndTime > t) {
						AnimationFrame newFrame = new AnimationFrame(frame);
						//newFrame.Sprite		= frame.Sprite;
						newFrame.StartTime	= GMath.Max(frame.StartTime, t);
						newFrame.Duration	= GMath.Min(frame.EndTime, t + alternateDelayTicks) - newFrame.StartTime;
						newAnimation.AddFrame(newFrame);
					}
				}
			}

			animation.ClearFrames();
			animation.AddFrameRange(newAnimation.GetFrames());
			return this;
		}

		public AnimationBuilder CreateClone(Animation reference) {
			animation = new Animation(reference);
			return this;
		}

		public AnimationBuilder MakeDynamic(int numSubStrips, Point2I offset) {
			Animation subStrip = animation;

			for (int i = 1; i < numSubStrips; i++) {
				subStrip.NextStrip = new Animation();
				subStrip = subStrip.NextStrip;
				subStrip.LoopMode = animation.LoopMode;

				for (int j = 0; j < animation.FrameCount; j++) {
					AnimationFrame frame = new AnimationFrame(animation.GetFrameAt(j));
					if (frame.HasSource) {
						frame.SourceIndex += offset * i;
					}
					subStrip.AddFrame(frame);
				}
			}
			animation = subStrip;
			return this;
		}

		public AnimationBuilder Clip(Rectangle2I clipping) {
			for (Animation anim = animation; anim != null; anim = anim.NextStrip) {
				for (int i = 0; i < anim.FrameCount; i++)
					anim.GetFrameAt(i).Clip(clipping);
			}
			return this;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}

		public SpritePaletteArgs PaletteArgs {
			get { return paletteArgs; }
			set { paletteArgs = value; }
		}
	}
}
