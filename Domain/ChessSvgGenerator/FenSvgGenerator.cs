using Svg;
using Chess;
using System.Text;

namespace Domain.ChessSvgGenerator;

public partial class FenSvgGenerator : IFenSvgGenerator
{
    public enum ColorKit
    {
        Standart = 1
    }

    private readonly ColorKit colorKit;
    private const int MAX_BOARD_SIZE = 8;
   
    private readonly ChessBoard? board;
    private readonly SvgDocument Svg;
    private string letter => "abcdefgh";
    private float SizeCell => Size / MAX_BOARD_SIZE;

    public int Size { get; private set; }

    private FenSvgGenerator(int size, string fen, ColorKit colorKit)
    {
        if (!ChessBoard.TryLoadFromFen(fen, out board))
            throw new InvalidDataException("Could not load FEN");

        Size = size;
        this.colorKit = colorKit;

        Svg = new()
        {
            Width = size,
            Height = size
        };

        CreateSvgImage(); // запускаю создание изображения
    }

    public static FenSvgGenerator Create(
        int size = 360,
        string fen = ChessHelper.DefaultFen,
        ColorKit colorKit = ColorKit.Standart)
            => new(size, fen, colorKit);

    private void CreateSvgBoard()
    {
        SvgRectangle rect = new()
        {
            X = 0,
            Y = 0,
            Width = Size,
            Height = Size,
            Fill = new SvgColourServer(System.Drawing.Color.FromArgb(181, 136, 99))
        };
        Svg.Children.Add(rect); // контур

        for (int j = 0; j < MAX_BOARD_SIZE; j++)
        {
            for (int i = 0; i < MAX_BOARD_SIZE; i++)
            {
                if ((i + j) % 2 == 1)
                {
                    rect = new SvgRectangle()
                    {
                        X = Size / MAX_BOARD_SIZE * (MAX_BOARD_SIZE - j) - Size / MAX_BOARD_SIZE,
                        Y = Size / MAX_BOARD_SIZE * i,
                        Width = Size / MAX_BOARD_SIZE,
                        Height = Size / MAX_BOARD_SIZE,
                        Fill = new SvgColourServer(System.Drawing.Color.FromArgb(240, 217, 181))
                    };
                    Svg.Children.Add(rect);
                }
            }
        }
    }

    private void CreateSvgImage()
    {
        CreateSvgBoard(); // рисую доску

        for (int j = 0; j < MAX_BOARD_SIZE; j++)
        {
            for (int i = 0; i < MAX_BOARD_SIZE; i++)
            {
                var piece = board![letter[i], (short)(MAX_BOARD_SIZE - j)];

                if (piece is not null)
                {
                    ISvgBaseShapeParams pieceParams = SvgBasePieceParams.Create(
                       piece: piece,
                       widthSvg: SizeCell,
                       heightSvg: SizeCell,
                       xSvg: i * SizeCell,
                       ySvg: j * SizeCell,
                       vbMixX: 0.0f,
                       vbMinY: 0.0f,
                       vbWidth: 100.00001f,
                       vbHeight: 100.00001f);

                    // создаю фигуру
                    SvgBaseShape svgShape = CreateShape(pieceParams);
                    Svg.Children.Add(svgShape.GetSvgPiece()); // добавляю фигуру в файл
                }
            }
        }
    }

    private SvgBaseShape CreateShape(ISvgBaseShapeParams pieceParams) => colorKit switch
    {
        ColorKit.Standart => StandartSvgShapes.Create(SvgStandartPieceParams.Create(pieceParams)), // на основе базовых создаю параметры стандартных фигур и создаю стандартную фигуру
        _ => throw new InvalidDataException("Данный набор цветов для фигур не найден")
    };

    public string GetSvgInBase64()
    {
        var bytes = Encoding.UTF8.GetBytes(GetSvgXML());
        var base64 = Convert.ToBase64String(bytes);
        
        return base64;
    }

    public string GetSvgXML() => Svg.GetXML();
}