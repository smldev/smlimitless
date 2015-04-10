using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SMLimitless.Forms
{
	public partial class PhysicsSettingsEditorForm : Form
	{
		public object SelectedObject
		{
			get
			{
				return this.PhysicsProperties.SelectedObject;
			}
			set
			{
				this.PhysicsProperties.SelectedObject = value;
			}
		}
		
		public PhysicsSettingsEditorForm()
		{
			InitializeComponent();
		}
	}
}
