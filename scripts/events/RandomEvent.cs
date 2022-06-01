using Godot;
using System;

public class RandomEvent : Event
{

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        if (GetChildCount() <= 1)
        {
            _End(eventHandler);
            return;
        }
        int index;
        Node randomChild;

        //do-while makes sure that it will eventually pick an event to start, not something else like a timer
        do
        {
            index = new Random().Next(0, GetChildCount());
            randomChild = GetChild(index);
        }
        while (!(randomChild is Event));

        var @event = (Event)randomChild;
        @event._Start(eventHandler);
        _End(eventHandler);
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        eventHandler.QueueEvent(this, GetNode(source), signal);
    }

}
