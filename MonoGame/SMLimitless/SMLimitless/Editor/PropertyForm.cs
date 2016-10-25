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
	/// <summary>
	/// A form that can display and modify properties for game objects.
	/// </summary>
	public partial class PropertyForm : Form
	{
		private object displayedObject;
		public object DisplayedObject
		{
			get { return displayedObject; }
			internal set
			{
				displayedObject = value;
				if (PropertyGridSelectedObject != null) { PropertyGridSelectedObject.SelectedObject = value; }
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyForm"/> class.
		/// </summary>
		/// <param name="displayedObject">An <see cref="EditorSelectedObject"/> instance currently in use by the level editor.</param>
		/// <param name="showDialogButtons">A value that indicates if OK and Cancel button should be visible.</param>
		public PropertyForm(object displayedObject, bool showDialogButtons = false)
		{
			DisplayedObject = displayedObject;
			InitializeComponent();
			PropertyGridSelectedObject.SelectedObject = displayedObject;

			if (showDialogButtons)
			{
				CreateDialogButtons();
			}
		}

		private void CreateDialogButtons()
		{
			// Extend the form.
			Size = new Size(Size.Width, Size.Height + 40);

			// Create a new button
			Button okButton = new Button();
			okButton.Text = "OK";
			okButton.Size = new Size(100, 30);
			okButton.Location = new Point(Size.Width - 105, Size.Height - 35);
			okButton.Click += OkButton_Click;
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
	}
}
