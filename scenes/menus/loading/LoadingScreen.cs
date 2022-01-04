using Godot;
using System;

public class LoadingScreen : Control
{
	public Label loadingLabel;
	Game game;

	public override async void _Ready()
	{
		loadingLabel = (Label)GetNode("CanvasLayer/Label");
		game = (Game)GetNode("/root/Game");
	}

	public void SetLoadingProgress(int value)
	{
		loadingLabel.Text = "Loading (" + value + "%)...";
		var playerAnimIcon = (Control)GetNode("CanvasLayer/PlayerAnimIcon");
		var width = GetViewport().Size.x;
		var newPos = new Vector2(width * (value / 100f), playerAnimIcon.RectPosition.y);
		playerAnimIcon.SetGlobalPosition(newPos);
	}

	public override void _Process(float delta)
	{
		SetLoadingProgress(game.loadProgress);
	}
}
