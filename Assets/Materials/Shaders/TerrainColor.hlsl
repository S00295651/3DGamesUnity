float4 getBaseColorFromIndex(int index)
{
    float4 colors[10] = {
        _baseColor0, _baseColor1, _baseColor2, _baseColor3, _baseColor4,
        _baseColor5, _baseColor6, _baseColor7, _baseColor8, _baseColor9
    };
    return colors[index];
}

float getBaseStartHeightFromIndex(int index)
{
    float heights[10] = {
        _baseStartHeight0, _baseStartHeight1, _baseStartHeight2,
        _baseStartHeight3, _baseStartHeight4, _baseStartHeight5,
        _baseStartHeight6, _baseStartHeight7, _baseStartHeight8,
        _baseStartHeight9
    };
    return heights[index];
}

float inverseLerp(float a, float b, float v)
{
    return saturate((v - a) / (b - a));
}

void GetColorFromPosition_float(float3 WorldPos, out float4 TerrainColor)
{
    float heightPercent = inverseLerp(_minHeight, _maxHeight, WorldPos.y);
    TerrainColor = float4(0, 0, 0, 1);

    for (int i = 0; i < _baseColorCount; i++)
    {
        float drawStrength = inverseLerp(
            -0.1 - 1e-4,
            0.1,
            heightPercent - getBaseStartHeightFromIndex(i)
        );
        TerrainColor = TerrainColor * (1 - drawStrength)
                     + getBaseColorFromIndex(i) * drawStrength;
    }
}