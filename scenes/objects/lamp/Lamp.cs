using Godot;
using System;

public class Lamp : OmniLight
{
    private Particles flameParticles;

    public override void _Ready()
    {
        flameParticles = (Particles)GetNode("Particles");
    }

    public override void _Process(float delta)
    {
        if (LightEnergy < .01f)
        {
            flameParticles.SetEmitting(false);
        } else
        {
            flameParticles.SetEmitting(true);
        }
    }
}
