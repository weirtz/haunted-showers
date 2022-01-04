using Godot;
using System;

public class CleaningTorso : CleaningBodyPart
{

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        player.HUD.ShowScrubbingMotion(true);
        player.SetCameraLock(true);
        player.animTreePlayer.SetTransition("body_state", Player.BodyAnim.ARM_FR_SCRUB);
        player.SetHandState(Player.HandsAnim.RIGHT, false, true);
        eventHandler.QueueFreeEvent<CleaningTorso>(player, "back", this);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (isRunning)
        {
            player.animTreePlayer.SetScrubTimeScale(Player.BodyPartEnum.TORSO, cleaning.scrubAnimSpeed);
            if (!player.bodyProgressValues.torso.isBurned)
                player.bodyProgressValues.torso -= cleaning.scrubRate;
            player.bodyProgressValues.torso.isBurned = burnBodyPart;

            if (player.camera.bodyAreaLookingAt != Player.BodyPartEnum.TORSO)
            {
                _End(eventHandler);
                return;
            }
        }
        else if (alreadyStarted && cleaning.isRunning && !burnBodyPart)
        {
            player.bodyProgressValues.torso.isBurned = burnBodyPart;
        }


    }

    public override void _End(EventHandler eventHandler)
    {
        if (!isRunning)
            return;
        base._End(eventHandler);
        player.SetCameraLock(false);
        player.HUD.ShowScrubbingMotion(false);
        eventHandler.QueueEvent(this, cleaning, "torso_enter");
    }

}
