using Godot;

using GrassDefense.Scripts;

using System;

public class SpawnPointRegister : Node
{ 
    public override void _Ready()
    {
        var points = GetChildren();
        foreach(var point in points)
        {
            if(point is Node2D node)
            {
                Singletons.EnemiesPool.AddSpawnPoint(node.GlobalPosition);
            }
        }
    }

}
