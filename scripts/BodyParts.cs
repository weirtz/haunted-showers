using Godot;
using System;

public class BodyParts : MeshInstance
{
    public Player player;
    public Tween tween;
    private Events events;

    public override void _Ready()
    {
        player = (Player)GetNode("/root/Main/Player");
        tween = (Tween)GetNode("Tween");
        events = (Events)GetNode("/root/Main/Events");
    }

    public override void _Process(float delta)
    {
		if (!events.cleaningEvent.isCleaning)
        {
            SetRotation(player.yaw.Rotation);
        }
    }

}
