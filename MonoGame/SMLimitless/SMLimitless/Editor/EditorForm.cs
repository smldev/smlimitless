using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Editor
{
	public partial class EditorForm : Form
	{
		private Level level;
		private Section section;

		public EditorState EditorState { get; private set; }

		public EditorForm(Level level, Section section)
		{
			InitializeComponent();

			this.level = level;
			this.section = section;

			PropertyLevel.SelectedObject = level;
			PropertySection.SelectedObject = section;
		}
	}

	public enum EditorState
	{
		ObjectSelected,
		Cursor,
		Delete
	}
}
