using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMLimitless.Sprites.InternalSprites;

namespace SMLimitless.Editor
{
	public partial class PropertyForm : Form
	{
		private EditorSelectedObject selectedObject;

		public PropertyForm(EditorSelectedObject selectedObject)
		{
			this.selectedObject = selectedObject;
			this.selectedObject.SelectedObjectChanged += SelectedObject_SelectedObjectChanged;
			InitializeComponent();
		}

		private void SelectedObject_SelectedObjectChanged(object sender, EventArgs e)
		{
			if (selectedObject.SelectedObjectType == EditorSelectedObjectType.Tile)
			{
				PropertyGridSelectedObject.SelectedObject = selectedObject.SelectedTile;
			}
			else if (selectedObject.SelectedObjectType == EditorSelectedObjectType.Sprite)
			{
				PropertyGridSelectedObject.SelectedObject = selectedObject.SelectedSprite;
			}
			else
			{
				PropertyGridSelectedObject.SelectedObject = null;
			}
		}

		private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			selectedObject.SelectedObjectChanged -= SelectedObject_SelectedObjectChanged;
		}
	}
}
