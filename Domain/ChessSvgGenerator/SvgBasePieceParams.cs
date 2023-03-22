namespace Domain.ChessSvgGenerator;

public class SvgBasePieceParams : ISvgBaseShapeParams
{
    public Chess.Piece piece { get; private set; }
    public float width { get; private set; }
    public float height { get; private set; }
    public float x { get; private set; }
    public float y { get; private set; }
    public float vbMixX { get; private set; }
    public float vbMinY { get; private set; }
    public float vbWidth { get; private set; }
    public float vbHeight { get; private set; }


    private SvgBasePieceParams(Chess.Piece piece, float width, float height, float x, float y, float vbMixX, float vbMinY, float vbWidth, float vbHeight)
    {
        this.piece = piece;
        this.width = width;
        this.height = height;
        this.x = x;
        this.y = y;
        this.vbMixX = vbMixX;
        this.vbMinY = vbMinY;
        this.vbWidth = vbWidth;
        this.vbHeight = vbHeight;
    }

    public static SvgBasePieceParams Create(Chess.Piece piece, float widthSvg, float heightSvg, float xSvg, float ySvg, float vbMixX, float vbMinY, float vbWidth, float vbHeight)
        => new(piece, widthSvg, heightSvg, xSvg, ySvg, vbMixX, vbMinY, vbWidth, vbHeight);
}
