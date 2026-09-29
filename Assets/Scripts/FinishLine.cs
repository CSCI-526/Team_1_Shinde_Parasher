using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        gameManager.ReachFinish();
    }
}