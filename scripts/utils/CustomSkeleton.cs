using Godot;
using System;

//[Tool] //enable to debug camera position realtime in engine 
public class CustomSkeleton : Skeleton
{
    BoneAttachment boneAttachment;


    public override void _Ready()
    {
        boneAttachment = (BoneAttachment)GetNode("BoneAttachment");
    }

    public override void _Process(float delta)
    {
        var player = GetOwner();
        var pitch = (Spatial)player.GetNode("Yaw/Pitch");
        var trans = pitch.GlobalTransform;
        trans.origin = boneAttachment.GlobalTransform.origin;
        pitch.GlobalTransform = trans;
        pitch.Scale = Vector3.One;
    }
}
