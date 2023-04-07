/*

Business Board Stuff 
In Progress

*/



using Godot;
using System;
using System.Collections.Generic;

public class Business_Board : Basic_Board
{

    int totalMoney = 500000; // 5 lakh total rupee that will be distrubuted among the four players
    BS_Panel bsPanel;


    public override void _Ready()
    {

        this.boardType = Board_Type.Business_Board;
        base._Ready();

        configure(24, 0, 10000000, 1);
        this.GetNode<Dice>("Dice").diceType = "universal";

        float perPersonMoney = Convert.ToInt64(totalMoney / playersData.Count);
        foreach (Player_Data player in playersData)
        {
            player.money = (int)perPersonMoney;
        }

        bsPanel = this.GetNode<BS_Panel>("BS_Panel");


    }

    public override void _Process(float delta)
    {
        if (!this.GetNode<Notification>("Notification").Visible)
        {
            base._Process(delta);
        }

    }

    // returns wether to run the next function or not 
    // it is used in the basic piece
    public override void pieceReachedTargetLocationAction(Basic_Piece piece)
    {
        base.pieceReachedTargetLocationAction(piece);

        Business_Board_Piece tPiece = piece as Business_Board_Piece;

        GD.Print("TPiece :- ",tPiece);
        Ticket tick = tPiece.getCurrentRefereceRect() as Ticket;
        string name = tick.GetNode<Label>("Name").Text.ToLower();

        List<Tuple<string, int>> specialData = new List<Tuple<string, int>>()
        {
            new Tuple<string,int>("rest_house",1),
            new Tuple<string,int>("jail",2)
        };

        Notification noti = this.GetNode<Notification>("Notification");
        Player_Data pData = basf.data.currPlayerData;


        if (name == "start")
        {
            next();
            return;
        }


        foreach (Tuple<string, int> data in specialData)
        {
            if (name == data.Item1)
            {
                pData.skipDie += data.Item2;
                noti.isToPassNext = true;
                noti.pop($"\n {basf.data.currPlayerData.playerName} yours total {pData.skipDie} dies will skip");
                return;
            }
        }


        if (name == "party" || (name == "start" && tPiece.isOneRoundDone))
        {
            noti.isToPassNext = true;
            noti.pop($"You have got a bonus of party of 2000");
            pData.money += 2000; // bonus
        }

        else if (name == "parady")
        {
            Data_Manager dm = new Data_Manager("data/data_fields/ParadyDataField.txt");
            int rolledValue = basf.data.rolledDice.getRolledValue();
            dm.loadData(rolledValue.ToString(), "RolledValue");

            int toBePayed = int.Parse(dm.getData("Loan"));
            int bonus = int.Parse(dm.getData("Bonus"));


            if (toBePayed != 0)
            {
                bsPanel.Visible = true;
                Paying_Node pN = bsPanel.GetNode<Paying_Node>("Paying_Node");
                pN.Visible = true;
                pN.amountToBePaid = toBePayed;
                pN.toggle();
                bsPanel.waitTimer.Start();
                pN.setMessage($"You need to pay some heed due to {rolledValue} chance as per our game rules !");
            }
            else
            {
                pData.money += bonus;
                noti.isToPassNext = true;
                noti.pop($"You have got a bonus of {bonus} as that of {rolledValue} chance as per our game rule");
            }
            resetCam();
        }

        else if (tick.ownedBuy != pData.playerName)
        {
            bsPanel.setTicket(tick);
            data.targetPiece.GetNode<Camera2D>("Camera2D").Current = false;
            data.board.GetNode<Camera2D>("MainCamera").Current = true;
        }


    }

    public void resetCam()
    {
        data.targetPiece.GetNode<Camera2D>("Camera2D").Current = false;
        data.board.GetNode<Camera2D>("MainCamera").Current = true;

    }

    public void passOnNextDie()
    {
        resetCam();
        data.board.next(true);
        playerPlayingIndex++;
        if (playerPlayingIndex > playersData.Count - 1)
        {
            playerPlayingIndex = 0;
        }
    }

}
