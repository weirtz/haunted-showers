using Godot;
using System;

public class SoundEvent : Event
{
    [Export]
    public NodePath audioPlayerSource;
    public AudioStreamPlayer3D audioPlayer;

    public override void _Ready()
    {
        audioPlayer = GetNode<AudioStreamPlayer3D>(audioPlayerSource);
    }

    public override async void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        if (!audioPlayer.IsPlaying())
            audioPlayer.Play();
        await ToSignal(audioPlayer, "finished");
        _End(eventHandler);
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        audioPlayer.Stop();
        if (signal == "" || signal == "Null" || signal == null)
            return;
        eventHandler.QueueEvent(this, GetNode(source), signal);
    }
}
