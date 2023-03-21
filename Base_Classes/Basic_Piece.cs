using System;
using Godot;

public class Basic_Piece : Button
{
    public string pieceType;



    public int boardPos = 1;
    public int startBoardPos = 1;
    int currentStep = 1;


    public bool isUnlocked = false;

    public bool isInHouseEnterZone = false;

    public Vector2 startPos;



    public int transitionAreaLength = 52;
    public int houseEnterZoneLength = 6;
    public int maxMoves = 57; // ((trasitionAreaLength + houseEnterZOneLEngth) - 1) (one is subtracted because first position of the piece is not calculated)

    int movingStep = 0;

    float speed = 10;
    Global_Data data;

    Godot.Collections.Array houseEnterZoneRects;
    Basic_Func basf;
    Vector2 orgSize;

    bool isInHouse = false;

    public override void _Ready()
    {
        base._Ready();
        basf = new Basic_Func(this);
        data = basf.data;

        orgSize = this.RectSize;
        startPos = this.RectGlobalPosition;

        Node2D hParent = this.GetParent().GetParent() as Node2D;

        startBoardPos = int.Parse(hParent.GetNode<Label>("Start_Board_Pos").Text);
        boardPos = startBoardPos;

        pieceType = hParent.Name;

        houseEnterZoneRects = hParent.GetNode<Node2D>("House_Enter_Zone").GetChildren();

    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!isInHouse)
        {

            // for giving the movement to the piece
            // all the data is been taken from the ludo_board class
            // data.canPlay removed from the below condition oky
            if ((isPressedOverCurRect() || this.Pressed) && this.pieceType == data.currentPlayingType && !data.isPieceTransioning && data.rolledDice != null)
            {
                int diceVal = data.rolledDice.getRolledValue();

                if (this.canMove(diceVal) && data.currPlayerData.playerType == Player_Type.Non_AI)
                {
                    this.move(diceVal);
                }
            }

            if (isUnlocked && (basf.data.targetPiece == this || data.testingPieces.Contains(this)))
            {

                setBackToOrignalStuff();

                Vector2 rectPos = getCurrentRefereceRect().RectGlobalPosition;


                this.RectGlobalPosition = new Vector2(basf.followUpPoint(this.RectGlobalPosition.x, rectPos.x, speed), basf.followUpPoint(this.RectGlobalPosition.y, rectPos.y, speed));


                if (this.RectGlobalPosition == rectPos)
                {
                    
                    isInHouse = (currentStep == maxMoves);

                    if (movingStep > 0)
                    {
                        currentStep++;
                        movingStep--;
                        boardPos = moveOnBoard(boardPos, 1);
                    }
                    else if (pieceType == basf.data.currentPlayingType)
                    {
                        if (data.targetPiece == this)
                        {
                            basf.data.ludoBoard.next();
                        }
                    }
                }
            }

        }


    }

    public bool canMove(int stepIncre)
    {
        return (((currentStep + stepIncre) <= maxMoves) && (isUnlocked || stepIncre == 6));
    }

    public void move(int movingStep)
    {

        // inUnlocked is check in order to excute the second statement if the piece is not unlocked while though even having the dice value is 6
        if (canMove(movingStep) && isUnlocked)
        {
            this.movingStep = movingStep;
        }
        else if (movingStep == 6)
        {
            this.isUnlocked = true;
        }

        if (this.isUnlocked)
        {
            data.isPieceTransioning = true;
            data.targetPiece = this;
        }

    }

    public int moveOnBoard(int currentBoardPos, int increment = 1)
    {
        currentBoardPos += increment;

        if (currentBoardPos > transitionAreaLength)
        {
            currentBoardPos = currentBoardPos - transitionAreaLength;
        }

        else if (currentBoardPos < 1)
        {
            currentBoardPos = transitionAreaLength - Math.Abs(currentBoardPos);
        }

        if (currentStep > transitionAreaLength - 1 && !isInHouseEnterZone)
        {
            currentBoardPos = 1;
            isInHouseEnterZone = true;
        }

        return currentBoardPos;
    }


    public bool getIsUnlocked()
    {
        return isUnlocked;
    }

    public int getCurrentStep()
    {
        return currentStep;
    }

    public int getCurrentBoardPos()
    {
        return boardPos;
    }
    public bool getIsInHouseEnterZone()
    {
        return isInHouseEnterZone;
    }

    public void setBackToOrignalStuff()
    {
        this.RectSize = orgSize;

        ReferenceRect rect = getCurrentRefereceRect();
        this.RectGlobalPosition = rect.RectGlobalPosition;
    }


    public ReferenceRect getCurrentRefereceRect()
    {
        // GD.Print(System.Reflection.MethodBase.GetCurrentMethod());
        try
        {
            ReferenceRect boardRect;
            if (!isInHouseEnterZone)
            {
                boardRect = basf.data.boardRects[boardPos - 1] as ReferenceRect;
            }
            else
            {
                boardRect = houseEnterZoneRects[boardPos - 1] as ReferenceRect;
            }
            return boardRect;
        }
        catch (System.Exception ex)
        {
            GD.Print(boardPos);
            GD.Print(pieceType);
            GD.Print(this);
            GD.Print(this.Name);
            GD.Print(isInHouseEnterZone);
            GD.Print(System.Reflection.MethodBase.GetCurrentMethod());
            return null;
        }


    }

    public bool getIsWin()
    {
        return isInHouse;
    }

    public string getCurrentRefereceRectType()
    {
        return getCurrentRefereceRect().GetNode<Label>("Type").Text.ToLower();
    }

    public bool isPressedOverCurRect()
    {

        ReferenceRect cRect = getCurrentRefereceRect();

        return basf.isMouseInsideRect(cRect.RectGlobalPosition, cRect.RectSize, this.GetGlobalMousePosition()) && Input.IsActionJustPressed("Mouse_Pressed");
    }


}