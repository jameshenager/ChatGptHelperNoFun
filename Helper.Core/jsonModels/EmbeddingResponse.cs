// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
namespace Helper.Core.jsonModels;

public class EmbeddingResponse
{
    public List<EmbeddingData?>? data { get; set; }
    public string? model { get; set; }
    public string? @object { get; set; }
    public Usage? usage { get; set; }
}