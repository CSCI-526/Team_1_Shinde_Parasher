using UnityEngine;

public class PointPickup : MonoBehaviour
{
    public int pointValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointValue);
        }

        Destroy(gameObject);
    }
}