/*

Business Board Stuff 
In Progress

*/



using Godot;
using System;

public class BS_Panel : Node2D
{
    public Ticket tarTicket;
    Basic_Func basf;

    Buying_Node buyingN;
    Paying_Node payingN;
    public Timer waitTimer;
    public Notification noti;

    public override void _Ready()
    {
        noti = this.GetNode<Notification>("%Notification");
        basf = new Basic_Func(this);
        buyingN = this.GetNode<Buying_Node>("Buying_Node");
        payingN = this.GetNode<Paying_Node>("Paying_Node");
        waitTimer = basf.getTimer(60, "OverDie", true);
        this.AddChild(waitTimer);
    }
    void setLText(string name, string text)
    {
        this.GetNode<Label>(name).Text = text;
    }
    public override void _Process(float delta)
    {
        Label tickName = this.GetNode<Label>("Ticket_Name");
        if (this.tarTicket != null)
        {
            tickName.Text = $"{tarTicket.name}";
            tickName.Visible = true;
        }
        else
        {
            tickName.Visible = false;
        }
        if (!waitTimer.IsStopped())
        {
            setLText("Time", $"TimeLeft :- {(int)waitTimer.TimeLeft}");
            setLText("Name", $"Player Name :- {basf.data.currPlayerData.playerName}");
            setLText("Available_Money", $"Money :- {basf.data.currPlayerData.money}");
        }
    }

    public void OverDie()
    {
        if (buyingN.Visible)
        {
            buyingN.over();
        }
        else
        {
            payingN.over();
        }
    }

    public void hideTheChilds()
    {
        payingN.Visible = buyingN.Visible = false;
    }


    public void setTicket(Ticket ticket)
    {
        this.Visible = true;
        waitTimer.Start();
        this.tarTicket = ticket;
        if (tarTicket.isOwned)
        {
            payingN.ticket = ticket;
            payingN.setMortageUsingTicket();
            payingN.toggle();
        }
        else
        {
            buyingN.toggle();
        }



    }
}
