using Godot;
using System;

public class HotWaterBurn : Event
{
    [Export]
    public NodePath cleaningEventPath;


    private float timeBeingBurned;
    private float timeSinceBurn;

    public override void _Ready()
    {

    }

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
    }

    public override void _Process(float delta)
    {
        if (isRunning)
        {
            var cleaning = (Cleaning)GetNode(cleaningEventPath);

            if (cleaning.isRunning)
            {
                player.bodyProgressValues.arm_L.isBurned = true;
                player.bodyProgressValues.arm_R.isBurned = true;
                player.bodyProgressValues.head.isBurned = true;
                player.bodyProgressValues.torso.isBurned = true;
                player.bodyProgressValues.legs.isBurned = true;

                timeBeingBurned += delta;
            } else
            {
                timeSinceBurn += delta;
                if (timeSinceBurn > timeBeingBurned)
                {
                    player.bodyProgressValues.arm_L.isBurned = false;
                    player.bodyProgressValues.arm_R.isBurned = false;
                    player.bodyProgressValues.head.isBurned = false;
                    player.bodyProgressValues.torso.isBurned = false;
                    player.bodyProgressValues.legs.isBurned = false;
                    _End(eventHandler);
                }
            }
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        timeBeingBurned = 0f;
        timeSinceBurn = 0f;
    }
}
