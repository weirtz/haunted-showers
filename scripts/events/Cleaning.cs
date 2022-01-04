using Godot;
using System;

public class Cleaning : Event
{
   
    [Signal]
    public delegate void legs_enter();
    [Signal]
    public delegate void torso_enter();
    [Signal]
    public delegate void arm_L_enter();
    [Signal]
    public delegate void arm_R_enter();
    [Signal]
    public delegate void head_enter();

    public float scrubRate;

    public float scrubAnimSpeed;
    private const float scrubAnimFactor = 1.5f;
    public const float criticalScrubRate = 0.9f;

    public bool scrubMoving = false;
    private float lastMouseSpeed;

    public Event cleaningLegs;
    public Event cleaningArmLeft;
    public Event cleaningArmRight;
    public Event cleaningHead;
    public Event cleaningTorso;

    public override async void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);

        cleaningLegs = (Event)GetNode("CleaningLegs");
        cleaningArmLeft = (Event)GetNode("CleaningArmLeft");
        cleaningArmRight = (Event)GetNode("CleaningArmRight");
        cleaningHead = (Event)GetNode("CleaningHead");
        cleaningTorso = (Event)GetNode("CleaningTorso");

        eventHandler.QueueFreeEvent<Cleaning>(player, "interact_knob", this);
        player = eventHandler.player;
        player.HUD.ShowCleaningHUD(true);
        player.HUD.ShowPopUpHUD(HUDController.PopUps.PICKUP_SOAP, false);
        player.showerMounted.pickupSound.Play();
        player.HUD.ShowPopUpHUD(HUDController.PopUps.BODY_PART, true);

        player.showerMounted.exitArea.SetCollisionLayerBit(1, false);
        player.showerMounted.exitArea.SetCollisionMaskBit(1, false);

        player.LockPositionWithFacing(true, player.showerMounted.playerFacingAngle);
        player.SlideToPosition(player.showerMounted.mountPos.GlobalTransform.origin);
        player.SetHandState(Player.HandsAnim.RIGHT, false, true);
        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.INTERACT);
        await ToSignal(player.animTreePlayer, "anim_finished");
        player.showerMounted.soapBar.SetVisible(false);

    }

    public override async void _Process(float delta)
    {
        if (isRunning)
        {
            player.HUD.ShowCleaningHUD(true);

            var cleaningAnything = cleaningLegs.isRunning 
                || cleaningTorso.isRunning 
                || cleaningArmLeft.isRunning 
                || cleaningArmRight.isRunning 
                || cleaningHead.isRunning;

            switch (player.camera.bodyAreaLookingAt)
            {
                case Player.BodyPartEnum.LEGS:
                    if (!cleaningAnything)
                        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.BEND_LEGS);
                    if (Input.IsActionJustPressed("select"))
                        EmitSignal("legs_enter");
                    break;
                case Player.BodyPartEnum.TORSO:
                    if (!cleaningAnything)
                        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.ARM_FR);
                    if (Input.IsActionJustPressed("select"))
                        EmitSignal("torso_enter");
                    break;
                case Player.BodyPartEnum.ARM_L:
                    if (!cleaningAnything)
                        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.ARM_RL);
                    if (Input.IsActionJustPressed("select"))
                        EmitSignal("arm_L_enter");
                    break;
                case Player.BodyPartEnum.ARM_R:
                    if (!cleaningAnything)
                        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.ARM_LR);
                    if (Input.IsActionJustPressed("select"))
                        EmitSignal("arm_R_enter");
                    break;
                case Player.BodyPartEnum.HEAD:
                    if (!cleaningAnything)
                        player.animTreePlayer.TrySetTransition("body_state", Player.BodyAnim.ARMS_ABOVE_HEAD);
                    if (Input.IsActionJustPressed("select"))
                        EmitSignal("head_enter");
                    break;
                case Player.BodyPartEnum.NONE:
                    if (Input.IsActionJustPressed("stomp"))
                        player.Stomp();
                    if (!cleaningLegs.isRunning && !cleaningArmLeft.isRunning && !cleaningArmRight.isRunning)
                    {
                        await ToSignal(player.animTreePlayer, "anim_finished");
                    }
                    break;
            }

            SetScrubSpeed(scrubMoving);
        } 
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (isRunning)
        {
            if (@event is InputEventMouseMotion eventMouseMotion)
            {
                var sensitivity = 300f;
                var maxSpeed = 250f;
                scrubMoving = true;
                lastMouseSpeed = Mathf.Sqrt(Mathf.Abs(Mathf.Clamp(eventMouseMotion.Speed.x / GetProcessDeltaTime(), -sensitivity * maxSpeed, sensitivity * maxSpeed)));
                lastMouseSpeed /= sensitivity;
            }

            
        }
    }

    private void SetScrubSpeed(bool moving)
    {
        var tween = (Tween)player.GetNode("PhysicsTween");

        if (!moving)
        {
            scrubRate = Mathf.Lerp(scrubRate, 0f, 0.1f);
        } else
        {
            scrubRate = Mathf.Lerp(scrubRate, lastMouseSpeed, 0.1f);
        }

        scrubAnimSpeed = scrubRate * scrubAnimFactor;
        scrubMoving = false;
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.showerMounted.soapBar.SetVisible(true);
        player.HUD.ShowCleaningHUD(false);
        player.LockPositionWithFacing(false, 0f);
        player.showerMounted.exitArea.SetCollisionLayerBit(1, true);
        player.showerMounted.exitArea.SetCollisionMaskBit(1, true);
        if (player.objectHolding != null)
            player.objectHolding.Drop(player);
        eventHandler.QueueEvent(this, GetNode<Node>(source), signal);
        player.animTreePlayer.SetTransitionCooldown("body_state", Player.BodyAnim.IDLE);
        player.SetHandState(Player.HandsAnim.NONE, false, false);
    }
}
