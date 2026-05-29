using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using WordText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace FeelmwLogistika.Blazor.Infrastructure.Formatting;

public readonly record struct RequiredFieldValue(string Text, bool IsWarning);

public static class RequiredFieldFormatter
{
    public const string Placeholder = "???";

    public static RequiredFieldValue Prepare(string? value, bool required)
    {
        bool isWarning = required && string.IsNullOrWhiteSpace(value);
        return new RequiredFieldValue(isWarning ? Placeholder : value ?? "", isWarning);
    }

    public static void SetExcelValue(IXLCell cell, string? value, bool required)
    {
        RequiredFieldValue prepared = Prepare(value, required);
        cell.Value = prepared.Text;

        if (prepared.IsWarning)
        {
            ApplyExcelWarning(cell);
        }
    }

    public static Run CreateDocumentRun(string? value, bool required)
    {
        RequiredFieldValue prepared = Prepare(value, required);
        Run run = new(new WordText(prepared.Text) { Space = SpaceProcessingModeValues.Preserve });

        if (prepared.IsWarning)
        {
            ApplyDocumentWarning(run);
        }

        return run;
    }

    public static void ApplyExcelWarning(IXLCell cell)
    {
        cell.Style.Fill.BackgroundColor = XLColor.Yellow;
        cell.Style.Font.FontColor = XLColor.DarkRed;
        cell.Style.Font.Bold = true;
    }

    public static void ApplyDocumentWarning(Run run)
    {
        RunProperties properties = run.GetFirstChild<RunProperties>() ?? run.PrependChild(new RunProperties());
        properties.Bold = new Bold();
        properties.Color = new Color { Val = "C00000" };
        properties.Shading = new Shading
        {
            Val = ShadingPatternValues.Clear,
            Color = "auto",
            Fill = "FFF2CC"
        };
    }
}
