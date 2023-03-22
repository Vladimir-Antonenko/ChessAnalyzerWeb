namespace Domain.ChessSvgGenerator;

public class SvgStandartPieceParams : ISvgBaseShapeParams
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

    private SvgStandartPieceParams(ISvgBaseShapeParams baseShapeParams)
    {
        this.piece = baseShapeParams.piece;
        this.width = baseShapeParams.width;
        this.height = baseShapeParams.height;
        this.x = baseShapeParams.x;
        this.y = baseShapeParams.y;
        this.vbMixX = baseShapeParams.vbMixX;
        this.vbMinY = baseShapeParams.vbMinY;
        this.vbWidth = baseShapeParams.vbWidth;
        this.vbHeight = baseShapeParams.vbHeight;

        // плюс какие-то дополнительные параметры если будут
    }

    public static SvgStandartPieceParams Create(ISvgBaseShapeParams baseShapeParams)
        => new(baseShapeParams);
}
