﻿using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control;

namespace ZeldaOracle.Game.Entities {

	/// <summary>An instance of an interaction occurring between two entities for a
	/// specific interaction type. This class is created and managed by the Room
	/// Control's Interaction Manager.</summary>
	public class InteractionInstance {

		private InteractionType type;
		private Entity actionEntity;
		private Entity reactionEntity;
		private EventArgs arguments;
		private HitBox actionBox;
		private HitBox reactionBox;
		private int duration;
		private bool isProtected;
		private bool stayAlive;
		private bool autoDetected;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InteractionInstance() {
			duration = 0;
			arguments = null;
			isProtected = false;
			autoDetected = true;
		}


		//-----------------------------------------------------------------------------
		// Queries
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if this is still a valid interaction.</summary>
		public bool IsValid() {
			return (reactionEntity.IsAliveAndInRoom &&
				reactionEntity.Interactions.IsEnabled &&
				(!AutoDetected ||
					(actionEntity.Interactions.InteractionType == type &&
					actionEntity.IsAliveAndInRoom &&
					actionEntity.Interactions.IsEnabled)));
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The type interaction of interaction.</summary>
		public InteractionType Type {
			get { return type; }
			set { type = value; }
		}

		/// <summary>The entity causing the interaction.</summary>
		public Entity ActionEntity {
			get { return actionEntity; }
			set { actionEntity = value; }
		}

		/// <summary>The entity reacting to the interaction.</summary>
		public Entity ReactionEntity {
			get { return reactionEntity; }
			set { reactionEntity = value; }
		}

		/// <summary>The interaction-box for the action entity.</summary>
		public HitBox ActionBox {
			get { return actionBox; }
			set { actionBox = value; }
		}

		/// <summary>The interaction-box for the reaction entity.</summary>
		public HitBox ReactionBox {
			get { return reactionBox; }
			set { reactionBox = value; }
		}

		/// <summary>The interaction event arguments sent by the action entity.
		/// </summary>
		public EventArgs Arguments {
			get { return arguments; }
			set { arguments = value; }
		}

		/// <summary>The duration in ticks that this interaction instance has been
		/// occuring.</summary>
		public int Duration {
			get { return duration; }
			set { duration = value; }
		}

		/// <summary>Used by the InteractionManager to check which interactions should
		/// be removed.</summary>
		public bool StayAlive {
			get { return stayAlive; }
			set { stayAlive = value; }
		}

		/// <summary>Used by the InteractionManager to check if this interaction is
		/// being protected by another interaction.</summary>
		public bool IsProtected {
			get { return isProtected; }
			set { isProtected = value; }
		}
		
		/// <summary>True if this interaction was automatically detected by the
		/// Interaction Manager. If this is false, then an entity must have called one
		/// of the InteractionManger's Trigger methods.</summary>
		public bool AutoDetected {
			get { return autoDetected; }
			set { autoDetected = value; }
		}

		/// <summary>Get a reference to the Room Control.</summary>
		public RoomControl RoomControl {
			get { return actionEntity.RoomControl; }
		}
	}
}
