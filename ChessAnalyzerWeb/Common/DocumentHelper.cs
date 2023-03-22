using HtmlRendererCore.PdfSharp;

namespace ChessAnalyzerApi.Common;

public static class DocumentHelper
{
    public static string PdfSharpConvert(string html)
    {
        var result = string.Empty;

        using (var stream = new MemoryStream())
        {
            var pdf = PdfGenerator.GeneratePdf(html, PdfSharpCore.PageSize.A4);

            pdf.Save(stream);

            result = Convert.ToBase64String(stream.ToArray());
        }

        return result;
    }
}