using Godot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassDefense.Scripts
{
    public class Singletons : Node
    {
        private static Node _instance;

        public override void _Ready()
        {
            _instance = this;
        }

        public static Random Random { get; } = new Random();
        public static EnemiesPool EnemiesPool 
        {
            get => _enemiesPool;
            set => _enemiesPool = value;
        }
        private static EnemiesPool _enemiesPool;

        public static GrassGrow GrassGrow
        {
            get => _grassGrow;
            set => _grassGrow = value;
        }
        private static GrassGrow _grassGrow;

        public static GameUtilities GameUtilities
        {
            get => _gameUtilities;
            set => _gameUtilities = value;
        }
        private static GameUtilities _gameUtilities;
        

        public static InteractiveSceneLoader SceneLoader
        {
            get
            {
                if(_sceneLoader == null)
                {
                    _sceneLoader = _instance.GetNode<InteractiveSceneLoader>($"/root/{nameof(InteractiveSceneLoader)}");
                }
                return _sceneLoader;
            }
        }
        private static InteractiveSceneLoader _sceneLoader;

        public static Settings Settings
        {
            get
            {
                if(_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;
            }
        }
        private static Settings _settings;

        public static ScoreCounter ScoreCounter
        {
            get => _scoreCounter;
            set => _scoreCounter = value;
        }
        private static ScoreCounter _scoreCounter;

    }
}
