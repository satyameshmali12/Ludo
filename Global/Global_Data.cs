using System;
using Godot;
using System.Collections;
using System.Collections.Generic;

public class Global_Data:Node
{

    public List<Dictionary<string, string>> playerData;
    public string navigationUrl = "";

    public bool isGameCompleted = false;

    public List<Player_Data> winPlayerData = new List<Player_Data>();    
    public string currentPlayingType = "";
    public Dice rolledDice = null;
    public bool isPieceTransioning = false;
    public Godot.Collections.Array boardRects;
    public Godot.Collections.Array allRects;
    public Basic_Piece targetPiece = null;
    public ArrayList testingPieces = new ArrayList();
    public Basic_Board board = null;
    public Player_Data currPlayerData = null;
    public bool canPlay = false;

    public bool isTarLocFuncTri = false;

    

    public override void _Process(float delta)
    {
        base._Process(delta);

    }

    public ReferenceRect getReferenceRect(int boardPos)
    {
        return boardRects[boardPos-1] as ReferenceRect;
    }
}