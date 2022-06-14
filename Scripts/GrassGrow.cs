using Godot;

using GrassDefense.Scripts;

using System;
using System.Collections.Generic;

public class GrassGrow : TileMap
{
    private const int MapWidth = 16;
    private const int MapHeight = 8;

    private readonly static (int x, int y)[] _neighboursToCheck =
    {
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1)
    };

    public int MapSize => MapWidth * MapHeight;
    public float RealCellSize { get; private set; } = 0f;

    [Export(PropertyHint.Range, "0,1,0.01")] private float _topMargin = 0;
    [Export(PropertyHint.Range, "0,1,0.01")] private float _bottomMargin = 0;

    private Random _random = new Random();
    private Cell[,] _cells = new Cell[MapWidth, MapHeight];

    private float _timeElapsed = 0;

    public override void _Ready()
    {
        Singletons.GrassGrow = this;
        SetupBackground();
        Singletons.GameUtilities.OnWindowSizeChanged += RecalculateSize;

        for(int x = 0; x < MapWidth; x++)
        {
            for(int y = 0; y < MapHeight; y++)
            {
                SetCell(new CellPosition(x, y), new Cell(CellType.Dirt));
            }
        }


        var grassX = _random.Next(0, MapWidth);
        var grassY = _random.Next(0, MapHeight);

        SetCell(new CellPosition(grassX, grassY), new Cell(CellType.Grass));
    }

    public override void _ExitTree()
    {
        Singletons.GameUtilities.OnWindowSizeChanged -= RecalculateSize;
        Singletons.GrassGrow = null;
    }

    public override void _Process(float delta)
    {
        _timeElapsed += delta;
        
        if(_timeElapsed >= 0.75f)
        {
            var updateCount = _random.Next(0, 6);
            for(int i = 0; i < updateCount; i++)
            {
                var cellX = _random.Next(0, MapWidth);
                var cellY = _random.Next(0, MapHeight);

                if(GetCellFromBuffer(new CellPosition(cellX, cellY)).CellType != CellType.Dirt)
                {
                    continue;
                }

                for(int j = 0; j < _neighboursToCheck.Length; j++)
                {
                    var neighbour = GetCellFromBuffer(new CellPosition(cellX + _neighboursToCheck[j].x, cellY + _neighboursToCheck[j].y));
                    if(neighbour.CellType != CellType.Dirt)
                    {
                        SetCell(new CellPosition(cellX, cellY), new Cell(neighbour.CellType));
                        continue;
                    }
                }
            }
            _timeElapsed = 0;
        }
    }

    public int GetGrassesCount()
    {
        var count = 0;

        for(int x = 0; x < MapWidth; x++)
        {
            for(int y = 0; y < MapHeight; y++)
            {
                if(GetCellFromBuffer(new CellPosition(x, y)).CellType != CellType.Dirt)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public List<CellPosition> GetCellsOfType(CellType type)
    {
        var cells = new List<CellPosition>();

        for(int x = 0; x < MapWidth; x++)
        {
            for(int y = 0; y < MapHeight; y++)
            {
                var cell = GetCellFromBuffer(new CellPosition(x, y));
                if(cell.CellType == type)
                {
                    cells.Add(new CellPosition(x, y));
                }
            }
        }

        return cells;
    }

    public void DestroyGrass(CellPosition position)
    {
        SetCell(position, new Cell(CellType.Dirt));
    }

    private void SetupBackground()
    {
        for (int x = -2; GetCell(x, -1) != (int)CellType.Invalid; x--)
        {
            for (int y = -1; GetCell(x, y) != (int)CellType.Invalid; y++)
            {
                SetCell(x, y, (int)CellType.Grass, _random.NextBoolean(), _random.NextBoolean());
            }
        }

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = MapHeight + 1; GetCell(x, y) != (int)CellType.Invalid; y++)
            {
                SetCell(x, y, (int)CellType.Grass, _random.NextBoolean(), _random.NextBoolean());
            }
        }

        for (int x = MapWidth + 1; GetCell(x, -1) != (int)CellType.Invalid; x++)
        {
            for (int y = -1; GetCell(x, y) != (int)CellType.Invalid; y++)
            {
                SetCell(x, y, (int)CellType.Grass, _random.NextBoolean(), _random.NextBoolean());
            }
        }
    }

    private void RecalculateSize(object _, WindowSizeChangedEventArgs args)
    {
        var viewportSize = args.NewSize;
        var top = viewportSize.y * _topMargin;
        var bottom = viewportSize.y * (1 - _bottomMargin);
        var height = bottom - top;
        var width = (height / MapHeight) * MapWidth;
        var left = (viewportSize.x / 2) - (width / 2);

        Position = new Vector2(left, top);
        Scale = new Vector2(width / (64 * MapWidth), height / (64 * MapHeight));
        RealCellSize = width / MapWidth;
    }

    private Cell GetCellFromBuffer(CellPosition position)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= MapWidth || position.Y >= MapHeight)
        {
            return new Cell(CellType.Dirt);
        }

        return _cells[position.X, position.Y];
    }

    private void SetCell(CellPosition position, Cell cell)
    {
        if (position.X < 0 || position.Y < 0 || position.X >= MapWidth || position.Y >= MapHeight)
        {
            return;
        }

        _cells[position.X, position.Y] = cell;
        SetCell(position.X, position.Y, (int)cell.CellType, cell.FlipX, cell.FlipY);
    }

}
