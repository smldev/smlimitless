using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Physics;

namespace SMLimitless.Screens
{
	public sealed class AnimationTestScreen : Screen
	{
		private const string ResourceName = "SMB3PlayerMarioSmallToSuper";

		private ComplexGraphicsObject graphics;
		private bool isRunning = true;
		private Camera2D camera = new Camera2D();
		private Vector2 graphicPosition = new Vector2(16f, 64f);
		private Vector2 hitboxPosition = new Vector2(16f, 64f);
		private Vector2 hitboxSize = new Vector2(29f, 32f);
		private int objectIndex = 0;
		private Dictionary<Keys, Direction> keyDirectionMapping;

		public override void Draw()
		{
			graphics.Draw(graphicPosition, Color.White);
			GameServices.SpriteBatch.DrawRectangleEdges(hitboxPosition, hitboxSize, Color.Red);

			string objectInfo;
			string graphicInfo;
			string hitboxInfo;
			string cycleInfo;
			GetDebugText(out objectInfo, out graphicInfo, out hitboxInfo, out cycleInfo);

			GameServices.DebugFont.DrawString(objectInfo, new Vector2(16f, 0f));
			GameServices.DebugFont.DrawString(graphicInfo, new Vector2(16f, 16f));
			GameServices.DebugFont.DrawString(hitboxInfo, new Vector2(16f, 32f));
			GameServices.DebugFont.DrawString(cycleInfo, new Vector2(16f, 48f));
		}

		public override void Initialize(Screen owner, string parameters)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource(ResourceName);
			GameServices.Camera = camera;
			camera.Zoom = 8f;

			keyDirectionMapping = new Dictionary<Keys, Direction>()
			{
				{ Keys.Up, Direction.Up },
				{ Keys.Down, Direction.Down },
				{ Keys.Left, Direction.Left },
				{ Keys.Right, Direction.Right }
			};
		}

		public override void LoadContent()
		{
			graphics.LoadContent();

			// Because IReadOnlyList<T> doesn't have an IndexOf method, we have to substitute the
			// following (see http://stackoverflow.com/a/11082569/2709212)
			for (int i = 0; i < graphics.ObjectNames.Count; i++)
			{
				if (graphics.CurrentObjectName == graphics.ObjectNames[i])
				{
					objectIndex = i;
					break;
				}
			}
		}

		public override void Update()
		{
			// Controls:
			// J - increase animation speed by 1%
			// K - decrease animation speed by 1%
			// space - play/pause
			// Q - (while paused) previous frame
			// W - (while paused) next frame
			// T - reverse animation
			// A - (for CGOs) previous object
			// S - (for CGOs) next object
			// Z - zoom out by 0.1x
			// X - zoom in by 0.1x
			// arrow keys - move graphic onscreen
			// Ctrl+arrow keys - move hitbox
			// Alt+arrow keys - resize hitbox

			if (isRunning) { graphics.Update(); }

			if (InputManager.IsCurrentKeyPress(Keys.J))
			{
				graphics.AdjustSpeed(0.01f);
			}
			else if (InputManager.IsCurrentKeyPress(Keys.K))
			{
				graphics.AdjustSpeed(-0.01f);
			}
			else if (InputManager.IsCurrentKeyPress(Keys.Space))
			{
				isRunning = !isRunning;
			}
			else if (InputManager.IsNewKeyPress(Keys.Q))
			{
				graphics.PreviousFrame();
			}
			else if (InputManager.IsNewKeyPress(Keys.W))
			{
				graphics.NextFrame();
			}
			else if (InputManager.IsNewKeyPress(Keys.T))
			{
				graphics.IsReversed = !graphics.IsReversed;
			}
			else if (InputManager.IsNewKeyPress(Keys.A))
			{
				if (objectIndex == 0) { objectIndex = graphics.ObjectNames.Count - 1; }
				else { objectIndex--; }
				graphics.CurrentObjectName = graphics.ObjectNames[objectIndex];
			}
			else if (InputManager.IsNewKeyPress(Keys.S))
			{
				if (objectIndex == graphics.ObjectNames.Count - 1) { objectIndex = 0; }
				else { objectIndex++; }
				graphics.CurrentObjectName = graphics.ObjectNames[objectIndex];
			}
			else if (InputManager.IsCurrentKeyPress(Keys.Z))
			{
				camera.Zoom -= camera.Zoom * 0.1f;
			}
			else if (InputManager.IsCurrentKeyPress(Keys.X))
			{
				camera.Zoom += camera.Zoom * 0.1f;
			}
			CheckForMoveInput();
		}

		private void CheckForMoveInput()
		{
			// Why? Because I'm lazy.
			var ctrlPressed =
				InputManager.IsCurrentKeyPress(Keys.LeftControl) || InputManager.IsCurrentKeyPress(Keys.RightControl);
			var altPressed =
				InputManager.IsCurrentKeyPress(Keys.LeftAlt) || InputManager.IsCurrentKeyPress(Keys.RightAlt);

			foreach (var kvp in keyDirectionMapping)
			{
				if (InputManager.IsCurrentKeyPress(kvp.Key))
				{
					if (ctrlPressed) { hitboxPosition = hitboxPosition.Move(kvp.Value, 1f); }
					else if (altPressed) { hitboxSize = hitboxSize.Move(kvp.Value, 1f); }
					else { graphicPosition = graphicPosition.Move(kvp.Value, 1f); }
				}
			}
		}

		private void GetDebugText(out string objectInfo, out string graphicInfo, out string hitboxInfo, out string cycleInfo)
		{
			string objectNumberInfo = $"#{objectIndex + 1}/{graphics.ObjectNames.Count}";
			string objectNameInfo = $"{graphics.CurrentObjectName}";
			string graphicSizeData = $"Graphic X: {graphicPosition.X:F0} Y: {graphicPosition.Y:F0} W: {graphics.FrameSize.X:F0} H: {graphics.FrameSize.Y:F0}";
			string hitboxSizeData = $"Hitbox X: {hitboxPosition.X:F0} Y: {hitboxPosition.Y:F0} W: {hitboxSize.X:F0} H: {hitboxSize.Y}";

			objectInfo = $"Object {objectNumberInfo}: {objectNameInfo}";
			graphicInfo = graphicSizeData;
			hitboxInfo = hitboxSizeData;

			var cycleLength = graphics.AnimationCycleLength;
			if (cycleLength >= 0m)
			{
				cycleInfo = $"Cycle: {cycleLength:F6} seconds";
			}
			else
			{
				cycleInfo = "Static";
			}
		}
	}
}
