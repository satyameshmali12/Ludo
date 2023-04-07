/*

Business Board Stuff 
In Progress

*/



using Godot;
using System;

public class SB_Interlinking : Node2D
{
    public Label priceTag;
    public BS_Panel parPanel;
    public Button confirm;
    public Basic_Func basf;
    public Business_Board board;
    public override void _Ready()
    {
        board = this.GetParent().GetParent<Business_Board>();
        basf = new Basic_Func(this);
        confirm = this.GetNode<Button>("Confirm");
        parPanel = this.GetParent<BS_Panel>();
        priceTag = this.GetParent().GetNode<Label>("Price");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (confirm.Pressed && Input.IsActionJustPressed("Mouse_Pressed"))
        {
            if (!parPanel.noti.Visible)
            {
                checkConfirmation();
            }
        }
    }

    public void setMessage(string message)
    {
        this.GetParent().GetNode<Label>("Task").Text = message;

    }

    public virtual void toggle()
    {
        this.Visible = true;
    }

    public virtual void over()
    {

    }

    public virtual void checkConfirmation() { }
}
