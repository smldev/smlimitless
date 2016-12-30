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
			this.TabPageSections = new System.Windows.Forms.TabPage();
			this.TextSectionName = new System.Windows.Forms.TextBox();
			this.StaticLabelName = new System.Windows.Forms.Label();
			this.ButtonSwitchTo = new System.Windows.Forms.Button();
			this.ButtonSetAsStart = new System.Windows.Forms.Button();
			this.ButtonRemoveSection = new System.Windows.Forms.Button();
			this.ButtonUpdateName = new System.Windows.Forms.Button();
			this.ListSections = new System.Windows.Forms.ListView();
			this.ColumnID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ColumnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.StaticLabelSections = new System.Windows.Forms.Label();
			this.TabPageSectionExits = new System.Windows.Forms.TabPage();
			this.ButtonUpdate = new System.Windows.Forms.Button();
			this.GroupDestination = new System.Windows.Forms.GroupBox();
			this.StaticLabelDestinationSectionID = new System.Windows.Forms.Label();
			this.TextDestinationSectionID = new System.Windows.Forms.TextBox();
			this.LabelDestinationSectionName = new System.Windows.Forms.Label();
			this.TextDestinationY = new System.Windows.Forms.TextBox();
			this.StaticLabelDestinationY = new System.Windows.Forms.Label();
			this.TextDestinationX = new System.Windows.Forms.TextBox();
			this.StaticLabelDestinationX = new System.Windows.Forms.Label();
			this.RadioDestinationDoor = new System.Windows.Forms.RadioButton();
			this.RadioDestinationPipeRight = new System.Windows.Forms.RadioButton();
			this.RadioDestinationPipeLeft = new System.Windows.Forms.RadioButton();
			this.RadioDestinationPipeUp = new System.Windows.Forms.RadioButton();
			this.RadioDestinationPipeDown = new System.Windows.Forms.RadioButton();
			this.StaticLabelDestinationBehavior = new System.Windows.Forms.Label();
			this.GroupSourceSettings = new System.Windows.Forms.GroupBox();
			this.StaticLabelSourceSectionID = new System.Windows.Forms.Label();
			this.TextSourceSectionID = new System.Windows.Forms.TextBox();
			this.LabelSourceSectionName = new System.Windows.Forms.Label();
			this.TestSourceY = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.TextSourceX = new System.Windows.Forms.TextBox();
			this.StaticLabelSourceX = new System.Windows.Forms.Label();
			this.RadioSourceDoor = new System.Windows.Forms.RadioButton();
			this.RadioSourcePipeRight = new System.Windows.Forms.RadioButton();
			this.RadioSourcePipeLeft = new System.Windows.Forms.RadioButton();
			this.RadioSourcePipeUp = new System.Windows.Forms.RadioButton();
			this.RadioSourcePipeDown = new System.Windows.Forms.RadioButton();
			this.StaticLabelSourceBehavior = new System.Windows.Forms.Label();
			this.ButtonDeleteExit = new System.Windows.Forms.Button();
			this.ButtonNew = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.TabPageTiles.SuspendLayout();
			this.TabPageSprites.SuspendLayout();
			this.TabPageSettings.SuspendLayout();
			this.TabPageSections.SuspendLayout();
			this.TabPageSectionExits.SuspendLayout();
			this.GroupDestination.SuspendLayout();
			this.GroupSourceSettings.SuspendLayout();
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
			this.tabControl1.Controls.Add(this.TabPageSections);
			this.tabControl1.Controls.Add(this.TabPageSectionExits);
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
			// TabPageSections
			// 
			this.TabPageSections.Controls.Add(this.TextSectionName);
			this.TabPageSections.Controls.Add(this.StaticLabelName);
			this.TabPageSections.Controls.Add(this.ButtonSwitchTo);
			this.TabPageSections.Controls.Add(this.ButtonSetAsStart);
			this.TabPageSections.Controls.Add(this.ButtonRemoveSection);
			this.TabPageSections.Controls.Add(this.ButtonUpdateName);
			this.TabPageSections.Controls.Add(this.ListSections);
			this.TabPageSections.Controls.Add(this.StaticLabelSections);
			this.TabPageSections.Location = new System.Drawing.Point(4, 22);
			this.TabPageSections.Name = "TabPageSections";
			this.TabPageSections.Padding = new System.Windows.Forms.Padding(3);
			this.TabPageSections.Size = new System.Drawing.Size(651, 263);
			this.TabPageSections.TabIndex = 3;
			this.TabPageSections.Text = "Sections";
			this.TabPageSections.UseVisualStyleBackColor = true;
			// 
			// TextSectionName
			// 
			this.TextSectionName.Location = new System.Drawing.Point(266, 24);
			this.TextSectionName.Name = "TextSectionName";
			this.TextSectionName.Size = new System.Drawing.Size(156, 22);
			this.TextSectionName.TabIndex = 7;
			// 
			// StaticLabelName
			// 
			this.StaticLabelName.AutoSize = true;
			this.StaticLabelName.Location = new System.Drawing.Point(216, 29);
			this.StaticLabelName.Name = "StaticLabelName";
			this.StaticLabelName.Size = new System.Drawing.Size(39, 13);
			this.StaticLabelName.TabIndex = 6;
			this.StaticLabelName.Text = "Name:";
			// 
			// ButtonSwitchTo
			// 
			this.ButtonSwitchTo.Location = new System.Drawing.Point(266, 54);
			this.ButtonSwitchTo.Name = "ButtonSwitchTo";
			this.ButtonSwitchTo.Size = new System.Drawing.Size(75, 23);
			this.ButtonSwitchTo.TabIndex = 5;
			this.ButtonSwitchTo.Text = "&Switch To";
			this.ButtonSwitchTo.UseVisualStyleBackColor = true;
			this.ButtonSwitchTo.Click += new System.EventHandler(this.ButtonSwitchTo_Click);
			// 
			// ButtonSetAsStart
			// 
			this.ButtonSetAsStart.Location = new System.Drawing.Point(347, 54);
			this.ButtonSetAsStart.Name = "ButtonSetAsStart";
			this.ButtonSetAsStart.Size = new System.Drawing.Size(75, 23);
			this.ButtonSetAsStart.TabIndex = 4;
			this.ButtonSetAsStart.Text = "S&et as Start";
			this.ButtonSetAsStart.UseVisualStyleBackColor = true;
			this.ButtonSetAsStart.Click += new System.EventHandler(this.ButtonSetAsStart_Click);
			// 
			// ButtonRemoveSection
			// 
			this.ButtonRemoveSection.Location = new System.Drawing.Point(428, 54);
			this.ButtonRemoveSection.Name = "ButtonRemoveSection";
			this.ButtonRemoveSection.Size = new System.Drawing.Size(75, 23);
			this.ButtonRemoveSection.TabIndex = 3;
			this.ButtonRemoveSection.Text = "&Remove";
			this.ButtonRemoveSection.UseVisualStyleBackColor = true;
			// 
			// ButtonUpdateName
			// 
			this.ButtonUpdateName.Location = new System.Drawing.Point(428, 24);
			this.ButtonUpdateName.Name = "ButtonUpdateName";
			this.ButtonUpdateName.Size = new System.Drawing.Size(75, 23);
			this.ButtonUpdateName.TabIndex = 2;
			this.ButtonUpdateName.Text = "&Update";
			this.ButtonUpdateName.UseVisualStyleBackColor = true;
			this.ButtonUpdateName.Click += new System.EventHandler(this.ButtonUpdateName_Click);
			// 
			// ListSections
			// 
			this.ListSections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnID,
            this.ColumnName});
			this.ListSections.HideSelection = false;
			this.ListSections.Location = new System.Drawing.Point(10, 24);
			this.ListSections.MultiSelect = false;
			this.ListSections.Name = "ListSections";
			this.ListSections.Size = new System.Drawing.Size(200, 233);
			this.ListSections.TabIndex = 1;
			this.ListSections.UseCompatibleStateImageBehavior = false;
			this.ListSections.View = System.Windows.Forms.View.Details;
			this.ListSections.SelectedIndexChanged += new System.EventHandler(this.ListSections_SelectedIndexChanged);
			// 
			// ColumnID
			// 
			this.ColumnID.Text = "ID";
			this.ColumnID.Width = 30;
			// 
			// ColumnName
			// 
			this.ColumnName.Text = "Name";
			this.ColumnName.Width = 156;
			// 
			// StaticLabelSections
			// 
			this.StaticLabelSections.AutoSize = true;
			this.StaticLabelSections.Location = new System.Drawing.Point(7, 7);
			this.StaticLabelSections.Name = "StaticLabelSections";
			this.StaticLabelSections.Size = new System.Drawing.Size(53, 13);
			this.StaticLabelSections.TabIndex = 0;
			this.StaticLabelSections.Text = "Sections:";
			// 
			// TabPageSectionExits
			// 
			this.TabPageSectionExits.Controls.Add(this.ButtonUpdate);
			this.TabPageSectionExits.Controls.Add(this.GroupDestination);
			this.TabPageSectionExits.Controls.Add(this.GroupSourceSettings);
			this.TabPageSectionExits.Controls.Add(this.ButtonDeleteExit);
			this.TabPageSectionExits.Controls.Add(this.ButtonNew);
			this.TabPageSectionExits.Location = new System.Drawing.Point(4, 22);
			this.TabPageSectionExits.Name = "TabPageSectionExits";
			this.TabPageSectionExits.Padding = new System.Windows.Forms.Padding(3);
			this.TabPageSectionExits.Size = new System.Drawing.Size(651, 263);
			this.TabPageSectionExits.TabIndex = 4;
			this.TabPageSectionExits.Text = "Section Exits";
			this.TabPageSectionExits.UseVisualStyleBackColor = true;
			// 
			// ButtonUpdate
			// 
			this.ButtonUpdate.Location = new System.Drawing.Point(391, 203);
			this.ButtonUpdate.Name = "ButtonUpdate";
			this.ButtonUpdate.Size = new System.Drawing.Size(75, 23);
			this.ButtonUpdate.TabIndex = 14;
			this.ButtonUpdate.Text = ";";
			this.ButtonUpdate.UseVisualStyleBackColor = true;
			this.ButtonUpdate.Click += new System.EventHandler(this.ButtonUpdate_Click);
			// 
			// GroupDestination
			// 
			this.GroupDestination.Controls.Add(this.StaticLabelDestinationSectionID);
			this.GroupDestination.Controls.Add(this.TextDestinationSectionID);
			this.GroupDestination.Controls.Add(this.LabelDestinationSectionName);
			this.GroupDestination.Controls.Add(this.TextDestinationY);
			this.GroupDestination.Controls.Add(this.StaticLabelDestinationY);
			this.GroupDestination.Controls.Add(this.TextDestinationX);
			this.GroupDestination.Controls.Add(this.StaticLabelDestinationX);
			this.GroupDestination.Controls.Add(this.RadioDestinationDoor);
			this.GroupDestination.Controls.Add(this.RadioDestinationPipeRight);
			this.GroupDestination.Controls.Add(this.RadioDestinationPipeLeft);
			this.GroupDestination.Controls.Add(this.RadioDestinationPipeUp);
			this.GroupDestination.Controls.Add(this.RadioDestinationPipeDown);
			this.GroupDestination.Controls.Add(this.StaticLabelDestinationBehavior);
			this.GroupDestination.Location = new System.Drawing.Point(240, 37);
			this.GroupDestination.Name = "GroupDestination";
			this.GroupDestination.Size = new System.Drawing.Size(227, 159);
			this.GroupDestination.TabIndex = 13;
			this.GroupDestination.TabStop = false;
			this.GroupDestination.Text = "Source";
			// 
			// StaticLabelDestinationSectionID
			// 
			this.StaticLabelDestinationSectionID.AutoSize = true;
			this.StaticLabelDestinationSectionID.Location = new System.Drawing.Point(96, 37);
			this.StaticLabelDestinationSectionID.Name = "StaticLabelDestinationSectionID";
			this.StaticLabelDestinationSectionID.Size = new System.Drawing.Size(62, 13);
			this.StaticLabelDestinationSectionID.TabIndex = 12;
			this.StaticLabelDestinationSectionID.Text = "Section ID:";
			// 
			// TextDestinationSectionID
			// 
			this.TextDestinationSectionID.Location = new System.Drawing.Point(164, 34);
			this.TextDestinationSectionID.Name = "TextDestinationSectionID";
			this.TextDestinationSectionID.Size = new System.Drawing.Size(54, 22);
			this.TextDestinationSectionID.TabIndex = 11;
			this.TextDestinationSectionID.Text = "198";
			// 
			// LabelDestinationSectionName
			// 
			this.LabelDestinationSectionName.AutoSize = true;
			this.LabelDestinationSectionName.Location = new System.Drawing.Point(97, 61);
			this.LabelDestinationSectionName.Name = "LabelDestinationSectionName";
			this.LabelDestinationSectionName.Size = new System.Drawing.Size(62, 13);
			this.LabelDestinationSectionName.TabIndex = 10;
			this.LabelDestinationSectionName.Text = "Name: \"{0}\"";
			// 
			// TextDestinationY
			// 
			this.TextDestinationY.Location = new System.Drawing.Point(119, 106);
			this.TextDestinationY.Name = "TextDestinationY";
			this.TextDestinationY.Size = new System.Drawing.Size(100, 22);
			this.TextDestinationY.TabIndex = 9;
			// 
			// StaticLabelDestinationY
			// 
			this.StaticLabelDestinationY.AutoSize = true;
			this.StaticLabelDestinationY.Location = new System.Drawing.Point(97, 109);
			this.StaticLabelDestinationY.Name = "StaticLabelDestinationY";
			this.StaticLabelDestinationY.Size = new System.Drawing.Size(15, 13);
			this.StaticLabelDestinationY.TabIndex = 8;
			this.StaticLabelDestinationY.Text = "Y:";
			// 
			// TextDestinationX
			// 
			this.TextDestinationX.Location = new System.Drawing.Point(119, 82);
			this.TextDestinationX.Name = "TextDestinationX";
			this.TextDestinationX.Size = new System.Drawing.Size(100, 22);
			this.TextDestinationX.TabIndex = 7;
			// 
			// StaticLabelDestinationX
			// 
			this.StaticLabelDestinationX.AutoSize = true;
			this.StaticLabelDestinationX.Location = new System.Drawing.Point(97, 85);
			this.StaticLabelDestinationX.Name = "StaticLabelDestinationX";
			this.StaticLabelDestinationX.Size = new System.Drawing.Size(16, 13);
			this.StaticLabelDestinationX.TabIndex = 6;
			this.StaticLabelDestinationX.Text = "X:";
			// 
			// RadioDestinationDoor
			// 
			this.RadioDestinationDoor.AutoSize = true;
			this.RadioDestinationDoor.Location = new System.Drawing.Point(9, 131);
			this.RadioDestinationDoor.Name = "RadioDestinationDoor";
			this.RadioDestinationDoor.Size = new System.Drawing.Size(51, 17);
			this.RadioDestinationDoor.TabIndex = 5;
			this.RadioDestinationDoor.TabStop = true;
			this.RadioDestinationDoor.Text = "D&oor";
			this.RadioDestinationDoor.UseVisualStyleBackColor = true;
			// 
			// RadioDestinationPipeRight
			// 
			this.RadioDestinationPipeRight.AutoSize = true;
			this.RadioDestinationPipeRight.Location = new System.Drawing.Point(9, 107);
			this.RadioDestinationPipeRight.Name = "RadioDestinationPipeRight";
			this.RadioDestinationPipeRight.Size = new System.Drawing.Size(78, 17);
			this.RadioDestinationPipeRight.TabIndex = 4;
			this.RadioDestinationPipeRight.TabStop = true;
			this.RadioDestinationPipeRight.Text = "Pipe &Right";
			this.RadioDestinationPipeRight.UseVisualStyleBackColor = true;
			// 
			// RadioDestinationPipeLeft
			// 
			this.RadioDestinationPipeLeft.AutoSize = true;
			this.RadioDestinationPipeLeft.Location = new System.Drawing.Point(9, 83);
			this.RadioDestinationPipeLeft.Name = "RadioDestinationPipeLeft";
			this.RadioDestinationPipeLeft.Size = new System.Drawing.Size(69, 17);
			this.RadioDestinationPipeLeft.TabIndex = 3;
			this.RadioDestinationPipeLeft.TabStop = true;
			this.RadioDestinationPipeLeft.Text = "Pip&e Left";
			this.RadioDestinationPipeLeft.UseVisualStyleBackColor = true;
			// 
			// RadioDestinationPipeUp
			// 
			this.RadioDestinationPipeUp.AutoSize = true;
			this.RadioDestinationPipeUp.Location = new System.Drawing.Point(9, 59);
			this.RadioDestinationPipeUp.Name = "RadioDestinationPipeUp";
			this.RadioDestinationPipeUp.Size = new System.Drawing.Size(65, 17);
			this.RadioDestinationPipeUp.TabIndex = 2;
			this.RadioDestinationPipeUp.TabStop = true;
			this.RadioDestinationPipeUp.Text = "P&ipe Up";
			this.RadioDestinationPipeUp.UseVisualStyleBackColor = true;
			// 
			// RadioDestinationPipeDown
			// 
			this.RadioDestinationPipeDown.AutoSize = true;
			this.RadioDestinationPipeDown.Location = new System.Drawing.Point(9, 35);
			this.RadioDestinationPipeDown.Name = "RadioDestinationPipeDown";
			this.RadioDestinationPipeDown.Size = new System.Drawing.Size(81, 17);
			this.RadioDestinationPipeDown.TabIndex = 1;
			this.RadioDestinationPipeDown.TabStop = true;
			this.RadioDestinationPipeDown.Text = "&Pipe Down";
			this.RadioDestinationPipeDown.UseVisualStyleBackColor = true;
			// 
			// StaticLabelDestinationBehavior
			// 
			this.StaticLabelDestinationBehavior.AutoSize = true;
			this.StaticLabelDestinationBehavior.Location = new System.Drawing.Point(6, 18);
			this.StaticLabelDestinationBehavior.Name = "StaticLabelDestinationBehavior";
			this.StaticLabelDestinationBehavior.Size = new System.Drawing.Size(52, 13);
			this.StaticLabelDestinationBehavior.TabIndex = 0;
			this.StaticLabelDestinationBehavior.Text = "Behavior";
			// 
			// GroupSourceSettings
			// 
			this.GroupSourceSettings.Controls.Add(this.StaticLabelSourceSectionID);
			this.GroupSourceSettings.Controls.Add(this.TextSourceSectionID);
			this.GroupSourceSettings.Controls.Add(this.LabelSourceSectionName);
			this.GroupSourceSettings.Controls.Add(this.TestSourceY);
			this.GroupSourceSettings.Controls.Add(this.label2);
			this.GroupSourceSettings.Controls.Add(this.TextSourceX);
			this.GroupSourceSettings.Controls.Add(this.StaticLabelSourceX);
			this.GroupSourceSettings.Controls.Add(this.RadioSourceDoor);
			this.GroupSourceSettings.Controls.Add(this.RadioSourcePipeRight);
			this.GroupSourceSettings.Controls.Add(this.RadioSourcePipeLeft);
			this.GroupSourceSettings.Controls.Add(this.RadioSourcePipeUp);
			this.GroupSourceSettings.Controls.Add(this.RadioSourcePipeDown);
			this.GroupSourceSettings.Controls.Add(this.StaticLabelSourceBehavior);
			this.GroupSourceSettings.Location = new System.Drawing.Point(7, 37);
			this.GroupSourceSettings.Name = "GroupSourceSettings";
			this.GroupSourceSettings.Size = new System.Drawing.Size(227, 159);
			this.GroupSourceSettings.TabIndex = 2;
			this.GroupSourceSettings.TabStop = false;
			this.GroupSourceSettings.Text = "Source";
			// 
			// StaticLabelSourceSectionID
			// 
			this.StaticLabelSourceSectionID.AutoSize = true;
			this.StaticLabelSourceSectionID.Location = new System.Drawing.Point(96, 37);
			this.StaticLabelSourceSectionID.Name = "StaticLabelSourceSectionID";
			this.StaticLabelSourceSectionID.Size = new System.Drawing.Size(62, 13);
			this.StaticLabelSourceSectionID.TabIndex = 12;
			this.StaticLabelSourceSectionID.Text = "Section ID:";
			// 
			// TextSourceSectionID
			// 
			this.TextSourceSectionID.Location = new System.Drawing.Point(164, 34);
			this.TextSourceSectionID.Name = "TextSourceSectionID";
			this.TextSourceSectionID.Size = new System.Drawing.Size(54, 22);
			this.TextSourceSectionID.TabIndex = 11;
			this.TextSourceSectionID.Text = "198";
			// 
			// LabelSourceSectionName
			// 
			this.LabelSourceSectionName.AutoSize = true;
			this.LabelSourceSectionName.Location = new System.Drawing.Point(97, 61);
			this.LabelSourceSectionName.Name = "LabelSourceSectionName";
			this.LabelSourceSectionName.Size = new System.Drawing.Size(62, 13);
			this.LabelSourceSectionName.TabIndex = 10;
			this.LabelSourceSectionName.Text = "Name: \"{0}\"";
			// 
			// TestSourceY
			// 
			this.TestSourceY.Location = new System.Drawing.Point(119, 106);
			this.TestSourceY.Name = "TestSourceY";
			this.TestSourceY.Size = new System.Drawing.Size(100, 22);
			this.TestSourceY.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(97, 109);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Y:";
			// 
			// TextSourceX
			// 
			this.TextSourceX.Location = new System.Drawing.Point(119, 82);
			this.TextSourceX.Name = "TextSourceX";
			this.TextSourceX.Size = new System.Drawing.Size(100, 22);
			this.TextSourceX.TabIndex = 7;
			// 
			// StaticLabelSourceX
			// 
			this.StaticLabelSourceX.AutoSize = true;
			this.StaticLabelSourceX.Location = new System.Drawing.Point(97, 85);
			this.StaticLabelSourceX.Name = "StaticLabelSourceX";
			this.StaticLabelSourceX.Size = new System.Drawing.Size(16, 13);
			this.StaticLabelSourceX.TabIndex = 6;
			this.StaticLabelSourceX.Text = "X:";
			// 
			// RadioSourceDoor
			// 
			this.RadioSourceDoor.AutoSize = true;
			this.RadioSourceDoor.Location = new System.Drawing.Point(9, 131);
			this.RadioSourceDoor.Name = "RadioSourceDoor";
			this.RadioSourceDoor.Size = new System.Drawing.Size(51, 17);
			this.RadioSourceDoor.TabIndex = 5;
			this.RadioSourceDoor.TabStop = true;
			this.RadioSourceDoor.Text = "&Door";
			this.RadioSourceDoor.UseVisualStyleBackColor = true;
			// 
			// RadioSourcePipeRight
			// 
			this.RadioSourcePipeRight.AutoSize = true;
			this.RadioSourcePipeRight.Location = new System.Drawing.Point(9, 107);
			this.RadioSourcePipeRight.Name = "RadioSourcePipeRight";
			this.RadioSourcePipeRight.Size = new System.Drawing.Size(78, 17);
			this.RadioSourcePipeRight.TabIndex = 4;
			this.RadioSourcePipeRight.TabStop = true;
			this.RadioSourcePipeRight.Text = "Pipe &Right";
			this.RadioSourcePipeRight.UseVisualStyleBackColor = true;
			// 
			// RadioSourcePipeLeft
			// 
			this.RadioSourcePipeLeft.AutoSize = true;
			this.RadioSourcePipeLeft.Location = new System.Drawing.Point(9, 83);
			this.RadioSourcePipeLeft.Name = "RadioSourcePipeLeft";
			this.RadioSourcePipeLeft.Size = new System.Drawing.Size(69, 17);
			this.RadioSourcePipeLeft.TabIndex = 3;
			this.RadioSourcePipeLeft.TabStop = true;
			this.RadioSourcePipeLeft.Text = "Pip&e Left";
			this.RadioSourcePipeLeft.UseVisualStyleBackColor = true;
			// 
			// RadioSourcePipeUp
			// 
			this.RadioSourcePipeUp.AutoSize = true;
			this.RadioSourcePipeUp.Location = new System.Drawing.Point(9, 59);
			this.RadioSourcePipeUp.Name = "RadioSourcePipeUp";
			this.RadioSourcePipeUp.Size = new System.Drawing.Size(65, 17);
			this.RadioSourcePipeUp.TabIndex = 2;
			this.RadioSourcePipeUp.TabStop = true;
			this.RadioSourcePipeUp.Text = "P&ipe Up";
			this.RadioSourcePipeUp.UseVisualStyleBackColor = true;
			// 
			// RadioSourcePipeDown
			// 
			this.RadioSourcePipeDown.AutoSize = true;
			this.RadioSourcePipeDown.Location = new System.Drawing.Point(9, 35);
			this.RadioSourcePipeDown.Name = "RadioSourcePipeDown";
			this.RadioSourcePipeDown.Size = new System.Drawing.Size(81, 17);
			this.RadioSourcePipeDown.TabIndex = 1;
			this.RadioSourcePipeDown.TabStop = true;
			this.RadioSourcePipeDown.Text = "&Pipe Down";
			this.RadioSourcePipeDown.UseVisualStyleBackColor = true;
			// 
			// StaticLabelSourceBehavior
			// 
			this.StaticLabelSourceBehavior.AutoSize = true;
			this.StaticLabelSourceBehavior.Location = new System.Drawing.Point(6, 18);
			this.StaticLabelSourceBehavior.Name = "StaticLabelSourceBehavior";
			this.StaticLabelSourceBehavior.Size = new System.Drawing.Size(52, 13);
			this.StaticLabelSourceBehavior.TabIndex = 0;
			this.StaticLabelSourceBehavior.Text = "Behavior";
			// 
			// ButtonDeleteExit
			// 
			this.ButtonDeleteExit.Location = new System.Drawing.Point(88, 7);
			this.ButtonDeleteExit.Name = "ButtonDeleteExit";
			this.ButtonDeleteExit.Size = new System.Drawing.Size(75, 23);
			this.ButtonDeleteExit.TabIndex = 1;
			this.ButtonDeleteExit.Text = "&Delete";
			this.ButtonDeleteExit.UseVisualStyleBackColor = true;
			// 
			// ButtonNew
			// 
			this.ButtonNew.Location = new System.Drawing.Point(7, 7);
			this.ButtonNew.Name = "ButtonNew";
			this.ButtonNew.Size = new System.Drawing.Size(75, 23);
			this.ButtonNew.TabIndex = 0;
			this.ButtonNew.Text = "&New...";
			this.ButtonNew.UseVisualStyleBackColor = true;
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
			this.TabPageSections.ResumeLayout(false);
			this.TabPageSections.PerformLayout();
			this.TabPageSectionExits.ResumeLayout(false);
			this.GroupDestination.ResumeLayout(false);
			this.GroupDestination.PerformLayout();
			this.GroupSourceSettings.ResumeLayout(false);
			this.GroupSourceSettings.PerformLayout();
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
		private System.Windows.Forms.TabPage TabPageSections;
		private System.Windows.Forms.TextBox TextSectionName;
		private System.Windows.Forms.Label StaticLabelName;
		private System.Windows.Forms.Button ButtonSwitchTo;
		private System.Windows.Forms.Button ButtonSetAsStart;
		private System.Windows.Forms.Button ButtonRemoveSection;
		private System.Windows.Forms.Button ButtonUpdateName;
		private System.Windows.Forms.ListView ListSections;
		private System.Windows.Forms.ColumnHeader ColumnID;
		private System.Windows.Forms.ColumnHeader ColumnName;
		private System.Windows.Forms.Label StaticLabelSections;
		private System.Windows.Forms.TabPage TabPageSectionExits;
		private System.Windows.Forms.GroupBox GroupSourceSettings;
		private System.Windows.Forms.Label StaticLabelSourceBehavior;
		private System.Windows.Forms.Button ButtonDeleteExit;
		private System.Windows.Forms.Button ButtonNew;
		private System.Windows.Forms.GroupBox GroupDestination;
		private System.Windows.Forms.Label StaticLabelDestinationSectionID;
		private System.Windows.Forms.TextBox TextDestinationSectionID;
		private System.Windows.Forms.Label LabelDestinationSectionName;
		private System.Windows.Forms.TextBox TextDestinationY;
		private System.Windows.Forms.Label StaticLabelDestinationY;
		private System.Windows.Forms.TextBox TextDestinationX;
		private System.Windows.Forms.Label StaticLabelDestinationX;
		private System.Windows.Forms.RadioButton RadioDestinationDoor;
		private System.Windows.Forms.RadioButton RadioDestinationPipeRight;
		private System.Windows.Forms.RadioButton RadioDestinationPipeLeft;
		private System.Windows.Forms.RadioButton RadioDestinationPipeUp;
		private System.Windows.Forms.RadioButton RadioDestinationPipeDown;
		private System.Windows.Forms.Label StaticLabelDestinationBehavior;
		private System.Windows.Forms.Label StaticLabelSourceSectionID;
		private System.Windows.Forms.TextBox TextSourceSectionID;
		private System.Windows.Forms.Label LabelSourceSectionName;
		private System.Windows.Forms.TextBox TestSourceY;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TextSourceX;
		private System.Windows.Forms.Label StaticLabelSourceX;
		private System.Windows.Forms.RadioButton RadioSourceDoor;
		private System.Windows.Forms.RadioButton RadioSourcePipeRight;
		private System.Windows.Forms.RadioButton RadioSourcePipeLeft;
		private System.Windows.Forms.RadioButton RadioSourcePipeUp;
		private System.Windows.Forms.RadioButton RadioSourcePipeDown;
		private System.Windows.Forms.Button ButtonUpdate;
	}
}