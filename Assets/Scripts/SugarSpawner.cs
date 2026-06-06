using UnityEngine;

public class SugarSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject sugarPrefab;

    [Header("Spawn Settings")]
    public int totalSugarCount = 30;
    public float floatHeight = 0.8f;

    [Header("Height Control")]
    [Tooltip("Sugar will only spawn if the hill height is above this number (prevents sand/water spawn)")]
    public float minSpawnHeight = 2.5f; 

    void Start()
    {
        if (sugarPrefab == null)
        {
            Debug.LogError("SUGAR SPAWNER: Please drag your Sugar Prefab into the inspector slot!");
            return;
        }

        SpawnSugarOnIslands();
    }

    void SpawnSugarOnIslands()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("SUGAR SPAWNER: No active terrain found in the scene!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        
        // Auto-calculate the exact world bounds of your terrain asset
        float terrainMinX = terrain.transform.position.x;
        float terrainMaxX = terrainMinX + terrainData.size.x;
        float terrainMinZ = terrain.transform.position.z;
        float terrainMaxZ = terrainMinZ + terrainData.size.z;

        int spawnedCount = 0;
        int safetyLoop = 0;

        while (spawnedCount < totalSugarCount && safetyLoop < 2000)
        {
            safetyLoop++;

            // Pick a random spot inside your actual terrain dimensions
            float randomX = Random.Range(terrainMinX, terrainMaxX);
            float randomZ = Random.Range(terrainMinZ, terrainMaxZ);

            // Directly read the exact surface height from the terrain data
            float groundY = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrain.transform.position.y;

            // Check if this height matches your green island hills instead of the low beach sand
            if (groundY >= minSpawnHeight)
            {
                Vector3 spawnPosition = new Vector3(randomX, groundY + floatHeight, randomZ);
                
                // Spawn the sugar block object
                Instantiate(sugarPrefab, spawnPosition, Quaternion.identity);
                spawnedCount++;
            }
        }

        Debug.Log("SUGAR SPAWNER COMPLETE: Spawned " + spawnedCount + " floating crystals across the islands. Loop attempts: " + safetyLoop);
    }
}