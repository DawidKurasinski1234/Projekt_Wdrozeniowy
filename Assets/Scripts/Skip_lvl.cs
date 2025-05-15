using UnityEngine;
using UnityEngine.SceneManagement;

public class Skip_lvl : MonoBehaviour
{
    public void Skip_button()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Sprawdzenie, czy istnieje kolejny poziom w Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            print("To jest ostatni poziom. Gratulacje!");
            // Mo¿esz tutaj dodaæ kod do wczytania menu g³ównego lub innego ekranu
        }
    }
    
}
