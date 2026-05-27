namespace FeelmwLogistika.Blazor.Models;

public class DocumentGenerationResult
{
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = "";
    public List<string> Warnings { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public bool Success => Content.Length > 0 && Errors.Count == 0;
}
