using Godot;
using System;

public class CleaningLegs : CleaningBodyPart
{

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        player.SetCameraLock(true);
        player.HUD.ShowScrubbingMotion(true);

        player.pitchClamped = false;
        player.animTreePlayer.SetTransition("body_state", Player.BodyAnim.BEND_SCRUB);
        player.SetHandState(Player.HandsAnim.BOTH, true, true);
        eventHandler.QueueFreeEvent<CleaningLegs>(player, "back", this);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            player.animTreePlayer.SetScrubTimeScale(Player.BodyPartEnum.LEGS, cleaning.scrubAnimSpeed);
            if (!player.bodyProgressValues.legs.isBurned)
                player.bodyProgressValues.legs -= cleaning.scrubRate;
            player.bodyProgressValues.legs.isBurned = burnBodyPart;
        }
        else if (alreadyStarted && cleaning.isRunning && !burnBodyPart)
        {
            player.bodyProgressValues.legs.isBurned = burnBodyPart;
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        if (!isRunning)
            return;
        base._End(eventHandler);
        player.SetCameraLock(false);
        player.HUD.ShowScrubbingMotion(false);
        player.pitchClamped = true;
        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.IDLE);
        player.SetHandState(Player.HandsAnim.RIGHT, false, true);
        eventHandler.QueueEvent(this, cleaning, "legs_enter");
    }

}
