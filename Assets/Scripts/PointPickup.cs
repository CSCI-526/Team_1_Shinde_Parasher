using UnityEngine;

public class PointPickup : MonoBehaviour
{
    [SerializeField] private int pointValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Add score through GameManager
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.AddScore(pointValue);
        }

        // Add weight through WeightSystem
        WeightSystem weightSystem = other.GetComponent<WeightSystem>();

        if (weightSystem != null)
        {
            weightSystem.AddWeightForPoint();
        }

        // Remove the pickup
        Destroy(gameObject);
    }
}