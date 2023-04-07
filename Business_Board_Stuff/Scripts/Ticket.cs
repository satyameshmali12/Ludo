using Godot;

public class Ticket:ReferenceRect
{

    public string name;
    public string groupType;
    public int price;
    public string ownedBuy = "";
    public bool isOwned = false;
    public int mortgage = 0;

    Data_Manager data_Manager;

    public override void _Ready()
    {
        data_Manager = new Data_Manager("data/data_fields/BusinessBoardRectDataField.txt");
        name = this.GetNode<Label>("Name").Text.ToLower();
        data_Manager.loadData(name,"Name");
        price = int.Parse(data_Manager.getData("Price"));
        groupType = data_Manager.getData("Group_Type");
        mortgage = int.Parse(data_Manager.getData("Mortgage"));
    }

}