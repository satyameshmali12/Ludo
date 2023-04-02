using Godot;
using System;

public class Home : Control
{
    Data_Taker dataTaker;
    string lastSelectedOption = null;
    Basic_Func basf;
    public override void _Ready()
    {
        this.GetTree().Paused = false;
        basf = new Basic_Func(this);
        basf.data.isGameCompleted = false;

        dataTaker = this.GetNode<Data_Taker>("Data_Taker");

    }

    public override void _Process(float delta)
    {

        foreach (Button but in this.GetNode("Start_Buttons").GetChildren())
        {
            if(but.Pressed && !dataTaker.Visible && Input.IsActionJustPressed("Mouse_Pressed"))
            {
                if(lastSelectedOption!=but.Name)
                {
                    dataTaker.resetData();
                }
                dataTaker.Visible = true;
                basf.data.navigationUrl = but.GetNode<Label>("Navigation_Url").Text;
                lastSelectedOption = but.Name;
            }
        }
        
    }

}
