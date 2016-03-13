namespace SMLimitless.Forms
{
	partial class CollisionDebuggerForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollisionDebuggerForm));
			this.StaticLabelTimeScale = new System.Windows.Forms.Label();
			this.TextTimeScale = new System.Windows.Forms.TextBox();
			this.ButtonSetTimeScale = new System.Windows.Forms.Button();
			this.TextCollisionInfo = new System.Windows.Forms.TextBox();
			this.TextTileInfo = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// StaticLabelTimeScale
			// 
			this.StaticLabelTimeScale.AutoSize = true;
			this.StaticLabelTimeScale.Location = new System.Drawing.Point(14, 16);
			this.StaticLabelTimeScale.Name = "StaticLabelTimeScale";
			this.StaticLabelTimeScale.Size = new System.Drawing.Size(62, 13);
			this.StaticLabelTimeScale.TabIndex = 0;
			this.StaticLabelTimeScale.Text = "Time Scale:";
			// 
			// TextTimeScale
			// 
			this.TextTimeScale.Location = new System.Drawing.Point(82, 13);
			this.TextTimeScale.Name = "TextTimeScale";
			this.TextTimeScale.Size = new System.Drawing.Size(137, 22);
			this.TextTimeScale.TabIndex = 1;
			this.TextTimeScale.Text = "1.0";
			this.TextTimeScale.TextChanged += new System.EventHandler(this.TextTimeScale_TextChanged);
			// 
			// ButtonSetTimeScale
			// 
			this.ButtonSetTimeScale.Location = new System.Drawing.Point(225, 11);
			this.ButtonSetTimeScale.Name = "ButtonSetTimeScale";
			this.ButtonSetTimeScale.Size = new System.Drawing.Size(47, 24);
			this.ButtonSetTimeScale.TabIndex = 2;
			this.ButtonSetTimeScale.Text = "&Set";
			this.ButtonSetTimeScale.UseVisualStyleBackColor = true;
			this.ButtonSetTimeScale.Click += new System.EventHandler(this.ButtonSetTimeScale_Click);
			// 
			// TextCollisionInfo
			// 
			this.TextCollisionInfo.Location = new System.Drawing.Point(17, 47);
			this.TextCollisionInfo.Multiline = true;
			this.TextCollisionInfo.Name = "TextCollisionInfo";
			this.TextCollisionInfo.ReadOnly = true;
			this.TextCollisionInfo.Size = new System.Drawing.Size(255, 150);
			this.TextCollisionInfo.TabIndex = 3;
			this.TextCollisionInfo.Text = "Sprite Type: [type]\r\nLocation: [x], [y], Size: [x], [y]\r\nVelocity (px/sec): [x], " +
    "[y]\r\nAcceleration (px/sec²): [x], [y]\r\nColliding with [number] tiles\r\nSlope coll" +
    "ision? [yes/no]";
			// 
			// TextTileInfo
			// 
			this.TextTileInfo.Location = new System.Drawing.Point(17, 204);
			this.TextTileInfo.Multiline = true;
			this.TextTileInfo.Name = "TextTileInfo";
			this.TextTileInfo.ReadOnly = true;
			this.TextTileInfo.Size = new System.Drawing.Size(255, 102);
			this.TextTileInfo.TabIndex = 4;
			this.TextTileInfo.Text = "Tile Type: [type]\r\nLocation: [x], [y], Size: [x], [y]\r\nShape: [shape]\r\nSloped Sid" +
    "es: [sides]\r\n";
			// 
			// CollisionDebuggerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 318);
			this.Controls.Add(this.TextTileInfo);
			this.Controls.Add(this.TextCollisionInfo);
			this.Controls.Add(this.ButtonSetTimeScale);
			this.Controls.Add(this.TextTimeScale);
			this.Controls.Add(this.StaticLabelTimeScale);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "CollisionDebuggerForm";
			this.Text = "Collision Debugger";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label StaticLabelTimeScale;
		private System.Windows.Forms.TextBox TextTimeScale;
		private System.Windows.Forms.Button ButtonSetTimeScale;
		private System.Windows.Forms.TextBox TextCollisionInfo;
		private System.Windows.Forms.TextBox TextTileInfo;
	}
}