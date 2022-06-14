using Godot;

using GrassDefense.Scripts;

using System;

public class BackButton : Button
{
    [Export] private string _mainMenuScenePath;

    public override void _Pressed()
    {
        Singletons.SceneLoader.ChangeScene(_mainMenuScenePath);
    }
}
