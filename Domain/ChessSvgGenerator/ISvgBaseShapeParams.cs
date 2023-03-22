namespace Domain.ChessSvgGenerator;

public interface ISvgBaseShapeParams
{
    public Chess.Piece piece { get; }
    public float width { get; }
    public float height { get; }
    public float x { get; }
    public float y { get; }
    public float vbMixX { get; }
    public float vbMinY { get; }
    public float vbWidth { get; }
    public float vbHeight { get; }
}
