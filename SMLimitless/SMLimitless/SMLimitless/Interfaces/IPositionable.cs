using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SMLimitless.Interfaces
{
    public interface IPositionable
    {
        Vector2 Position { get; }
        Vector2 Size { get; }
    }
}
