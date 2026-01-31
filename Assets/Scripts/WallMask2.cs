using UnityEngine;

public class WallMask2 : MonoBehaviour
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

        if (!pm.HasMask(MaskType.Mask2))
        {
            Debug.Log("WallMask2: jugador sin Mask2 -> muerte");
            GameManager.Instance?.PlayerDied();
        }
        else
        {
            Debug.Log("WallMask2: jugador protegido por Mask2");
        }
    }
}
