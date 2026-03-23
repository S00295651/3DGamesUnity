using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] layers;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        Shader.SetGlobalInt("layerCount", layers.Length);
        Shader.SetGlobalVectorArray("baseColours", System.Array.ConvertAll(layers, x => (Vector4)x.tint));
        Shader.SetGlobalFloatArray("baseStartHeights", System.Array.ConvertAll(layers, x => x.startHeight));
        Shader.SetGlobalFloatArray("baseBlends", System.Array.ConvertAll(layers, x => x.blendStrength));
        Shader.SetGlobalFloatArray("baseColourStrength", System.Array.ConvertAll(layers, x => x.tintStrength));
        Shader.SetGlobalFloatArray("baseTextureScales", System.Array.ConvertAll(layers, x => x.textureScale));

        Texture2DArray texturesArray = GenerateTextureArray(System.Array.ConvertAll(layers, x => x.texture));
        Shader.SetGlobalTexture("baseTextures", texturesArray);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);
            }
        }
        textureArray.Apply();
        return textureArray;
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        Shader.SetGlobalFloat("minHeight", minHeight);
        Shader.SetGlobalFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Layer
    {
        public string name;
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;
    }
}