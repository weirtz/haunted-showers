using Godot;
using System;

public class GameEvent : Event
{
    public bool completed;
    public bool paused;

    [Signal]
    public delegate void halfway_progress();

    public override void _Ready()
    {
        completed = false;
        paused = false;
    }

    public override void _Start(EventHandler eventHandler) 
    {
        base._Start(eventHandler);
    }

    public override void _Process(float delta) 
    {
        if (isRunning) 
        {
            if (Input.IsActionJustPressed("escape")) 
            {
                if (!paused) 
                {
                    paused = true;
                    var pauseScene = (PackedScene)ResourceLoader.Load("res://scenes/menus/ingame/PauseMenu.tscn");
                    var pauseMenu = (CanvasLayer)pauseScene.Instance();
                    player.HUD.DisableAll(true);
                    player.HUD.AddChild(pauseMenu);
                }
                else 
                {
                    paused = false;
                    player.HUD.DisableAll(false);
                }

            }

            var head = false;
            var armL = false;
            var armR = false;
            var torso = false;
            var legs = false;
            
            if (player.bodyProgressValues.head.dirtiness <= 0)
                head = true;
            else
                head = false;
           
            if (player.bodyProgressValues.torso.dirtiness <= 0)
                torso = true;
            else
                torso = false;
            
            if (player.bodyProgressValues.arm_L.dirtiness <= 0)
                armL = true;
            else
                armL = false;
            
            if (player.bodyProgressValues.arm_R.dirtiness <= 0)
                armR = true;
            else
                armR = false;

            if (player.bodyProgressValues.legs.dirtiness <= 0)
                legs = true;
            else
                legs = false;
            
            completed = (head && torso && armL && armR && legs);

            var progress = player.bodyProgressValues;
            var total = progress.head.dirtiness + progress.torso.dirtiness + progress.arm_L.dirtiness + progress.arm_R.dirtiness + progress.legs.dirtiness;
            var average = total / 5f;
            if (average <= 50f)
                EmitSignal("halfway_progress");

            if (completed) 
            {
                _End(eventHandler);
            }
        }     
    }

    public override void _End(EventHandler eventHandler) 
    {
        base._End(eventHandler);
        var levelCompleteScene = (PackedScene)ResourceLoader.Load("res://scenes/menus/ingame/LevelComplete.tscn");
        GetTree().GetCurrentScene().AddChild(levelCompleteScene.Instance());
    }
}
