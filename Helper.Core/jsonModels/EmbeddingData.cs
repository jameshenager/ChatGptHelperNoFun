// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
namespace Helper.Core.jsonModels;

public class EmbeddingData
{
    public List<float> embedding { get; set; } /* 1536. 2^1536 ~= 2.4*10^425 */
    public int index { get; set; }
    public string @object { get; set; }
}