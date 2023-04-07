using System;
using Godot;
using System.Collections.Generic;

public class Player_Data
{
    public string pieceType;
    Node2D house;
    public Player_Type playerType;
    public bool isPlayerWin = false;
    public Dice dice;
    public string playerName;
    int playerPlayingIndex;
    public int money = 0;
    public int skipDie = 0;
    public Player_Data(Node2D piece,Player_Type playerType,string playerName,Dice dice)
    {
        this.house = piece;
        this.pieceType = piece.Name;
        this.playerType = playerType;   
        this.playerName = playerName;
        this.dice = dice;
    }

    public Node2D getHouse()
    {
        return house;
    }
}