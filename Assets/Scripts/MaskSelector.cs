using UnityEngine;
using UnityEngine.UI;

public class MaskSelector : MonoBehaviour
{
    [Header("Referencias a los cuadrados")]
    public Image centerSquare;  // Cuadrado AZUL (índice 0 - Mask1)
    public Image rightSquare;   // Cuadrado VERDE (índice 1 - Mask2)  
    public Image leftSquare;    // Cuadrado ROJO (índice 2 - Mask3)

    [Header("Referencias a las flechas")]
    public GameObject leftArrow;  // Flecha que apunta a la izquierda
    public GameObject rightArrow; // Flecha que apunta a la derecha

    [Header("Configuración visual")]
    public float selectedScale = 1.3f;    // Escala cuando está seleccionado (más grande)
    public float deselectedScale = 0.8f;  // Escala cuando NO está seleccionado (más chico)

    private Image[] squares;
    private Color[] originalColors;
    private PlayerControls playerControls;

    void Start()
    {
        // Guardamos los cuadrados en orden lógico: 0=azul, 1=verde, 2=rojo
        squares = new Image[] { centerSquare, rightSquare, leftSquare };

        // Guardamos los colores originales de cada sprite
        originalColors = new Color[squares.Length];
        for (int i = 0; i < squares.Length; i++)
        {
            if (squares[i] != null)
            {
                originalColors[i] = squares[i].color;
            }
        }

        // Obtenemos referencia al PlayerControls del jugador
        playerControls = FindFirstObjectByType<PlayerControls>();
        if (playerControls == null)
        {
            Debug.LogWarning("[MaskSelector] No se encontró PlayerControls en la escena");
        }

        // Mostramos la selección inicial
        UpdateVisuals();
    }

    void Update()
    {
        // REMOVIDO: Ya no detectamos input aquí, lo hace PlayerControls
        // Solo actualizamos los visuales basándonos en el estado del PlayerControls
        if (playerControls != null)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (playerControls == null) return;

        int currentIndex = playerControls.GetCurrentMaskIndex();

        // Actualizamos SOLO la escala de todos los cuadrados
        for (int i = 0; i < squares.Length; i++)
        {
            if (squares[i] == null) continue;

            // Restauramos el color original del sprite
            squares[i].color = originalColors[i];

            // Cambiamos solo la escala
            if (i == currentIndex)
            {
                // Este es el seleccionado - más grande
                squares[i].transform.localScale = Vector3.one * selectedScale;
            }
            else
            {
                // No está seleccionado - más chico
                squares[i].transform.localScale = Vector3.one * deselectedScale;
            }
        }

        UpdateArrows();
    }

    private void UpdateArrows()
    {
        // Siempre mostramos ambas flechas (porque es circular)
        if (leftArrow != null) leftArrow.SetActive(true);
        if (rightArrow != null) rightArrow.SetActive(true);
    }

    // Método público para forzar actualización visual
    public void ForceUpdateVisuals()
    {
        UpdateVisuals();
    }
}