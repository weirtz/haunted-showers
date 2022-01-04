using Godot;
using System;

public class CleaningHead : CleaningBodyPart
{

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        player.HUD.ShowScrubbingMotion(true);
        player.animTreePlayer.SetTransition("body_state", Player.BodyAnim.HEAD_SCRUB);
        player.SetCameraLock(true);
        player.SetHandState(Player.HandsAnim.BOTH, true, true);
        eventHandler.QueueFreeEvent<CleaningHead>(player, "back", this);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            player.animTreePlayer.SetScrubTimeScale(Player.BodyPartEnum.HEAD, cleaning.scrubAnimSpeed);
            if (!player.bodyProgressValues.head.isBurned)
                player.bodyProgressValues.head -= cleaning.scrubRate;
            player.bodyProgressValues.head.isBurned = burnBodyPart;
            if (player.camera.bodyAreaLookingAt != Player.BodyPartEnum.HEAD)
            {
                _End(eventHandler);
                return;
            }
            if (Input.IsActionJustPressed("back"))
            {
                _End(eventHandler);
                return;
            }
        }
        else if (alreadyStarted && cleaning.isRunning && !burnBodyPart)
        {
            player.bodyProgressValues.head.isBurned = burnBodyPart;
        }

    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.SetCameraLock(false);
        player.HUD.ShowScrubbingMotion(false);
        player.SetHandState(Player.HandsAnim.RIGHT, false, true);
        eventHandler.QueueEvent(this, cleaning, "head_enter");
    }

}
