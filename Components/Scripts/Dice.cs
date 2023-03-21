using Godot;
using System;

public class Dice : Node2D
{
    float diceRotatingTime = 1f;
    Timer rotationTimer;
    AnimatedSprite diceAnim;
    Basic_Func basf;

    ReferenceRect diceRect;

    bool isDiceRolled = false;

    string diceType;

    public bool isToRollMiserably = false;

    bool isToGiveOne = false;

    public override void _Ready()
    {
        
        basf = new Basic_Func(this);

        diceType = this.GetParent().Name;

        diceAnim = this.GetNode<AnimatedSprite>("Dice_Animation");

        rotationTimer = basf.getTimer(1, "Dice_Rolled_Over", true);
        this.AddChild(rotationTimer);

        diceRect = this.GetNode<ReferenceRect>("Dice_Rect");


    }

    public override void _Process(float delta)
    {

        if (Input.IsActionJustPressed("testing2"))
        {
            isToGiveOne = true;
        }

        bool isMouseInsideRect = basf.isMouseInsideRect(diceRect.RectGlobalPosition, diceRect.RectSize, this.GetGlobalMousePosition());
        if ((isMouseInsideRect && Input.IsActionJustPressed("Mouse_Pressed") && diceType == basf.data.currentPlayingType && basf.data.rolledDice == null && !isDiceRolled) || isToRollMiserably)
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
        // if (isToGiveOne)
        // {
        //     return 1;
        // }
        // else
        // {
        // return 5;
        return 1;
        // return (diceAnim.Frame + 1); // 1 added as the indexing starts from the 0th index
        // }
    }


}
