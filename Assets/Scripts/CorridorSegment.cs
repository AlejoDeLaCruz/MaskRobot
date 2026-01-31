using UnityEngine;

public class CorridorSegment : MonoBehaviour
{
    [Tooltip("Start point: donde este segmento se considera 'anclado' (normalmente borde izquierdo)")]
    public Transform startPoint;

    [Tooltip("End point: donde se engancha el siguiente segmento (normalmente borde derecho)")]
    public Transform endPoint;
}