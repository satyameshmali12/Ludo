using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class Ludo_Board : Basic_Board
{

    public override void _Ready()
    {
        
        boardType = Board_Type.Ludo_Board;

        base._Ready();

        Godot.Collections.Array arr = this.GetNode<Node2D>("Houses").GetChildren();


        foreach (Node2D house in arr)
        {
            playersData.Add(
                new Player_Data(house, Player_Type.Non_AI, "hello1", arr.IndexOf(house), house.GetNode<Dice>("Dice"))
            );
        }

    }

    public override void _Process(float delta)
    {
        base._Process(delta);

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
            if (piece.getCurrentStep() + diceVal <= transitionAreaLength)
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
            if (piece.getCurrentStep() <= transitionAreaLength && piece.getCurrentRefereceRectType() != "checkpoint")
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

    public override void playerWinAction(Player_Data winPlayer)
    {
        base.playerWinAction(winPlayer);
        Node2D winView = winPlayer.getHouse().GetNode<Node2D>("Win_View");
        winView.Visible = true;
        winView.GetNode<Label>("Rank").Text = (playersData.IndexOf(winPlayer) + 1).ToString();
    }

    public override void nextExtraInnerConfig()
    {
        base.nextExtraInnerConfig();
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
    }

}
