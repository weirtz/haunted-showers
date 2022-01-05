using Godot;
using System;

public class Splash : CanvasLayer
{

	public override async void _Ready()
	{
		var timer = (Timer)GetNode("Timer");
	}

	public void _on_Timer_timeout()
	{
		var mainMenu = (PackedScene)ResourceLoader.Load("res://scenes/menus/main/MainMenu.tscn");
			GetTree().ChangeSceneTo(mainMenu);
			this.QueueFree();
	}

}


