﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Game.Main.ResourceBuilders
{
	public class AnimationBuilder {
		private Animation animation;
		private SpriteSheet sheet;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public AnimationBuilder() {
			animation = null;
			sheet = null;
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

		public Animation End()
		{
			Animation temp = animation;
			animation = null;
			return temp;
		}

		//-----------------------------------------------------------------------------
		// Building
		//-----------------------------------------------------------------------------

		public AnimationBuilder InsertFrameStrip(int time, int duration, int sheetX, int sheetY, int length, int offsetX = 0, int offsetY = 0, int relX = 1, int relY = 0) {
			for (int i = 0; i < length; ++i)
				InsertFrame(time + (duration * i), duration, sheetX + (i * relX), sheetY + (i * relY), offsetX, offsetY);
			return this;
		}

		public AnimationBuilder AddFrameStrip(int duration, int sheetX, int sheetY, int length, int offsetX = 0, int offsetY = 0, int relX = 1, int relY = 0) {
			return InsertFrameStrip(animation.Duration, duration, sheetX, sheetY, length, offsetX, offsetY, relX, relY);
		}

		public AnimationBuilder AddFrame(int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			Sprite spr = new Sprite(sheet, sheetX, sheetY, offsetX, offsetY);
			return AddFrame(duration, spr);
		}

		public AnimationBuilder AddFrame(int duration, Sprite sprite) {
			return InsertFrame(animation.Duration, duration, sprite);
		}

		public AnimationBuilder AddPart(int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return AddPart(new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder AddPart(Sprite sprite) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.Frames[animation.Frames.Count - 1];
			return InsertFrame(prevFrame.StartTime, prevFrame.Duration, sprite);
		}

		public AnimationBuilder AddPart(int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return AddPart(duration, new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder AddPart(int duration, Sprite sprite) {
			//assert(m_strip->getNumFrames() > 0);
			AnimationFrame prevFrame = animation.Frames[animation.Frames.Count - 1];
			return InsertFrame(prevFrame.StartTime, duration, sprite);
		}

		public AnimationBuilder InsertFrame(int time, int duration, int sheetX, int sheetY, int offsetX = 0, int offsetY = 0) {
			return InsertFrame(time, duration, new Sprite(sheet, sheetX, sheetY, offsetX, offsetY));
		}

		public AnimationBuilder InsertFrame(int time, int duration, Sprite sprite) {
			animation.AddFrame(time, duration, sprite);
			return this;
		}

		public AnimationBuilder AddDelay(int duration) {
			animation.Duration += duration;
			return this;
		}

		public AnimationBuilder CreateSubStrip() {
			animation.NextStrip = new Animation();
			animation = animation.NextStrip;
			return this;
		}

		
		//-----------------------------------------------------------------------------
		// Midifications
		//-----------------------------------------------------------------------------

		public AnimationBuilder SetDuration(int duration) {
			animation.Duration = duration;
			return this;
		}

		public AnimationBuilder SetLoops(int numLoops) {
			animation.LoopCount = numLoops;
			return this;
		}

		public AnimationBuilder SetLooped(bool isLooped) {
			animation.IsLooped = isLooped;
			return this;
		}

		public AnimationBuilder SetSheet(SpriteSheet sheet) {
			this.sheet = sheet;
			return this;
		}

		public AnimationBuilder Offset(int x, int y) {
			for (int i = 0; i < animation.Frames.Count; i++)
				animation.Frames[i].Sprite.DrawOffset += new Point2I(x, y);
			return this;
		}

		public AnimationBuilder MakeQuad() {
			int numFrames = animation.Frames.Count;
			AnimationFrame[] frames = new AnimationFrame[numFrames];

			for (int i = 0; i < numFrames; ++i)
				frames[i] = animation.Frames[i];

			for (int i = 0; i < numFrames; ++i) {
				for (int x = 0; x < 2; ++x) {
					for (int y = 0; y < 2; ++y) {
						if (x > 0 || y > 0) {
							frames[i].Sprite.DrawOffset = new Point2I(8 * x, 8 * y);
							animation.AddFrame(frames[i]);
						}
					}
				}
			}
			return this;
		}

		public AnimationBuilder MakeDynamic(int numSubStrips, int offsetX, int offsetY) {
			Animation subStrip = animation;
			Point2I offset = new Point2I(offsetX, offsetY);

			for (int i = 1; i < numSubStrips; i++) {
				subStrip.NextStrip = new Animation();
				subStrip = subStrip.NextStrip;
				subStrip.LoopCount = animation.LoopCount;

				for (int j = 0; j < animation.Frames.Count; j++) {
					AnimationFrame frame = new AnimationFrame(animation.Frames[j]);
					frame.Sprite.SourceRect = new Rectangle2I(
						frame.Sprite.SourceRect.Point + (i * ((sheet.CellSize + sheet.Spacing) * offset)),
						frame.Sprite.SourceRect.Size
					);
					subStrip.AddFrame(frame);
				}
			}
			animation = subStrip;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public SpriteSheet SpriteSheet {
			get { return sheet; }
			set { sheet = value; }
		}
		
		public Animation Animation {
			get { return animation; }
			set { animation = value; }
		}

	}
}