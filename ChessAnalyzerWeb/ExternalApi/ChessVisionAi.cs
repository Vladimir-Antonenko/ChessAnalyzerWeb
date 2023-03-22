using System;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.ExternalApi;

public class ChessVisionAi
{
    public Color Orientation { get; private set; }
    public string Fen { get; private set; }
    public byte[]? Image { get; private set; }

    private ChessVisionAi(string fen, Color orientation)
    {
        Fen = fen;
        Orientation = orientation;
    }

    public void GetImage()
    {
        throw new NotImplementedException("Получение картинок с ChessVisionAi пока не реализовано");
        // написать тут
    }

    private string GetTextColor() => Orientation.Equals(Color.White) ? "white" : "black";

    private string GetImageUrl() => $@"https://fen2image.chessvision.ai/{Fen}?pov={GetTextColor()}";

    public static ChessVisionAi Create(string fen, Color orientation) => new(fen, orientation);
}