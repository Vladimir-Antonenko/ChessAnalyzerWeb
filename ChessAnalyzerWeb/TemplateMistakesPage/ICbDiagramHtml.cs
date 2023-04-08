using HtmlAgilityPack;

namespace ChessAnalyzerApi.TemplateMistakesPage;

public interface ICbDiagramHtml
{
    public HtmlNode GetDiagramElement();
}