using Godot;
using System;



public class LightsOffEvent : Event
{
    [Export]
    public NodePath lightgroupPath;

    public override void _Ready()
    {

    }

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        var lights = (LightGroup)GetNode(lightgroupPath);
        var sound = (AudioStreamPlayer3D)lights.GetNode("light_flicker");
        if (lights.lightsOn) 
        {
            lights.FlickerAllOff();
            sound.Play();
        }
        _End(eventHandler);
    }
}
