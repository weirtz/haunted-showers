using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody
{
	[Signal]
	public delegate void pickup();
	[Signal]
	public delegate void drop();
	[Signal]
	public delegate void back();
	[Signal]
	public delegate void interact_knob();
	[Signal]
	public delegate void pickup_soap();
	[Signal]
	public delegate void put_down_soap();
	[Signal]
	public delegate void random_event();
	[Signal]
	public delegate void entered_shower();
	[Signal]
	public delegate void exited_shower();
	[Signal]
	public delegate void stomp();

	public HeadCamera camera;
	private Spatial pitch;
	public const float lookSensitivity = 10f;
	public Spatial yaw;
	const float gravity = 2400.0f;
	public Vector3 velocity;
	const float jumpSpeed = 700f;
	private float forwardSpeed = 0f;
	private float sideSpeed = 0f;
	public const float maxSpeed = 20f;
	public bool isWalking;
	public Tween tween;
	private Vector2 mouseAngle;
	private Vector3 jumpVelocity;
	private float cameraPitch;
	public bool pitchClamped;
	public bool yawLocked;
	private bool movementLocked;
	private Skeleton skeleton;
	private Vector3 handRelPlayer;
	private Vector3 globalHandPos;
	private bool isMounting;
	private Body body;
	public Grabbable objectHolding;
	public int money;
	public CustomAnimTreePlayer animTreePlayer;
	public BaseShower showerMounted;
	public bool gravityDisabled;
	public bool pitchLocked;
	public BodyProgressValues bodyProgressValues;
	public HUDController HUD;
	private bool hasMelonMaskOn;
	private bool debugMode = false;
	private Vector3 intitialRotation;
	private bool gettingDirtier;
	private float dirtyRate = 0.05f;

	public override void _Ready()
	{
		pitch = GetNode("Yaw/Pitch") as Spatial;
		camera = (HeadCamera)(GetNode("Yaw/Pitch/Camera"));
		body = (Body)GetNode("Body");
		yaw = GetNode("Yaw") as Spatial;
		HUD = (HUDController)GetNode("HUD");
		Input.SetMouseMode(Input.MouseMode.Captured);
		tween = GetNode("WalkSpeed") as Tween;
		pitchClamped = true;
		pitchLocked = false;
		yawLocked = false;
		movementLocked = false;
		isMounting = false;
		skeleton = (Skeleton)GetNode("Body/Armature/metarig/Skeleton");
		handRelPlayer = skeleton.GetBoneGlobalPose(29).origin;
		animTreePlayer = (CustomAnimTreePlayer)GetNode("Body/Armature/AnimationTreePlayer");
		gravityDisabled = false;
		intitialRotation = RotationDegrees;
		gettingDirtier = false;

		bodyProgressValues = new BodyProgressValues
		(
		new BodyPart(BodyPartEnum.HEAD), 
		new BodyPart(BodyPartEnum.TORSO), 
		new BodyPart(BodyPartEnum.ARM_L), 
		new BodyPart(BodyPartEnum.ARM_R), 
		new BodyPart(BodyPartEnum.LEGS)
		);

		Connect("interact_knob", this, "KnobCallback");
	}

	public void KnobCallback()
	{
		GD.Print("KnobCallback");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if (pitchLocked && yawLocked)
				return;
			mouseAngle.x = -eventMouseMotion.Relative.x * (lookSensitivity / 10000);
			mouseAngle.y = -eventMouseMotion.Relative.y * (lookSensitivity / 100);
			if (!pitchLocked)
				cameraPitch -= mouseAngle.y;

			if (pitchClamped)
				cameraPitch = Mathf.Clamp(cameraPitch, -110, 120);
			else
				cameraPitch = Mathf.Clamp(cameraPitch, 95, 120);
			//Change zoom in 
			camera.Fov = Mathf.Clamp(145-cameraPitch, 45, 55);
			//rotate camera
			if (!yawLocked)
				yaw.RotateObjectLocal(Vector3.Up, mouseAngle.x * (lookSensitivity / 10));
				
			pitch.SetRotationDegrees(new Vector3(cameraPitch, 0, 0));

			mouseAngle = Vector2.Zero;

			
		}

	}

	public override void _Process(float delta)
	{
		((Position3D)GetNode("PositionRelPlayer")).GlobalTransform = ((Position3D)camera.GetNode("Position3D")).GlobalTransform;
		if (!movementLocked)
			body.SetFacingAngle(yaw.RotationDegrees.y);

		if (Input.IsActionJustPressed("noclip"))
			ToggleDebugMode();

		//makes player dirtier when wearing mask or out of shower
		if (gettingDirtier)
		{
			bodyProgressValues += new BodyProgressValues(dirtyRate, dirtyRate, dirtyRate, dirtyRate, dirtyRate);
		}
		SetBurningPartsRed(bodyProgressValues);

	}

	public void GetDirtier(bool makeDirty, float customRate)
	{
		gettingDirtier = makeDirty;
		dirtyRate = customRate;
	}

	public void GetDirtier(bool makeDirty)
	{
		gettingDirtier = makeDirty;
	}

	private void SetBurningPartsRed(BodyProgressValues bodyProgress)
	{
		var burnAnimationTree = (AnimationTreePlayer)GetNode("BurnAnimationTreePlayer");
		var burnAnimation = (AnimationPlayer)GetNode("BurnAnimationTreePlayer/BurnAnimation");

		string anim = "";

		if (bodyProgressValues.arm_L.isBurned)
			anim += 2;
		if (bodyProgressValues.arm_R.isBurned)
			anim += 3;
		if (bodyProgressValues.torso.isBurned)
			anim += 4;
		if (bodyProgressValues.legs.isBurned)
			anim += 5;
		if (anim != "")
		{
			burnAnimationTree.AnimationNodeSetAnimation("current_anim", burnAnimation.GetAnimation(anim));
			burnAnimationTree.TransitionNodeSetCurrent("transition", 1);
		}
		else
		{
			burnAnimationTree.TransitionNodeSetCurrent("transition", 0);
		}
	}

	public void Stomp()
	{
		animTreePlayer.SetTransitionCooldown("body_state", BodyAnim.CLIMB_OVER);
		animTreePlayer.QueueSetTransition("body_state", BodyAnim.IDLE);
		EmitSignal("stomp");
	}

	public override void _PhysicsProcess(float delta)
	{
		//walking
		var forwardDirection = -yaw.GlobalTransform.basis.z;
		var sideDirection = -yaw.GlobalTransform.basis.z.Cross(Vector3.Down);
		velocity = (forwardDirection * forwardSpeed) + (sideDirection * sideSpeed);

		//forward
		SmoothSpeedUpOnAction("forwardSpeed", -maxSpeed, "move_forward");
		//backward
		SmoothSpeedUpOnAction("forwardSpeed", maxSpeed, "move_backward");
		//left
		SmoothSpeedUpOnAction("sideSpeed", -maxSpeed, "move_left");
		//right
		SmoothSpeedUpOnAction("sideSpeed", maxSpeed, "move_right");
		//end walking

		if (Input.IsActionJustPressed("back"))
		{
			EmitSignal("back");
		}

		if (Input.IsActionJustPressed("debug_tokens"))
		{
			money++;
		} 
		
		if (Input.IsActionJustPressed("load"))
		{
			Game game = (Game)GetNode("/root/Game");
			game.LoadNodes();
		}
		
		if (Input.IsActionJustPressed("ui_up"))
		{
			EmitSignal("random_event");
			
		}

		if (Input.IsActionJustPressed("save"))
		{
			Game game = (Game)GetNode("/root/Game");
			game.SaveNodes();
		}
		
		var motion = jumpVelocity * delta;
		MoveAndSlide(motion, Vector3.Up);
		if (!IsOnFloor() && !gravityDisabled)
		{
			jumpVelocity.y -= delta * gravity;
		} else if (gravityDisabled)
		{
			jumpVelocity.y = 0;
		}

		if (Input.IsActionPressed("noclip_up") && debugMode)
		{
			jumpVelocity.y = jumpSpeed;
		}

		if (Input.IsActionPressed("noclip_down") && debugMode)
		{
			jumpVelocity.y -= jumpSpeed;
		}

		//Play walk anim with speed from this script
		animTreePlayer.SetActive(true);
		if (!isMounting)
			DoWalkingAnim(sideSpeed, forwardSpeed, maxSpeed);

		//apply movement
		MoveAndSlide(velocity * delta * 60f);
	}

	private void SmoothSpeedUpOnAction(string speedProperty, float velocity, string inputAction)
	{
		if (!movementLocked)
		{
			var speed = (float)Get(speedProperty);
			if (Input.IsActionJustPressed(inputAction))
			{
				tween.SetActive(true);
				tween.InterpolateProperty(this, speedProperty, speed, velocity, .25f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
				tween.Start();
			}
			if (Input.IsActionJustReleased(inputAction))
			{
				tween.SetActive(true);
				tween.InterpolateProperty(this, speedProperty, speed, 0, .25f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
				tween.Start();
			}
		}
	} 

	public void SetCameraLock(bool locked)
	{
		if (locked)
		{
			pitchLocked = true;
			yawLocked = true;
		} else
		{
			pitchLocked = false;
			yawLocked = false;
		}
	}

	public void SetMelonMask(bool on)
	{
		hasMelonMaskOn = on;
		var previousRate = dirtyRate;

		if (on)
		{
			previousRate = dirtyRate;
			GetDirtier(true, 0.05f);
			HUD.ShowMelonOverlay(true);
			SetCollisionLayerBit(10, false);
			SetCollisionMaskBit(10, false);
		}
		else
		{
			HUD.ShowMelonOverlay(false);
			GetDirtier(true, ShowerMounted.dirtyRateWhileOff);
			SetCollisionLayerBit(10, true);
			SetCollisionMaskBit(10, true);
		}
	}

	public void SetHandState(HandsAnim handsAnim, bool hasSoapLeft, bool hasSoapRight)
	{
		var soapLeft = (MeshInstance)skeleton.GetNode("soap_bar_L");
		var soapRight = (MeshInstance)skeleton.GetNode("soap_bar_R");
		soapLeft.SetVisible(hasSoapLeft);
		soapRight.SetVisible(hasSoapRight);
		animTreePlayer.SetTransition("hand_state", handsAnim);
	}

	public void LockPositionWithFacing(bool locked, float facingAngle)
	{
		movementLocked = locked;
		if (locked)
		{
			var originalRotation = body.GetRotationDegrees();
			var finalRotation = new Vector3(0,  facingAngle - intitialRotation.y, 0);
			var tween = (Tween)GetNode("PhysicsTween");
			tween.SetActive(true);
			tween.InterpolateMethod(body, "set_rotation_degrees", originalRotation, finalRotation, 2f, Tween.TransitionType.Cubic, Tween.EaseType.In);
			tween.Start();
		}
	}

	public void SlideToPosition(Vector3 pos)
	{
		var tween = (Tween)GetNode("PhysicsTween");
		tween.SetActive(true);
		tween.InterpolateProperty(this, "translation", this.Translation, pos, 2f, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
		tween.Start();
	}

	public async void MountShower(BaseShower shower, float bodyAngle)
	{
		showerMounted = shower;
		
		var tween = (Tween)GetNode("PhysicsTween");
		var mountPos = shower.mountPos.GlobalTransform.origin;
		var staticBody = shower.collision;

		isMounting = true;
		animTreePlayer.SetTransition("body_state", BodyAnim.CLIMB_OVER);

		SlideToPosition(mountPos);

		staticBody.SetCollisionLayerBit(0, false);
		staticBody.SetCollisionMaskBit(0, false);
		gravityDisabled = true;
		await ToSignal(tween, "tween_completed");
		isMounting = false;
		animTreePlayer.TrySetTransition("body_state", BodyAnim.IDLE);
		staticBody.SetCollisionLayerBit(0, true);
		staticBody.SetCollisionMaskBit(0, true);
		gravityDisabled = false;
		EmitSignal("entered_shower");
	}

	public async void UnMountShower(BaseShower shower)
	{
		var tween = (Tween)GetNode("PhysicsTween");
		var unMountPos = shower.unMountPos.GlobalTransform.origin;
		var staticBody = shower.collision;

		isMounting = true;
		animTreePlayer.SetTransition("body_state", BodyAnim.CLIMB_OVER);

		tween.SetActive(true);
		tween.InterpolateProperty(this, "translation", this.Translation, unMountPos, 2f, Tween.TransitionType.Cubic, Tween.EaseType.InOut);
		tween.Start();

		staticBody.SetCollisionLayerBit(0, false);
		staticBody.SetCollisionMaskBit(0, false);
		gravityDisabled = true;
		await ToSignal(tween, "tween_completed");
		isMounting = false;
		staticBody.SetCollisionLayerBit(0, true);
		staticBody.SetCollisionMaskBit(0, true);
		gravityDisabled = false;
		animTreePlayer.TrySetTransition("body_state", BodyAnim.IDLE);
		EmitSignal("exited_shower");
	}

	public void ToggleDebugMode()
	{
		debugMode = !debugMode;
		if (debugMode)
		{
			gravityDisabled = true;
			SetCollisionLayerBit(0, false);
			SetCollisionMaskBit(0, false);
		} else
		{
			gravityDisabled = false;
			SetCollisionLayerBit(0, true);
			SetCollisionMaskBit(0, true);
		} 
	}

	public void DoWalkingAnim(float speedX, float speedZ, float maxSpeed)
	{
		var scale = 1.2f * (maxSpeed /10f);
		var resultantSpeed = Mathf.Sqrt(Mathf.Pow(speedZ, 2) + Mathf.Pow(speedX, 2));
		var timeScale = Mathf.Abs(resultantSpeed / maxSpeed) * scale;
		//prevents animation from getting wonky; values above this break it
		timeScale = Mathf.Clamp(timeScale, 0, 2.4f);

		if (timeScale != 0f)
		{
			animTreePlayer.TimescaleNodeSetScale("walk_anim_speed", timeScale);
			animTreePlayer.TrySetTransition("body_state", BodyAnim.WALK);
			
		}
		animTreePlayer.TimescaleNodeSetScale("walk_anim_speed", timeScale);
		animTreePlayer.Blend2NodeSetAmount("walk_blend", timeScale / scale);
	}

	public System.Collections.Generic.Dictionary<object, object> Save()
	{
		return new System.Collections.Generic.Dictionary<object, object>()
		{
			{ "money", money },
		};
	}

	public enum BodyPartEnum
	{
		HEAD,
		ARMS,
		ARM_L,
		ARM_R,
		TORSO,
		LEGS,
		NONE
	}

	public enum HandsAnim
	{
		NONE,
		LEFT,
		RIGHT,
		BOTH,
	}

	public enum BodyAnim
	{
		IDLE                    = 0,
		ARM_FL                  = 1,
		ARM_FL_SCRUB            = 2,
		ARM_LR                  = 3,
		ARM_LR_SCRUB            = 4,
		ARM_FR                  = 5,
		ARM_FR_SCRUB            = 6,
		ARM_RL                  = 7,
		ARM_RL_SCRUB            = 8,
		BEND_LEGS               = 9,
		BEND_SCRUB              = 10,
		WALK                    = 11,  
		ARMS_ABOVE_HEAD         = 12,
		HEAD_SCRUB              = 13,
		CLIMB_OVER              = 14,
		INTERACT                = 15,
		PICKUP                  = 16,
	}

	public struct BodyProgressValues : IComparer<float>
	{
		public BodyPart head, torso, arm_L, arm_R, legs;

		public BodyProgressValues(BodyPart head, BodyPart torso, BodyPart arm_L, BodyPart arm_R, BodyPart legs)
		{
			this.head = head;
			this.torso = torso;
			this.arm_L = arm_L;
			this.arm_R = arm_R;
			this.legs = legs;
		}

		public BodyProgressValues(float head, float torso, float arm_L, float arm_R, float legs)
		{
			this.head = new BodyPart(BodyPartEnum.HEAD, head);
			this.torso = new BodyPart(BodyPartEnum.TORSO, torso);
			this.arm_L = new BodyPart(BodyPartEnum.ARM_L, arm_L);
			this.arm_R = new BodyPart(BodyPartEnum.ARM_R, arm_R);
			this.legs = new BodyPart(BodyPartEnum.LEGS, legs);
		}

		public BodyPart GetBodyPart(BodyPartEnum bodyPart)
		{
			var part = new BodyPart();
			if (bodyPart == BodyPartEnum.HEAD)
				part = head;
			if (bodyPart == BodyPartEnum.ARM_L)
				part = arm_L;
			if (bodyPart == BodyPartEnum.ARM_R)
				part = arm_R;
			if (bodyPart == BodyPartEnum.TORSO)
				part = torso;
			if (bodyPart == BodyPartEnum.LEGS)
				part = legs;
			return part;
		}

		public int Compare(float x, float y)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public static bool operator  ==(BodyProgressValues bp1, BodyProgressValues bp2)
		{
			return bp1.Equals(bp2);
		}

		public static bool operator !=(BodyProgressValues bp1, BodyProgressValues bp2)
		{
			return !bp1.Equals(bp2);
		}

		public static BodyProgressValues operator +(BodyProgressValues bp1, BodyProgressValues bp2)
		{
			return new BodyProgressValues(bp1.head + bp2.head, bp1.torso + bp2.torso, bp1.arm_L + bp2.arm_L, bp1.arm_R + bp2.arm_R, bp1.legs + bp2.legs);
		}

	}

	public struct BodyPart
	{
		public BodyPartEnum part;
		public float dirtiness;
		public bool isBurned;

		public BodyPart(BodyPartEnum part)
		{
			this.part = part;
			dirtiness = 100f;
			isBurned = false;
		}

		public BodyPart(BodyPartEnum part, float initialDirtiness)
		{
			this.part = part;
			dirtiness = initialDirtiness;
			isBurned = false;
		}

		public static BodyPart operator +(BodyPart bp1, BodyPart bp2)
		{
			bp1.dirtiness = Mathf.Clamp(bp1.dirtiness + bp2.dirtiness, 0f, 100f);
			return bp1;
		}

		public static BodyPart operator -(BodyPart bp1, BodyPart bp2)
		{
			bp1.dirtiness = Mathf.Clamp(bp1.dirtiness - bp2.dirtiness, 0f, 100f);
			return bp1;
		}

		public static BodyPart operator +(BodyPart bp1, float increment)
		{
			bp1.dirtiness = Mathf.Clamp(bp1.dirtiness + increment, 0f, 100f);
			return bp1;
		}

		public static BodyPart operator -(BodyPart bp1, float decrement)
		{
			bp1.dirtiness = Mathf.Clamp(bp1.dirtiness - decrement, 0f, 100f);
			return bp1;
		}
	}
}


