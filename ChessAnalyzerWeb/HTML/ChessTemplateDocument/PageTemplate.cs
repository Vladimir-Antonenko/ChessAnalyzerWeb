using HtmlAgilityPack;
using Domain.GameAggregate;
using ChessAnalyzerApi.Extensions;
using Domain.Extensions;

namespace ChessAnalyzerApi.UI.ChessTemplateDocument;

internal class PageTemplate
{
    private const int BOARD_SIZE = 400;
    private readonly string _linkPattern;
    private readonly HtmlDocument doc = new();
    private readonly int _numPage;

    private PageTemplate(List<ICbDiagramHtml> cbDiagrams, int numPage, string linkPattern)
    {
        _linkPattern = linkPattern;
        _numPage = numPage < 1 ? 1 : numPage; // если меньше чем первая страница, то принудительно ставлю первую
        doc.Load(new Uri(Path.Combine(Environment.CurrentDirectory, @"../../../HTML\ChessTemplateDocument\ChessbaseTemplate.html")).AbsolutePath);
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
        foreach(var diagram in cbDiagrams)
        {
            var diagramElem = diagram.GetDiagramElement();
            AddDiagramNode(diagramElem);
        }
    }

    private void EditLinkPart()
    {
        int i = _numPage;
        foreach (var link in doc.DocumentNode.ChildNodes.Where(x => x.Id.Contains("link")).OrderBy(x=>x.Id))
        {
            link.SetAttributeValue("href", $"{_linkPattern}{i}");
            i++;
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

    public static PageTemplate Create(List<ICbDiagramHtml> cbDiagrams, int numPage, string linkPattern) => new(cbDiagrams, numPage, linkPattern);

    public static PageTemplate Create(PagedList<Position> mistakes, int numPage, string linkPattern) => new(mistakes, numPage, linkPattern);

    public string GetHtml() => doc.DocumentNode.InnerHtml;
}