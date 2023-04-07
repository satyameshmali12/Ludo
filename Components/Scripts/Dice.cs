using Godot;
using System;

public class Dice : Node2D
{
    Timer rotationTimer;
    AnimatedSprite diceAnim;
    Basic_Func basf;

    ReferenceRect diceRect;

    bool isDiceRolled = false;

    public string diceType;

    public bool isToRollMiserably = false;

    AudioStreamPlayer2D rollTune;

    public override void _Ready()
    {
        
        basf = new Basic_Func(this);

        diceType = this.GetParent().Name;

        diceAnim = this.GetNode<AnimatedSprite>("Dice_Animation");

        rotationTimer = basf.getTimer(1, "Dice_Rolled_Over", true);
        this.AddChild(rotationTimer);

        diceRect = this.GetNode<ReferenceRect>("Dice_Rect");

        rollTune = this.GetNode<AudioStreamPlayer2D>("Tune");


    }

    public override void _Process(float delta)
    {


        bool isMouseInsideRect = basf.isMouseInsideRect(diceRect.RectGlobalPosition, diceRect.RectSize, this.GetGlobalMousePosition());
        if ((isMouseInsideRect && Input.IsActionJustPressed("Mouse_Pressed") && (diceType == basf.data.currentPlayingType || diceType.ToLower()=="universal") && basf.data.rolledDice == null && !isDiceRolled) && basf.data.currPlayerData.playerType==Player_Type.Non_AI || isToRollMiserably)
        {
            if (rotationTimer.IsStopped())
            {
                isToRollMiserably = false;
                basf.data.rolledDice = this;

                float waitTimeRan = (float)(GD.RandRange(1, 1.4));
                float speedScaleRan = (float)(GD.RandRange(1, 1.4));

                rotationTimer.WaitTime = waitTimeRan;
                diceAnim.SpeedScale = speedScaleRan;

                diceAnim.Play();
                rotationTimer.Start();
                rollTune.Play();
                
            }
        }

    }
    public void Dice_Rolled_Over()
    {
        diceAnim.Stop();
        isDiceRolled = true;
    }

    public bool getIsDiceRolled()
    {
        return isDiceRolled;
    }

    public void setDiceRolled(bool isDiceRolled)
    {
        this.isDiceRolled = isDiceRolled;
    }

    public int getRolledValue()
    {
        // return 6;
        return (diceAnim.Frame + 1); // 1 added as the indexing starts from the 0th index
    }

    public void makeUniversal()
    {
        diceType = "Universal";
    }


}
