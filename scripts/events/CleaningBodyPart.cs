using Godot;
using System;

public class CleaningBodyPart : Event
{
    private float timeElapsed;
    private float netPercentCleaned;
    public bool burnBodyPart;
    public bool alreadyStarted = false;
    public Cleaning cleaning;

    public override void _Start(EventHandler eventHandler)
    {
        base._Start(eventHandler);
        cleaning = (Cleaning)GetParent();
        alreadyStarted = true;
        player.HUD.ShowBurnWarning(false);
        player.HUD.ShowPopUpHUD(HUDController.PopUps.BODY_PART, false);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (isRunning)
        {

            if (burnBodyPart)
                player.HUD.ShowBurnWarning(true);
            else
                player.HUD.ShowBurnWarning(false);

            timeElapsed += delta;          
            
            // GD.Print("Clean rate: " + cleaning.scrubRate);

            if (cleaning.scrubRate > Cleaning.criticalScrubRate)
            {

                netPercentCleaned += cleaning.scrubRate;
                burnBodyPart = true;
                timeElapsed = 0f;

            }
            var rateCooldown = netPercentCleaned / timeElapsed / timeElapsed;
            
            if (rateCooldown < Cleaning.criticalScrubRate)
            {
                burnBodyPart = false;
            }
            //GD.Print("scrubRate :" + cleaning.scrubRate, ", rateCooldown: " + rateCooldown);
        }
        else
        {
            if (alreadyStarted  && cleaning.isRunning )
            {
                timeElapsed += delta;


                var rateCooldown = netPercentCleaned / timeElapsed / timeElapsed;

                if (rateCooldown < Cleaning.criticalScrubRate)
                {
                    burnBodyPart = false;
                    netPercentCleaned = 0;
                    timeElapsed = 0;
                }
            }
                
            
        }
    }

    public override void _End(EventHandler eventHandler)
    {
        base._End(eventHandler);
        player.HUD.ShowBurnWarning(false);
    }
}
