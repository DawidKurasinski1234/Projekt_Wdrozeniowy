using UnityEngine;
using UnityEngine.SceneManagement;
public class Minigame1Start : MonoBehaviour
{
        public void RunMinigame1()
    {
        SceneManager.LoadSceneAsync(3); //wczytanie minigry 1
    }
        
  }
