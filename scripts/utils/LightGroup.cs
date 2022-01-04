using Godot;
using System;

public class LightGroup : Spatial
{

    [Export]
    public float LightEnergy;
    private AnimationPlayer lightsAnimation;
    public AudioStreamPlayer3D flickerSound;
    public bool lightsOn;

    public override void _Ready()
    {
        lightsOn = true;
        flickerSound = (AudioStreamPlayer3D)GetNode("light_flicker");
        lightsAnimation = (AnimationPlayer)GetNode("AnimationPlayer");
    }

    public override void _Process(float delta)
    {
        foreach (Node child in GetChildren())
        {
            if (child is OmniLight light)
                light.LightEnergy = LightEnergy;
        }
    }
    
    public void FlickerAllOff()
    {
        lightsOn = false;
        lightsAnimation.Play("lights_off");
    }

    public void AllOff()
    {
        lightsOn = false;
        UpdateLight(0f);
    }

    public void FlickerAllOn()
    {
        lightsOn = true;
        lightsAnimation.PlayBackwards("lights_off");
    }

    public void UpdateLight(float value)
    {
        LightEnergy = value;
    }

}
