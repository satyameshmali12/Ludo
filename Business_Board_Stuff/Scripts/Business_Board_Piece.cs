using System;
using Godot;


public class Business_Board_Piece : Basic_Piece
{

    public bool isOneRoundDone = false;
    // public int skipDie = 0;
    public override void _Ready()
    {
        base._Ready();
        speed = 4;
    }

    public override bool canMove(int stepIncre)
    {
        return (isUnlocked || stepIncre==6);
    }


    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void move(int movingStep)
    {
        int lastMovStep = this.movingStep;
        base.move(movingStep);
        if (isUnlocked && lastMovStep != this.movingStep)
        {
            this.GetNode<Camera2D>("Camera2D").Current = true;
        }
    }

    public override int moveOnBoard(int currentBoardPos, int increment = 1)
    {
        isOneRoundDone = (currentBoardPos + increment > data.board.transitionAreaLength) ? true : isOneRoundDone;

        return base.moveOnBoard(currentBoardPos, increment);
    }





}