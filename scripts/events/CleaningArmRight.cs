using Godot;
using System;

public class CleaningArmRight : CleaningBodyPart
{
    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        player.SetCameraLock(true);
        player.HUD.ShowScrubbingMotion(true);
        player.animTreePlayer.SetTransition("body_state", Player.BodyAnim.ARM_LR_SCRUB);
        player.SetHandState(Player.HandsAnim.LEFT, true, false);
        eventHandler.QueueFreeEvent<CleaningArmRight>(player, "back", this);

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            player.animTreePlayer.SetScrubTimeScale(Player.BodyPartEnum.ARM_R, cleaning.scrubAnimSpeed);
            if (!player.bodyProgressValues.arm_R.isBurned)
                player.bodyProgressValues.arm_R -= cleaning.scrubRate;
            player.bodyProgressValues.arm_R.isBurned = burnBodyPart;
        }
        else if (alreadyStarted && cleaning.isRunning && !burnBodyPart)
        {
            player.bodyProgressValues.arm_R.isBurned = burnBodyPart;
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.SetCameraLock(false);
        player.HUD.ShowScrubbingMotion(false);
        eventHandler.QueueEvent(this, cleaning, "arm_R_enter");
    }
}
