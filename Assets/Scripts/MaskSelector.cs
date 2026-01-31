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

    [Header("Atenuado para NO seleccionado")]
    [Range(0f, 1f)]
    public float deselectedAlpha = 0.6f;     // Alpha que tendrán los no seleccionados (0 = completamente transparente, 1 = opaco)
    [Range(0f, 1f)]
    public float desaturateAmount = 0.5f;    // Cuánto se mezcla con gris (0 = color original, 1 = totalmente gris)

    private Image[] squares;
    private Color[] originalColors;
    private PlayerControls playerControls;

    void Start()
    {
        // Guardamos los cuadrados en orden lógico: 0=azul, 1=verde, 2=rojo
        squares = new Image[] { centerSquare, rightSquare, leftSquare };

        // Guardamos los colores originales de cada sprite (incluye alpha original)
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
        if (playerControls != null)
        {
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (playerControls == null) return;

        int currentIndex = playerControls.GetCurrentMaskIndex();

        for (int i = 0; i < squares.Length; i++)
        {
            if (squares[i] == null) continue;

            // Determinamos el color objetivo
            Color targetColor = originalColors[i];

            if (i == currentIndex)
            {
                // Seleccionado: restauramos color y alpha original (puedes forzar alpha a 1 si querés)
                targetColor = originalColors[i];
                targetColor.a = originalColors[i].a; // mantiene el alpha original
                squares[i].transform.localScale = Vector3.one * selectedScale;
            }
            else
            {
                // No seleccionado: lo atenuamos -> mezclamos con gris y reducimos alpha
                Color grayVersion = Color.Lerp(originalColors[i], Color.gray, desaturateAmount);
                // Aplicamos alpha deseado multiplicando por el alpha original para respetar transparencia base
                grayVersion.a = originalColors[i].a * deselectedAlpha;
                targetColor = grayVersion;

                squares[i].transform.localScale = Vector3.one * deselectedScale;
            }

            // Asignamos el color calculado
            squares[i].color = targetColor;
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
