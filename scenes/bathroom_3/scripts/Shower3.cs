using Godot;
using System;
using System.Threading.Tasks;

public class Shower3 : BaseShower
{

    public override void _Ready()
    {
        mountPos = (Position3D)GetNode("MountPos");
        unMountPos = (Position3D)GetNode("UnMountPos");
        collision = (StaticBody)GetNode("StaticBody");
        player = (Player)GetTree().CurrentScene.GetNode("Player");
        enterArea = (Area)GetNode("EnterCollision");
        exitArea = (Area)GetNode("ExitCollision");
        particles = (Particles)GetNode("ShowerParticles");
        soapBar = (MeshInstance)GetNode("soap bar");
        showerRunningSound = (AudioStreamPlayer3D)GetNode("sounds/shower_running");
        knobSound = (AudioStreamPlayer3D)GetNode("sounds/knob_1");
        enterSound = (AudioStreamPlayer3D)GetNode("sounds/step_in");
        pickupSound = (AudioStreamPlayer3D)GetNode("sounds/pickup");
        openSound = (AudioStreamPlayer3D)GetNode("sounds/curtain_open");
    }

    public override void _Process(float delta) 
    {

        if (player != null && player.camera.collisionLookingAt != null) 
        {
            if (Input.IsActionJustPressed("select") && player.camera.collisionLookingAt.IsInGroup("left_click_enter"))
            {
                switch (player.camera.collisionLookingAt.Name)
                {
                    case "EnterCollision":
                        GD.Print("Do mount shower");
                        enterSound.Play();
                        player.MountShower(this, playerFacingAngle);
                        break;
                }
            }
            if (Input.IsActionJustPressed("back") && player.camera.collisionLookingAt.IsInGroup("right_click_enter"))
            {
                switch (player.camera.collisionLookingAt.Name)
                {
                    case "ExitCollision":
                        GD.Print("Do unmount shower");
                        player.UnMountShower(this);
                        break;
                }
            }
        }
         
    }
}
