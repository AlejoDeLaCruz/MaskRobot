using UnityEngine;

public class SegmentData : MonoBehaviour
{
    [Tooltip("Ancho X del segmento (usar para posicionar el siguiente).")]
    public float width = 10f;

    [Tooltip("Offset aplicable al spawn del segmento respecto a la posición base del generador.")]
    public Vector2 spawnOffset = Vector2.zero;
}
