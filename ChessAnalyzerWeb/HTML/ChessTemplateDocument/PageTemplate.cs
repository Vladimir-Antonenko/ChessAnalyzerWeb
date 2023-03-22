using System;
using System.IO;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;

namespace ChessAnalyzerApi.UI.ChessTemplateDocument;

internal class PageTemplate
{
    private readonly HtmlDocument doc = new();
    private readonly int _numPage;

    private PageTemplate(List<ICbDiagramHtml> cbDiagrams, int numPage)
    {
        _numPage = numPage;
        doc.Load(new Uri(Path.Combine(Environment.CurrentDirectory, @"../../../UI\ChessTemplateDocument\ChessbaseTemplate.html")).AbsolutePath);
        CreateDiagramPart(cbDiagrams);
        EditLinkPart();
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
            link.SetAttributeValue("href", $"games/{i}"); // тут надо подумать какой адрес указать для ссылок вперед/назад
            i++;
        }
    }

    public static PageTemplate Create(List<ICbDiagramHtml> cbDiagrams, int numPage) => new(cbDiagrams, numPage);

    public string GetHtml() => doc.DocumentNode.InnerHtml;
}