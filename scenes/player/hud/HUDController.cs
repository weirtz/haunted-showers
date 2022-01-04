using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class HUDController : CanvasLayer
{
	private Player player;

	public TextureRect headHint;
	public TextureRect torsoHint;
	public TextureRect legsHint;
	public TextureRect leftHint;
	public TextureRect rightHint;

	public TextureProgress headProgress;
	public TextureProgress armLProgress;
	public TextureProgress armRProgress;
	public TextureProgress torsoProgress;
	public TextureProgress legsProgress;

	public TextureRect crosshair;

	private bool showCleaningHUD;

	private bool HUDdisabled;

	public override void _Ready()
	{
		HUDdisabled = false;

		player = (Player)GetOwner();
		crosshair = (TextureRect)GetNode("GUI/Crosshair/CrosshairTexture");

		headHint = (TextureRect)GetNode("BodyHints/Head");
		torsoHint = (TextureRect)GetNode("BodyHints/Torso");
		legsHint = (TextureRect)GetNode("BodyHints/Legs");
		leftHint = (TextureRect)GetNode("BodyHints/Arm_L");
		rightHint = (TextureRect)GetNode("BodyHints/Arm_R");

		headProgress = (TextureProgress)GetNode("BodyProgress/Head/Head");
		armLProgress = (TextureProgress)GetNode("BodyProgress/Arm_L/Arm_L");
		armRProgress = (TextureProgress)GetNode("BodyProgress/Arm_R/Arm_R");
		torsoProgress = (TextureProgress)GetNode("BodyProgress/Torso/Torso");
		legsProgress = (TextureProgress)GetNode("BodyProgress/Legs/Legs");

		showCleaningHUD = false;
	}

	public override void _Process(float delta)
	{
		var head = player.bodyProgressValues.head.dirtiness;
		var torso = player.bodyProgressValues.torso.dirtiness;
		var arm_L = player.bodyProgressValues.arm_L.dirtiness;
		var arm_R = player.bodyProgressValues.arm_R.dirtiness;
		var legs = player.bodyProgressValues.legs.dirtiness;

		//set the body parts' gui to the current progress value
		headProgress.SetValue(head);
		torsoProgress.SetValue(torso);
		armLProgress.SetValue(arm_L);
		armRProgress.SetValue(arm_R);
		legsProgress.SetValue(legs);
		
		//set 'lowest part(s)' OR 'those at 0' to *visible*
		if (head >= 100 || head == GetHighestBodyValue())
			headHint.SetVisible(true);
		else
			headHint.SetVisible(false);
		if (torso >= 100 || torso == GetHighestBodyValue())
			torsoHint.SetVisible(true);
		else
			torsoHint.SetVisible(false);
		if (arm_L >= 100 || arm_L == GetHighestBodyValue())
			leftHint.SetVisible(true);
		else
			leftHint.SetVisible(false);
		if (arm_R >= 100 || arm_R == GetHighestBodyValue())
			rightHint.SetVisible(true);
		else
			rightHint.SetVisible(false);
		if (legs >= 100 || legs == GetHighestBodyValue())
			legsHint.SetVisible(true);
		else
			legsHint.SetVisible(false);

		//if looking at that body part, hide its hint
		HideWhileLookingAt(headHint);
		HideWhileLookingAt(torsoHint);
		HideWhileLookingAt(legsHint);
		HideWhileLookingAt(leftHint);
		HideWhileLookingAt(rightHint);

		DoAnimationOnEmpty(headProgress);
		DoAnimationOnEmpty(torsoProgress);
		DoAnimationOnEmpty(legsProgress);
		DoAnimationOnEmpty(armLProgress);
		DoAnimationOnEmpty(armRProgress);

		CleaningHUDVisible(showCleaningHUD);

		if (HUDdisabled)
		{
			headHint.SetVisible(false);
			torsoHint.SetVisible(false);
			leftHint.SetVisible(false);
			rightHint.SetVisible(false);
			legsHint.SetVisible(false);
			ShowBurnWarning(false);
			ShowCleaningHUD(false);
			ShowScrubbingMotion(false);
			ShowPopUpHUD(false);
			var GUI = (Control)GetNode("GUI");
			var bodyProgress = (Control)GetNode("BodyProgress");
			GUI.SetVisible(false);
			bodyProgress.SetVisible(false);
		} else
		{
			var GUI = (Control)GetNode("GUI");
			var bodyProgress = (Control)GetNode("BodyProgress");
			GUI.SetVisible(true);
			bodyProgress.SetVisible(true);
		}
	}

	public void DisableAll(bool disabled)
	{
		HUDdisabled = disabled;
	}

	public void ShowCleaningHUD(bool visible)
	{
		showCleaningHUD = visible;
	}

	public async void ShowPopUpHUD(PopUps popUp, bool show)
	{

		var texture = (TextureRect)GetNode("PopUps/TextureRects/texture");
		var anim = (AnimationPlayer)GetNode("PopUps/AnimationPlayer");



		if (anim.IsPlaying())
			await ToSignal(anim, "animation_finished");

		if (show)
		{
			switch (popUp)
			{
				case PopUps.BODY_PART:
					texture.Texture = (Texture)ResourceLoader.Load("res://assets/gui/shower/body-part.png");
					break;
				case PopUps.PICKUP_SOAP:
					texture.Texture = (Texture)ResourceLoader.Load("res://assets/gui/shower/soap-pick-up.png");
					break;
				case PopUps.SHOWER:
					texture.Texture = (Texture)ResourceLoader.Load("res://assets/gui/shower/shower-on.png");
					break;
			}
			anim.Play("hide", -1, -1, true);
		}  
		else
		{
			anim.Play("hide");
		}


	}

	public void ShowPopUpHUD(bool show)
	{
		var texture1 = (TextureRect)GetNode("PopUps/TextureRects/texture");
		if (show == false)
		{
			texture1.SetVisible(show);
		}
	}

	public void ShowMelonOverlay(bool show)
	{
		if (show)
		{
			var animOn = (AnimationPlayer)GetNode("GUI/MelonOverlay/AnimationPlayer");
			animOn.Play("put_on");
		}
		else
		{
			var animOff = (AnimationPlayer)GetNode("GUI/MelonOverlay/AnimationPlayer");
			animOff.Play("put_on", -1, -1, true);
		}
	}

	public void ShowScrubbingMotion(bool show)
	{
		var anim = (AnimatedSprite)GetNode("GUI/Crosshair/scrub-ui");
		var crosshair = (TextureRect)GetNode("GUI/Crosshair/CrosshairTexture");
		anim.SetVisible(show);
		crosshair.SetVisible(!show);
	}

	public void ShowBurnWarning(bool show)
	{
		if (show)
			ShowScrubbingMotion(!show);
		var anim = (AnimatedSprite)GetNode("GUI/Crosshair/scrub-burn");
		var crosshair = (TextureRect)GetNode("GUI/Crosshair/CrosshairTexture");
		anim.SetVisible(show);
		crosshair.SetVisible(!show);
	}

	private void CleaningHUDVisible(bool show)
	{
		if (!show)
		{
			headHint.SetVisible(show);
			torsoHint.SetVisible(show);
			legsHint.SetVisible(show);
			leftHint.SetVisible(show);
			rightHint.SetVisible(show);
		}
	}

	private void DoAnimationOnEmpty(TextureProgress textureProgress)
	{
		if (textureProgress.Value <= 0)
		{
			//var color = new Color(.066f, .138f, 0f, .75f);
			//textureProgress.SetSelfModulate(color);
		}
	}
	/*changed float to double*/
	private double GetHighestBodyValue()
	{
		/*Changed from float to double*/
		var valuesList = new List<double>();

		valuesList.Add(headProgress.Value);
		valuesList.Add(torsoProgress.Value);
		valuesList.Add(armLProgress.Value);
		valuesList.Add(armRProgress.Value);
		valuesList.Add(legsProgress.Value);

		var max = valuesList.Max();

		return max;
	}

	private void HideWhileLookingAt(TextureRect bodyHint)
	{
		if (player.camera.bodyAreaLookingAt.ToString().ToLower().Equals(bodyHint.Name.ToLower()))
			bodyHint.SetVisible(false);
	}

	private bool HintLookingAt(TextureRect bodyHint) 
	{
		if (player.camera.bodyAreaLookingAt.ToString().ToLower().Equals(bodyHint.Name.ToLower()))
			return true;
		else
			return false;
	}

	private bool HintLookingAt(Player.BodyPartEnum bodyHint)
	{
		if (player.camera.bodyAreaLookingAt == bodyHint)
			return true;
		else
			return false;
	}

	public enum PopUps
	{
		SHOWER,
		PICKUP_SOAP,
		BODY_PART,
	}
}
