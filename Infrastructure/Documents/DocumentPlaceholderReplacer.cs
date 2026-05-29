using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using FeelmwLogistika.Blazor.Infrastructure.Formatting;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FeelmwLogistika.Blazor.Infrastructure.Documents;

internal static class DocumentPlaceholderReplacer
{
    public static void Replace(Body body, IEnumerable<DocumentPlaceholderValue> values)
    {
        List<DocumentPlaceholderValue> replacements = values
            .Where(value => !string.IsNullOrWhiteSpace(value.Marker))
            .ToList();

        if (replacements.Count == 0)
        {
            return;
        }

        foreach (Paragraph paragraph in body.Descendants<Paragraph>().ToList())
        {
            string text = ParagraphText(paragraph);
            if (!replacements.Any(value => text.Contains(value.Marker, StringComparison.Ordinal)))
            {
                continue;
            }

            ReplaceParagraph(paragraph, text, replacements);
        }
    }

    public static string ParagraphText(Paragraph paragraph)
    {
        return string.Concat(paragraph.Descendants<WordText>().Select(text => text.Text));
    }

    private static void ReplaceParagraph(Paragraph paragraph, string text, IReadOnlyList<DocumentPlaceholderValue> replacements)
    {
        List<OpenXmlElement> oldChildren = paragraph.ChildElements.ToList();
        foreach (OpenXmlElement child in oldChildren.Where(child => child is Run or Hyperlink))
        {
            child.Remove();
        }

        int current = 0;
        while (current < text.Length)
        {
            (int index, DocumentPlaceholderValue replacement) = FindNext(text, current, replacements);
            if (index < 0)
            {
                AppendPlainText(paragraph, text[current..]);
                break;
            }

            if (index > current)
            {
                AppendPlainText(paragraph, text[current..index]);
            }

            paragraph.AppendChild(replacement.Required
                ? RequiredFieldFormatter.CreateDocumentRun(replacement.Value, required: true)
                : CreatePlainRun(replacement.Value ?? ""));

            current = index + replacement.Marker.Length;
        }
    }

    private static (int Index, DocumentPlaceholderValue Replacement) FindNext(string text, int start, IReadOnlyList<DocumentPlaceholderValue> replacements)
    {
        int nextIndex = -1;
        DocumentPlaceholderValue nextReplacement = default;

        foreach (DocumentPlaceholderValue replacement in replacements)
        {
            int index = text.IndexOf(replacement.Marker, start, StringComparison.Ordinal);
            if (index >= 0 && (nextIndex < 0 || index < nextIndex))
            {
                nextIndex = index;
                nextReplacement = replacement;
            }
        }

        return (nextIndex, nextReplacement);
    }

    private static void AppendPlainText(Paragraph paragraph, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            paragraph.AppendChild(CreatePlainRun(value));
        }
    }

    private static Run CreatePlainRun(string value)
    {
        return new Run(new WordText(value) { Space = SpaceProcessingModeValues.Preserve });
    }
}
