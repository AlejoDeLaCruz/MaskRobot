using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;     // Player
    public float offsetX = 2f;   // Qué tan adelante va la cámara
    public float smoothSpeed = 5f;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z; // normalmente -10
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offsetX,
            fixedY,
            fixedZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}