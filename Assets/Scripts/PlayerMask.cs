using UnityEngine;

public enum MaskType { None = 0, Mask1 = 1, Mask2 = 2, Mask3 = 3 }

public class PlayerMask : MonoBehaviour
{
    public MaskType currentMask = MaskType.None;

    // Equipar una mascara
    public void EquipMask(MaskType mask)
    {
        currentMask = mask;
        Debug.Log($"Player equipó {mask}");
    }

    // Quitar mascara
    public void RemoveMask()
    {
        currentMask = MaskType.None;
        Debug.Log("Player quitó la máscara");
    }

    // Comprobar si tiene cierta mascara puesta
    public bool HasMask(MaskType mask)
    {
        return currentMask == mask;
    }
}