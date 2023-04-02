using Godot;

public class Progress_Bar : Node2D
{
    Godot.Collections.Array targetPieces = null;
    Sprite barIcon;

    Basic_Func basf;
    Button path;


    public override void _Ready()
    {
        base._Ready();
        basf = new Basic_Func(this);
        barIcon = this.GetNode<Sprite>("Piece");
        path = this.GetNode<Button>("Path");
        targetPieces = this.GetParent().GetNode("Pieces").GetChildren();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        int maxPos = basf.data.board.maxMoves*targetPieces.Count;
        int totalPieceTarPos = 0;
        foreach (Basic_Piece piece in targetPieces)
        {
            totalPieceTarPos+=piece.currentStep;
        }
        float percentage = (totalPieceTarPos * 100) / maxPos;

        float progressValue = (path.RectSize.y / 100) * percentage;

        barIcon.GlobalPosition = new Vector2(barIcon.GlobalPosition.x, path.RectGlobalPosition.y + path.RectSize.y - progressValue);
    }
}