using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1); //wczytywanie sceny losowania kraju
    }

    public void OpenPassportMenu()
    {
        SceneManager.LoadSceneAsync("passport_menu");//wczytywnie sceny paszportu
    }

    public void OpenMinigame1()
    {
        SceneManager.LoadSceneAsync(2); // wczytywanie sceny minigry 1
    }
}