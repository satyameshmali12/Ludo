using System;
using Godot;

public class Basic_Piece : Button
{
    public string pieceType;



    public int boardPos = 1;
    public int startBoardPos = 1;
    public int currentStep = 1;


    public bool isUnlocked = false;

    public bool isInHouseEnterZone = false;

    public Vector2 startPos;



    public int movingStep = 0;

    public float speed = 10;

    public Global_Data data;

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
            if ((isPressedOverCurRect() || this.Pressed) && this.pieceType == data.currentPlayingType && !data.isPieceTransioning && data.rolledDice != null)
            {
                data.board.PlayerMove(this);
            }

            if (isUnlocked && (basf.data.targetPiece == this) || basf.data.testingPieces.Contains(this))
            {

                setBackToOrignalStuff();

                Vector2 rectPos = getCurrentRefereceRect().RectGlobalPosition;


                this.RectGlobalPosition = new Vector2(basf.followUpPoint(this.RectGlobalPosition.x, rectPos.x, speed), basf.followUpPoint(this.RectGlobalPosition.y, rectPos.y, speed));


                if (this.RectGlobalPosition == rectPos)
                {

                    isInHouse = (currentStep == data.board.maxMoves);
                    if (isInHouse)
                    {
                        basf.data.board.next();
                        return;
                    }

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
                            int boardPosBef = boardPos;

                            // checking whether the above function has alter the position or not
                            // if so then not changing the die

                            if (boardPos == boardPosBef && !data.isTarLocFuncTri)
                            {
                                basf.data.board.pieceReachedTargetLocationAction(this);
                                data.isTarLocFuncTri = true;
                                if (data.board.boardType!=Board_Type.Business_Board)
                                {
                                    basf.data.board.next();
                                }
                            }

                        }
                    }
                }
            }

        }


    }

    public virtual bool canMove(int stepIncre)
    {
        return (((currentStep + stepIncre) <= data.board.maxMoves) && (isUnlocked || stepIncre == 6));
    }

    public virtual void move(int movingStep)
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

    public virtual int moveOnBoard(int currentBoardPos, int increment = 1)
    {
        currentBoardPos += increment;
        int tAreaLen = data.board.transitionAreaLength;

        if (currentBoardPos > tAreaLen)
        {
            currentBoardPos = currentBoardPos - tAreaLen;
        }

        else if (currentBoardPos < 1)
        {
            currentBoardPos = tAreaLen - Math.Abs(currentBoardPos);
        }

        if (currentStep > tAreaLen - 1 && !isInHouseEnterZone && data.board.houseEnterZoneLength > 0)
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
    }


    public ReferenceRect getCurrentRefereceRect()
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

    public Camera2D getCamera()
    {
        return this.GetNode<Camera2D>("Camera2D");
    }

}