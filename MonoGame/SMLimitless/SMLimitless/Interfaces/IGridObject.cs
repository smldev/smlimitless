using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Collections;

namespace SMLimitless.Interfaces
{
    public interface IGridObject<T> where T : IPositionable
    {
        SizedGrid<T> Grid { get; }
        Point GridCell { get; }

        T Neighbor(Direction direction);
    }
}
