using Godot;
using System;

public class ShowerMounted : Event
{
    private ShowerOn showerOn;
    public const float dirtyRateWhileOff = 0.01f;

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        showerOn = (ShowerOn)GetNode("ShowerOn");
        player.GetDirtier(false);
        player.showerMounted.enterArea.SetCollisionLayerBit(1, false);
        player.showerMounted.enterArea.SetCollisionMaskBit(1, false);
        player.showerMounted.Open(false);
        player.HUD.ShowPopUpHUD(HUDController.PopUps.SHOWER, true);
        eventHandler.QueueFreeEvent<ShowerMounted>(player, "exited_shower", this);
        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.IDLE);
        player.showerMounted.exitArea.SetCollisionLayerBit(1, true);
        player.showerMounted.exitArea.SetCollisionMaskBit(1, true);

    }

    public override void _Process(float delta)
    {
        if (isRunning)
        {
            if (showerOn.isRunning)
            {
                if (Input.IsActionJustPressed("select") && player.camera.areaLookingAt != null)
                {
                    switch (player.camera.areaLookingAt.Name)
                    {
                        case "soap":             
                            player.EmitSignal("pickup_soap");
                            break;
                    }
                }
            }

            if (Input.IsActionJustPressed("select") && player.camera.areaLookingAt != null)
            {
                switch (player.camera.areaLookingAt.Name)
                {
                    case "knob_hot":
                        player.EmitSignal("interact_knob");
                        player.showerMounted.knobSound.Play();
                        break;
                    case "knob_cold":
                        player.EmitSignal("interact_knob");
                        player.showerMounted.knobSound.Play();
                        break;
                    case "knob":
                        player.EmitSignal("interact_knob");
                        player.showerMounted.knobSound.Play();
                        break;
                }
            }
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        eventHandler.QueueEvent(this, GetNode<Node>(source), signal);
        player.HUD.ShowCleaningHUD(false);
        player.GetDirtier(true, dirtyRateWhileOff);
        player.showerMounted.enterArea.SetCollisionLayerBit(1, true);
        player.showerMounted.enterArea.SetCollisionMaskBit(1, true);
        player.showerMounted.exitArea.SetCollisionLayerBit(1, false);
        player.showerMounted.exitArea.SetCollisionMaskBit(1, false);
    }
}
