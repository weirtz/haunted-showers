using Godot;
using System;

public class EventTimer : Timer
{
    private EventHandler eventHandler;

    public void ConnectEventHandler(EventHandler eventHandler) 
    {
        this.eventHandler = eventHandler;
    }

    public override async void _Process(float delta)
    {
        await ToSignal(this, "timeout");
        foreach (Node child in GetChildren())
        {
            if (child is Event @event) 
                @event._Start(eventHandler);
        } 
    }
}
