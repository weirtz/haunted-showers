using Godot;
using System;

public class DrainHandEvent : Event
{
	[Export]
	public NodePath drainhandPath;
	[Export]
	public float creepSpeed;
	[Signal]
	public delegate void grabbed_player();
	public Spatial drainHand;
	public AnimationPlayer animationPlayer;
	public float handReach;
	private float lastAnimTime;

	public override void _Ready()
	{
		drainHand = (Spatial)GetNode(drainhandPath);
		animationPlayer = (AnimationPlayer)drainHand.GetNode("Scene Root/AnimationPlayer");
		lastAnimTime = 0f;
	}

	public override void _Start(EventHandler eventHandler) 
	{
		base._Start(eventHandler);
		eventHandler.QueueFreeEvent<DrainHandEvent>(player, "stomp", this);
		animationPlayer.Play("shrink", 1f, creepSpeed, false);
		drainHand.SetVisible(true);
	}

	public override void _Process(float delta)
	{
		if (isRunning)
		{
			if (animationPlayer.GetCurrentAnimation() == "shrink")
				handReach = animationPlayer.GetCurrentAnimationPosition() / animationPlayer.GetCurrentAnimationLength();
			lastAnimTime = animationPlayer.GetCurrentAnimationPosition();
			
			if (handReach > .95f)
			{
				animationPlayer.Play("grab", 1f, 1f, false);
				EmitSignal("grabbed_player");
			}

			var cleaning = (Cleaning)GetParent();
			if (!cleaning.isRunning)
				EndNoStart(eventHandler);
		}
	}
	
	public void EndNoStart(EventHandler eventHandler)
	{
		base._End(eventHandler);
		animationPlayer.Play("shrink", 3f, -.1f, true);
	}

	public override async void _End(EventHandler eventHandler)
	{
		base._End(eventHandler);
		animationPlayer.Play("shrink", 1f, -2f, true);
		await ToSignal(animationPlayer, "animation_finished");
		_Start(eventHandler);
	}
}
