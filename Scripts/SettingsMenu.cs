using Godot;

using GrassDefense.Scripts;

using System;

public class SettingsMenu : VBoxContainer
{
    private const string SettingsSection = "Settings";
    private const string MusicValue = "Music";
    private const string SoundValue = "Sound";

    private const string SettingsFilePath = "user://settings.ini";

    [Export] private NodePath _musicButtonPath;
    [Export] private NodePath _soundButtonPath;

    private Button _musicButton;
    private Button _soundButton;

    private ConfigFile _config;

    public override void _Ready()
    {
        _config = new ConfigFile();
        _config.Load(SettingsFilePath);

        Singletons.Settings.Music = (bool)_config.GetValue(SettingsSection, MusicValue, true);
        Singletons.Settings.Sound = (bool)_config.GetValue(SettingsSection, SoundValue, true);

        _musicButton = GetNode<Button>(_musicButtonPath);
        _soundButton = GetNode<Button>(_soundButtonPath);

        _musicButton.Text = $"Music: {(Singletons.Settings.Music ? "ON" : "OFF")}";
        _soundButton.Text = $"Sound: {(Singletons.Settings.Sound ? "ON" : "OFF")}";
    }

    public void MusicButtonPressed()
    {
        Singletons.Settings.Music = !Singletons.Settings.Music;
        _musicButton.Text = $"Music: {(Singletons.Settings.Music ? "ON" : "OFF")}";
        _config.SetValue(SettingsSection, MusicValue, Singletons.Settings.Music);
        _config.Save(SettingsFilePath);
    }

    public void SoundButtonPressed()
    {
        Singletons.Settings.Sound = !Singletons.Settings.Sound;
        _soundButton.Text = $"Sound: {(Singletons.Settings.Sound ? "ON" : "OFF")}";
        _config.SetValue(SettingsSection, SoundValue, Singletons.Settings.Sound);
        _config.Save(SettingsFilePath);
    }
}
