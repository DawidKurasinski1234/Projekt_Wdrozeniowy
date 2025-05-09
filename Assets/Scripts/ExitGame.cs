using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Wy³¹cza tryb play w edytorze
#else
        Application.Quit(); // Wy³¹cza zbudowan¹ aplikacjê
#endif
    }
}