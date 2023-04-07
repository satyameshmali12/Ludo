/*

Business Board Stuff 
In Progress

*/



using Godot;
using System;

public class Notification : Node2D
{
    Basic_Func basf;
    public bool isToPassNext = false;
    public override void _Ready()
    {
        base._Ready();
        basf = new Basic_Func(this);
    }
    public override void _Process(float delta)
    {
        base._Process(delta);
        if(this.GetNode<Button>("Oky").Pressed)
        {
            hide();
        }
    }
    public void pop(string text)
    {
        Vector2 offset = new Vector2(40,40);
        Camera2D cam = basf.data.targetPiece.getCamera();
        this.GlobalPosition = cam.GlobalPosition-new Vector2(170,150);
        this.Visible = true;
        this.GetNode<Label>("Message").Text = text;
    }
    
    public void hide(bool _isToPassNext = false)
    {
        if(isToPassNext || _isToPassNext)
        {
            this.GetParent<Business_Board>().passOnNextDie();
            BS_Panel bsPanel = this.GetParent().GetNode<BS_Panel>("BS_Panel");
            bsPanel.GetNode<Paying_Node>("Paying_Node").ticket = null;
            bsPanel.hideTheChilds();
            bsPanel.Visible = false;
            bsPanel.waitTimer.Stop();
            bsPanel.tarTicket = null;
        }
        this.Visible = isToPassNext = false;
    }


}
