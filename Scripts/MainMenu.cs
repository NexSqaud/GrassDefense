using Godot;

using GrassDefense.Scripts;

using System;

public class MainMenu : VBoxContainer
{
    [Export] private NodePath _settingsMenuPath;
    [Export] private string _gameScenePath;

    private Control _settingsMenu;

    public void PlayButtonPressed()
    {
        Singletons.SceneLoader.ChangeScene(_gameScenePath);
    }

    public void SettingsButtonPressed()
    {
        if(_settingsMenu == null)
        {
            _settingsMenu = GetNode<Control>(_settingsMenuPath);
        }

        Visible = false;
        _settingsMenu.Visible = true;
    }

    public void SettingsBackButtonPressed()
    {
        _settingsMenu.Visible = false;
        Visible = true;
    }

    public void QuitButtonPressed()
    {
        GetTree().Quit(0);
    }
}
