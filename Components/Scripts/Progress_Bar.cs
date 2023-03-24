using Godot;

public class Progress_Bar : Node2D
{
    Basic_Piece targetPiece = null;
    Sprite barIcon;

    Basic_Func basf;
    Button path;


    public override void _Ready()
    {
        base._Ready();
        basf = new Basic_Func(this);
        barIcon = this.GetNode<Sprite>("Piece");
        path = this.GetNode<Button>("Path");
        targetPiece = this.GetParent().GetNode("Pieces").GetChildren()[0] as Basic_Piece;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        int maxPos = basf.data.board.maxMoves;
        int pieceTarPos = targetPiece.boardPos;
        float percentage = (pieceTarPos * 100) / maxPos;

        float progressValue = (path.RectSize.y / 100) * percentage;

        barIcon.GlobalPosition = new Vector2(barIcon.GlobalPosition.x, path.RectGlobalPosition.y + path.RectSize.y - progressValue);
    }
}