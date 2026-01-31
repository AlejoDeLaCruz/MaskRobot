using System.Collections.Generic;
using UnityEngine;

// Pool simple que guarda instancias por "prefab key" (usamos el prefab como key).
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    // Diccionario: prefab -> stack de instancias
    private Dictionary<GameObject, Stack<GameObject>> pool = new Dictionary<GameObject, Stack<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Obtiene una instancia (si no hay crea una nueva)
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pool.ContainsKey(prefab)) pool[prefab] = new Stack<GameObject>();

        var stack = pool[prefab];
        GameObject go;
        if (stack.Count > 0)
        {
            go = stack.Pop();
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);
        }
        else
        {
            go = Instantiate(prefab, position, rotation);
            // Añadimos componente para saber a qué prefab pertenece (útil para devolverlo)
            var info = go.GetComponent<PooledObject>();
            if (info == null) info = go.AddComponent<PooledObject>();
            info.originalPrefab = prefab;
        }

        return go;
    }

    // Devuelve al pool (desactiva)
    public void Release(GameObject go)
    {
        if (go == null) return;
        var info = go.GetComponent<PooledObject>();
        if (info == null || info.originalPrefab == null)
        {
            Destroy(go); // no sabemos a qué pool va: destruyelo
            return;
        }

        go.SetActive(false);
        // opcional: resetear transform, variables, etc.

        if (!pool.ContainsKey(info.originalPrefab)) pool[info.originalPrefab] = new Stack<GameObject>();
        pool[info.originalPrefab].Push(go);
    }
}