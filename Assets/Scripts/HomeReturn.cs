using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeReturn : MonoBehaviour
{
    public void ReturnHome()
    {
        SceneManager.LoadSceneAsync(1); //powrót do menu g³ównego
    }
}
