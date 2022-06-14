using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public class Enemy : KinematicBody2D
    {
        [Export(PropertyHint.Range, "2,200,1")] private float _speed = 50f;
        [Export(PropertyHint.Range, "1,10,1")] private int _maxHealth = 5;
        [Export] private NodePath _healthBarPath;
        [Export] private NodePath _spritePath;
        [Export] private NodePath _collisitonPath;

        private CellPosition _target;
        private Vector2? _targetPosition;

        private float _grassEatProcess;
        private int _health = 0;
        private ProgressBar _healthBar;
        private Sprite _sprite;
        private CollisionShape2D _collision;

        private bool _dontMove = false;

        public override void _Ready()
        {
            Singletons.GameUtilities.OnWindowSizeChanged += RecalculateSize;
            RecalculateSize(null, new WindowSizeChangedEventArgs() { NewSize = OS.WindowSize, OldSize = OS.WindowSize });

            _health = _maxHealth;
            _healthBar = GetNode<ProgressBar>(_healthBarPath);
            _healthBar.MaxValue = _maxHealth;
            _healthBar.Value = _health;

            _sprite = GetNode<Sprite>(_spritePath);
            _collision = GetNode<CollisionShape2D>(_collisitonPath);

            if (!Singletons.GameUtilities.GameStarted)
            {
                Singletons.EnemiesPool.AddEnemyPrefab(this);
                _dontMove = true;
            }
        }

        public override void _ExitTree()
        {
            Singletons.GameUtilities.OnWindowSizeChanged -= RecalculateSize;
        }

        public override void _PhysicsProcess(float delta)
        {
            if(_dontMove)
            {
                return;
            }

            var walkDistance = _speed * Scale.x;

            if(walkDistance > 0 && _targetPosition != null)
            {
                var distanceToPoint = GlobalPosition.DistanceTo(_targetPosition.Value);
                var direction = GlobalPosition.DirectionTo(_targetPosition.Value);

                _sprite.LookAt(_targetPosition.Value);
                _sprite.RotationDegrees += 90;
                _collision.RotationDegrees = _sprite.RotationDegrees;

                if (walkDistance <= distanceToPoint)
                {
                    MoveAndSlide(direction * walkDistance);
                }
                else
                {
                    MoveAndSlide(direction * distanceToPoint);
                    _targetPosition = null;
                }
            }

            if(_targetPosition == null)
            {
                _grassEatProcess += delta / 5f;
                if(_grassEatProcess >= 1f)
                {
                    Singletons.GrassGrow.DestroyGrass(_target);
                    SetTargetCell(Singletons.EnemiesPool.GetNewTarget());
                    _grassEatProcess = 0;
                }
            }
        }

        public override void _InputEvent(Godot.Object viewport, InputEvent @event, int shapeIdx)
        {
            if(@event is InputEventMouse mouseEvent)
            {
                if(mouseEvent.IsPressed() && (mouseEvent.ButtonMask & (int)ButtonList.MaskLeft) != 0)
                {
                    _health--;
                }
            }
            else if(@event is InputEventScreenTouch touchScreenEvent && touchScreenEvent.Pressed)
            {
                _health--;
            }
            else
            {
                return;
            }

            _healthBar.Value = _health;

            if(_health == 0)
            {
                Singletons.ScoreCounter.EnemyDefeated();
                QueueFree();
            }
        }

        public void SetTargetCell(CellPosition position)
        {
            var cellSize = Singletons.GrassGrow.RealCellSize;
            _target = position;
            _targetPosition = Singletons.GrassGrow.GlobalPosition +
                new Vector2(position.X * cellSize + (cellSize / 2), position.Y * cellSize + (cellSize / 2));
        }

        private void RecalculateSize(object _, WindowSizeChangedEventArgs args)
        {
            Scale = new Vector2(Singletons.GrassGrow.RealCellSize / 64f, Singletons.GrassGrow.RealCellSize / 64f);
            GlobalPosition *= (args.NewSize / args.OldSize);
            SetTargetCell(_target);
        }
    }
}
