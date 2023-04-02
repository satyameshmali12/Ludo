using Godot;
using System.Collections.Generic;
using System.Collections;

public class Data_Taker : Control
{
    // public string navigationUrl;
    int currentEditingIndex = 0;
    int minPlayer = 2;
    OptionButton playerType;
    Dictionary<string, string> getRawPlayerData(string type, string name, bool isPlaying, bool isAI)
    {
        return new Dictionary<string, string>() { { "Type", type }, { "Name", name }, { "Is_Playing", isPlaying.ToString() }, { "Is_AI", isAI.ToString() } };
    }

    List<Dictionary<string, string>> rawPlayerData;
    List<Dictionary<string,string>> rawPlayerDataCopy;
    Basic_Func basf;

    Timer doubleClickTimer;
    Vector2 distance = Vector2.Zero;


    List<Dictionary<string,string>> getListCopy(List<Dictionary<string,string>> data)
    {
        List<Dictionary<string,string>> copyData = new List<Dictionary<string,string>>();
        foreach (Dictionary<string,string> cDa in data)
        {
            copyData.Add(
                new Dictionary<string,string>(){{ "Type", cDa["Type"] }, { "Name", cDa["Name"] }, { "Is_Playing", cDa["Is_Playing"] }, { "Is_AI", cDa["Is_AI"] }}
            );
        }
        return copyData;
    }
    bool isDoubleClick = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        basf = new Basic_Func(this);
        doubleClickTimer = basf.getTimer(1,"OverDoubleClick",true);
        this.AddChild(doubleClickTimer);
        rawPlayerData = new List<Dictionary<string, string>>()
        {
            getRawPlayerData("Green","Green",false,false),
            getRawPlayerData("Yellow","Yellow",false,false),
            getRawPlayerData("Blue","Blue",false,false),
            getRawPlayerData("Red","Red",false,false),
        };
        rawPlayerDataCopy = getListCopy(rawPlayerData);
        playerType = this.GetNode<OptionButton>("Inputs/Player_Type");
        playerType.AddItem("AI");
        playerType.AddItem("Non_AI");
        loadAvaiDataOnScreen();
    }

    public override void _Process(float delta)
    {

        if(Input.IsActionJustPressed("Mouse_Pressed"))
        {
            if(!doubleClickTimer.IsStopped())
            {
                isDoubleClick = true;
            }
            else{
                doubleClickTimer.Start();
            }
        }
        
        isDoubleClick = (!Input.IsActionPressed("Mouse_Pressed"))?false:isDoubleClick;

        if(isDoubleClick)
        {
            if(distance==Vector2.Zero)
            {
                distance = this.GetLocalMousePosition();
            }
            this.RectGlobalPosition = this.GetGlobalMousePosition()-distance;
        }
        else{
            distance = Vector2.Zero;
        }

        int change = (Input.IsActionJustPressed("ui_left")) ? -1 : (Input.IsActionJustPressed("ui_right")) ? 1 : 0;
        if (change != 0)
        {
            currentEditingIndex += change;
            currentEditingIndex = basf.minMaxer(currentEditingIndex, 3, 0);
            loadAvaiDataOnScreen();
        }

        // storing the data to the rawPlayerData
        rawPlayerData[currentEditingIndex]["Name"] = this.GetNode<TextEdit>("Inputs/Name").Text;
        rawPlayerData[currentEditingIndex]["Is_AI"] = ((playerType.GetItemText(playerType.Selected)=="AI")?"true":"false");



        Button addButton = this.GetNode<Button>("Add");
        addButton.Text = (bool.Parse(rawPlayerData[currentEditingIndex]["Is_Playing"])) ? "Remove" : "Add";
        if (addButton.Pressed && Input.IsActionJustPressed("Mouse_Pressed"))
        {
            rawPlayerData[currentEditingIndex]["Is_Playing"] = (!bool.Parse(rawPlayerData[currentEditingIndex]["Is_Playing"])).ToString();
        }

        if(this.GetNode<Button>("Close_Button").Pressed)
        {
            this.Visible = false;
        }

        int playingCount = 0;
        foreach (Dictionary<string,string> data in rawPlayerData)
        {
            if(bool.Parse(data["Is_Playing"]))
            {
                playingCount++;
            }
        }
        Button navBut = this.GetNode<Button>("Navigate");
        navBut.Visible = (playingCount>1); 
        if(navBut.Pressed)
        {
            basf.data.playerData = rawPlayerData;
            this.GetTree().ChangeScene(basf.data. navigationUrl);
        }


    }

    public void loadAvaiDataOnScreen()
    {
        this.GetNode<TextEdit>("Inputs/Name").Text = rawPlayerData[currentEditingIndex]["Name"];
        this.GetNode<Label>("Labels/Type_Label").Text = rawPlayerData[currentEditingIndex]["Type"];

        string type = (bool.Parse(rawPlayerData[currentEditingIndex]["Is_AI"])?"AI":"Non_AI");
        for (int i = 0; i < playerType.GetItemCount(); i++)
        {
            if(playerType.GetItemText(i)==type)
            {
                playerType.Select(i);
            }
        }
    }
    public void resetData()
    {

        rawPlayerData = getListCopy(rawPlayerDataCopy);
        loadAvaiDataOnScreen();
    }

    public void OverDoubleClick(){}

}

