using Godot;

using GrassDefense.Scripts;

using System;
using System.Collections.Generic;

public class ScoreCounter : Label
{
    [Export] private Godot.Collections.Array<NodePath> _scoreLabelsPaths;

    private List<Label> _scoreLabels = new List<Label>();

    private int _score;
    private float _timeCounter;

    public override void _Ready()
    {
        Singletons.ScoreCounter = this;

        for(int i = 0; i < _scoreLabelsPaths.Count; i++)
        {
            _scoreLabels.Add(GetNode<Label>(_scoreLabelsPaths[i]));
        }
    }

    public override void _ExitTree()
    {
        Singletons.ScoreCounter = null;
    }

    public override void _Process(float delta)
    {
        _timeCounter += delta;

        if (_timeCounter >= 1f && !Singletons.GameUtilities.Lose)
        {
            _timeCounter -= 1f;
            TimeElapsed();
        }

    }

    public void EnemyDefeated()
    {
        _score += 50;
        UpdateScoreLabel();
    }

    public void TimeElapsed()
    {
        _score += 10;
        UpdateScoreLabel();
    }

    private void UpdateScoreLabel()
    {
        for (int i = 0; i < _scoreLabels.Count; i++)
        {
            _scoreLabels[i].Text = $"Score: {_score}";
        }
    }

}
