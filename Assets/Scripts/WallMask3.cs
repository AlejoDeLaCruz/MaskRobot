using UnityEngine;

public class WallMask3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMask pm = other.GetComponent<PlayerMask>();
        if (pm == null)
        {
            Debug.LogWarning("Player no tiene PlayerMask component.");
            GameManager.Instance?.PlayerDied();
            return;
        }

        if (!pm.HasMask(MaskType.Mask3))
        {
            Debug.Log("WallMask3: jugador sin Mask3 -> muerte");
            GameManager.Instance?.PlayerDied();
        }
        else
        {
            Debug.Log("WallMask3: jugador protegido por Mask3");
        }
    }
}
