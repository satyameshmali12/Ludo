using Godot;
using System;
using System.Collections.Generic;

public class Win_Screen : Pause_Menu
{
    bool isDataLoaded = false;

    public override void _Ready()
    {
        base._Ready();

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (basf.data.isGameCompleted && !isDataLoaded)
        {
            Godot.Collections.Array winLabels = this.GetNode("Win_Labels").GetChildren();
            List<Player_Data> playerData = basf.data.winPlayerData;
            foreach (Node2D item in winLabels)
            {
                int winLabInd = winLabels.IndexOf(item);
                if (winLabInd < playerData.Count - 1 || winLabels.IndexOf(item) == winLabels.Count - 1)
                {
                    item.GetNode<Label>("Name").Text = playerData[(winLabInd > playerData.Count - 1) ? playerData.Count - 1 : winLabInd].playerName;
                }
                else
                {
                    item.Visible = false;
                }
            }
            isDataLoaded = true;
        }
    }
}
