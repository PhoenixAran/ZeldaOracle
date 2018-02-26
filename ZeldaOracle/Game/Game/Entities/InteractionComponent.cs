﻿using System;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities {
	
	public class InteractionComponent : EntityComponent {

		//private Rectangle2F[] interactionBoxes;
		//private Direction direction;
		private Rectangle2F interactionBox;

		/// <summary>The interaction manager.</summary>
		private InteractionEventManager interactionManager;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InteractionComponent(Entity entity) :
			base(entity)
		{
			interactionBox		= Rectangle2F.Zero;
			interactionManager	= null;
		}


		//-----------------------------------------------------------------------------
		// Configuration
		//-----------------------------------------------------------------------------
		
		public void EnableInteractionCallbacks() {
			if (interactionManager == null)
				interactionManager = new InteractionEventManager(entity);
		}

		// Get the interaction handler for the given interaction type.
		public InteractionHandler GetInteraction(InteractionType type) {
			if (interactionManager == null)
				return null;
			return interactionManager[type];
		}

		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		public void SetReaction(InteractionType type,
			params InteractionStaticDelegate[] reactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, reactions);
		}

		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		public void SetReaction(InteractionType type,
			params InteractionMemberDelegate[] reactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, reactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction, memberReactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction1, staticReaction2, memberReactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			InteractionStaticDelegate staticReaction3,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction1, staticReaction2,
				staticReaction3, memberReactions);
		}

		

		//-----------------------------------------------------------------------------
		// Interaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger an interaction.</summary>
		public void Trigger(InteractionType type, Entity sender) {
			if (interactionManager != null)
				interactionManager.Trigger(type, sender);
		}

		/// <summary>Trigger an interaction with the given arguments.</summary>
		public void Trigger(InteractionType type, Entity sender, EventArgs args) {
			if (interactionManager != null)
				interactionManager.Trigger(type, sender, args);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public InteractionEventManager InteractionManager {
			get { return interactionManager; }
		}
	}
}
