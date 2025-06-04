using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public GameObject minigameCanvas; // Przypisz Canvas minigry w Inspektorze
    public GameObject pauseMenuCanvas; // Przypisz Canvas menu pauzy w Inspektorze
    public GameObject Score;
    private bool isPaused = false;

    void Start()
    {
        // Na pocz¹tku gry, upewnij siê, ¿e menu pauzy jest wy³¹czone, a minigra w³¹czona
        minigameCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f; // Upewnij siê, ¿e gra nie jest spauzowana na pocz¹tku
    }

    // Funkcja do w³¹czania/wy³¹czania menu pauzy
    public void TogglePauseMenu()
    {
        isPaused = !isPaused; // Zmieñ stan pauzy

        if (isPaused)
        {
            // Gdy gra jest spauzowana
            pauseMenuCanvas.SetActive(true); // W³¹cz Canvas menu pauzy
            minigameCanvas.SetActive(false); // Wy³¹cz Canvas minigry
            Score.SetActive(false);
            Time.timeScale = 0f; // Zatrzymaj czas w grze
        }
        else
        {
            // Gdy gra jest wznowiona
            pauseMenuCanvas.SetActive(false); // Wy³¹cz Canvas menu pauzy
            minigameCanvas.SetActive(true); // W³¹cz Canvas minigry
            Score.SetActive(true);
            Time.timeScale = 1f; // Wznów czas w grze
        }
    }

    // Funkcja do wywo³ywania z przycisku "Wznów" w menu pauzy
    public void ResumeGame()
    {
        TogglePauseMenu(); // Wznowi grê
    }
}