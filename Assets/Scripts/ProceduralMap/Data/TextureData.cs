using UnityEngine;

public class TextureData : UpdatableData
{
    public void ApplyToMaterial(Material material)
    {
        material.SetColor("backgroundColour", baseColour);
        material.SetColor("lightColour", lightColour);
        material.SetColor("darkColour", darkColour);
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
