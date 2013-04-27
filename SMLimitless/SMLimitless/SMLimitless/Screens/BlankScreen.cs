using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SMLimitless.Screens.Effects;
using SMLimitless.Interfaces;

namespace SMLimitless.Screens
{
    public class BlankScreen : Screen
    {
        public override void Initialize(Screen owner, string parameters)
        {
            Owner = owner;
            effect = new FadeEffect();
            effect.Set(EffectDirection.Forward, Color.BlueViolet);
        }

        public override void LoadContent() { }

        public override void Update()
        {
            effect.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                Exit();
            }
        }

        public override void Draw()
        {
            effect.Draw();
        }
    }
}
