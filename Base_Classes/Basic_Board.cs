using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class Basic_Board : Node2D
{
    public Board_Type boardType;
    AudioStreamPlayer2D startMusic;
    public List<Node> piecesData = new List<Node>();
    public List<Player_Data> playersData = new List<Player_Data>();
    public int playerPlayingIndex = 0;
    public Basic_Func basf;
    public Global_Data data;
    public List<Player_Data> winPlayerData = new List<Player_Data>();


    public int transitionAreaLength = 52;
    public int houseEnterZoneLength = 6;
    public int maxMoves = 57; // ((trasitionAreaLength + houseEnterZOneLEngth) - 1) (one is subtracted because first position of the piece is not calculated)
    public int singHousePieceCount = 4;

    bool isFirstRender = true;

    Button pauseButton;
    Pause_Menu pauseMenu;
    public void configure(int transitionAreaLength, int houseEnterZoneLength, int maxMoves, int singHousePieceCount)
    {
        this.transitionAreaLength = transitionAreaLength;
        this.houseEnterZoneLength = houseEnterZoneLength;
        this.maxMoves = maxMoves;
        this.singHousePieceCount = singHousePieceCount;
    }

    public override void _Ready()
    {

        this.GetTree().Paused = false;

        basf = new Basic_Func(this);
        data = basf.data;


        // doing the setting for a new game
        data.isGameCompleted = false;
        reset();


        data.boardRects = this.GetNode<Node2D>("Rects").GetChildren();
        data.allRects = data.boardRects.Duplicate();

        pauseButton = this.GetNode<Button>("Pause_Button");
        pauseButton.PauseMode = PauseModeEnum.Process;
        pauseMenu = this.GetNode<Pause_Menu>("Pause_Menu");

        foreach (Dictionary<string, string> data in basf.data.playerData)
        {

            foreach (Node2D house in this.GetNode("Houses").GetChildren())
            {
                if (house.Name == data["Type"] && bool.Parse(data["Is_Playing"]))
                {
                    Dice dice = house.HasNode(new NodePath("Dice")) ? house.GetNode<Dice>("Dice") : this.GetNode<Dice>("Dice");
                    playersData.Add(new Player_Data(house, (bool.Parse(data["Is_AI"]) ? Player_Type.AI : Player_Type.Non_AI), data["Name"], dice));
                    break;
                }
            }
        }


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
            foreach (Basic_Piece piece in house.GetNode<Node2D>("Pieces").GetChildren())
            {
                piecesData.Add(piece); // collecting all the pieces at one place
            }
        }

        startMusic = this.GetNode<AudioStreamPlayer2D>("Start_Music");
        startMusic.Play();

        basf.data.board = this;


    }


    public override void _Process(float delta)
    {

        if (!data.isGameCompleted)
        {
            if (winPlayerData.Count >= playersData.Count - 1)
            {
                foreach (Player_Data data in playersData)
                {
                    if (!data.isPlayerWin)
                    {
                        // addng the data of the last remaining data
                        winPlayerData.Add(data);
                        break;
                    }
                }
                data.isGameCompleted = true;
                if(boardType==Board_Type.Business_Board)
                {
                    winPlayerData.Reverse();
                }
                data.winPlayerData = winPlayerData;
                this.GetNode<Win_Screen>("Win_Screen").Visible = true;
            }

            if (boardType != Board_Type.Business_Board)
            {
                foreach (Label lab in this.GetNode("Current_Die_Display").GetChildren())
                {
                    lab.Visible = (lab.Name.ToLower() == data.currentPlayingType.ToLower());
                    lab.Text = data.currentPlayingType;
                }
            }

            if (isFirstRender)
            {
                setPieceSizes();
                isFirstRender = false;
            }

            if (pauseButton.Pressed && Input.IsActionJustPressed("Mouse_Pressed"))
            {
                pauseMenu.Visible = !pauseMenu.Visible;
            }
            pauseButton.Visible = !pauseMenu.Visible;



            Player_Data pData = playersData[playerPlayingIndex];
            data.currentPlayingType = pData.pieceType;
            data.currPlayerData = pData;

            if (!data.isPieceTransioning)
            {
                data.isTarLocFuncTri = false;
                Dice dice = data.rolledDice;
                if (pData.skipDie > 0)
                {
                    pData.skipDie--;
                    next();
                    return;
                }

                if (dice != null)
                {
                    if (dice.getIsDiceRolled())
                    {
                        int movingStep = dice.getRolledValue();

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

                        pieceSelectExtension();

                        // passing the current player die if the playble piece for the specific house is zero
                        if (playablePieces.Count == 0 || (playablePieces.Count == 1 && !playablePieces[0].canMove(dice.getRolledValue())))
                        {
                            next();
                        }
                        else if (playablePieces.Count == 1 || !playablePieces[0].isUnlocked)
                        {
                            playablePieces[0].move(dice.getRolledValue());
                        }

                        // for AI
                        else if (pData.playerType == Player_Type.AI)
                        {
                            AIMove(dice, playablePieces);
                        }

                    }
                }
                // changing the die if the player has win
                else if (pData.isPlayerWin || winPlayerData.Contains(pData))
                {
                    next();
                    playerPlayingIndex = basf.minMaxer(playerPlayingIndex + 1, playersData.Count - 1, 0);
                }
                else if (pData.playerType == Player_Type.AI)
                {
                    pData.dice.isToRollMiserably = true;
                }
            }

            processExtension();

        }

    }

    public virtual void pieceReachedTargetLocationAction(Basic_Piece piece) {}

    public virtual void processExtension() { }
    public void next(bool isManuallyCalled = false)
    {

        if (data.targetPiece != null)
        {
            nextExtraInnerConfig();

            int inHouseCount = 0;

            foreach (Basic_Piece piece in data.targetPiece.GetParent().GetChildren())
            {
                if (piece.getIsWin())
                {
                    inHouseCount++;
                }
            }

            // making the player win and poping the crown if all the 4 pieces reached the main point
            if (inHouseCount == singHousePieceCount && !winPlayerData.Contains(playersData[playerPlayingIndex]))
            {
                Player_Data pData = playersData[playerPlayingIndex];
                pData.isPlayerWin = true;
                winPlayerData.Add(pData);
                playerWinAction(pData);
            }
        }

        if (data.rolledDice != null)
        {
            if (data.rolledDice.getRolledValue() != 6 && !isManuallyCalled)
            {
                playerPlayingIndex++;
                if (playerPlayingIndex > playersData.Count - 1)
                {
                    playerPlayingIndex = 0;
                }
            }
            data.rolledDice.setDiceRolled(false);
            data.rolledDice = null;
        }

        data.isPieceTransioning = false;
        data.targetPiece = null;
        setPieceSizes();
    }
    public virtual void AIMove(Dice dice, List<Basic_Piece> playablePieces) { }
    public virtual void PlayerMove(Basic_Piece piece) { }
    public virtual void playerWinAction(Player_Data winPlayer) { }
    public virtual void nextExtraInnerConfig() { }
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
    public void setPieceSizes()

    {
        foreach (ReferenceRect rect in data.allRects)
        {

            ArrayList specRectPieces = new ArrayList();

            foreach (Basic_Piece piece in piecesData)
            {
                if (rect == piece.getCurrentRefereceRect() && (piece.isUnlocked || boardType != Board_Type.Ludo_Board))
                {
                    specRectPieces.Add(piece);
                }
            }

            if (specRectPieces.Count > 0)
            {
                Vector2 refRectSize = rect.RectSize;

                int maxOneLineItem = 3;
                int xDividingValue = ((specRectPieces.Count) >= maxOneLineItem) ? maxOneLineItem : specRectPieces.Count;

                int yDividingValue = Convert.ToInt32(Math.Ceiling(specRectPieces.Count / 3d));

                float managedXSize = (refRectSize.x < 65) ? refRectSize.x : 65;
                float managedYSize = (refRectSize.y < 65) ? refRectSize.y : 65;
                float xChange = (refRectSize.x - managedXSize) / 2;
                float yChange = (refRectSize.y - managedYSize) / 2;

                float xIncrement = managedYSize / xDividingValue;
                float yIncrement = managedYSize / xDividingValue;

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
                            _Piece.RectGlobalPosition = new Vector2(xChange + rect.RectGlobalPosition.x + xStart, yChange + rect.RectGlobalPosition.y + yStart + (yMultiplier * yIncrement));
                            xStart += xIncrement;
                        }

                        inLinePieces.Clear();
                        yMultiplier = (yMultiplier++ <= 3) ? yMultiplier++ : 0;
                    }

                }
            }

        }

    }

    public virtual void pieceSelectExtension() { }

    public virtual void reset()
    {
        data.isPieceTransioning = false;
        data.rolledDice = null;
        data.targetPiece = null;
    }
}