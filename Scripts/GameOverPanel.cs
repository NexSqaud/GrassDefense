using Godot;

using GrassDefense.Scripts;

using System;

public class GameOverPanel : Panel
{
    public override void _Ready()
    {
        Singletons.GameUtilities.OnLose += Lose;      
    }

    private void Lose(object _, EventArgs args)
    {
        Visible = true;
    }

}
