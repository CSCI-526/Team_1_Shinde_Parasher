using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int targetScore = 8;
    public float gameTime = 40f;

    [Header("Current Game State")]
    public int score = 0;
    public float timeRemaining;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text weightText;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Player")]
    public WeightSystem playerWeightSystem;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timeRemaining = gameTime;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        // Automatically find player's WeightSystem
        if (playerWeightSystem == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerWeightSystem =
                    player.GetComponent<WeightSystem>();
            }
        }

        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        // =====================================================
        // TIMER
        // =====================================================

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            LoseGame();
            return;
        }

        // =====================================================
        // SPACE = DROP ONE POINT
        // =====================================================

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropPoint();
        }

        UpdateUI();
    }

    // =========================================================
    // ADD SCORE
    // =========================================================

    public void AddScore(int amount)
    {
        if (gameEnded)
            return;

        score += amount;

        // Never allow negative score.
        score = Mathf.Max(score, 0);

        Debug.Log("Score: " + score);

        UpdateUI();
    }

    // =========================================================
    // DROP ONE POINT
    // =========================================================

    public void DropPoint()
    {
        if (gameEnded)
            return;

        // Cannot drop if score is already zero.
        if (score <= 0)
        {
            Debug.Log(
                "Cannot drop point. Score is already 0."
            );

            return;
        }

        // Make sure WeightSystem exists.
        if (playerWeightSystem == null)
        {
            Debug.LogWarning(
                "Player WeightSystem not found."
            );

            return;
        }

        // Reduce score by exactly one.
        score--;

        // Reduce weight by one point's weight amount.
        playerWeightSystem.RemoveWeightForPoint();

        // Safety: score can never go below zero.
        score = Mathf.Max(score, 0);

        Debug.Log(
            "Dropped 1 point. Score: " +
            score +
            " | Weight: " +
            playerWeightSystem.GetCurrentWeight()
        );

        UpdateUI();
    }

    // =========================================================
    // WIN
    // =========================================================

    public void WinGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        Debug.Log("YOU WIN!");
    }

    // =========================================================
    // FINISH LINE
    // =========================================================

    public void ReachFinish()
    {
        if (gameEnded)
            return;

        if (score >= targetScore)
        {
            WinGame();
        }
        else
        {
            Debug.Log(
                "Not enough points! Need " +
                targetScore +
                ", currently have " +
                score +
                ". Restarting..."
            );

            RestartGame();
        }
    }

    // =========================================================
    // LOSE
    // =========================================================

    private void LoseGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        Debug.Log("TIME'S UP! YOU LOSE!");
    }

    // =========================================================
    // RESTART
    // =========================================================

    public void RestartGame()
    {
        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }

    // =========================================================
    // UI
    // =========================================================

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + score;
        }

        if (timeText != null)
        {
            timeText.text =
                "Time: " +
                Mathf.CeilToInt(timeRemaining);
        }

        if (weightText != null &&
            playerWeightSystem != null)
        {
            float currentWeight =
                playerWeightSystem.GetCurrentWeight();

            weightText.text =
                "Weight: " +
                currentWeight.ToString("F1") +
                "x";
        }
    }
}