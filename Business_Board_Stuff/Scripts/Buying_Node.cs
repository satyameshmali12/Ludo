/*

Business Board Stuff 
In Progress

*/





using Godot;
using System;

public class Buying_Node : SB_Interlinking
{
    public override void _Ready()
    {
        base._Ready();

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if(this.Visible)
        {
            // this.GetParent().GetNode<Label>("Task").Text = 
            setMessage("Did you wanted to buy this?");
        }

        if(this.GetNode<Button>("Next").Pressed && Input.IsActionJustPressed("Mouse_Pressed"))
        {
            parPanel.noti.hide(true);
        }
    }

    public override void toggle()
    {
        base.toggle();
        priceTag.Text = $"Price :- {parPanel.tarTicket.price}";
    }

    public override void checkConfirmation()
    {
        base.checkConfirmation();

        Ticket tick = parPanel.tarTicket;
        Player_Data playerData = basf.data.currPlayerData;
        if(tick.price<=playerData.money)
        {
            tick.ownedBuy = playerData.playerName;
            tick.isOwned = true;
            playerData.money-=tick.price;
            parPanel.noti.isToPassNext = true;
            parPanel.noti.pop("Hey You have buyed The Ticket Press Oky for next die");
            
        }
    }

    public override void over()
    {
        base.over();
        parPanel.noti.hide(true);
    }




}
