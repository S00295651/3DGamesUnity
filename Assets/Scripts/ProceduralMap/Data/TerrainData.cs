using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData")]
public class TerrainData : UpdatableData
{
    public float uniformScale = 10f;

    public bool useFlatShading;
    public bool useFalloff;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
}
