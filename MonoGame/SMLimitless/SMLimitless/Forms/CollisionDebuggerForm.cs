using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Forms
{
	public partial class CollisionDebuggerForm : Form
	{
		internal Section Section { get; set; }
		internal Sprite SelectedSprite { get; set; }
		public float TimeScale { get; private set; }

		public CollisionDebuggerForm()
		{
			InitializeComponent();
		}

		private void TextTimeScale_TextChanged(object sender, EventArgs e)
		{
			float timeScale;
			ButtonSetTimeScale.Enabled = float.TryParse(TextTimeScale.Text, out timeScale);
		}

		private void ButtonSetTimeScale_Click(object sender, EventArgs e)
		{
			TimeScale = float.Parse(TextTimeScale.Text);
		}

		public void Update(int numberOfCollidingTiles, bool slopeCollisionOccurred, Vector2 totalOffset)
		{
			TextCollisionInfo.Text = GetCollisionInformation(numberOfCollidingTiles, slopeCollisionOccurred, totalOffset);
		}

		public void SetTileInfo(Tile tile)
		{
			TextTileInfo.Text = GetTileInformation(tile);
		}

		private string GetCollisionInformation(int numberOfCollidingTiles, bool slopeCollisionOccurred, Vector2 totalOffset)
		{
			if (SelectedSprite == null) { return "No sprite selected."; }

			StringBuilder resultBuilder = new StringBuilder();
			resultBuilder.AppendLine($"Sprite Type: {SelectedSprite.GetType().FullName}");
			resultBuilder.Append($"Position: {SelectedSprite.Position.X.ToString("F1")}, {SelectedSprite.Position.Y.ToString("F1")}, ");
			resultBuilder.AppendLine($"Size: {SelectedSprite.Size.X.ToString("F1")}, {SelectedSprite.Size.Y.ToString("F1")}");
			resultBuilder.AppendLine($"Velocity (px/sec): {SelectedSprite.Velocity.X.ToString("F1")}, {SelectedSprite.Velocity.Y.ToString("F1")}");
			resultBuilder.AppendLine($"Acceleration (px/sec²): {SelectedSprite.Acceleration.X.ToString("F1")}, {SelectedSprite.Acceleration.Y.ToString("F1")}");
			resultBuilder.AppendLine($"Embedded? {((SelectedSprite.IsEmbedded) ? "Yes" : "No")}");
			resultBuilder.AppendLine($"Total offset: {totalOffset.X.ToString("F1")}, {totalOffset.Y.ToString("F1")}");
			resultBuilder.AppendLine($"Colliding with {numberOfCollidingTiles} tiles.");
			resultBuilder.AppendLine($"Slope collisions? {((slopeCollisionOccurred) ? "Yes" : "No" )}");

			return resultBuilder.ToString();
		}

		private string GetTileInformation(Tile tile)
		{
			if (tile == null) { return "No tile under cursor."; }

			StringBuilder resultBuilder = new StringBuilder();
			resultBuilder.AppendLine($"Tile Type: {tile.GetType().FullName}");
			resultBuilder.Append($"Position: {tile.Position.X}, {tile.Position.Y}, Size: {tile.Size.X}, {tile.Size.Y}");
			resultBuilder.AppendLine($"Tile Shape: {tile.TileShape}");
			resultBuilder.AppendLine($"Sloped Sides: {((tile.TileShape == Physics.CollidableShape.RightTriangle) ? tile.SlopedSides.ToString() : "N/A")}");

			return resultBuilder.ToString();
		}
	}
}
