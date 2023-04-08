using HtmlAgilityPack;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using Domain.Common;

namespace ChessAnalyzerApi.TemplateMistakesPage;

internal class PageTemplate
{
    private const int BOARD_SIZE = 400;
    private readonly string _link;
    private readonly HtmlDocument doc = new();
    private readonly int _numPage;

    private PageTemplate(List<ICbDiagramHtml> cbDiagrams, int numPage, string link)
    {
        _link = link;
        _numPage = numPage < 1 ? 1 : numPage; // если меньше чем первая страница, то принудительно ставлю первую
        doc.Load(Path.Combine(Environment.CurrentDirectory, @"TemplateMistakesPage\ChessbaseTemplate.html"));
        CreateDiagramPart(cbDiagrams);
        EditLinkPart();
    }

    private PageTemplate(PagedList<Position> mistakes, int numPage, string linkPattern) : this(CreateFromPositions(mistakes), numPage, linkPattern)
    {
    }

    private void AddDiagramNode(HtmlNode diagram, string pathTo = "//body")
    {
        var htmlBody = doc.DocumentNode.SelectSingleNode(pathTo);
        htmlBody.AppendChild(diagram);
    }

    private void CreateDiagramPart(List<ICbDiagramHtml> cbDiagrams)
    {
        foreach (var diagram in cbDiagrams)
        {
            var diagramElem = diagram.GetDiagramElement();
            AddDiagramNode(diagramElem);
        }
    }

    private void EditLinkPart()
    {
        int i = _numPage;
        var removeDigit = _link.LastIndexOf("/");
        var link = _link.Remove(removeDigit, _link.Length - removeDigit);

        var backLink = doc.GetElementbyId("link1");
        var nextLink = doc.GetElementbyId("link2");

        if (int.Parse(nextLink.GetAttributeValue("alt", "2")) == i)
        {
            nextLink.SetAttributeValue("href", $"{link}/{i}");
            nextLink.SetAttributeValue("alt", $"{i}");
            backLink.SetAttributeValue("href", $"{link}/{--i}");
            backLink.SetAttributeValue("alt", $"{i}");
        }
        else
        {
            backLink.SetAttributeValue("href", $"{link}/{i}");
            backLink.SetAttributeValue("alt", $"{i}");
            nextLink.SetAttributeValue("href", $"{link}/{++i}");
            nextLink.SetAttributeValue("alt", $"{i}");
        }
    }

    private static List<ICbDiagramHtml> CreateFromPositions(PagedList<Position> mistakes)
    {
        List<ICbDiagramHtml> Diagrams = new();

        foreach (var (pos, index) in mistakes.WithIndex())
        {
            CbDiagram cbDiagram = new()
            {
                Fen = pos.Fen,
                Size = BOARD_SIZE.ToString(),
                Id = index.ToString(),
                Legend = string.Empty,
                Solution = pos.PositionEvaluation?.OneMove ?? string.Empty,
                Title = string.Empty
            };
            Diagrams.Add(CbDiagramHtml.Create(cbDiagram));
        }

        return Diagrams;
    }

    // public static PageTemplate Create(List<ICbDiagramHtml> cbDiagrams, int numPage, string link) => new(cbDiagrams, numPage, link);

    public static PageTemplate Create(PagedList<Position> mistakes, int numPage, string requestUrl) => new(mistakes, numPage, requestUrl);

    public string GetHtml() => doc.DocumentNode.InnerHtml;
}