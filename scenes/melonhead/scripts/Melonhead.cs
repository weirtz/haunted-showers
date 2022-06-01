using Godot;
using System;

public class Melonhead : KinematicBody
{
    public CustomAnimTreePlayer animTreePlayer;
    public Tween moveTween;
    public Position3D forward;
    public RayCast ray;
    private AnimationPlayer lookingScan;

    public override void _Ready()
    {
        animTreePlayer = (CustomAnimTreePlayer)GetNode("body/AnimationTreePlayer");
        moveTween = (Tween)GetNode("MoveTween");
        ray = (RayCast)GetNode("LookingRay");
        forward = (Position3D)ray.GetNode("Forward");
        lookingScan = (AnimationPlayer)GetNode("LookingRay/AnimationPlayer");
    }

    public override void _Process(float delta)
    {
        int randomFacing = new Random().Next(-1, 2);

        if (ray.GetCollider() is KinematicBody kinematicBody)
        {
            if (kinematicBody is Player player)
            {
                animTreePlayer.TrySetTransition("master_state", BodyAnimEnum.STAND_UP);
                TurnHeading(randomFacing, true);
                MoveForward(5);
                ChargePlayer(player);
            }
            else
            {
                animTreePlayer.TrySetTransition("master_state", BodyAnimEnum.WALK);

                TurnHeading(randomFacing, false);
                MoveForward(2);
            }
        }
        else
        {
            animTreePlayer.TrySetTransition("master_state", BodyAnimEnum.WALK);
            TurnHeading(randomFacing, false);
            MoveForward(2);
        }



    }

    public void ChargePlayer(Player player)
    {
        animTreePlayer.SetTransitionCooldown("master_state", BodyAnimEnum.STAND_UP_ACTION);
        animTreePlayer.TrySetTransition("tongue_state", BodyAnimEnum.TONGUE_WIGGLE);
        animTreePlayer.TimescaleNodeSetScale("charge_arms_speed", (float)(new Random().Next(1,5)));
        var scale = Scale;
        LookAt(player.GlobalTransform.origin, Vector3.Up);
        Scale = scale;
    }

    public void MoveForward(float speed)
    {
        MoveAndSlide(-GlobalTransform.basis.z * speed, Vector3.Up);
    }

    public async void TurnHeading(float angle, bool seesPlayer)
    {

        var tween = (Tween)GetNode("RotateTween");

        var frontRay = (RayCast)GetNode("CollisionRays/FrontRay");

        //Turn around if wall in front
        int randomDirection = new Random().Next(0, 2);
        if (frontRay.IsColliding() && frontRay.GetCollider() is StaticBody staticBody)
        {
            if (randomDirection > 0)
                angle = Mathf.Pi;
            else
                angle = -Mathf.Pi;
        }

        if (seesPlayer)
        {
            tween.StopAll();
            tween.SetActive(false);
            lookingScan.Stop();
            return;
        }
        if (tween.IsActive())
            return;
        if (!lookingScan.IsPlaying())
            lookingScan.Play("scan");

        var originalRotation = Rotation;
        var finalRotation = new Vector3(0, originalRotation.y + angle, 0);
        tween.SetActive(true);
        tween.InterpolateMethod(this, "set_rotation", originalRotation, finalRotation, 2f, Tween.TransitionType.Cubic, Tween.EaseType.In);
        tween.Start();
        await ToSignal(tween, "tween_completed");
        tween.SetActive(false);
        
    }

    public enum BodyAnimEnum
    {
        IDLE            = 0,
        WALK            = 1,
        TONGUE_WIGGLE       = 1,
        STAND_UP        = 2,
        STAND_UP_ACTION = 3,
    }

    public enum WalkEnum
    {
        FORWARD,
        BACK,
        LEFT,
        RIGHT,
    }
}
