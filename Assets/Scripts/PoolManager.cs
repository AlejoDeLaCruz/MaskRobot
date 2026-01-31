using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolDef
    {
        public string name;
        public GameObject prefab;
        public int size = 5;
    }

    public PoolDef[] pools;
    private Dictionary<int, Queue<GameObject>> poolDict = new Dictionary<int, Queue<GameObject>>();

    void Awake()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            var q = new Queue<GameObject>();
            var container = new GameObject(pools[i].name + "_Pool");
            container.transform.SetParent(transform);
            for (int j = 0; j < pools[i].size; j++)
            {
                var obj = Instantiate(pools[i].prefab, container.transform);
                obj.SetActive(false);
                q.Enqueue(obj);
            }
            poolDict[i] = q;
        }
    }

    public GameObject Spawn(int prefabIndex)
    {
        if (!poolDict.ContainsKey(prefabIndex))
        {
            Debug.LogError("Pool no existe index: " + prefabIndex);
            return null;
        }

        var q = poolDict[prefabIndex];
        var obj = q.Dequeue();
        obj.SetActive(true);
        q.Enqueue(obj);
        // Lo dejamos activo y sin parent (o podés parentearlo donde quieras)
        obj.transform.SetParent(null);
        return obj;
    }

    public void Despawn(GameObject go)
    {
        // Desactivar y devolver al pool (lo ponemos como hijo del pool correspondiente si encontramos el prefab)
        go.SetActive(false);
        // Opcional: reparentear a algun container; este ejemplo no lo hace automáticamente
    }
}