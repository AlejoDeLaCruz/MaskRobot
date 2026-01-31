using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;     // Player
    public Vector3 offset;       // Distancia entre cámara y player
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
