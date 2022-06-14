using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public struct Cell
    {
        public CellType CellType;
        public bool FlipX;
        public bool FlipY;

        public Cell(CellType type)
        {
            CellType = type;
            FlipX = Singletons.Random.NextBoolean();
            FlipY = Singletons.Random.NextBoolean();
        }
    }

    public struct CellPosition
    {
        public int X;
        public int Y;

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    public enum CellType : int
    {
        Invalid = -1,
        Dirt = 0,
        Grass = 1
    }
}
