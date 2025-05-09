using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Wy��cza tryb play w edytorze
#else
        Application.Quit(); // Wy��cza zbudowan� aplikacj�
#endif
    }
}