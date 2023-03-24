using Godot;
using Godot.Collections;
using System;
using System.Collections;

public class Snake_Board : Basic_Board
{
    bool testingResize = true;

    ArrayList boardData = new ArrayList();

    public override void _Ready()
    {

        boardType = Board_Type.Snake_Board;

        base._Ready();
        Godot.Collections.Array arr = this.GetNode<Node2D>("Houses").GetChildren();


        foreach (Node2D house in arr)
        {
            playersData.Add(
                new Player_Data(house, Player_Type.Non_AI, "hello1", arr.IndexOf(house), this.GetNode<Dice>("Dice"))
            );
        }

        configure(100, 0, 100, 1);
        
        Dictionary<string,string> getDataDic(int start,int end,string type)
        {
            return new Dictionary<string,string>(){{"start",start.ToString()},{"end",end.ToString()},{"name",type}};
        }

        boardData = new ArrayList()
        {
            getDataDic(14,34,"transition"),
            getDataDic(21,61,"transition"),
            getDataDic(66,86,"transition"),
            getDataDic(77,97,"transtion"),
            getDataDic(70,92,"transition"),
            getDataDic(45,3,"transition"),
            getDataDic(70,92,"transition"),
            getDataDic(74,33,"transition"),
            getDataDic(30,9,"transition"),
            getDataDic(82,64,"transition"),
        };

        this.GetNode<Dice>("Dice").diceType = "universal";

    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        foreach (Label lab in this.GetNode("Current_Die_Display").GetChildren())
        {
            lab.Visible = (lab.Name.ToLower()==data.currentPlayingType.ToLower());
            lab.Text = data.currentPlayingType;
        }

        if(data.targetPiece!=null)
        {
            data.targetPiece.GetParent().GetParent().GetNode<Sprite>("Lock_Bg").Visible = !data.targetPiece.isUnlocked;
        }
    }

    public override void pieceReachedTargetLocationAction(Basic_Piece piece)
    {
        foreach (Dictionary<string,string> dic in boardData)
        {
            if(dic["name"].ToLower()=="transition" && piece.boardPos==int.Parse(dic["start"]))
            {
                piece.boardPos = int.Parse(dic["end"]);
                piece.currentStep = int.Parse(dic["end"]);
                break;
            }
        }
    }
    
    public override void playerWinAction(Player_Data winPlayer) 
    {
        Node2D house = winPlayer.getHouse();
        house.GetNode<Node2D>("Win_Condition").Visible = true;
        house.GetNode<Label>("Win_Condition/Rank").Text = (winPlayerData.IndexOf(winPlayer)+1).ToString();
    }



}
