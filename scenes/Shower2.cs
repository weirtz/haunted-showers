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
        playerFacingAngle = 0f;
        soapBar = (MeshInstance)GetNode("soap bar");
        audioPlayer = (AudioStreamPlayer3D)GetNode("AudioStreamPlayer3D");
    }
    public override void _Process(float delta) 
    {

        if (player != null && player.camera.areaLookingAt != null) 
        {
            if (Input.IsActionJustPressed("select") && player.camera.areaLookingAt.IsInGroup("left_click_enter"))
            {
                switch (player.camera.areaLookingAt.Name)
                {
                    case "EnterCollision":
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
                    case "ExitCollision":
                        GD.Print("Do unmount shower");
                        player.UnMountShower(this);
                        break;
                }
            }
        }
        if (player != null && player.camera.bodyLookingAt != null) 
        {
            if (Input.IsActionJustPressed("select") && player.camera.bodyLookingAt.IsInGroup("left_click_interact")) 
            {
                GD.Print(player.camera.bodyLookingAt.Name);
                switch (player.camera.bodyLookingAt.Name)
                {
                    case "CurtainCollision":
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
            //player.MountShower(this, 90f);
        }
    }

    public void OnExit(Godot.Object body) 
    {
        if (body is Player player)
        {
            player.EmitSignal("exited_shower");
            //player.UnMountShower(this);
        }
    }
}
