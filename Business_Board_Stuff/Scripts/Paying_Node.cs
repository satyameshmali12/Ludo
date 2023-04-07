/*

Business Board Stuff 
In Progress

*/



using Godot;
using System;
using System.Linq;


public class Paying_Node : SB_Interlinking
{
    public int amountToBePaid = 0;
    public Ticket ticket = null;
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void toggle()
    {
        base.toggle();
        priceTag.Text = $"Price :- {amountToBePaid}";
        Label lab = this.GetNode<Label>("Owner_Name");
        if (ticket != null)
        {

            lab.Text = $"labTicket Owned By :- {ticket.name}";
            lab.Visible = true;
        }
        else
        {
            lab.Visible = false;
        }
    }

    public override void checkConfirmation()
    {
        base.checkConfirmation();
        Player_Data playerData = basf.data.currPlayerData;
        if (playerData.money >= amountToBePaid)
        {
            playerData.money -= amountToBePaid;
            parPanel.noti.isToPassNext = true;
            parPanel.noti.pop("Mortgage Payed You Can move to out !!!");
            if (ticket != null)
            {
                foreach (Player_Data pData in board.playersData)
                {
                    if (pData.playerName == ticket.ownedBuy)
                    {
                        pData.money += amountToBePaid;
                        break;
                    }
                }
            }
        }

    }

    public void setMortageUsingTicket()
    {
        string ownedBuy = ticket.name;
        int totalLoan = 0;
        int numberOfTickFou = 0;
        foreach (Ticket tick in basf.data.allRects)
        {
            if (tick.isOwned && ticket.ownedBuy == ownedBuy && tick.groupType == ticket.groupType)
            {
                totalLoan += tick.mortgage;
                numberOfTickFou++;
            }
        }
        amountToBePaid = (numberOfTickFou > 3) ? totalLoan : ticket.mortgage;

    }

    public override void over()
    {
        base.over();
        board.winPlayerData.Add(basf.data.currPlayerData);
        parPanel.noti.hide(true);
    }
}
