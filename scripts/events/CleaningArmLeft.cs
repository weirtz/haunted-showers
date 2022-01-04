using Godot;
using System;

public class CleaningArmLeft : CleaningBodyPart
{
    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        player.SetCameraLock(true);
        player.HUD.ShowScrubbingMotion(true);
        player.animTreePlayer.SetTransition("body_state", Player.BodyAnim.ARM_RL_SCRUB);
        player.SetHandState(Player.HandsAnim.RIGHT, false, true);
        eventHandler.QueueFreeEvent<CleaningArmLeft>(player, "back", this);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            player.animTreePlayer.SetScrubTimeScale(Player.BodyPartEnum.ARM_L, cleaning.scrubAnimSpeed);
            if (!player.bodyProgressValues.arm_L.isBurned)
                player.bodyProgressValues.arm_L -= cleaning.scrubRate;
            player.bodyProgressValues.arm_L.isBurned = burnBodyPart;
        }
        else if (alreadyStarted && cleaning.isRunning && !burnBodyPart)
        {
            player.bodyProgressValues.arm_L.isBurned = burnBodyPart;
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.SetCameraLock(false);
        player.HUD.ShowScrubbingMotion(false);
        eventHandler.QueueEvent(this, cleaning, "arm_L_enter");
    }
}
