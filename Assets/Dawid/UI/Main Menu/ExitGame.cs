using UnityEngine;
using UnityEngine.UI; // Opcjonalnie, je�li chcesz np. dynamicznie przypisa� przycisk

public class QuitGameHandler : MonoBehaviour
{
    // Publiczna metoda, kt�r� pod��czysz do zdarzenia OnClick() przycisku w Inspektorze
    public void QuitApplication()
    {
        // Log do konsoli, aby upewni� si�, �e metoda zosta�a wywo�ana (przydatne podczas test�w)
        Debug.Log("Otrzymano ��danie wyj�cia z aplikacji.");

        // Wywo�anie funkcji zamykaj�cej aplikacj�
        // Dzia�a na buildach Standalone (Windows, Mac, Linux) oraz na Androidzie.
        // Wa�ne: W edytorze Unity ta funkcja zatrzyma tryb Play, ale nie zamknie samego edytora.
        Application.Quit();

        // Dodatkowa instrukcja dla edytora (opcjonalna, ale dobra praktyka)
        // U�ywamy dyrektyw preprocesora, aby ten kod by� kompilowany tylko w edytorze
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}