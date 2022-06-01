using Godot;
using System;

public class LevelComplete : CanvasLayer
{
    private Timer displayTimer;
    private AudioStreamPlayer audioFlush;

    public override void _Ready()
    {
        var nextButton = (CustomButton)GetNode("NextLevel");
        nextButton.Connect("pressed", this, "OnNextGame");
        var exitButton = (CustomButton)GetNode("Exit");
        exitButton.Connect("pressed", this, "OnExit");
        audioFlush = (AudioStreamPlayer)GetNode("AudioStreamPlayer");
        var player = (Player)GetTree().GetCurrentScene().GetNode("Player");
        player.HUD.DisableAll(true);
        Input.SetMouseMode(Input.MouseMode.Visible);
    }

    public async void OnNextGame()
    {
        var fadeBlackAnim = (AnimationPlayer)GetNode("ColorRect/AnimationPlayer");
        fadeBlackAnim.Play("fade_to_black");
        audioFlush.Play();
        displayTimer = (Timer)GetNode("DisplayTimer");
        displayTimer.Start();
        await ToSignal(displayTimer, "timeout");
        Game game = (Game)GetNode("/root/Game");
        switch (GetTree().CurrentScene.GetFilename())
        {
            case "res://scenes/bathroom_1/scene_1.tscn":
                game.next_level = "res://scenes/bathroom_2/scene_2.tscn";
                break;
            case "res://scenes/bathroom_2/scene_2.tscn":
                game.next_level = "res://scenes/bathroom_3/scene_3.tscn";
                break;
            case "res://scenes/bathroom_3/scene_3.tscn":
                game.next_level = "res://scenes/bathroom_4/scene_4.tscn";
                break;
            case "res://scenes/bathroom_4/scene_4.tscn":
                game.next_level = "res://scenes/bathroom_4/scene_5.tscn";
                break;
        }
        game.SaveNodes();
        game.BackgroundLoadScene(game.next_level);
    }

    public async void OnExit()
    {
        var fadeBlackAnim = (AnimationPlayer)GetNode("ColorRect/AnimationPlayer");
        fadeBlackAnim.Play("fade_to_black");
        audioFlush.Play();
        displayTimer = (Timer)GetNode("DisplayTimer");
        displayTimer.Start();
        await ToSignal(displayTimer, "timeout");
        Game game = (Game)GetNode("/root/Game");
        game.BackgroundLoadScene("res://scenes/menus/main/MainMenu.tscn");
    }

}
