using Godot;
using System;

public class PauseMenu : CanvasLayer
{
	public override void _Ready()
	{
		var exitButton = (CustomButton)GetNode("PauseMenu/VBox/Exit");
		var settingsButton = (CustomButton)GetNode("PauseMenu/VBox/Settings");
		Input.SetMouseMode(Input.MouseMode.Visible);
		exitButton.Connect("pressed", this, "OnMainMenuButton");
	}

	public void OnMainMenuButton() 
	{
		Game game = (Game)GetNode("/root/Game");
		game.BackgroundLoadScene("res://scenes/menus/main/MainMenu.tscn");
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("escape")) 
		{
			Input.SetMouseMode(Input.MouseMode.Captured);
			QueueFree();
		}
	}

}
