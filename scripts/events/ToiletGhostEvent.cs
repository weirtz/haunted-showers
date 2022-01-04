using Godot;
using System;

public class ToiletGhostEvent : Event
{
    [Export]
    public NodePath toiletghostPath;
    public Spatial toiletGhost;

    public override void _Ready()
    {
        toiletGhost = (Spatial)GetNode(toiletghostPath);
    }

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        var animationPlayer = (AnimationPlayer)toiletGhost.GetNode("armature/AnimationPlayer");
        animationPlayer.Play("arm_R_pull");
        _End(eventHandler);
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
    }
}
