﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;

namespace ZeldaEditor.PropertiesEditor {
	public class ZeldaPropertyGrid : PropertyGrid {

		private EditorControl editorControl;
		private IPropertyObject propertyObject;
		private PropertiesContainer propertiesContainer;
		//private Dictionary<string, CustomPropertyEditor> typeEditors;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaPropertyGrid() {
			editorControl       = null;
			propertyObject      = null;
			propertiesContainer = new PropertiesContainer(this);
			//typeEditors         = new Dictionary<string, CustomPropertyEditor>();

			this.SelectedObject         = propertiesContainer;
			this.PropertyValueChanged   += OnPropertyChange;
			this.IsMiscCategoryLabelHidden = false;
			this.ShowAdvancedOptions = false;
			this.AdvancedOptionsMenu = null;
		}

		public void Initialize(EditorControl editorControl) {
			this.editorControl = editorControl;
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/*public UITypeEditor GetUITypeEditor(string editorType) {
			if (!typeEditors.ContainsKey(editorType))
				return null;
			return typeEditors[editorType];
		}*/


		//-----------------------------------------------------------------------------
		// Properties Methods
		//-----------------------------------------------------------------------------

		/*public class NewEditor : ITypeEditor {

			public static readonly DependencyProperty ValueProperty =
				DependencyProperty.Register(
					"Value", typeof(string), typeof(NewEditor),
						new FrameworkPropertyMetadata(
							null,
							FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
			public string Value {
				get { return (string)GetValue(ValueProperty); }
				set { SetValue(ValueProperty, value); }
			}

			FrameworkElement ITypeEditor.ResolveEditor(PropertyItem propertyItem) {
				Binding binding = new Binding("Value");
				binding.Source = propertyItem;
				binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
				BindingOperations.SetBinding(this, LastNameUserControlEditor.ValueProperty, binding);
				return this;
			}
		}*/

		public void OpenProperties(IPropertyObject propertyObject) {
			editorControl.EditorWindow.UpdatePropertyPreview(propertyObject);
			this.propertyObject = propertyObject;
			propertiesContainer.Set(propertyObject.Properties);
			Stopwatch watch = new Stopwatch();
			watch.Start();
			UpdateContainerHelper();
			Console.WriteLine(watch.ElapsedMilliseconds);
		}

		public void UpdateProperties() {
			Update();
		}

		public void RefreshProperties() {
			UpdateContainerHelper();
		}

		public void CloseProperties() {
			propertiesContainer.Clear();
			UpdateContainerHelper();
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnPropertyChange(object sender, PropertyValueChangedEventArgs e) {
			CustomPropertyDescriptor propertyDescriptor = ((PropertyItem)e.OriginalSource).PropertyDescriptor as CustomPropertyDescriptor;
			Property property = propertyDescriptor.Property;
			PropertyDocumentation propertyDoc = property.GetRootDocumentation();
			editorControl.IsModified = true;
			// Handle special property editor-types.
			if (propertyDoc != null && propertyDoc.EditorType == "script") {
				string oldValue = e.OldValue.ToString();
				string newValue = e.NewValue as string;

				Script oldScript = editorControl.World.GetScript(oldValue);
				Script newScript = editorControl.World.GetScript(newValue);
				bool isNewScriptInvalid = (newScript == null && newValue.Length > 0);

				// When a script property is changed from a hidden script to something else.
				if (oldScript != null && oldScript.IsHidden && newScript != oldScript) {
					// Delete the old script from the world (because it is now unreferenced).
					editorControl.World.RemoveScript(oldScript);
					Console.WriteLine("Deleted unreferenced script '" + oldValue + "'");

					// Don't allow the user to reference other hidden scripts.
					if (newScript != null && newScript.IsHidden)
						isNewScriptInvalid = true;
				}

				// Show a message if the script is invalid.
				if (isNewScriptInvalid)
					TriggerMessageBox.Show(Application.Current.MainWindow, MessageIcon.Warning, "'" + newValue + "' is not a valid script name.", "Invalid Name");
			}
			else if (property.Name == "id") {
				editorControl.EditorWindow.UpdatePropertyPreview(propertyObject);
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			Focus();
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public Properties PropertyList {
			get { return propertyObject.Properties; }
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}
	}
}
