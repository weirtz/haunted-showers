using Godot;
using System;

public class Shower2 : BaseShower
{

    public override void _Ready()
    {
        mountPos = (Position3D)GetNode("MountPos");
        unMountPos = (Position3D)GetNode("UnMountPos");
        player = (Player)GetTree().CurrentScene.GetNode("Player");
        enterArea = (Area)GetNode("EnterCollision");
        exitArea = (Area)GetNode("ExitCollision");
        particles = (Particles)GetNode("ShowerParticles");
        enterArea.Connect("body_entered", this, "OnEnter");
        enterArea.Connect("body_exited", this, "OnExit");
        openCloseCollision = (StaticBody)GetNode("curtain/CurtainCollision");
        soapBar = (MeshInstance)GetNode("soap bar");
        showerRunningSound = (AudioStreamPlayer3D)GetNode("sounds/shower_running");
        knobSound = (AudioStreamPlayer3D)GetNode("sounds/knob_1");
        enterSound = (AudioStreamPlayer3D)GetNode("sounds/step_in");
        pickupSound = (AudioStreamPlayer3D)GetNode("sounds/pickup");
        openSound = (AudioStreamPlayer3D)GetNode("sounds/curtain_open");
    }
    public override void _Process(float delta) 
    {
        if (player != null && player.camera.bodyLookingAt != null) 
        {
            if (Input.IsActionJustPressed("select") && player.camera.bodyLookingAt.IsInGroup("left_click_interact")) 
            {
                switch (player.camera.bodyLookingAt.Name)
                {
                    case "CurtainCollision":
                        openSound.Play();
                        Open(!opened);
                        break;
                }
            }
        }
    }

    public override void Open(bool state)
    {
        var anim = (AnimationPlayer)GetNode("curtain/AnimationPlayer");
        if (!anim.IsPlaying())
        {
            if (state)
            {
                opened = true;
                anim.Play("open");
            }
            else
            {
                opened = false;
                anim.Play("open", -1, -1, true);
            }
        }
        
    }
    
    public void OnEnter(Godot.Object body) 
    {
        if (body is Player player)
        {
            player.showerMounted = this;
            player.EmitSignal("entered_shower");
        }
    }

    public void OnExit(Godot.Object body) 
    {
        if (body is Player player)
        {
            player.EmitSignal("exited_shower");
        }
    }
}
