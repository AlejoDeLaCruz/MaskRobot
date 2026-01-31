using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Configuración de Máscaras")]
    [Tooltip("Número total de máscaras disponibles (sin contar None)")]
    public int totalMasks = 3;
    [Tooltip("Índice de la máscara inicial (0 = Mask1, 1 = Mask2, 2 = Mask3)")]
    public int startMaskIndex = 0;

    private int currentMaskIndex = 0;
    private PlayerMask playerMask;
    private MaskSelector maskSelector; // NUEVO

    void Start()
    {
        // Obtenemos el componente PlayerMask
        playerMask = GetComponent<PlayerMask>();
        if (playerMask == null)
        {
            Debug.LogError("[PlayerControls] No se encontró el componente PlayerMask en el jugador!");
            return;
        }

        // Obtenemos el MaskSelector de la UI - NUEVO
        maskSelector = FindFirstObjectByType<MaskSelector>();

        // Validamos que haya al menos una máscara
        if (totalMasks < 1)
        {
            Debug.LogError("[PlayerControls] Debe haber al menos 1 máscara. Ajustando a 1.");
            totalMasks = 1;
        }

        // Aseguramos que el índice inicial esté en rango válido
        currentMaskIndex = Mathf.Clamp(startMaskIndex, 0, totalMasks - 1);

        // Equipamos la máscara inicial
        EquipCurrentMask();
    }

    void Update()
    {
        // Flecha izquierda -> máscara anterior (con wrap-around)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousMask();
        }
        // Flecha derecha -> máscara siguiente (con wrap-around)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextMask();
        }
    }

    public void NextMask()
    {
        currentMaskIndex = (currentMaskIndex + 1) % totalMasks;
        EquipCurrentMask();
        NotifyUI(); // NUEVO
    }

    public void PreviousMask()
    {
        currentMaskIndex = (currentMaskIndex - 1 + totalMasks) % totalMasks;
        EquipCurrentMask();
        NotifyUI(); // NUEVO
    }

    private void EquipCurrentMask()
    {
        if (playerMask == null) return;

        // Convertimos el índice (0, 1, 2) al MaskType correspondiente (Mask1, Mask2, Mask3)
        MaskType maskToEquip = (MaskType)(currentMaskIndex + 1);
        playerMask.EquipMask(maskToEquip);

        Debug.Log($"[PlayerControls] Máscara cambiada a índice {currentMaskIndex} -> {maskToEquip}");
    }

    // NUEVO: Notificar al UI cuando cambia la máscara
    private void NotifyUI()
    {
        if (maskSelector != null)
        {
            maskSelector.ForceUpdateVisuals();
        }
    }

    // Métodos públicos para que otros scripts puedan consultar/modificar la máscara
    public int GetCurrentMaskIndex()
    {
        return currentMaskIndex;
    }

    public MaskType GetCurrentMaskType()
    {
        return playerMask != null ? playerMask.currentMask : MaskType.None;
    }

    public void SetMask(int index)
    {
        if (index >= 0 && index < totalMasks)
        {
            currentMaskIndex = index;
            EquipCurrentMask();
            NotifyUI(); // NUEVO
        }
        else
        {
            Debug.LogWarning($"[PlayerControls] Índice de máscara inválido: {index}");
        }
    }

    public int GetTotalMasks()
    {
        return totalMasks;
    }
}