using Godot;
using System;
using System.Collections.Generic;

public class InteractiveSceneLoader : Node
{
    private const int MaxTime = 100;


    private ResourceInteractiveLoader _loader;
    private Dictionary<string, Node> _loadedScenes;
    private Node _currentScene;
    private Node _newScene;
    private ProgressBar _progressBar;
    private int _waitFrame;

    public override void _Ready()
    {
        var root = GetTree().Root;
        _currentScene = root.GetChild(root.GetChildCount() - 1);
        _loadedScenes = new Dictionary<string, Node>();

        _progressBar = new ProgressBar
        {
            MarginTop = 0,
            MarginLeft = 0,
            MarginBottom = 0,
            MarginRight = 0,

            AnchorTop = 0.4f,
            AnchorLeft = 0.3f,
            AnchorBottom = 0.6f,
            AnchorRight = 0.6f,
            
            MinValue = 0,
            MaxValue = 1,
            Name = "Progress"
        };
        AddChild(_progressBar);
        _progressBar.Hide();

    }

    public override void _Process(float delta)
    {
        if (_loader == null)
        {
            return;
        }

        if(_waitFrame > 0)
        {
            _waitFrame--;
            return;
        }

        var time = OS.GetTicksMsec();
        while(OS.GetTicksMsec() < time + MaxTime)
        {
            var error = _loader.Poll();

            if(error == Error.FileEof)
            {
                // Loaded
                _newScene = ((PackedScene)_loader.GetResource()).Instance();
                //_loadedScenes.Add(_loader.GetResource().ResourcePath, _newScene);
                SetScene();
                break;
            }
            else if(error == Error.Ok)
            {
                UpdateProgress();
            }
            else
            {
                // Error
                GD.PrintErr($"Error while loading scene: {error}");
                _loader = null;
                break;
            }


        }

    }

    public void ChangeScene(string scenePath)
    {
        if (_loader != null)
        {
            throw new InvalidOperationException("Loader is loading scene");
        }
        if (!ResourceLoader.HasCached(scenePath))
        {
            GD.Print("Not cached!");
            _loader = ResourceLoader.LoadInteractive(scenePath);
        }
        else
        {
            GD.Print("Cached!");
            _currentScene.QueueFree();
            var scene = ResourceLoader.Load<PackedScene>(scenePath, null, true);
            _currentScene = scene.Instance();
            GetNode("/root").AddChild(_currentScene);
            GetTree().CurrentScene = _currentScene;
            return;
        }
        if(_loader == null)
        {
            // Error
            return;
        }

        _currentScene.QueueFree();

        // Show loading screen
        _progressBar.Show();


        _waitFrame = 1;
    }

    private void SetScene()
    {
        GetNode("/root").AddChild(_newScene);
        _currentScene = _newScene;
        GetTree().CurrentScene = _newScene;
        _newScene = null;
        _progressBar.Hide();
        _loader = null;
    }

    private void UpdateProgress()
    {
        _progressBar.Value = (float)_loader.GetStage() / _loader.GetStageCount();
    }

}
