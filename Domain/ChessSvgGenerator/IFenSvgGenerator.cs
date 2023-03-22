namespace Domain.ChessSvgGenerator;

public interface IFenSvgGenerator
{
    public string GetSvgInBase64();
    public string GetSvgXML();
}
