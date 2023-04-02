using Godot;
using System;

public class Pause_Menu : Node2D
{
    public Basic_Func basf;

    public override void _Ready()
    {
        basf = new Basic_Func(this);
        this.PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        if (this.Visible)
        {
            this.GetTree().Paused = true;

            if (this.HasNode("Resume") && this.GetNode<Button>("Resume").Pressed)
            {
                this.Visible = false;
                this.GetTree().Paused = false;
            }

            if(this.GetNode<Button>("Restart").Pressed)
            {
                this.GetTree().ChangeScene(basf.data.navigationUrl);
            }
            
            else if(this.GetNode<Button>("Home").Pressed)
            {
                this.GetTree().ChangeScene("res://Components/Scenes/Home.tscn");
            }
        }

    }
}
