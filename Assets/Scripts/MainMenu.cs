using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Passport.Init();
        Passport.CurrentCountry = Passport.ChooseCountry();
        SceneManager.LoadSceneAsync(1); //wczytywanie sceny losowania kraju
    }

    public void OpenPassportMenu()
    {
        Passport.Init();
        SceneManager.LoadSceneAsync("passport_menu");//wczytywnie sceny paszportu
    }

    public void OpenMinigame1()
    {
        SceneManager.LoadSceneAsync(2); // wczytywanie sceny minigry 1
    }
}