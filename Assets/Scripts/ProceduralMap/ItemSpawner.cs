using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject prefab;
    [Range(0f, 1f)] public float weight = 1f;
}

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public SpawnEntry[] spawnEntries;

    public int minItemsPerChunk = 1;
    public int maxItemsPerChunk = 4;

    public LayerMask terrainLayer;

    public float minSpawnHeight = 2f;

    public int maxPlacementAttempts = 15;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnItemsOnChunk(
        Vector2 chunkPosition,
        MapData mapData,
        int chunkSize,
        float uniformScale,
        AnimationCurve heightCurve,
        float heightMultiplier)
    {
        int count = Random.Range(minItemsPerChunk, maxItemsPerChunk + 1);
        float worldHalfSize = chunkSize * uniformScale * 0.5f;
        int hmSize = mapData.heightMap.GetLength(0);

        for (int i = 0; i < count; i++)
        {
            for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
            {
                float localU = Random.Range(0f, 1f);
                float localV = Random.Range(0f, 1f);

                float rx = chunkPosition.x * uniformScale + (localU - 0.5f) * chunkSize * uniformScale;
                float rz = chunkPosition.y * uniformScale + (localV - 0.5f) * chunkSize * uniformScale;

                int hmX = Mathf.Clamp(Mathf.RoundToInt(localU * (hmSize - 1)), 0, hmSize - 1);
                int hmY = Mathf.Clamp(Mathf.RoundToInt(localV * (hmSize - 1)), 0, hmSize - 1);

                float normalizedHeight = mapData.heightMap[hmX, hmY];
                float expectedWorldY = heightCurve.Evaluate(normalizedHeight) * heightMultiplier * uniformScale;

                if (expectedWorldY < minSpawnHeight)
                    continue;

                Vector3 rayOrigin = new Vector3(rx, expectedWorldY + 500f, rz);
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
                {
                    if (hit.point.y < minSpawnHeight)
                        continue;

                    GameObject prefab = PickPrefab();
                    if (prefab != null)
                        Instantiate(prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                    break;
                }
            }
        }
    }

    GameObject PickPrefab()
    {
        float totalWeight = 0f;
        foreach (var e in spawnEntries) totalWeight += e.weight;

        float roll = Random.Range(0f, totalWeight);
        foreach (var e in spawnEntries)
        {
            roll -= e.weight;
            if (roll <= 0f) return e.prefab;
        }
        return null;
    }
}
