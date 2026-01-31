using UnityEngine;

[RequireComponent(typeof(PlayerMask))]
public class PlayerControls : MonoBehaviour
{
    private PlayerMask pm;

    private void Awake()
    {
        pm = GetComponent<PlayerMask>();
        if (pm == null)
        {
            Debug.LogWarning("[PlayerControls] No se encontró PlayerMask en el GameObject.");
        }
    }

    private void Update()
    {
        // Flecha izquierda -> Mask1
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetMask(MaskType.Mask1);
        }

        // Flecha abajo -> Mask2
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetMask(MaskType.Mask2);
        }

        // Flecha derecha -> Mask3
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetMask(MaskType.Mask3);
        }

        // (Opcional) Flecha arriba -> quitar máscara
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RemoveMask();
        }
    }

    private void SetMask(MaskType mask)
    {
        if (pm == null) return;

        if (pm.currentMask == mask)
        {
            Debug.Log($"[PlayerControls] Ya tenés la {mask}");
            return;
        }

        pm.EquipMask(mask);
        Debug.Log($"[PlayerControls] Mascara cambiada a {mask}");
    }

    private void RemoveMask()
    {
        if (pm == null) return;

        pm.RemoveMask();
        Debug.Log("[PlayerControls] Máscara quitada (None)");
    }
}