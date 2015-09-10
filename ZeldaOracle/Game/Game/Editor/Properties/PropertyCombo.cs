﻿using System;
using System.Collections.Generic;
using System.Text;

using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;

using GameFramework.MyGame.Editor;

namespace GameFramework.MyGame.Editor.Properties {
/** <summary>
 * The base property class to extend properties from.
 * </summary> */
public class PropertyCombo : Property {

	//========== CONSTANTS ===========
	#region Constants

	private const int MaxListRows = 5;

	#endregion
	//========== DELEGATES ===========
	#region Delegates

	#endregion
	//=========== MEMBERS ============
	#region Members

	// Containment
	/** <summary> The list of strings in the combo box. </summary> */
	protected List<string> nameList;
	/** <summary> The list of values in the combo box. </summary> */
	protected List<object> valueList;
	/** <summary> The current selection in the combo box. </summary> */
	protected int selection;

	// Editing
	/** <summary> True if the combo box drop down is open. </summary> */
	protected bool dropDown;
	/** <summary> The scroll position of the combo list. </summary> */
	protected int listPosition;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default property. </summary> */
	public PropertyCombo()
		: base() {

		// Containment
		this.nameList		= new List<string>();
		this.valueList		= new List<object>();
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
		this.listPosition	= 0;
	}

	/** <summary> Constructs the default property. </summary> */
	public PropertyCombo(string name, string value, string defaultValue, PropertyAction action = null)
		: base(name, value, defaultValue, action) {

		// Containment
		this.nameList		= new List<string>();
		this.valueList		= new List<object>();
		this.selection		= -1;

		// Editing
		this.dropDown		= false;
		this.listPosition	= 0;
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties
	//--------------------------------
	#region Containment

	/** <summary> Gets the list of names. </summary> */
	public List<string> NameList {
		get { return nameList; }
	}
	/** <summary> Gets the list of values. </summary> */
	public List<object> ValueList {
		get { return valueList; }
	}

	#endregion
	//--------------------------------
	#region Values

	/** <summary> Gets or sets the current selection. </summary> */
	public int Selection {
		get { return selection; }
		set {
			selection	= value;
			if (selection >= 0 && selection < nameList.Count) {
				text		= nameList[value];
				this.value	= nameList[value];
			}
		}
	}
	/** <summary> Gets the name of the current selection. </summary> */
	public string SelectionName {
		get { return ((selection >= 0 && selection < nameList.Count) ? nameList[selection] : ""); }
	}
	/** <summary> Gets the value of the current selection. </summary> */
	public object SelectionValue {
		get { return ((selection >= 0 && selection < valueList.Count) ? valueList[selection] : null); }
	}
	/** <summary> Gets the number of items in the list. </summary> */
	public int ItemCount {
		get { return GMath.Max(nameList.Count, valueList.Count); }
	}
	/** <summary> Returns true if the value is invalid. </summary> */
	public override bool IsInvalid {
		get {
			for (int i = 0; i < nameList.Count; i++) {
				if (nameList[i] == text)
					return false;
			}
			return true;
		}
	}

	#endregion
	//--------------------------------
	#endregion
	//========== MANAGEMENT ==========
	#region Management

	/** <summary> Adds an item to the combo box. </summary> */
	public void AddItem(string name, object value = null) {
		nameList.Add(name);
		valueList.Add(value);
	}
	/** <summary> Removes all items from the combo box. </summary> */
	public void ClearItems() {
		nameList.Clear();
		valueList.Clear();

		selection = -1;
	}
	/** <summary> Sets the list of items in the combo box. </summary> */
	public void SetItemList(string[] nameList, object[] valueList) {
		this.nameList.Clear();
		this.nameList.AddRange(nameList);
		this.valueList.Clear();
		this.valueList.AddRange(valueList);

		selection = -1;
	}

	#endregion
	//=========== UPDATING ===========
	#region Updating

	/** <summary> Called every step to update the property. </summary> */
	public override void Update(double time, Point2I position) {

		base.Update(time, position);

		if (editing)
			StopEditing();

		if (grid.DropDownOpen && grid.DropDownProperty != this)
			return;

		if (dropDown && !grid.DropDownOpen)
			grid.OpenDropDown(this);
		else if (!dropDown && grid.DropDownOpen)
			grid.CloseDropDown();

		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition + SeparatorPadding, 0),
													new Point2I(grid.ValueWidth - SeparatorPadding * 2, RowHeight));

		bool clickedInside = false;

		if (valueRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
			dropDown = !dropDown;
			listPosition = 0;
			clickedInside = true;
		}

		int listWidth = 200;

		for (int i = 0; i < nameList.Count; i++) {
			listWidth = (int)GMath.Max(listWidth, grid.Font.MeasureString(nameList[i]).X + TextPadding * 3);
		}

		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		if (dropDown) {

			if (Mouse.IsWheelUp() && listPosition > 0) {
				listPosition--;
			}
			else if (Mouse.IsWheelDown() && listPosition < ItemCount - MaxListRows) {
				listPosition++;
			}

			for (int i = 0; i < nameList.Count && i < MaxListRows; i++) {
				if (itemRect.Contains(mousePos) && Mouse.IsButtonPressed(MouseButtons.Left)) {
					selection = i + listPosition;
					text = nameList[i + listPosition];
					value = nameList[i + listPosition];
					dropDown = false;
					clickedInside = true;
					changed = true;
					if (action != null)
						action();
				}
				itemRect.Y += RowHeight;
			}

			if (!clickedInside && Mouse.IsButtonPressed(MouseButtons.Left)) {
				dropDown = false;
			}
		}
	}

	#endregion
	//=========== DRAWING ============
	#region Drawing

	/** <summary> Called every step to draw the property. </summary> */
	public override void Draw(Graphics2D g, Point2I position, int indentLevel = 0) {

		base.Draw(g, position, indentLevel);


		Point2I mousePos		= (Point2I)Mouse.GetPosition();
		//Point2I pos = position + new Point2I(grid.NamePosition, RowHeight);
		Point2I pos = position + new Point2I(grid.NamePosition + grid.NameWidth - 9 - 10, RowHeight);
		Rectangle2I addRect	= new Rectangle2I(pos + new Point2I(4, 5), new Point2I(9, 9));


		int listWidth = 200;

		for (int i = 0; i < nameList.Count; i++) {
			listWidth = (int)GMath.Max(listWidth, grid.Font.MeasureString(nameList[i]).X + TextPadding * 3);
		}
		Rectangle2I itemRect = new Rectangle2I(position + new Point2I(grid.GridWidth - listWidth - 1, RowHeight), new Point2I(listWidth, RowHeight));

		Rectangle2I buttonRect	= new Rectangle2I(position + new Point2I(grid.GridWidth - 19, 0), new Point2I(18, 18));

		g.FillRectangle(buttonRect, colorBackground);

		buttonRect.Inflate(-2, -2);
		g.FillRectangle(buttonRect, colorCursor);

		g.DrawSprite(grid.SpriteSheet["dropdown"], position + new Point2I(grid.GridWidth - 18, 1), Color.White);

		if (dropDown) {



			for (int i = 0; i < nameList.Count && i < MaxListRows; i++) {
				g.FillRectangle(itemRect, colorBackground);

				if (selection == i + listPosition) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorTextBackgroundHighlight);
					g.DrawString(grid.Font, nameList[i + listPosition], new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorTextHighlight);
				}
				else if (itemRect.Contains(mousePos)) {
					g.FillRectangle(itemRect.Inflated(-2, -2), colorBackgroundHighlight);
					g.DrawString(grid.Font, nameList[i + listPosition], new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
				}
				else {
					g.DrawString(grid.Font, nameList[i + listPosition], new Point2I(itemRect.X + TextPadding, (int)itemRect.Center.Y), Align.Left | Align.Int, colorText);
				}


				itemRect.Y += RowHeight;
			}

			itemRect.Height = itemRect.Y - RowHeight - position.Y;
			itemRect.Y = position.Y + RowHeight;

			itemRect.Inflate(1, 1);

			g.DrawRectangle(itemRect, 1, colorArrow);

			Rectangle2I valueRect	= new Rectangle2I(position + new Point2I(grid.ValuePosition - 1, -1),
														new Point2I(grid.ValueWidth + 1, RowHeight + 1));
			g.DrawRectangle(valueRect, 1, colorArrow);
		}

	}

	#endregion
	//========== MANAGEMENT ==========
	#region Management


	#endregion
}
} // end namespace
