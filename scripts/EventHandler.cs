using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EventHandler : Node
{
	//Event Singletons
	public Event game;

	public Cleaning cleaning;

	private Event currentEvent;
	public Player player;

	public override void _Ready()
	{
		player = (Player)GetTree().CurrentScene.GetNode("Player");

		//get basic event Nodes
		game = (Event)FindNode("Game");

		//Entry here
		var children = GetChildren();
		foreach (Node child in GetChildren())
		{
			if (child is Event @event)
			{
				GD.Print("Event Handler got Event " + child.Name);
				if (@event.manualStart)
				{
					continue;
				}
				else if (@event.signalStart)
				{
					@event._AwaitStart(this);
				}
				else
				{
					@event._Start(this);
				}
			}
		}

	}

	public override void _Process(float delta)
	{

	}

	public async Task<Event> GetEventOnSignal(Event @event, string signalSourcePath, string signal)
	{
		await ToSignal(GetNode(signalSourcePath), signal);
		return @event;
	}

	public async Task<Event> GetEventOnSignal(Event @event, Node signalSource, string signal)
	{
		await ToSignal(signalSource, signal);
		return @event;
	}


	public void StartEvent<T>(Event @event) where T : Event
	{
		try
		{
			var name = @event.Name;
			((T)@event)._Start(this);
		}
		catch (InvalidCastException e)
		{
			Type type = ((T)@event).GetType();
			GD.Print("Cannot cast to ", @event.Name);
		}
	}

	public void StartEvent(Event @event)
	{
		try
		{
			var name = @event.Name;
			((Event)@event)._Start(this);
		}
		catch (InvalidCastException e)
		{
			Type type = ((Event)@event).GetType();
			GD.Print("Cannot cast to ", @event.Name);
		}
	}

	public void EndEvent<T>(Event @event) where T : Event
	{
		try
		{
			var name = @event.Name;
			((T)@event)._End(this);
		}
		catch (InvalidCastException e)
		{
			Type type = ((T)@event).GetType();
			GD.Print("Cannot cast to ", @event.Name);
		}
	}

	public async void QueueEvent(Event @event, Godot.Object signalSource, string signalName)
	{
		GD.Print("[AWAITSTART]" + @event.Name);
		await ToSignal(signalSource, signalName);
		@event._Start(this);
	}

	public async void QueueFreeEvent<T>(Godot.Object source, string signal, Event @event) where T : Event
	{
		GD.Print("[AWAITSTOP]" + @event.Name);
		await ToSignal(source, signal);
		EndEvent<T>(@event);
	}

	public void NextEvent(string path)
	{
		GD.Print("Add NextEvent ", path);
		currentEvent = (Event)GetNode(path);
	}

	public void NextEvent(Event @event)
	{
		currentEvent = @event;
	}

	public T GetEvent<T>(string path) where T:Event
	{
		T result = null;
		try
		{
			result = (T)((Event)GetNode(path));
		}
		catch (InvalidCastException e)
		{
			GD.Print("Cannot cast to ", GetNode(path).Name);
		}
		return result;
		
	}

	public Event GetEvent(string path)
	{
		return (Event)GetNode(path);
	}
}
