namespace Helper.Core.Utils;

public static class MathHelper
{
    public static float DotProduct(float[] vector1, float[] vector2)
    {
        float dotProduct = 0;
        for (var i = 0; i < vector1.Length; i++) { dotProduct += vector1[i] * vector2[i]; }

        return dotProduct;
    }
}