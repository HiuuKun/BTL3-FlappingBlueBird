using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject gameOverMenu;

    [Header("Scoring")]
    public BirdController player;
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        // When the game first opens, show menu and pause
        startMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        Time.timeScale = 0; 
    }

    public void StartGame()
    {
        startMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        Time.timeScale = 1; // Unpause the game
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);

        if (player != null && finalScoreText != null)
        {
            int currentScore = player.experience;
            // Get the saved high score (default to 0 if none exists)
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            // Check if we have a new high score
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

            // Update the text to show: Score | High Score
            finalScoreText.text = $"Score: {currentScore} \n  High Score: {highScore}";
        }

        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SpawnManager.ResetSpawnState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}