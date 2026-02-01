using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Configuración de Máscaras")]
    [Tooltip("Número total de máscaras disponibles (sin contar None)")]
    public int totalMasks = 3;
    [Tooltip("Índice de la máscara inicial (0 = Mask1, 1 = Mask2, 2 = Mask3)")]
    public int startMaskIndex = 0;

    [Header("Audio")]
    [Tooltip("Clip de audio que se reproduce al cambiar de máscara")]
    public AudioClip maskChangeSound;
    [Tooltip("Volumen del sonido (0.0 a 1.0)")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private int currentMaskIndex = 0;
    private PlayerMask playerMask;
    private MaskSelector maskSelector;
    private AudioSource audioSource;

    void Start()
    {
        // Obtenemos el componente PlayerMask
        playerMask = GetComponent<PlayerMask>();
        if (playerMask == null)
        {
            Debug.LogError("[PlayerControls] No se encontró el componente PlayerMask en el jugador!");
            return;
        }

        // Obtenemos el MaskSelector de la UI
        maskSelector = FindFirstObjectByType<MaskSelector>();

        // Configuramos el AudioSource
        SetupAudioSource();

        // Validamos que haya al menos una máscara
        if (totalMasks < 1)
        {
            Debug.LogError("[PlayerControls] Debe haber al menos 1 máscara. Ajustando a 1.");
            totalMasks = 1;
        }

        // Aseguramos que el índice inicial esté en rango válido
        currentMaskIndex = Mathf.Clamp(startMaskIndex, 0, totalMasks - 1);

        // Equipamos la máscara inicial (sin sonido)
        EquipCurrentMask(false);
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

    private void SetupAudioSource()
    {
        // Buscamos si ya hay un AudioSource
        audioSource = GetComponent<AudioSource>();

        // Si no existe, lo creamos
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuramos el AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void PlayMaskChangeSound()
    {
        if (audioSource != null && maskChangeSound != null)
        {
            audioSource.PlayOneShot(maskChangeSound, soundVolume);
        }
    }

    public void NextMask()
    {
        currentMaskIndex = (currentMaskIndex + 1) % totalMasks;
        EquipCurrentMask(true);
        NotifyUI();
    }

    public void PreviousMask()
    {
        currentMaskIndex = (currentMaskIndex - 1 + totalMasks) % totalMasks;
        EquipCurrentMask(true);
        NotifyUI();
    }

    private void EquipCurrentMask(bool playSound = true)
    {
        if (playerMask == null) return;

        // Convertimos el índice (0, 1, 2) al MaskType correspondiente (Mask1, Mask2, Mask3)
        MaskType maskToEquip = (MaskType)(currentMaskIndex + 1);
        playerMask.EquipMask(maskToEquip);

        // Reproducimos el sonido si está habilitado
        if (playSound)
        {
            PlayMaskChangeSound();
        }

        Debug.Log($"[PlayerControls] Máscara cambiada a índice {currentMaskIndex} -> {maskToEquip}");
    }

    private void NotifyUI()
    {
        if (maskSelector != null)
        {
            maskSelector.ForceUpdateVisuals();
        }
    }

    // Métodos públicos
    public int GetCurrentMaskIndex()
    {
        return currentMaskIndex;
    }

    public MaskType GetCurrentMaskType()
    {
        return playerMask != null ? playerMask.currentMask : MaskType.None;
    }

    public void SetMask(int index, bool playSound = true)
    {
        if (index >= 0 && index < totalMasks)
        {
            currentMaskIndex = index;
            EquipCurrentMask(playSound);
            NotifyUI();
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