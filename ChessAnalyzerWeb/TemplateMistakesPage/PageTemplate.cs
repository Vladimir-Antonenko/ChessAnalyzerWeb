using Domain.Common;
using HtmlAgilityPack;
using Domain.Extensions;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.TemplateMistakesPage;

/// <summary>
/// Создает шаблон страницы для показа позиций в которых были сделаны ошибки
/// </summary>
internal class PageTemplate
{
    private const int BOARD_SIZE = 400;
    private readonly string _link;
    private readonly HtmlDocument doc = new();
    private readonly int _numPage;

    private PageTemplate(PagedList<Position> mistakes, int numPage, string linkPattern)
    {
        _link = linkPattern;
        _numPage = numPage < 1 ? 1 : numPage; // если меньше чем первая страница, то принудительно ставлю первую
        doc.Load(Path.Combine(Environment.CurrentDirectory, @"TemplateMistakesPage\ChessbaseTemplate.html"));
        CreateDiagramPart(mistakes);
        EditLinkPart(mistakes.TotalPages);
    }

    private void AddDiagramNode(HtmlNode diagram, string pathTo = "//body")
    {
        var htmlBody = doc.DocumentNode.SelectSingleNode(pathTo);
        htmlBody.AppendChild(diagram);
    }

    /// <summary>
    /// Создает часть html документа, которая содержит Chessbase диаграммы
    /// </summary>
    /// <param name="mistakes">Позиции в которых были совершены ошибки</param>
    private void CreateDiagramPart(PagedList<Position> mistakes)
    {
        var cbDiagrams = CreateFromPositions(mistakes);

        foreach (var diagram in cbDiagrams)
        {
            var diagramElem = diagram.GetDiagramElement();
            AddDiagramNode(diagramElem);
        }
    }

    private void EditLinkPart(int totalPages)
    {
        var removeDigit = _link.LastIndexOf("/");
        var link = _link.Remove(removeDigit, _link.Length - removeDigit);

        var backLink = doc.GetElementbyId("link1");
        var nextLink = doc.GetElementbyId("link2");

        if (_numPage <= 1) // первичная инициализация
        {
            backLink.SetAttributeValue("href", $"{link}/{1}");
            nextLink.SetAttributeValue("href", $"{link}/{2}");
        }
        else
        {
            if(_numPage >= totalPages)
            {
                backLink.SetAttributeValue("href", $"{link}/{ _numPage - 1 }");
                nextLink.SetAttributeValue("href", $"{link}/{ _numPage }");
            }
            else
            {
                backLink.SetAttributeValue("href", $"{link}/{ _numPage - 1 }");
                nextLink.SetAttributeValue("href", $"{link}/{ _numPage + 1 }");
            }
        }
    }

    private List<ICbDiagramHtml> CreateFromPositions(PagedList<Position> mistakes)
    {
        List<ICbDiagramHtml> diagrams = new();

        foreach (var (pos, index) in mistakes.WithIndex())
        {
            CbDiagram cbDiagram = new()
            {
                Fen = pos.Fen,
                Size = BOARD_SIZE.ToString(),
                Id = index.ToString(),
                Legend = string.Empty,
                Solution = pos.PositionEvaluation?.OneMove ?? string.Empty,
                Title = $"Был сделан ход {pos.YourMove}"
            };
            diagrams.Add(CbDiagramHtml.Create(cbDiagram));
        }

        return diagrams;
    }

    // public static PageTemplate Create(List<ICbDiagramHtml> cbDiagrams, int numPage, string link) => new(cbDiagrams, numPage, link);

    public static PageTemplate Create(PagedList<Position> mistakes, int numPage, string requestUrl) => new(mistakes, numPage, requestUrl);

    public string GetHtml() => doc.DocumentNode.InnerHtml;
}