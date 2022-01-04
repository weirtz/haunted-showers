using Godot;
using System;

public class Splash : CanvasLayer
{

    public override async void _Ready()
    {
        var animPlayer = (AnimationPlayer)GetNode("Cover/AnimationPlayer");
        var timer = (Timer)GetNode("Timer");
        var audioPlayer = (AudioStreamPlayer)GetNode("AudioStreamPlayer");
        timer.Connect("timeout", this, "OnTimerTimeout");
        animPlayer.Play("logo_reveal");
    }

    public void OnTimerTimeout() 
    {
        var mainMenu = (PackedScene)ResourceLoader.Load("res://scenes/menus/main/MainMenu.tscn");
        GetTree().ChangeSceneTo(mainMenu);
        this.QueueFree();
    }
}
