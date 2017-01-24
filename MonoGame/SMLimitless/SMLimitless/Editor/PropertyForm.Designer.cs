namespace SMLimitless.Editor
{
	partial class PropertyForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PanelSettings = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// PanelSettings
			// 
			this.PanelSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PanelSettings.Location = new System.Drawing.Point(12, 12);
			this.PanelSettings.Name = "PanelSettings";
			this.PanelSettings.Size = new System.Drawing.Size(259, 386);
			this.PanelSettings.TabIndex = 0;
			// 
			// PropertyForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 411);
			this.Controls.Add(this.PanelSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "PropertyForm";
			this.Text = "Properties";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertyForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel PanelSettings;
	}
}