using UnityEngine;

public class WallMask1 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMask pm = other.GetComponent<PlayerMask>();
        if (pm == null)
        {
            Debug.LogWarning("Player no tiene PlayerMask component.");
            // Si quieres, consideramos que sin componente se muere:
            GameManager.Instance?.PlayerDied();
            return;
        }

        if (!pm.HasMask(MaskType.Mask1))
        {
            Debug.Log("WallMask1: jugador sin Mask1 -> muerte");
            GameManager.Instance?.PlayerDied();
        }
        else
        {
            Debug.Log("WallMask1: jugador protegido por Mask1");
        }
    }
}