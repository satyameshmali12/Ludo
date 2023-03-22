using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class Basic_Board : Node2D
{
    AudioStreamPlayer2D startMusic;

    public List<Node> piecesData = new List<Node>();
    public List<Player_Data> playersData = new List<Player_Data>();
    public int playerPlayingIndex = 0;
    public bool isDiceRolled;
    // bool canPlay;
    public Basic_Func basf;
    public Global_Data data;
    public int movingStep = 0;

    public List<Player_Data> winPlayerData = new List<Player_Data>();



    public override void _Ready()
    {

        basf = new Basic_Func(this);
        data = basf.data;

        data.boardRects = this.GetNode<Node2D>("Rects").GetChildren();
        data.allRects = data.boardRects.Duplicate();


        foreach (Node2D house in this.GetNode("Houses").GetChildren())
        {
            foreach (ReferenceRect rect in house.GetNode("House_Enter_Zone").GetChildren())
            {
                data.allRects.Add(rect);
            }
        }

        Godot.Collections.Array arr = this.GetNode<Node2D>("Houses").GetChildren();
        foreach (Node2D house in arr)
        {
            foreach (Node piece in house.GetNode<Node2D>("Pieces").GetChildren())
            {
                piece.SetScript(
                    ResourceLoader.Load<Reference>("res://Base_Classes/Basic_Piece.cs")
                );
                piecesData.Add(piece); // collecting all the pieces at one place
            }
        }

        startMusic = this.GetNode<AudioStreamPlayer2D>("Start_Music");
        startMusic.Play();

        basf.data.ludoBoard = this;

    }
    public override void _Process(float delta)
    {


        Player_Data pData = playersData[playerPlayingIndex];
        data.currentPlayingType = pData.pieceType;
        data.currPlayerData = pData;

        if (!data.isPieceTransioning)
        {
            if (data.rolledDice != null)
            {
                Dice dice = data.rolledDice;
                if (dice.getIsDiceRolled())
                {
                    movingStep = dice.getRolledValue();

                    ArrayList allPiecesOfCurrentType = new ArrayList();
                    List<Basic_Piece> playablePieces = new List<Basic_Piece>();

                    foreach (Basic_Piece piece in piecesData)
                    {
                        if (piece.pieceType == data.currentPlayingType)
                        {
                            allPiecesOfCurrentType.Add(piece);
                        }
                    }

                    // gathering all the playable pieces
                    foreach (Basic_Piece piece in allPiecesOfCurrentType)
                    {
                        if (piece.canMove(movingStep))
                        {
                            playablePieces.Add(piece);
                        }
                    }
                    // passing the current player die if the playble piece for the specific house is zero
                    if (playablePieces.Count == 0)
                    {
                        next();
                        return;
                    }

                    // for AI
                    if (pData.playerType == Player_Type.AI)
                    {

                        AIMove(dice, playablePieces);

                    }

                }
            }
            // changing the die if the player has win
            else if (pData.isPlayerWin)
            {
                next();
                playerPlayingIndex = basf.minMaxer(playerPlayingIndex + 1, 3, 0);
            }
            else if (pData.playerType == Player_Type.AI)
            {
                pData.dice.isToRollMiserably = true;
            }
        }
    }


    public void next(bool isManuallyCalled = false)
    {

        if (data.targetPiece != null)
        {
            Label targetRectLabel = data.getReferenceRect(data.targetPiece.boardPos).GetNode<Label>("Type");
            if (targetRectLabel.Text.ToLower() != "checkpoint")
            {
                foreach (Basic_Piece piece in piecesData)
                {
                    if (
                        piece.getCurrentBoardPos() == data.targetPiece.getCurrentBoardPos()
                        &&
                        piece != data.targetPiece
                        &&
                        piece.pieceType != data.targetPiece.pieceType
                        &&
                        piece.isUnlocked
                        &&
                        !piece.getIsInHouseEnterZone()
                        )
                    {
                        piece.isUnlocked = false;
                        piece.RectGlobalPosition = piece.startPos;
                        piece.boardPos = piece.startBoardPos;
                        piece.isInHouseEnterZone = false;
                    }
                }
            }
        }


        foreach (ReferenceRect rect in data.allRects)
        {

            ArrayList specRectPieces = new ArrayList();

            foreach (Basic_Piece piece in piecesData)
            {
                if (rect == piece.getCurrentRefereceRect() && piece.isUnlocked)
                {
                    specRectPieces.Add(piece);
                }
            }

            if (specRectPieces.Count > 1)
            {
                Vector2 refRectSize = rect.RectSize;

                int maxOneLineItem = 3;
                int xDividingValue = ((specRectPieces.Count) >= maxOneLineItem) ? maxOneLineItem : specRectPieces.Count;

                int yDividingValue = Convert.ToInt32(Math.Ceiling(specRectPieces.Count / 3d));

                float xIncrement = refRectSize.x / xDividingValue;
                float yIncrement = refRectSize.y / xDividingValue;

                int yMultiplier = 0;

                ArrayList inLinePieces = new ArrayList();


                foreach (Basic_Piece piece in specRectPieces)
                {

                    int pieceIndex = (specRectPieces.IndexOf(piece) + 1);

                    piece.RectSize = new Vector2(xIncrement, yIncrement);
                    inLinePieces.Add(piece);

                    float xStart = (refRectSize.x / 2) - ((inLinePieces.Count * xIncrement) / 2);
                    float yStart = (refRectSize.y / 2) - ((yDividingValue * yIncrement) / 2);



                    if ((pieceIndex % maxOneLineItem == 0) || (pieceIndex == specRectPieces.Count))
                    {

                        foreach (Basic_Piece _Piece in inLinePieces)
                        {
                            _Piece.RectGlobalPosition = new Vector2(rect.RectGlobalPosition.x + xStart, rect.RectGlobalPosition.y + yStart + (yMultiplier * yIncrement));
                            xStart += xIncrement;
                        }

                        inLinePieces.Clear();
                        yMultiplier = (yMultiplier++ <= 3) ? yMultiplier++ : 0;
                    }

                }

            }
            else
            {
                foreach (Basic_Piece piece in specRectPieces)
                {
                    piece.setBackToOrignalStuff();
                }
            }

        }

        if (data.targetPiece != null)
        {

            int inHouseCount = 0;

            foreach (Basic_Piece piece in data.targetPiece.GetParent().GetChildren())
            {
                if (piece.getIsWin())
                {
                    inHouseCount++;
                }
            }


            // making the player win and poping the crown if all the 4 pieces reached the main point
            if (inHouseCount == 4 && !winPlayerData.Contains(playersData[playerPlayingIndex]))
            {
                Player_Data pData = playersData[playerPlayingIndex];
                pData.isPlayerWin = true;
                winPlayerData.Add(pData);
                pData.popCrown(playersData);
            }
        }


        if (data.rolledDice != null)
        {
            if (data.rolledDice.getRolledValue() != 6)
            {
                playerPlayingIndex++;
                if (playerPlayingIndex > 3)
                {
                    playerPlayingIndex = 0;
                }
            }
            data.rolledDice.setDiceRolled(false);
            data.rolledDice = null;
            data.isPieceTransioning = false;
            data.targetPiece = null;
        }

    }

    public virtual void AIMove(Dice dice, List<Basic_Piece> playablePieces)
    {

    }
    public virtual void PlayerMove(Basic_Piece piece)
    {

    }

    public Basic_Piece getConditionalPiece(Basic_Piece piece, ReferenceRect targetRect, bool isToConCheckPoint = true)
    {

        foreach (Basic_Piece tarPiece in piecesData)
        {
            if (
                tarPiece.getCurrentRefereceRect() == targetRect
                &&
                tarPiece.pieceType != piece.pieceType
                &&
                (targetRect.GetNode<Label>("Type").Text.ToLower() != "checkpoint" || !isToConCheckPoint)
               )
            {
                return tarPiece;
            }
        }

        return null;

    }
}