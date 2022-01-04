using Godot;
using System;

public class TestShower : BaseShower
{

    public override void _Ready()
    {
        mountPos = (Position3D)GetNode("mount_pos");
        unMountPos = (Position3D)GetNode("unmount_pos");
        enterArea = (Area)GetNode("enterArea");
        exitArea = (Area)GetNode("exitArea");
        soapBar = (MeshInstance)GetNode("soap bar");
        showerRunningSound = (AudioStreamPlayer3D)GetNode("sounds/shower_running");
        particles = (Particles)GetNode("ShowerParticles");
        playerFacingAngle = 0f;
        player = (Player)GetTree().CurrentScene.GetNode("Player");
        collision = (StaticBody)GetNode("StaticBody");
        knobSound = (AudioStreamPlayer3D)GetNode("sounds/knob_1");
        enterSound = (AudioStreamPlayer3D)GetNode("sounds/step_in");
        pickupSound = (AudioStreamPlayer3D)GetNode("sounds/pickup");
        openSound = (AudioStreamPlayer3D)GetNode("sounds/curtain_open");
    }

    public override void _Process(float delta) 
    {

        if (player != null && player.camera.areaLookingAt != null) 
        {
            if (Input.IsActionJustPressed("select") && player.camera.areaLookingAt.IsInGroup("left_click_enter"))
            {
                switch (player.camera.areaLookingAt.Name)
                {
                    case "enterArea":
                        GD.Print("Do mount shower");
                        player.MountShower(this, 90f);
                        break;
                }
            }
            if (Input.IsActionJustPressed("back") && player.camera.areaLookingAt.IsInGroup("right_click_enter"))
            {
                GD.Print("Name : " + player.camera.areaLookingAt.Name);
                switch (player.camera.areaLookingAt.Name)
                {
                    case "exitArea":
                        GD.Print("Do unmount shower");
                        player.UnMountShower(this);
                        break;
                }
            }
        }
    }
}
