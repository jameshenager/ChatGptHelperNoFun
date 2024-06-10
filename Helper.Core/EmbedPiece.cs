namespace Helper.Core;

public class EmbedPiece
{
    public int EmbedThingId { get; set; }
    public required string Text { get; set; }
    public required float[] Vector { get; set; }

    public float DotProduct(EmbedPiece other)
    {
        float dotProduct = 0;
        for (var i = 0; i < Vector.Length; i++) { dotProduct += Vector[i] * other.Vector[i]; }
        return dotProduct;
    }

    public static byte[] SerializeVector(List<float> vector)
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream);
        foreach (var value in vector) { binaryWriter.Write(value); }

        var serializedVector = memoryStream.ToArray();
        return serializedVector;
    }

    public static float[] DeserializeVector(byte[] serializedVectorFromDatabase)
    {
        var deserializedVector = new List<float>();
        using var memoryStream = new MemoryStream(serializedVectorFromDatabase);
        using var binaryReader = new BinaryReader(memoryStream);
        while (memoryStream.Position < memoryStream.Length) { deserializedVector.Add(binaryReader.ReadSingle()); }

        return deserializedVector.ToArray();
    }
}