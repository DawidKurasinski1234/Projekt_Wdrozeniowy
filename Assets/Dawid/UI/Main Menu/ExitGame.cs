using UnityEngine;
using UnityEngine.UI; // Opcjonalnie, jeœli chcesz np. dynamicznie przypisaæ przycisk

public class QuitGameHandler : MonoBehaviour
{
    // Publiczna metoda, któr¹ pod³¹czysz do zdarzenia OnClick() przycisku w Inspektorze
    public void QuitApplication()
    {
        // Log do konsoli, aby upewniæ siê, ¿e metoda zosta³a wywo³ana (przydatne podczas testów)
        Debug.Log("Otrzymano ¿¹danie wyjœcia z aplikacji.");

        // Wywo³anie funkcji zamykaj¹cej aplikacjê
        // Dzia³a na buildach Standalone (Windows, Mac, Linux) oraz na Androidzie.
        // Wa¿ne: W edytorze Unity ta funkcja zatrzyma tryb Play, ale nie zamknie samego edytora.
        Application.Quit();

        // Dodatkowa instrukcja dla edytora (opcjonalna, ale dobra praktyka)
        // U¿ywamy dyrektyw preprocesora, aby ten kod by³ kompilowany tylko w edytorze
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}