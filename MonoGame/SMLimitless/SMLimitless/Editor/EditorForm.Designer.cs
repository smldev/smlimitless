namespace SMLimitless.Editor
{
	/// <summary>
	/// The form for the level editor's objects and properties.
	/// </summary>
	partial class EditorForm
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
			this.ButtonCursor = new System.Windows.Forms.Button();
			this.ButtonDelete = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.TabPageTiles = new System.Windows.Forms.TabPage();
			this.PanelTiles = new System.Windows.Forms.Panel();
			this.TabPageSprites = new System.Windows.Forms.TabPage();
			this.PanelSprites = new System.Windows.Forms.Panel();
			this.TabPageSettings = new System.Windows.Forms.TabPage();
			this.StaticLabelSectionSettings = new System.Windows.Forms.Label();
			this.PropertySection = new System.Windows.Forms.PropertyGrid();
			this.PropertyLevel = new System.Windows.Forms.PropertyGrid();
			this.StaticLabelLevelSettings = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.TabPageTiles.SuspendLayout();
			this.TabPageSprites.SuspendLayout();
			this.TabPageSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonCursor
			// 
			this.ButtonCursor.Location = new System.Drawing.Point(13, 13);
			this.ButtonCursor.Name = "ButtonCursor";
			this.ButtonCursor.Size = new System.Drawing.Size(75, 23);
			this.ButtonCursor.TabIndex = 0;
			this.ButtonCursor.Text = "Cursor";
			this.ButtonCursor.UseVisualStyleBackColor = true;
			this.ButtonCursor.Click += new System.EventHandler(this.ButtonCursor_Click);
			// 
			// ButtonDelete
			// 
			this.ButtonDelete.Location = new System.Drawing.Point(95, 13);
			this.ButtonDelete.Name = "ButtonDelete";
			this.ButtonDelete.Size = new System.Drawing.Size(75, 23);
			this.ButtonDelete.TabIndex = 1;
			this.ButtonDelete.Text = "Delete";
			this.ButtonDelete.UseVisualStyleBackColor = true;
			this.ButtonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.TabPageTiles);
			this.tabControl1.Controls.Add(this.TabPageSprites);
			this.tabControl1.Controls.Add(this.TabPageSettings);
			this.tabControl1.Location = new System.Drawing.Point(13, 43);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(659, 289);
			this.tabControl1.TabIndex = 2;
			// 
			// TabPageTiles
			// 
			this.TabPageTiles.Controls.Add(this.PanelTiles);
			this.TabPageTiles.Location = new System.Drawing.Point(4, 22);
			this.TabPageTiles.Name = "TabPageTiles";
			this.TabPageTiles.Padding = new System.Windows.Forms.Padding(3);
			this.TabPageTiles.Size = new System.Drawing.Size(651, 263);
			this.TabPageTiles.TabIndex = 0;
			this.TabPageTiles.Text = "Tiles";
			this.TabPageTiles.UseVisualStyleBackColor = true;
			// 
			// PanelTiles
			// 
			this.PanelTiles.AutoScroll = true;
			this.PanelTiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelTiles.Location = new System.Drawing.Point(3, 3);
			this.PanelTiles.Name = "PanelTiles";
			this.PanelTiles.Size = new System.Drawing.Size(645, 257);
			this.PanelTiles.TabIndex = 0;
			// 
			// TabPageSprites
			// 
			this.TabPageSprites.Controls.Add(this.PanelSprites);
			this.TabPageSprites.Location = new System.Drawing.Point(4, 22);
			this.TabPageSprites.Name = "TabPageSprites";
			this.TabPageSprites.Padding = new System.Windows.Forms.Padding(3);
			this.TabPageSprites.Size = new System.Drawing.Size(651, 263);
			this.TabPageSprites.TabIndex = 1;
			this.TabPageSprites.Text = "Sprites";
			this.TabPageSprites.UseVisualStyleBackColor = true;
			// 
			// PanelSprites
			// 
			this.PanelSprites.AutoSize = true;
			this.PanelSprites.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelSprites.Location = new System.Drawing.Point(3, 3);
			this.PanelSprites.Name = "PanelSprites";
			this.PanelSprites.Size = new System.Drawing.Size(645, 257);
			this.PanelSprites.TabIndex = 0;
			// 
			// TabPageSettings
			// 
			this.TabPageSettings.Controls.Add(this.StaticLabelSectionSettings);
			this.TabPageSettings.Controls.Add(this.PropertySection);
			this.TabPageSettings.Controls.Add(this.PropertyLevel);
			this.TabPageSettings.Controls.Add(this.StaticLabelLevelSettings);
			this.TabPageSettings.Location = new System.Drawing.Point(4, 22);
			this.TabPageSettings.Name = "TabPageSettings";
			this.TabPageSettings.Padding = new System.Windows.Forms.Padding(3);
			this.TabPageSettings.Size = new System.Drawing.Size(651, 263);
			this.TabPageSettings.TabIndex = 2;
			this.TabPageSettings.Text = "Settings";
			this.TabPageSettings.UseVisualStyleBackColor = true;
			// 
			// StaticLabelSectionSettings
			// 
			this.StaticLabelSectionSettings.AutoSize = true;
			this.StaticLabelSectionSettings.Location = new System.Drawing.Point(321, 7);
			this.StaticLabelSectionSettings.Name = "StaticLabelSectionSettings";
			this.StaticLabelSectionSettings.Size = new System.Drawing.Size(93, 13);
			this.StaticLabelSectionSettings.TabIndex = 3;
			this.StaticLabelSectionSettings.Text = "Section Settings:";
			// 
			// PropertySection
			// 
			this.PropertySection.Location = new System.Drawing.Point(324, 24);
			this.PropertySection.Name = "PropertySection";
			this.PropertySection.Size = new System.Drawing.Size(321, 233);
			this.PropertySection.TabIndex = 2;
			// 
			// PropertyLevel
			// 
			this.PropertyLevel.Location = new System.Drawing.Point(10, 24);
			this.PropertyLevel.Name = "PropertyLevel";
			this.PropertyLevel.Size = new System.Drawing.Size(308, 233);
			this.PropertyLevel.TabIndex = 1;
			// 
			// StaticLabelLevelSettings
			// 
			this.StaticLabelLevelSettings.AutoSize = true;
			this.StaticLabelLevelSettings.Location = new System.Drawing.Point(7, 7);
			this.StaticLabelLevelSettings.Name = "StaticLabelLevelSettings";
			this.StaticLabelLevelSettings.Size = new System.Drawing.Size(80, 13);
			this.StaticLabelLevelSettings.TabIndex = 0;
			this.StaticLabelLevelSettings.Text = "Level Settings:";
			// 
			// EditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(684, 344);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.ButtonDelete);
			this.Controls.Add(this.ButtonCursor);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "EditorForm";
			this.Text = "Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorForm_FormClosing);
			this.Load += new System.EventHandler(this.EditorForm_Load);
			this.tabControl1.ResumeLayout(false);
			this.TabPageTiles.ResumeLayout(false);
			this.TabPageSprites.ResumeLayout(false);
			this.TabPageSprites.PerformLayout();
			this.TabPageSettings.ResumeLayout(false);
			this.TabPageSettings.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button ButtonCursor;
		private System.Windows.Forms.Button ButtonDelete;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage TabPageTiles;
		private System.Windows.Forms.Panel PanelTiles;
		private System.Windows.Forms.TabPage TabPageSprites;
		private System.Windows.Forms.Panel PanelSprites;
		private System.Windows.Forms.TabPage TabPageSettings;
		private System.Windows.Forms.Label StaticLabelSectionSettings;
		private System.Windows.Forms.PropertyGrid PropertySection;
		private System.Windows.Forms.PropertyGrid PropertyLevel;
		private System.Windows.Forms.Label StaticLabelLevelSettings;
	}
}