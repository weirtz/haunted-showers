using Godot;
using System;

public class MelonMask : Grabbable
{
    [Signal]
    public delegate void put_on();

    public override void _Ready()
    {
        base._Ready();
    }

    public override void PickUp(Player player)
    {
        base.PickUp(player);
        holder.SetMelonMask(true);
        SetVisible(false);
        EmitSignal("put_on");
    }

    public override void Drop(Player player)
    {
        base.Drop(player);
        holder.SetMelonMask(false);
        SetVisible(true);
    }
}
