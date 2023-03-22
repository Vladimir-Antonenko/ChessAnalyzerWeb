using HtmlAgilityPack;

namespace ChessAnalyzerApi.UI.ChessTemplateDocument;

public interface ICbDiagramHtml
{
    public HtmlNode GetDiagramElement();
}
