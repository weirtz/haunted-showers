using Godot;
using System;

public class Event : Node
{
    [Export]
    public bool manualStart;
    [Export]
    public bool signalStart;
    [Export]
    public string signal;
    [Export]
    public NodePath source;

    public bool isRunning;
    public Player player;
    public EventHandler eventHandler;

    public override void _Ready()
    {
        isRunning = false;
    }

    public virtual async void _AwaitStart(EventHandler eventHandler)
    {
        if (source == "" || source == null || source == "Null")
            source = "..";

        GD.Print("[AWAITSTART]" + Name);

        this.eventHandler = eventHandler;
        player = eventHandler.player;
        await ToSignal(GetNode<Node>(source), signal);
        if (!isRunning)
            _Start(eventHandler);
    }

    public virtual void _Start(EventHandler eventHandler)
    {
        if (isRunning)
            return;
        GD.Print("[START]" + this.Name);
        this.eventHandler = eventHandler;
        isRunning = true;
        player = eventHandler.player;
        
        foreach (Node child in GetChildren())
        {
            if (child is Timer timer)
            {
                if (timer is EventTimer eventTimer)
                    eventTimer.ConnectEventHandler(eventHandler);          
                timer.Start();
            }
            if (child is Event @event)
            {
                if (@event.manualStart)
                {
                    continue;
                }
                else if (@event.signalStart)
                {
                    @event._AwaitStart(eventHandler);
                }
                else
                {
                    @event._Start(eventHandler);
                }
            }
        }
    }

    public virtual void _End(EventHandler eventHandler)
    {
        if (!isRunning)
            return;
        GD.Print("[End]" + this.Name);
        isRunning = false;
    }

    public Event GetEvent(string path)
    {
        return (Event)GetNode(path);
    }

    public Event GetEventFromRoot(string path)
    {
        return (Event)GetOwner().GetNode(path);
    }

    public bool Is(string name)
    {
        if (Name.Equals(name))
        {
            return true;
        } else
        {
            return false;
        }
    }
}
