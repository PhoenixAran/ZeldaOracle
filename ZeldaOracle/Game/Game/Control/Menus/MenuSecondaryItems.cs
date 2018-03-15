﻿using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Control.Menus {

	public class MenuSecondaryItems : InventoryMenu {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MenuSecondaryItems(GameManager gameManager)
			: base(gameManager)
		{
			//this.backgroundSprite = Resources.GetImage("UI/menu_key_items_a");
			this.background = GameData.SPR_BACKGROUND_MENU_KEY_ITEMS;

			SlotGroup group = new SlotGroup();
			currentSlotGroup = group;
			this.slotGroups.Add(group);
			Slot[,] slots = new Slot[5, 4];
			Slot ringBagSlot = null;


			Point2I gridSize = new Point2I(5, 4);

			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					if (y == gridSize.Y - 1) {
						if (x == 0)
							ringBagSlot = group.AddSlot(new Point2I(12, 80), 16);
						slots[x, y] = group.AddSlot(new Point2I(32 + 24 * x, 80), 16);
					}
					else {
						slots[x, y] = group.AddSlot(new Point2I(24 + 24 * x, 8 + 24 * y), (x == (gridSize.X - 1) ? 24 : 16));
						group.GetSlotAt(group.NumSlots - 1);
					}
				}
			}
			for (int y = 0; y < gridSize.Y; y++) {
				for (int x = 0; x < gridSize.X; x++) {
					if (x == 0 && y == gridSize.Y - 1)
						slots[x, y].SetConnection(Direction.Left, ringBagSlot);
					else if (x == 0)
						slots[x, y].SetConnection(Direction.Left, slots[gridSize.X - 1, (y + gridSize.Y - 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Left, slots[x - 1, y]);

					if (x == gridSize.X - 1 && y == gridSize.Y - 2)
						slots[x, y].SetConnection(Direction.Right, ringBagSlot);
					else if (x == gridSize.X - 1)
						slots[x, y].SetConnection(Direction.Right, slots[0, (y + 1) % gridSize.Y]);
					else
						slots[x, y].SetConnection(Direction.Right, slots[x + 1, y]);

					slots[x, y].SetConnection(Direction.Up, slots[x, (y + gridSize.Y - 1) % gridSize.Y]);
					slots[x, y].SetConnection(Direction.Down, slots[x, (y + 1) % gridSize.Y]);
				}
			}

			ringBagSlot.SetConnection(Direction.Left, slots[gridSize.X - 1, gridSize.Y - 2]);
			ringBagSlot.SetConnection(Direction.Right, slots[0, gridSize.Y - 1]);
			ringBagSlot.SetConnection(Direction.Up, slots[0, gridSize.Y - 2]);
			ringBagSlot.SetConnection(Direction.Down, slots[0, 0]);
		}
		

		//-----------------------------------------------------------------------------
		// Item Management
		//-----------------------------------------------------------------------------

		public void AddItem(ItemSecondary secondaryItem) {
			GetSecondarySlotAt(secondaryItem.SecondarySlot).SlotItem = secondaryItem;
		}

		public void RemoveItem(ItemSecondary secondaryItem) {
			GetSecondarySlotAt(secondaryItem.SecondarySlot).SlotItem = null;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Update() {
			base.Update();

			// Equip equipment.
			if (Controls.A.IsPressed()) {
				if (currentSlotGroup.CurrentSlotIndex >= currentSlotGroup.NumSlots - 6) {
					AudioSystem.PlaySound(GameData.SOUND_MENU_SELECT);
					ItemEquipment selectedItem = currentSlotGroup.CurrentSlot.SlotItem as ItemEquipment;
					GameControl.Inventory.EquipItem(selectedItem);
				}
			}
		}

		protected override void DrawMenu(Graphics2D g) {
			base.DrawMenu(g);
		}


		//-----------------------------------------------------------------------------
		// Slots
		//-----------------------------------------------------------------------------

		public Slot GetSecondarySlotAt(Point2I index) {
			return slotGroups[0].GetSlotAt(index.X + index.Y * 5);
		}
	}
}
