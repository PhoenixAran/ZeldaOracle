﻿using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities {

	[Flags]
	public enum NPCFlags {
		AlwaysFacePlayer	= 0x1,
		FacePlayerWhenNear	= 0x2,		// Face player when he is nearby.
		FacePlayerOnTalk	= 0x4,		// Face player when talked to.
		AnimateOnTalk		= 0x8,		// Animate when a different room event is playing.
		OnlyFaceHorizontal	= 0x10,		// Only face horizontally (left or right).
		OnlyFaceVertical	= 0x20,		// Only face vertically (up or down).

		Default				= FacePlayerOnTalk | FacePlayerWhenNear,
	};

	public class NPC : Unit {

		/// <summary>The direction to face when not near the player.</summary>
		private Direction defaultDirection;
		/// <summary>The radius of the diamond shape of tiles where the NPC will face
		/// the player (excluding the center tile).</summary>
		private int sightDistance;
		/// <summary>The default animation to play.</summary>
		private ISprite animationDefault;
		/// <summary>The animation to play when talking.</summary>
		private ISprite animationTalk;
		/// <summary>The message to display when talked.</summary>
		private Message message;

		private NPCFlags flags;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public NPC() {
			// Graphics
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;
			Graphics.DepthLayer				= DepthLayer.PlayerAndNPCs;
			Graphics.DrawOffset				= new Point2I(-8, -14);
			centerOffset					= Graphics.DrawOffset + new Point2I(8, 8);

			// Physics
			Physics.Enable(PhysicsFlags.HasGravity);
			//Physics.Enable(
			//	PhysicsFlags.Solid |
			//	PhysicsFlags.HasGravity);
			Physics.CollisionBox = new Rectangle2F(-8, -11, 16, 13);

			// Interactions
			Interactions.Enable();
			Interactions.InteractionBox = new Rectangle2F(-10, -15, 20, 19);
			Reactions[InteractionType.ButtonAction]
				.Set(Talk).Add(EntityReactions.TriggerButtonReaction);
			
			// General
			actionAlignDistance	= 5;
			flags = NPCFlags.FacePlayerOnTalk |
				NPCFlags.FacePlayerWhenNear;
			message = null;
			animationTalk = null;

			// Bounding box for talking is 4 pixels beyond the hard collision box (inclusive).
			// Alignment limit is a max 5 pixels in either direction (inclusive).
		}
		

		//-----------------------------------------------------------------------------
		// Reactions
		//-----------------------------------------------------------------------------

		public void Talk(Entity actionEntity, EventArgs args) {
			if (message != null) {
				if (animationTalk != null) {
					Graphics.PlayAnimation(animationTalk);
				}
				if (flags.HasFlag(NPCFlags.FacePlayerOnTalk)) {
					if (actionEntity is Player)
						direction = ((Player) actionEntity).Direction.Reverse();
					Graphics.SubStripIndex = direction;
				}
				GameControl.DisplayMessage(message, null, delegate() {
					if (animationTalk != null)
						Graphics.PlayAnimation(animationDefault);
				});
			}
		}
		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			Graphics.PlayAnimation(animationDefault);

			sightDistance	= 2;
			direction = Properties.Get<int>("direction", defaultDirection);

			Graphics.IsAnimatedWhenPaused	= flags.HasFlag(NPCFlags.AnimateOnTalk);
			Graphics.SubStripIndex			= direction;
		}

		public override void Update() {
			/*
			direction = defaultDirection;

			bool facePlayer = flags.HasFlag(NPCFlags.AlwaysFacePlayer);

			// Check if the player is nearby
			if (!facePlayer && flags.HasFlag(NPCFlags.FacePlayerWhenNear)) {
				Point2I a = RoomControl.GetTileLocation(Center);
				Point2I b = RoomControl.GetTileLocation(RoomControl.Player.Center);
				if (GMath.Abs(a.X - b.X) + GMath.Abs(a.Y - b.Y) <= sightDistance)
					facePlayer = true;
			}

			if (facePlayer) {
				if (flags.HasFlag(NPCFlags.OnlyFaceHorizontal)) {
					// Face the player horizontally
					direction = Direction.Right;
					if (RoomControl.Player.Center.X < Center.X)
						direction = Direction.Left;
				}
				else if (flags.HasFlag(NPCFlags.OnlyFaceVertical)) {
					// Face the player vertically
					direction = Direction.Down;
					if (RoomControl.Player.Center.Y < Center.Y)
						direction = Direction.Up;
				}
				else {
					// Face the player in all directions.
					Vector2F lookVector = RoomControl.Player.Center - Center;
					direction = Direction.FromVector(lookVector);
				}
			}
			*/
			
			Graphics.SubStripIndex = direction;

			base.Update();
		}



		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The animation to play when being talked to.</summary>
		public NPCFlags Flags {
			get { return flags; }
			set { flags = value; }
		}

		/// <summary>The direction to face when not near the player.</summary>
		//public Direction Direction {
		//	get { return defaultDirection; }
		//	set { defaultDirection = value; }
		//}

		/// <summary>The message to display when talking.</summary>
		public Message Message {
			get { return message; }
			set { message = value; }
		}

		/// <summary>The default animation to play.</summary>
		public ISprite DefaultAnimation {
			get { return animationDefault; }
			set { animationDefault = value; }
		}

		/// <summary>The animation to play when talking.</summary>
		public ISprite TalkAnimation {
			get { return animationTalk; }
			set { animationTalk = value; }
		}
	}
}
