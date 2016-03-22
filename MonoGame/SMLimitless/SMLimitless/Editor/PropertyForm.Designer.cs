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
			this.PropertyGridSelectedObject = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// PropertyGridSelectedObject
			// 
			this.PropertyGridSelectedObject.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PropertyGridSelectedObject.Location = new System.Drawing.Point(13, 13);
			this.PropertyGridSelectedObject.Name = "PropertyGridSelectedObject";
			this.PropertyGridSelectedObject.Size = new System.Drawing.Size(259, 386);
			this.PropertyGridSelectedObject.TabIndex = 0;
			// 
			// PropertyForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 411);
			this.Controls.Add(this.PropertyGridSelectedObject);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "PropertyForm";
			this.Text = "Properties";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertyForm_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid PropertyGridSelectedObject;
	}
}