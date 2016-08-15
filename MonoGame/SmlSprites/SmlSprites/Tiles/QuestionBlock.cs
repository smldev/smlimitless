using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.IO.LevelSerializers;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Tiles
{
	public sealed class QuestionBlock : Tile
	{
		private static PhysicsSetting<float> MaximumVisualDisplacement;
		private static PhysicsSetting<float> VisualDisplacementLength;
		private static PhysicsSetting<float> SpriteSpawnLength;

		private bool isEmpty = false;
		private AnimatedGraphicsObject questionGraphics;
		private StaticGraphicsObject emptyGraphics;
		private float visualDisplacementTarget;
		private float visualDisplacement;
		private Sprite spawningSprite;

		public Sprite ContainedSprite { get; set; }
		public string ContainedSpriteGraphicsName { get; private set; }
		public QuestionBlockItemReleaseType ReleaseType { get; private set; }
		public int Quantity { get; set; }
		public float TimeLimit { get; set; }

		public override string EditorCategory => "Interactive Tiles";
		public override float SurfaceFriction => 1f;

		static QuestionBlock()
		{
			MaximumVisualDisplacement = new PhysicsSetting<float>("Question Block: Max Visual Displacement (px)", 1f, 64f, 6f, PhysicsSettingType.FloatingPoint);
			VisualDisplacementLength = new PhysicsSetting<float>("Question Block: Visual Displacement Length (sec)", 0.01f, 10f, 0.75f, PhysicsSettingType.FloatingPoint);
			SpriteSpawnLength = new PhysicsSetting<float>("Question Block: Sprite Spawn Length (sec)", 0.01f, 10f, 1f, PhysicsSettingType.FloatingPoint);
		}

		public QuestionBlock()
		{
			Size = new Vector2(16f);
			TileShape = CollidableShape.Rectangle;
			RectSolidSides = TileRectSolidSides.Top | TileRectSolidSides.Left | TileRectSolidSides.Right | TileRectSolidSides.Bottom;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			ContainedSprite = TypeSerializer.DeserializeSprite(customObjects, "containedSprite");
			ReleaseType = (QuestionBlockItemReleaseType)customObjects.GetInt("releaseType");
			Quantity = customObjects.GetInt("quantity");
			TimeLimit = customObjects.GetFloat("timeLimit");
			isEmpty = customObjects.GetBool("isEmpty");
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				containedSprite = TypeSerializer.GetSpriteObjects(ContainedSprite),
				releaseType = (int)ReleaseType,
				quantity = Quantity,
				timeLimit = TimeLimit,
				isEmpty = this.isEmpty
			};
		}

		public override void Initialize(Section owner)
		{
			questionGraphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBQuestionBlock");
			emptyGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBEmptyBlock");
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			questionGraphics.LoadContent();
			emptyGraphics.LoadContent();
		}

		public override void Update()
		{
			if (!isEmpty) { questionGraphics.Update(); }
		}

		public override void Draw()
		{
			if (!isEmpty)
			{
				Vector2 displacedPosition = new Vector2(Position.X, Position.Y + visualDisplacement);
				questionGraphics.Draw(displacedPosition, Color.White);
			}
			else
			{
				emptyGraphics.Draw(Position, Color.White);
			}
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
			
		}
	}

	public enum QuestionBlockItemReleaseType
	{
		FixedQuantity,
		FixedTime,
		FixedTimeWithMaximumQuantity,
		FixedTimeWithBonusAction // to be supported
	}
}
