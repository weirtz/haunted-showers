using Godot;
using System;

public class MainMenu : Control
{

	private Game game;

	public override void _Ready()
	{
		game = (Game)GetNode("/root/Game");
		game.isFirstLaunch = true;
		game.LoadNodes();
		Input.SetMouseMode(Input.MouseMode.Visible);
		var playButton = (CustomButton)GetNode("CanvasLayer/Container/HBoxContainer");
		playButton.Connect("pressed", this, "OnPlayButton");
		var exitButton = (CustomButton)GetNode("CanvasLayer/Container/HBoxContainer4");
		exitButton.Connect("pressed", this, "OnExitButton");
		var newGameButton = (CustomButton)GetNode("CanvasLayer/Container/HBoxContainer2");
		newGameButton.Connect("pressed", this, "OnNewGame");

		if (game.isFirstLaunch)
		{
			playButton.MouseFilter = MouseFilterEnum.Ignore;
			newGameButton.MouseFilter = MouseFilterEnum.Pass;
		}
		else
		{
			playButton.MouseFilter = MouseFilterEnum.Pass;
			newGameButton.MouseFilter = MouseFilterEnum.Ignore;
		}
	}

	public void OnPlayButton()
	{
		Game game = (Game)GetNode("/root/Game");
		game.BackgroundLoadScene(game.next_level);
		
	}

	public void OnNewGame()
	{
		Game game = (Game)GetNode("/root/Game");
		game.next_level = "res://scenes/bathroom_1/scene_1.tscn";
		game.isFirstLaunch = false;
		game.SaveNodes();
		game.BackgroundLoadScene(game.next_level);
	}

	public void OnSettingsButton()
	{

	}

	public void OnExitButton()
	{
		GetTree().Quit();
	}

}
