using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject gameOverMenu;

    void Start()
    {
        startMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        Time.timeScale = 0; 
    }

    public void StartGame()
    {
        startMenu.SetActive(false);
        Time.timeScale = 1; 
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
        Time.timeScale = 0;  
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}