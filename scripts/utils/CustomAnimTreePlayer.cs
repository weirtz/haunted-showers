using Godot;
using System;

public class CustomAnimTreePlayer : AnimationTreePlayer
{
    [Signal]
    public delegate void anim_finished();

    public void TrySetTransition(string id, Enum @enum)
    {
        Timer timer = (Timer)GetNode("AnimCoolDown");
        if (timer.TimeLeft == 0)
        {
            int idx = Convert.ToInt32(@enum);
            TransitionNodeSetCurrent(id, idx);
            Cooldown();
        }
    }

    public override void _Process(float delta)
    {
        Timer timer = (Timer)GetNode("AnimCoolDown");
        if (timer.TimeLeft == 0)
        {
            EmitSignal("anim_finished");
        }
    }

    public void SetTransition(string id, Enum @enum)
    {
        int idx = Convert.ToInt32(@enum);
        TransitionNodeSetCurrent(id, idx);
    }

    public void SetTransitionCooldown(string id, Enum @enum)
    {
        SetTransition(id, @enum);
        Cooldown();
    }

    public void SetScrubTimeScale(Player.BodyPartEnum bodyPart, float scale)
    {
        var id = "";
        switch (bodyPart)
        {
            case Player.BodyPartEnum.HEAD:
                id = "scale_head_scrub";
                break;
            case Player.BodyPartEnum.ARM_L:
                id = "scale_arm_RL_scrub";
                break;
            case Player.BodyPartEnum.ARM_R:
                id = "scale_arm_LR_scrub";
                break;
            case Player.BodyPartEnum.TORSO:
                id = "scale_arm_FR_scrub";
                break;
            case Player.BodyPartEnum.LEGS:
                id = "scale_bend_scrub";
                break;
        }
        TimescaleNodeSetScale(id, scale);
    }

    public async void QueueSetTransition(string id, Enum @enum)
    {
        int idx = Convert.ToInt32(@enum);
        Timer timer = (Timer)GetNode("AnimCoolDown");
        await ToSignal(timer, "timeout");
        TransitionNodeSetCurrent(id, idx);
        Cooldown();
    }
    
    public void Cooldown()
    {
        Timer timer = (Timer)GetNode("AnimCoolDown");
        timer.Start();
    }
}
