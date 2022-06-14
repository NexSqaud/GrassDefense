using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public class EnemiesPool : Node
    {
        [Export] private NodePath _enemiesParentPath;
        private Node _enemiesParent;

        private List<Node2D> _enemies = new List<Node2D>();
        private List<Vector2> _spawnPoints = new List<Vector2>();

        private float _timeElapsed;
        private List<CellPosition> _knownCells = new List<CellPosition>();

        public override void _Ready()
        {
            Singletons.EnemiesPool = this;
            _enemiesParent = GetNode<Node>(_enemiesParentPath);

            Singletons.GameUtilities.OnWindowSizeChanged += RescaleSpawnPointsPositions;
        }

        public override void _ExitTree()
        {
            Singletons.GameUtilities.OnWindowSizeChanged -= RescaleSpawnPointsPositions;
            Singletons.EnemiesPool = null;
        }

        public override void _Process(float delta)
        {
            _timeElapsed += delta;

            if (_timeElapsed >= 5 - ((Singletons.Random.NextDouble() * 2.0) - 1.0))
            {
                _timeElapsed = 0;

                var newCells = UpdateCellsPositions();

                if(newCells.Count == 0)
                {
                    return;
                }

                for(int i = 0; i < newCells.Count; i++)
                {
                    var enemy = SpawnRandomEnemy();
                    enemy.SetTargetCell(newCells[i]);
                }

                if(_knownCells.Count >= (Singletons.GrassGrow.MapSize / 2) && _enemiesParent.GetChildCount() <= (Singletons.GrassGrow.MapSize / 2))
                {
                    var count = _knownCells.Count / 2;
                    for (int i = 0; i < count; i++)
                    {
                        var enemy = SpawnRandomEnemy();
                        enemy.SetTargetCell(_knownCells[Singletons.Random.Next(0, _knownCells.Count)]);
                    }
                }
            }
        }

        public CellPosition GetNewTarget()
        {
            var newCells = UpdateCellsPositions();

            if (newCells.Count == 0)
            {
                if (_knownCells.Count > 0)
                {
                    return _knownCells[Singletons.Random.Next(0, _knownCells.Count)];
                }
                else
                {
                    return new CellPosition(0, 0);
                }
            }

            return newCells[Singletons.Random.Next(0, newCells.Count)];
        }

        public void AddEnemyPrefab(Enemy enemy)
        {
            if(!_enemies.Contains(enemy) && !Singletons.GameUtilities.GameStarted)
            {
                _enemies.Add(enemy);
            }
        }

        public void AddSpawnPoint(Vector2 position)
        {
            if(!_spawnPoints.Contains(position))
            {
                _spawnPoints.Add(position);
            }
        }

        private List<CellPosition> UpdateCellsPositions()
        {
            var cells = Singletons.GrassGrow.GetCellsOfType(CellType.Grass);
            _knownCells.RemoveAll(x => !cells.Contains(x));
            var newCells = cells.Where(x => !_knownCells.Contains(x)).ToList();
            _knownCells.AddRange(newCells);
            return newCells;
        }

        private Enemy SpawnRandomEnemy()
        {
            var enemy = (Enemy)_enemies[Singletons.Random.Next(0, _enemies.Count)].Duplicate((int)(DuplicateFlags.Signals | DuplicateFlags.Groups | DuplicateFlags.Scripts));
            if(enemy.GetParent() != null)
            {
                enemy.GetParent().RemoveChild(enemy);
            }
            enemy.Visible = true;
            enemy.GlobalPosition = _spawnPoints[Singletons.Random.Next(0, _spawnPoints.Count)];
            _enemiesParent.AddChild(enemy);
            return enemy;
        }

        private void RescaleSpawnPointsPositions(object _, WindowSizeChangedEventArgs args)
        { 
            for(int i = 0; i < _spawnPoints.Count; i++)
            {
                _spawnPoints[i] *= (args.NewSize / args.OldSize);
            }
        }
    }
}
