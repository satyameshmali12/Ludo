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
    string playerName;
    int playerPlayingIndex;
    public Player_Data(Node2D piece,Player_Type playerType,string playerName,int playerPlayingIndex,Dice dice)
    {
        this.house = piece;
        this.pieceType = piece.Name;
        this.playerType = playerType;   
        this.playerName = playerName;
        this.playerPlayingIndex = playerPlayingIndex;
        this.dice = dice;
    }

    public void popCrown(List<Player_Data> playersData)
    {
        if(isPlayerWin)
        {
            Node2D winView = house.GetNode<Node2D>("Win_View"); 
            winView.Visible = true;
            winView.GetNode<Label>("Rank").Text = (playersData.IndexOf(this)+1).ToString();
        }
    }
}