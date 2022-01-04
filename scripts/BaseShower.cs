using Godot;
using System;

public class BaseShower : Spatial
{
    public Position3D mountPos;
    public Position3D unMountPos;
    public bool opened;
    public Area enterArea;
    public Area exitArea;
    public Player player;
    public StaticBody collision;
    public MeshInstance soapBar;
    public Particles particles;
    [Export]
    public float playerFacingAngle;
    public StaticBody openCloseCollision;
    public AudioStreamPlayer3D showerRunningSound;
    public AudioStreamPlayer3D enterSound;
    public AudioStreamPlayer3D openSound;
    public AudioStreamPlayer3D pickupSound;
    public AudioStreamPlayer3D knobSound;



    public override void _Ready()
    {

    }
    public virtual void Open(bool state)
    {

    }

}
