using HtmlAgilityPack;

namespace ChessAnalyzerApi.TemplateMistakesPage;

internal class CbDiagramHtml : ICbDiagramHtml
{
    private readonly HtmlNode Div;
    private readonly CbDiagram Diagram;

    private CbDiagramHtml(CbDiagram cbDiagram)
    {
        Diagram = cbDiagram;
        Div = new HtmlDocument().CreateElement("div");
        CreateDiagramNode();
    }

    private void CreateDiagramNode()
    {
        Div.AddClass("cbdiagram");
        Div.Attributes.Add("id", $"d{Diagram.Id}");
        Div.Attributes.Add("data-size", Diagram.Size);
        Div.Attributes.Add("data-fen", Diagram.Fen);
        Div.Attributes.Add("data-title", Diagram.Title);
        Div.Attributes.Add("data-legend", Diagram.Legend);
        Div.Attributes.Add("data-solution", Diagram.Solution);
    }

    public static CbDiagramHtml Create(CbDiagram cbDiagram) => new(cbDiagram);

    public HtmlNode GetDiagramElement() => Div;
}