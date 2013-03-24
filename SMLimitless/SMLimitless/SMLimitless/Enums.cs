using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless
{
    [Obsolete]
    public enum HorizontalFlip
    {
        Left,
        Right
    }

    [Obsolete]
    public enum VerticalFlip
    {
        Up,
        Down
    }

    // Credit to RCIX of StackExchange GameDev
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public enum SpriteDirection
    {
        FacePlayer,
        Left,
        Right
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    public enum TileCollisionType
    {
        Solid,
        TopSolid,
        Passive,
    }
}
