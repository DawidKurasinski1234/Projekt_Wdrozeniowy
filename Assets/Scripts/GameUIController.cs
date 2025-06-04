using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public GameObject minigameCanvas; // Przypisz Canvas minigry w Inspektorze
    public GameObject pauseMenuCanvas; // Przypisz Canvas menu pauzy w Inspektorze
    public GameObject Score;
    private bool isPaused = false;

    void Start()
    {
        // Na pocz�tku gry, upewnij si�, �e menu pauzy jest wy��czone, a minigra w��czona
        minigameCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Upewnij si�, �e gra nie jest spauzowana na pocz�tku
    }

    // Funkcja do w��czania/wy��czania menu pauzy
    public void TogglePauseMenu()
    {
        isPaused = !isPaused; // Zmie� stan pauzy

        if (isPaused)
        {
            // Gdy gra jest spauzowana
            pauseMenuCanvas.SetActive(true); // W��cz Canvas menu pauzy
            minigameCanvas.SetActive(false); // Wy��cz Canvas minigry
            Score.SetActive(false);
            Time.timeScale = 0f; // Zatrzymaj czas w grze
        }
        else
        {
            // Gdy gra jest wznowiona
            pauseMenuCanvas.SetActive(false); // Wy��cz Canvas menu pauzy
            minigameCanvas.SetActive(true); // W��cz Canvas minigry
            Score.SetActive(true);
            Time.timeScale = 1f; // Wzn�w czas w grze
        }
    }

    // Funkcja do wywo�ywania z przycisku "Wzn�w" w menu pauzy
    public void ResumeGame()
    {
        TogglePauseMenu(); // Wznowi gr�
    }
}