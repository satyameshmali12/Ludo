using Godot;
using System;

public class Pause_Menu : Node2D
{
    public override void _Ready()
    {
        this.PauseMode = PauseModeEnum.Process;
    }

    public override void _Process(float delta)
    {
        if (this.Visible)
        {
            this.GetTree().Paused = true;

            if (this.GetNode<Button>("Resume").Pressed)
            {

                this.Visible = false;
                this.GetTree().Paused = false;

            }
        }

    }
}
