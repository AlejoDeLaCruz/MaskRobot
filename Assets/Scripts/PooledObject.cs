using UnityEngine;

// Marca que la instancia pertenece a un prefab original
public class PooledObject : MonoBehaviour
{
    [HideInInspector] public GameObject originalPrefab;
}