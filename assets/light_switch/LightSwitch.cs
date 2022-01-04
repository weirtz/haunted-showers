using Godot;
using System;

public class LightSwitch : StaticBody
{
    [Export]
    public NodePath LightgroupPath;
    private LightGroup lightGroup;
    private AudioStreamPlayer3D audioPlayer;
    public AnimationPlayer animationPlayer;
    private Player player;

    public override void _Ready()
    {
        lightGroup = (LightGroup)GetNode(LightgroupPath);
        audioPlayer = (AudioStreamPlayer3D)GetNode("AudioStreamPlayer3D");
        animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        player = (Player)GetTree().CurrentScene.GetNode("Player");
    }

    public override void _Process(float delta)
    {
        if (player.camera.collisionLookingAt != null)
            if (player.camera.collisionLookingAt is LightSwitch lightSwitch)
            {
                if (Input.IsActionJustPressed("select"))
                    Switch();
            }
    }

    private void Switch()
    {
        if (lightGroup.lightsOn)
        {
            lightGroup.AllOff();
            audioPlayer.Play();
            animationPlayer.Play("flip_down", .1f);
        } else
        {
            lightGroup.FlickerAllOn();
            lightGroup.flickerSound.Play();
            audioPlayer.Play();
            animationPlayer.Play("flip_up", .1f);
        }
    }
}
