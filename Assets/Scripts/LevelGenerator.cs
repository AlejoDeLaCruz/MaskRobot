using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Prefabs")]
    public GameObject[] prefabs; // asignar 3 prefabs

    [Header("Spawn spacing")]
    public float initialSpawnXOffset = 10f; // primera aparición relativa al player.x
    public float minGap = 3f; // distancia mínima X entre spawns
    public float maxGap = 6f; // distancia máxima X entre spawns

    [Header("Y positioning (elige opción)")]
    public bool useYRange = true;
    public Vector2 yRange = new Vector2(-1f, 1f);
    public float[] possibleYPositions; // si prefieres posiciones fijas, llena el array y pon useYRange = false

    [Header("Trigger / Despawn")]
    public float spawnAheadDistance = 15f; // si player.x + spawnAheadDistance >= nextSpawnX -> spawn
    public float despawnBehindDistance = 12f; // si un objeto queda player.x - despawnBehindDistance -> se libera

    // estado interno
    private float nextSpawnX;
    private int lastPrefabIndex = -1;

    private void Start()
    {
        if (player == null) Debug.LogError("Asignar player al InfiniteSpawner");
        if (prefabs == null || prefabs.Length == 0) Debug.LogError("Asignar prefabs al InfiniteSpawner");

        nextSpawnX = player.position.x + initialSpawnXOffset;
    }

    private void Update()
    {
        // Mientras el jugador se acerque al punto de spawn, generamos nuevos objetos
        while (player.position.x + spawnAheadDistance >= nextSpawnX)
        {
            SpawnNext();
            float gap = Random.Range(minGap, maxGap);
            nextSpawnX += gap;
        }
    }

    private void SpawnNext()
    {
        // elegir índice aleatorio distinto al anterior
        if (prefabs.Length == 1)
        {
            // si solo hay 1 prefab (no es tu caso), no se puede evitar repetir
            SpawnAtIndex(0);
            return;
        }

        int idx;
        int attempts = 0;
        do
        {
            idx = Random.Range(0, prefabs.Length);
            attempts++;
            // seguridad para evitar bucle infinito
            if (attempts > 10) break;
        } while (idx == lastPrefabIndex);

        lastPrefabIndex = idx;
        SpawnAtIndex(idx);
    }

    private void SpawnAtIndex(int index)
    {
        Vector3 spawnPos = new Vector3(nextSpawnX, GetSpawnY(), 0f);
        GameObject go = PoolManager.Instance.Get(prefabs[index], spawnPos, Quaternion.identity);

        // si el objeto tiene script SpawnedItem, que conozca al jugador y distance para despawn
        var spawned = go.GetComponent<SpawnedItem>();
        if (spawned == null) spawned = go.AddComponent<SpawnedItem>();
        spawned.player = player;
        spawned.despawnBehindDistance = despawnBehindDistance;
    }

    private float GetSpawnY()
    {
        if (useYRange)
        {
            return Random.Range(yRange.x, yRange.y);
        }
        else
        {
            if (possibleYPositions != null && possibleYPositions.Length > 0)
                return possibleYPositions[Random.Range(0, possibleYPositions.Length)];
            else
                return 0f;
        }
    }

    // utilidad para forzar spawn (opcional)
    public void ForceSpawnNow()
    {
        SpawnNext();
        float gap = Random.Range(minGap, maxGap);
        nextSpawnX += gap;
    }
}