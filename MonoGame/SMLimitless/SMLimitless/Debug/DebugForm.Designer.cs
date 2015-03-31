namespace SMLimitless.Debug
{
	partial class DebugForm
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
			this.TextLog = new System.Windows.Forms.TextBox();
			this.TextCommand = new System.Windows.Forms.TextBox();
			this.ButtonSubmit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TextLog
			// 
			this.TextLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextLog.BackColor = System.Drawing.Color.White;
			this.TextLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextLog.Location = new System.Drawing.Point(13, 13);
			this.TextLog.Multiline = true;
			this.TextLog.Name = "TextLog";
			this.TextLog.ReadOnly = true;
			this.TextLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.TextLog.Size = new System.Drawing.Size(359, 210);
			this.TextLog.TabIndex = 2;
			// 
			// TextCommand
			// 
			this.TextCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextCommand.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextCommand.Location = new System.Drawing.Point(13, 229);
			this.TextCommand.Name = "TextCommand";
			this.TextCommand.Size = new System.Drawing.Size(278, 25);
			this.TextCommand.TabIndex = 0;
			this.TextCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextCommand_KeyDown);
			// 
			// ButtonSubmit
			// 
			this.ButtonSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonSubmit.Location = new System.Drawing.Point(297, 229);
			this.ButtonSubmit.Name = "ButtonSubmit";
			this.ButtonSubmit.Size = new System.Drawing.Size(75, 25);
			this.ButtonSubmit.TabIndex = 1;
			this.ButtonSubmit.Text = "Submit";
			this.ButtonSubmit.UseVisualStyleBackColor = true;
			this.ButtonSubmit.Click += new System.EventHandler(this.ButtonSubmit_Click);
			// 
			// DebugForm
			// 
			this.AcceptButton = this.ButtonSubmit;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 261);
			this.Controls.Add(this.ButtonSubmit);
			this.Controls.Add(this.TextCommand);
			this.Controls.Add(this.TextLog);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "DebugForm";
			this.Text = "Debug Window";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TextLog;
		private System.Windows.Forms.TextBox TextCommand;
		private System.Windows.Forms.Button ButtonSubmit;
	}
}