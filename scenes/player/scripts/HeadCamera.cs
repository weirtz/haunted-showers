using Godot;
using System;

public class HeadCamera : Camera
{
    public Player.BodyPartEnum bodyAreaLookingAt;
    private Player player;
    public Area areaLookingAt;
    public PhysicsBody bodyLookingAt;
    public CollisionObject collisionLookingAt;

    public enum AnimationType
    {
        LOOK_AT_LEGS,
        STOP_LOOK_AT_LEGS,
    }

    public override void _Ready()
    {
        player = (Player)GetOwner();
    }

    public override void _Process(float delta)
    {
        var ray = (RayCast)GetNode("LookingRay");

        //if area is for right or left click interaction - set crosshair
        var crosshair = player.HUD.crosshair;
        if (ray.GetCollider() is CollisionObject interactionObject)
        {
            if (interactionObject.IsInGroup("right_click_interact"))
            {
                var color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                var textureHand = (Texture)ResourceLoader.Load("res://assets/gui/mouse_right_interact.png");
                crosshair.SetScale(Vector2.One * 7.5f);
                crosshair.SetModulate(color);
                crosshair.SetTexture(textureHand);
            }

            if (interactionObject.IsInGroup("left_click_interact"))
            {
                var color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                var textureHand = (Texture)ResourceLoader.Load("res://assets/gui/mouse_left_interact.png");
                crosshair.SetScale(Vector2.One * 7.5f);
                crosshair.SetModulate(color);
                crosshair.SetTexture(textureHand);
            }

            if (interactionObject is Grabbable grabbable)
            {
                if (Input.IsActionJustPressed("select"))
                    grabbable.PickUp(player);
                if (Input.IsActionJustPressed("back"))
                    grabbable.Drop(player);
            }

        } else
        {
            var color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            var textureCrosshair = (Texture)ResourceLoader.Load("res://assets/gui/crosshair.png");
            crosshair.SetScale(Vector2.One *.1f);
            crosshair.SetModulate(color);
            crosshair.SetTexture(textureCrosshair);
        }

        //if area is for right or left click entering/exiting - set crosshair
        if (ray.GetCollider() is CollisionObject enterObject)
        {
            collisionLookingAt = enterObject;

            if (enterObject.IsInGroup("right_click_enter"))
            {
                var color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                var textureHand = (Texture)ResourceLoader.Load("res://assets/gui/shower/mouse_right_enter.png");
                crosshair.SetScale(Vector2.One * 7.5f);
                crosshair.SetModulate(color);
                crosshair.SetTexture(textureHand);
            }

            if (enterObject.IsInGroup("left_click_enter"))
            {
                var color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                var textureHand = (Texture)ResourceLoader.Load("res://assets/gui/shower/mouse_left_enter.png");
                crosshair.SetScale(Vector2.One * 7.5f);
                crosshair.SetModulate(color);
                crosshair.SetTexture(textureHand);
            }
        }
        else
        {
            var color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            var textureCrosshair = (Texture)ResourceLoader.Load("res://assets/gui/crosshair.png");
            crosshair.SetScale(Vector2.One * .1f);
            crosshair.SetModulate(color);
            crosshair.SetTexture(textureCrosshair);
        }

        if (ray.GetCollider() is Area lookingArea)
        {
            areaLookingAt = lookingArea;
        } else 
        {
            areaLookingAt = null;
        }

        if (ray.GetCollider() is PhysicsBody lookingBody)
        {
            bodyLookingAt = lookingBody;
        } else 
        {
            bodyLookingAt = null;
        }

        if (ray.GetCollider() is Area area)
        {
            switch (area.Name)
            {
                case "LegsCollision":
                    bodyAreaLookingAt = Player.BodyPartEnum.LEGS;
                    break;
                case "TorsoCollision":
                    bodyAreaLookingAt = Player.BodyPartEnum.TORSO;
                    break;
                case "HeadCollision":
                    bodyAreaLookingAt = Player.BodyPartEnum.HEAD;
                    break;
                case "ArmsCollision_R":
                    bodyAreaLookingAt = Player.BodyPartEnum.ARM_R;
                    break;
                case "ArmsCollision_L":
                    bodyAreaLookingAt = Player.BodyPartEnum.ARM_L;
                    break;
            }
        } else
        {
            bodyAreaLookingAt = Player.BodyPartEnum.NONE;
        }
        var pitch = (Spatial)GetParent();
    }

    public void BendCameraLooking(float angle)
    {
        var pitch = (Spatial)GetParent();
        var tween = (Tween)player.GetNode("PhysicsTween");
        tween.SetActive(true);
        tween.InterpolateMethod(pitch, "set_rotation_degrees", pitch.GetRotationDegrees(), new Vector3(angle, pitch.GetRotationDegrees().y, pitch.GetRotationDegrees().z), 2f, Tween.TransitionType.Circ, Tween.EaseType.Out); 
        tween.Start();
    }
    public void LookDown()
    {
        var anim = (AnimationPlayer)GetNode("AnimationPlayer");
        anim.Play("angle_down");
    }
    public void PlayAnimation(AnimationType animation)
    {
        var animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        switch (animation)
        {
            case AnimationType.LOOK_AT_LEGS:
                    
                animationPlayer.Play("look_at_legs");
                break;
            case AnimationType.STOP_LOOK_AT_LEGS:
                animationPlayer.Play("look_at_legs",-1,-1, true);
                break;
        }
    }
}
