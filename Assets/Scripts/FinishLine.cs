using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.score >= GameManager.Instance.targetScore)
        {
            GameManager.Instance.WinGame();
        }
        else
        {
            Debug.Log("You need more points!");
        }
    }
}