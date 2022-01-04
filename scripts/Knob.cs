using Godot;
using System;

public class Knob : StaticBody
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        animationPlayer = (AnimationPlayer)GetNode("knob/AnimationPlayer");    
    }

    public void TurnLeft()
    {
        animationPlayer.Play("turn_left");
    }

    public void TurnRight()
    {
        animationPlayer.Play("turn_right");
    }

    public void TurnWild()
    {
        animationPlayer.Play("default");
    }
//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
