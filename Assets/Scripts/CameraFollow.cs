using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = transform.position;

        // Follow the car horizontally and vertically
        targetPosition.x = target.position.x;
        targetPosition.y = target.position.y;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}