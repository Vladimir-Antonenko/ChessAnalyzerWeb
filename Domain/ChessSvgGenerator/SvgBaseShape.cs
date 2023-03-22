using Chess;
using Svg;

namespace Domain.ChessSvgGenerator;

public abstract class SvgBaseShape
{
    public SvgDocument pieceSvg { get; private set; }
    private Chess.Piece piece { get; }

    protected string IdColorGradientString => $"{piece.Color.AsChar}{piece.Type.AsChar}"; 

    public abstract IReadOnlyDictionary<System.Drawing.Color, float> StandartWhiteColors { get; }

    public abstract IReadOnlyDictionary<System.Drawing.Color, float> StandartBlackColors { get; }

    public SvgBaseShape(ISvgBaseShapeParams svgParams) 
    {
        piece = svgParams.piece;

        pieceSvg = new SvgDocument()
        {
            Width = svgParams.width,
            Height = svgParams.height,
            X = svgParams.x,
            Y = svgParams.y,
            ViewBox = new SvgViewBox()
            {
                MinX = svgParams.vbMixX,
                MinY = svgParams.vbMinY, 
                Width = svgParams.vbWidth,
                Height = svgParams.vbHeight
            }
        };

        DrawShape();
    }

    public SvgDocument GetSvgPiece() => pieceSvg;

    protected abstract void DrawKing();

    protected abstract void DrawQueen();

    protected abstract void DrawRook();
 
    protected abstract void DrawBishop();

    protected abstract void DrawKnight();

    protected abstract void DrawPawn();

    protected void DrawShape()
    {
        pieceSvg.Children.Clear(); // очищаю предыдущие зарисовки если они были

        switch (piece.Type.AsChar)
        {
            case 'p':
                DrawPawn();
                break;
            case 'n':
                DrawKnight();
                break;
            case 'b':
                DrawBishop();
                break;
            case 'r':
                DrawRook();
                break;
            case 'q':
                DrawQueen();
                break;
            case 'k':
                DrawKing();
                break;
        }
    }

    protected IReadOnlyDictionary<System.Drawing.Color, float> GetColorsInStandartKit() => piece.Color.AsChar switch
    {
        'b' => StandartBlackColors,
        'w' => StandartWhiteColors,
        _ => throw new InvalidDataException("Проблема определения цвета в цветовом наборе фигур")
    };
}
