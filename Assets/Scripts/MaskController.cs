using UnityEngine;

public class MaskController : MonoBehaviour
{
    [Tooltip("Referencia al script que maneja máscaras (debe tener GetCurrentMaskIndex)")]
    public MonoBehaviour playerControlsBehaviour; // usar MonoBehaviour para permitir FindFirstObjectByType o arrastrar el componente
    private PlayerControls playerControls;

    [Tooltip("Animator del jugador (si está en otro GameObject, arrastralo aquí)")]
    public Animator animator;

    [Tooltip("Nombres de los estados en el Animator (orden: máscara 0, 1, 2...)")]
    public string[] stateNames = new string[] { "Mask1", "Mask2", "Mask3" };

    int lastIndex = -1;

    void Start()
    {
        // Intentamos resolver PlayerControls (si no lo arrastraste)
        if (playerControlsBehaviour != null)
            playerControls = playerControlsBehaviour as PlayerControls;

        if (playerControls == null)
            playerControls = FindObjectOfType<PlayerControls>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Aplicar animación inicial
        if (playerControls != null)
            ForceUpdateAnimation(playerControls.GetCurrentMaskIndex());
    }

    void Update()
    {
        if (playerControls == null || animator == null) return;

        int idx = playerControls.GetCurrentMaskIndex();
        if (idx != lastIndex)
        {
            ForceUpdateAnimation(idx);
        }
    }

    private void ForceUpdateAnimation(int idx)
    {
        idx = Mathf.Clamp(idx, 0, stateNames.Length - 1);
        string state = stateNames[idx];

        // Reproduce el estado del animator directamente
        animator.Play(state);

        lastIndex = idx;
    }
}