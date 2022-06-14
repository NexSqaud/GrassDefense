using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public class GameUtilities : Node
    {
        public event EventHandler<WindowSizeChangedEventArgs> OnWindowSizeChanged;
        public event EventHandler OnLose;

        public bool GameStarted => _gameStarted;
        public bool Lose => _lose;

        private Vector2 _oldSize;
        private bool _gameStarted;
        private bool _lose;

        public override void _Ready()
        {
            Singletons.GameUtilities = this;

            _oldSize = OS.WindowSize;
        }

        public override void _ExitTree()
        {
            Singletons.GameUtilities = null;
        }

        public override void _Process(float delta)
        {
            if(!_gameStarted)
            {
                OnWindowSizeChanged?.Invoke(this, new WindowSizeChangedEventArgs() { OldSize = new Vector2(1280, 600), NewSize = OS.WindowSize });
            }

            if(_oldSize != OS.WindowSize)
            {
                var args = new WindowSizeChangedEventArgs()
                {
                    OldSize = _oldSize,
                    NewSize = OS.WindowSize
                };
                OnWindowSizeChanged?.Invoke(this, args);
                _oldSize = OS.WindowSize;
            }

            if(_gameStarted)
            {
                if(Singletons.GrassGrow.GetGrassesCount() == 0)
                {
                    OnLose?.Invoke(this, null);
                    _lose = true;
                }
            }

            _gameStarted = true;
        }
    }

    public struct WindowSizeChangedEventArgs
    {
        public Vector2 OldSize;
        public Vector2 NewSize;
    }
}
