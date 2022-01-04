using Godot;
using System;

public class RandomTimer : Timer
{
    [Export]
    public int minWaitTime;
    [Export]
    public int maxWaitTime;

    public override void _Ready()
    {
        WaitTime = GetNewTime();
    }

    public override async void _Process(float delta)
    {
        await ToSignal(this, "timeout");
        WaitTime = GetNewTime();
    }

    public float GetNewTime()
    {
        return new Random().Next(minWaitTime, maxWaitTime);
    }
}
