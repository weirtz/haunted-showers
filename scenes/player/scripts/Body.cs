using Godot;
using System;

public class Body : Spatial
{
    public Player player;

    public override void _Ready()
    {
        player = (Player)GetOwner();
    }

    public override void _Process(float delta)
    {

    }

    public void SetFacingAngle(float angle)
    {
        SetRotationDegrees(new Vector3(0, angle - 90f, 0));
    }
}
