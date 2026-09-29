using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int targetScore = 10;
    public float gameTime = 60f;

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

        // Automatically find the player's WeightSystem if not assigned
        if (playerWeightSystem == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerWeightSystem = player.GetComponent<WeightSystem>();
            }
        }

        UpdateUI();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            LoseGame();
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (gameEnded)
            return;

        score += amount;

        Debug.Log("Score: " + score);

        UpdateUI();
    }

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

    private void LoseGame()
    {
        gameEnded = true;

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        Debug.Log("TIME'S UP! YOU LOSE!");
    }

    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (timeText != null)
        {
            timeText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
        }

        if (weightText != null && playerWeightSystem != null)
        {
            float currentWeight = playerWeightSystem.GetCurrentWeight();

            weightText.text = "Weight: " + currentWeight.ToString("F1") + "x";
        }
    }
}