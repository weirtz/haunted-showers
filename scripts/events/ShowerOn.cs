using Godot;
using System;

public class ShowerOn : Event
{
    private BaseShower shower;
    private AnimationPlayer enviroAnim;
    private float lastAnimTime;
    private bool alreadyStarted = false;

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        eventHandler.QueueFreeEvent<ShowerOn>(player, "interact_knob", this);

        shower = player.showerMounted;

        player.showerMounted.showerRunningSound.Play();

        player.HUD.ShowPopUpHUD(HUDController.PopUps.SHOWER, false);

        player.HUD.ShowPopUpHUD(HUDController.PopUps.PICKUP_SOAP, true);

        var particles = shower.particles;
        particles.SetEmitting(true);

        player.animTreePlayer.SetTransitionCooldown("body_state", Player.BodyAnim.INTERACT);
        player.animTreePlayer.QueueSetTransition("body_state", Player.BodyAnim.IDLE);

        var environment = GetTree().CurrentScene.GetNode("Environment");
        enviroAnim = (AnimationPlayer)environment.GetNode("FogAnimation");
        enviroAnim.SetActive(true);
        alreadyStarted = true;
        enviroAnim.Play("Fog");
        enviroAnim.Seek(lastAnimTime);
        shower.showerRunningSound.Play();
    }

    public override void _Process(float delta)
    {
        if (alreadyStarted) 
        {
            if (enviroAnim.IsPlaying())
            {
                lastAnimTime = enviroAnim.GetCurrentAnimationPosition();
            }
        }

    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.showerMounted.showerRunningSound.Stop();
        var particles = shower.particles;
        particles.SetEmitting(false);
        eventHandler.QueueEvent(this, GetNode<Node>(source), signal);
        player.HUD.ShowPopUpHUD(HUDController.PopUps.PICKUP_SOAP, false);
        player.HUD.ShowPopUpHUD(HUDController.PopUps.SHOWER, true);
        enviroAnim.Play("Fog", -1, -1, false);
        enviroAnim.Seek(lastAnimTime);
        shower.showerRunningSound.Stop();
    }

}
