using Godot;
using System;

public class Grabbable : RigidBody
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";
    public Player holder;
    public bool pickedUp;
    public Transform lastPos;
    public string nodeName;
    private Vector3 objectScale;

    public override void _Ready()
    {
        pickedUp = false;
        lastPos = GlobalTransform;
        nodeName = Name;
    }

    public virtual void PickUp(Player player)
    {
        holder = player;
        pickedUp = true;
        objectScale = Scale;
        player.objectHolding = this;
        SetCollisionLayerBit(0, false);
        SetCollisionMaskBit(0, false);
        SetCollisionLayerBit(1, false);
        SetCollisionMaskBit(1, false);
        SetMode(ModeEnum.Static);
    }

    public virtual void Drop(Player player)
    {
        pickedUp = false;
        player.objectHolding = null;
        SetCollisionLayerBit(0, true);
        SetCollisionMaskBit(0, true);
        SetCollisionLayerBit(1, true);
        SetCollisionMaskBit(1, true);
        SetMode(ModeEnum.Rigid);
    }

    public override void _Process(float delta)
    {
        if (pickedUp)
        {
            var target = (Position3D)holder.camera.GetNode("Position3D");     
            GlobalTransform = target.GlobalTransform;
            Scale = objectScale;
            if (Input.IsActionJustPressed("back"))
                Drop(holder);
        }

    }

}
