using UnityEngine;

// Este script revisa su posición relativa al player y se devuelve al pool cuando queda muy atrás.
// Lo añadimos dinámicamente desde el spawner si no existe.
public class SpawnedItem : MonoBehaviour
{
    [HideInInspector] public Transform player;
    [HideInInspector] public float despawnBehindDistance = 12f;

    private void Update()
    {
        if (player == null) return;

        if (transform.position.x < player.position.x - despawnBehindDistance)
        {
            PoolManager.Instance.Release(gameObject);
        }
    }

    // Si tu objeto tiene animaciones, particulas, o necesita resetearse, crea un método ResetState() y llamalo antes de Release.
}