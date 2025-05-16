using UnityEngine;
using UnityEngine.SceneManagement;

public class Show_EndGameCanvas : MonoBehaviour
{
    [SerializeField] GameObject EndGame_Menu;
    public void Show_Congrats()
    {
        EndGame_Menu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Return_Home()
    {
        SceneManager.LoadScene("Main Menu");
    }
    
}