using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class Ludo_Board : Basic_Board
{

    public override void _Ready()
    {
        base._Ready();

        Godot.Collections.Array arr = this.GetNode<Node2D>("Houses").GetChildren();
        piecesData.Clear();
        foreach (Node2D house in arr)
        {
            playersData.Add(
                new Player_Data(house, Player_Type.Non_AI, "hello1", arr.IndexOf(house), house.GetNode<Dice>("Dice"))
            );

            foreach (Node piece in house.GetNode<Node2D>("Pieces").GetChildren())
            {
                piece.SetScript(
                    ResourceLoader.Load<Reference>("res://Base_Classes/Basic_Piece.cs")
                );
                piecesData.Add(piece); // collecting all the pieces at one place
            }
        }

        // playersData[playerPlayingIndex].playerType = Player_Type.Non_AI;

        // basf.data.ludoBoard = this;

        // void addTestPiece(int num, int move, string house)
        // {
        //     Basic_Piece bp = this.GetNode<Basic_Piece>($"Houses/{house}/Pieces/{num}");
        //     bp.isUnlocked = true;
        //     bp.move(move);
        //     data.testingPieces.Add(bp);
        // }

        // addTestPiece(0, 56, "Green");
        // addTestPiece(1, 56, "Green");
        // addTestPiece(2, 56, "Green");
        // addTestPiece(3, 55, "Green");


        // addTestPiece(0, 56, "Yellow");
        // addTestPiece(1, 56, "Yellow");
        // addTestPiece(2, 56, "Yellow");
        // addTestPiece(3, 55, "Yellow");

        // addTestPiece(0, 56, "Blue");
        // addTestPiece(1, 56, "Blue");
        // addTestPiece(2, 56, "Blue");
        // addTestPiece(3, 55, "Blue");

        // addTestPiece(0, 56, "Red");
        // addTestPiece(1, 56, "Red");
        // addTestPiece(2, 56, "Red");
        // addTestPiece(3, 55, "Red");

    }

    public override void _Process(float delta)
    {

        base._Process(delta);

        if (Input.IsActionJustPressed("testing"))
        {
            data.testingPieces.Clear();

            data.targetPiece = null;
            data.isPieceTransioning = false;
        }


    }

    public override void AIMove(Dice dice, List<Basic_Piece> playablePieces)
    {
        base.AIMove(dice, playablePieces);
        int diceVal = dice.getRolledValue();

        List<Tuple<Basic_Piece, Basic_Piece>> attackingPiece = new List<Tuple<Basic_Piece, Basic_Piece>>();

        // gathering the pieces who can kill other piece(can make their piece back to their home)

        Tuple<Basic_Piece, Basic_Piece> tarAttackPiece = null;
        foreach (Basic_Piece piece in playablePieces)
        {
            if (piece.getCurrentStep() + diceVal <= piece.transitionAreaLength)
            {
                int pieceNextBoardPos = piece.moveOnBoard(piece.boardPos, dice.getRolledValue());
                ReferenceRect targetRect = data.boardRects[pieceNextBoardPos - 1] as ReferenceRect;

                Basic_Piece attackablePiece = getConditionalPiece(piece, targetRect);
                if (attackablePiece != null)
                {
                    if (tarAttackPiece == null || tarAttackPiece.Item2.getCurrentStep() < attackablePiece.getCurrentStep())
                    {
                        tarAttackPiece = new Tuple<Basic_Piece, Basic_Piece>(piece, attackablePiece);
                    }
                }
            }
        }

        if (tarAttackPiece != null)
        {
            tarAttackPiece.Item1.move(diceVal);
            return;
        }

        // for defencing if not attacked

        List<Basic_Piece> defencingPieces = new List<Basic_Piece>();
        foreach (Basic_Piece piece in playablePieces)
        {
            if (piece.getCurrentStep() <= piece.transitionAreaLength && piece.getCurrentRefereceRectType() != "checkpoint")
            {
                for (int i = 1; i <= 6; i++)
                {
                    int conBoardPos = piece.moveOnBoard(piece.boardPos, -i);
                    ReferenceRect targetRect = data.getReferenceRect(conBoardPos);

                    if (getConditionalPiece(piece, targetRect, false) != null)
                    {
                        defencingPieces.Add(piece);
                        break;
                    }
                }
            }
        }


        if (defencingPieces.Count > 0)
        {

            // checking which defencing piece should be move
            Basic_Piece forwardPiece = defencingPieces[0];
            foreach (Basic_Piece piece in defencingPieces)
            {
                if (piece.getCurrentStep() > forwardPiece.getCurrentStep())
                {
                    forwardPiece = piece;
                }
            }
            forwardPiece.move(diceVal);
            return;
        }

        playablePieces[Convert.ToInt32(GD.RandRange(0, playablePieces.Count - 1))].move(diceVal);


    }

    public override void PlayerMove(Basic_Piece piece)
    {
        base.PlayerMove(piece);
        
        int diceVal = data.rolledDice.getRolledValue();
        if (piece.canMove(diceVal) && data.currPlayerData.playerType == Player_Type.Non_AI)
        {
            piece.move(diceVal);
        }
    }

}
