namespace SMLimitless.Forms
{
	partial class PhysicsSettingsEditorForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhysicsSettingsEditorForm));
			this.StaticLabelDescription = new System.Windows.Forms.Label();
			this.PanelSettings = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// StaticLabelDescription
			// 
			this.StaticLabelDescription.AutoSize = true;
			this.StaticLabelDescription.Location = new System.Drawing.Point(13, 13);
			this.StaticLabelDescription.Name = "StaticLabelDescription";
			this.StaticLabelDescription.Size = new System.Drawing.Size(349, 26);
			this.StaticLabelDescription.TabIndex = 0;
			this.StaticLabelDescription.Text = "Use this form to edit physics settings of the game or game objects.\r\nUseful for t" +
    "esting or debugging.";
			// 
			// PanelSettings
			// 
			this.PanelSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PanelSettings.AutoScroll = true;
			this.PanelSettings.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PanelSettings.Location = new System.Drawing.Point(13, 43);
			this.PanelSettings.Name = "PanelSettings";
			this.PanelSettings.Size = new System.Drawing.Size(410, 356);
			this.PanelSettings.TabIndex = 1;
			// 
			// PhysicsSettingsEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 411);
			this.Controls.Add(this.PanelSettings);
			this.Controls.Add(this.StaticLabelDescription);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "PhysicsSettingsEditorForm";
			this.Text = "Physics Settings Editor";
			this.ResumeLayout(false);
			this.PerformLayout();

		}


		#endregion

		private System.Windows.Forms.Label StaticLabelDescription;
		private System.Windows.Forms.Panel PanelSettings;
	}
}