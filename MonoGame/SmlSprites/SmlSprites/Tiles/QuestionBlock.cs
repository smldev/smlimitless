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
	public sealed class QuestionBlock : ItemBlock
	{
		private AnimatedGraphicsObject questionGraphics;
		private StaticGraphicsObject emptyGraphics;

		public override string EditorCategory => "Interactive Tiles";

		public override float SurfaceFriction => 1000f;

		public override void Initialize(Section owner)
		{
			questionGraphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBQuestionBlock");
			emptyGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBEmptyBlock");

			SetGraphicsObjects(questionGraphics, emptyGraphics);
			base.Initialize(owner);
		}
	}
}
