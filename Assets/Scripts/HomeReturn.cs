using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeReturn : MonoBehaviour
{
    public void ReturnHome()
    {
        SceneManager.LoadSceneAsync(0); //powr�t do menu g��wnego
    }
}
