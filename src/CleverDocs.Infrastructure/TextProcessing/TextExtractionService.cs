using CleverDocs.Core.Abstractions.Documents;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace CleverDocs.Infrastructure.TextProcessing;

public class TextExtractionService(Serilog.ILogger logger) : ITextExtractionService
{
    public async Task<string> ExtractTextAsync(string filePath, string contentType)
    {
        try
        {
            return contentType switch
            {
                "application/pdf" => ExtractTextFromPdf(filePath),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ExtractTextFromDocx(
                    filePath),
                "text/plain" or "text/markdown" => ExtractTextFromPlainText(filePath),
                _ => throw new NotSupportedException($"File type {contentType} is not supported.")
            };
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error while extracting text from file {FilePath}", filePath);
        }

        return string.Empty;
    }

    private string ExtractTextFromPdf(string filePath)
    {
        using var pdf = PdfDocument.Open(filePath);
        return string.Join("\n", pdf.GetPages().Select(p => p.Text));
    }
    
    private string ExtractTextFromDocx(string filePath)
    {
        using var doc = WordprocessingDocument.Open(filePath, false);
        return doc.MainDocumentPart?.Document.Body?.InnerText ?? "";
    }

    private string ExtractTextFromPlainText(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}