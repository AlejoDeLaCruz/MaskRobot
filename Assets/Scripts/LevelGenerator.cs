using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; private set; }

    [Header("Prefabs de segmentos (0,1,2...)")]
    public GameObject[] segmentPrefabs; // asignar prefabs en el inspector

    [Header("Configuración")]
    public Transform spawnParent;         // padre para los segmentos
    public int initialSegments = 3;       // cuántos spawnear al inicio
    public int maxActiveSegments = 6;     // cuando pase esto, reciclamos los más viejos
    public bool usePooling = true;

    // Estado interno
    private Queue<GameObject> activeQueue = new Queue<GameObject>();
    private Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();
    private float nextSpawnX = 0f; // posición X donde vamos a spawnear el próximo segmento

    // Guardamos el último índice generado para evitar repetirlo
    private int lastSpawnedIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (spawnParent == null) spawnParent = this.transform;

        // inicializar pools
        for (int i = 0; i < segmentPrefabs.Length; i++)
        {
            pools[i] = new Queue<GameObject>();
        }
    }

    private void Start()
    {
        if (segmentPrefabs == null || segmentPrefabs.Length == 0)
        {
            Debug.LogError("LevelGenerator: 'segmentPrefabs' está vacío. Asigná tus prefabs en el Inspector antes de ejecutar.");
            return;
        }

        // Spawn inicial sin repetir consecutivamente
        for (int i = 0; i < initialSegments; i++)
        {
            int idx = GetRandomIndexExcludingLast();
            SpawnSegment(idx);
        }
    }

    // Llamar desde SegmentEndTrigger
    public void RequestSpawnNext()
    {
        int idx = GetRandomIndexExcludingLast();
        SpawnSegment(idx);
    }

    // Devuelve un índice aleatorio entre 0..n-1 excluyendo lastSpawnedIndex (si es posible)
    private int GetRandomIndexExcludingLast()
    {
        int n = segmentPrefabs.Length;
        if (n == 0) return -1;
        if (n == 1) return 0; // sólo uno disponible
        if (lastSpawnedIndex < 0) return Random.Range(0, n); // primera vez

        // mapeo sin bucles: elegimos en rango [0, n-2] y lo "ajustamos" si pasa al lado del excluido
        int r = Random.Range(0, n - 1);
        return r >= lastSpawnedIndex ? r + 1 : r;
    }

    private void SpawnSegment(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= segmentPrefabs.Length)
        {
            Debug.LogWarning("SpawnSegment: índice fuera de rango.");
            return;
        }

        GameObject prefab = segmentPrefabs[prefabIndex];
        if (prefab == null)
        {
            Debug.LogWarning("SpawnSegment: prefab nulo en index " + prefabIndex);
            return;
        }

        // Obtener SegmentData del prefab para conocer width/offset
        SegmentData data = prefab.GetComponent<SegmentData>();
        float width = (data != null) ? data.width : EstimateWidth(prefab);
        Vector2 offset = (data != null) ? data.spawnOffset : Vector2.zero;

        // Obtener objeto (de pool o instanciar)
        GameObject go = null;
        if (usePooling && pools.ContainsKey(prefabIndex) && pools[prefabIndex].Count > 0)
        {
            go = pools[prefabIndex].Dequeue();
            go.SetActive(true);
        }

        if (go == null)
        {
            // Instanciamos directamente en la posición correcta y asignamos el parent
            Vector3 worldPos = spawnParent.TransformPoint(new Vector3(nextSpawnX + offset.x, offset.y, 0f));
            go = Instantiate(prefab, worldPos, Quaternion.identity, spawnParent);
            var inst = go.GetComponent<SegmentInstance>();
            if (inst == null) inst = go.AddComponent<SegmentInstance>();
            inst.prefabIndex = prefabIndex;
        }
        else
        {
            var inst = go.GetComponent<SegmentInstance>();
            if (inst == null) inst = go.AddComponent<SegmentInstance>();
            inst.prefabIndex = prefabIndex;

            // Si lo queremos posicionar RELATIVO al padre (localPosition)
            go.transform.SetParent(spawnParent, false); // false = mantener local transform cuando reasignamos parent
            go.transform.localPosition = new Vector3(nextSpawnX + offset.x, offset.y, 0f);
        }

        // resetear el EndTrigger para que pueda disparar nuevamente (importante para pooling)
        var endTrigger = go.GetComponentInChildren<SegmentEndTrigger>(true);
        if (endTrigger != null) endTrigger.ResetTrigger();

        // encolamos como activo
        activeQueue.Enqueue(go);

        // actualizar lastSpawnedIndex (importante: lo guardamos justo después del spawn)
        lastSpawnedIndex = prefabIndex;

        // avanzar el nextSpawnX por el ancho del segmento
        nextSpawnX += width;

        // si hay muchos segmentos en escena, reciclamos el más viejo
        if (activeQueue.Count > maxActiveSegments)
        {
            GameObject old = activeQueue.Dequeue();
            RecycleOrDestroy(old);
        }
    }

    private float EstimateWidth(GameObject go)
    {
        // Intento estimar ancho por algún renderer o collider
        SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) return sr.bounds.size.x;

        Collider2D c = go.GetComponentInChildren<Collider2D>();
        if (c != null) return c.bounds.size.x;

        // fallback
        return 10f;
    }

    private void RecycleOrDestroy(GameObject go)
    {
        if (go == null) return;

        var instance = go.GetComponent<SegmentInstance>();
        if (usePooling && instance != null && pools.ContainsKey(instance.prefabIndex))
        {
            // desactivar y poner en pool
            go.SetActive(false);
            pools[instance.prefabIndex].Enqueue(go);
        }
        else
        {
            Destroy(go);
        }
    }
}