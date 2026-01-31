using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs (3 tipos)")]
    public GameObject[] segmentPrefabs; // assign 3 prefabs (SegmentA, SegmentB, SegmentC)

    [Header("Pool")]
    public PoolManager poolManager; // referenciá el PoolManager creado

    [Header("Player")]
    public Transform player; // asignar o buscar por tag "Player"

    [Header("Spawning")]
    public int initialSegments = 6;
    public float spawnDistanceAhead = 30f;
    public float despawnDistanceBehind = 20f;

    [Header("Random")]
    [Range(0f, 1f)]
    public float randomnessBias = 0.5f; // (no usado en ejemplo simple, queda para tuneo)
    public int seed = 0; // 0 = random seed por tiempo

    // Internals
    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private Vector3 nextSegmentPosition = Vector3.zero; // donde queremos alinear el siguiente segmento (world)
    private System.Random rng;
    private int lastSegmentIndex = -1; // NUEVO: rastrear el último índice usado

    void Start()
    {
        if (player == null)
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo) player = pgo.transform;
        }

        rng = (seed != 0) ? new System.Random(seed) : new System.Random();

        // Inicializamos nextSegmentPosition donde queramos (ej: 0,0)
        nextSegmentPosition = Vector3.zero;

        // Spawn inicial
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnRandomSegment();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Asegurarnos de generar suficientes segmentos por delante del jugador
        while (nextSegmentPosition.x < player.position.x + spawnDistanceAhead)
        {
            SpawnRandomSegment();
        }

        // Eliminar (o despawnear) segmentos que queden muy atrás
        while (activeSegments.Count > 0)
        {
            var first = activeSegments.Peek();
            if (first.transform.position.x < player.position.x - despawnDistanceBehind)
            {
                var go = activeSegments.Dequeue();
                // Si usás pool, desactivalo y reparentalo al pool container
                if (poolManager != null)
                {
                    poolManager.Despawn(go);
                }
                else
                {
                    Destroy(go);
                }
            }
            else break;
        }
    }

    void SpawnRandomSegment()
    {
        if (segmentPrefabs == null || segmentPrefabs.Length == 0) return;

        int idx;
        int maxSegments = Mathf.Min(segmentPrefabs.Length, 3);

        // MODIFICADO: Evitar repetir el mismo segmento
        if (maxSegments > 1 && lastSegmentIndex != -1)
        {
            // Generar un índice diferente al anterior
            do
            {
                idx = rng.Next(0, maxSegments);
            } while (idx == lastSegmentIndex);
        }
        else
        {
            // Primera vez o solo hay un tipo de segmento
            idx = rng.Next(0, maxSegments);
        }

        lastSegmentIndex = idx; // NUEVO: guardar el índice actual

        GameObject segGO;
        // Usar pool si existe y tiene ese prefab
        if (poolManager != null)
        {
            segGO = poolManager.Spawn(idx);
            if (segGO == null)
            {
                // fallback a Instantiate
                segGO = Instantiate(segmentPrefabs[idx]);
            }
        }
        else
        {
            segGO = Instantiate(segmentPrefabs[idx]);
        }

        // Alinear el segmento para que su StartPoint coincida con nextSegmentPosition
        var segComp = segGO.GetComponent<CorridorSegment>();
        if (segComp == null || segComp.startPoint == null || segComp.endPoint == null)
        {
            Debug.LogWarning("Prefab necesita CorridorSegment con startPoint y endPoint asignados.");
            // fallback: posicionamos el objeto en nextSegmentPosition
            segGO.transform.position = nextSegmentPosition;
            activeSegments.Enqueue(segGO);
            // intentar actualizar nextSegmentPosition con extremo aproximado
            nextSegmentPosition += new Vector3(10f, 0f, 0f);
            return;
        }

        // Método robusto: instanciamos/activamos en algún lugar, medimos posición actual del startPoint,
        // y movemos el objeto en consecuencia.
        // (funciona tanto para objetos instanciados como para objetos activados desde pool)
        // asegurarse la rotación sea identity (puedes adaptarlo si rotás)
        segGO.transform.rotation = Quaternion.identity;

        // 1) calcular startPoint world actual
        Vector3 currentStartWorld = segComp.startPoint.position;

        // 2) mover todo el segmento por la diferencia
        Vector3 delta = nextSegmentPosition - currentStartWorld;
        segGO.transform.position += delta;

        // 3) encolar como activo
        activeSegments.Enqueue(segGO);

        // 4) calcular nuevo nextSegmentPosition como el world position del endPoint del segmento recién colocado
        Vector3 newEndWorld = segComp.endPoint.position;
        nextSegmentPosition = newEndWorld;
    }
}