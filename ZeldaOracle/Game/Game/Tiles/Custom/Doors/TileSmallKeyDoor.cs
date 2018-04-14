﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileSmallKeyDoor : TileDoor {

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileSmallKeyDoor() {
			animationOpen	= GameData.ANIM_TILE_SMALL_KEY_DOOR_OPEN;
			animationClose	= GameData.ANIM_TILE_SMALL_KEY_DOOR_CLOSE;
			openCloseSound	= null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool OnPush(Direction direction, float movementSpeed) {
			Area dungeon = RoomControl.Area;

			if (dungeon.SmallKeyCount > 0) {
				dungeon.SmallKeyCount--;
				Open();

				AudioSystem.PlaySound(GameData.SOUND_CHEST_OPEN);
				AudioSystem.PlaySound(GameData.SOUND_GET_ITEM);

				// Spawn the key effect.
				EffectUsedItem effect = new EffectUsedItem(GameData.SPR_REWARD_SMALL_KEY);
				RoomControl.SpawnEntity(effect, Center);
				
				// Disable this tile forever.
				Properties.Set("enabled", false);
				
				// Unlock doors connected to this one in the adjacent room.
				TileDataInstance connectedDoor = GetConnectedDoor();
				if (connectedDoor != null) {
					connectedDoor.ModifiedProperties.Set("enabled", false);
				}
				
				return true;
			}
			else {
				GameControl.DisplayMessage("You need a <red>key<red> for this door!");
			}

			return false;
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Initializes the properties and events for the tile type.</summary>
		public new static void InitializeTileData(TileData data) {
			data.ResetCondition = TileResetCondition.Never;
		}
	}
}
