using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do ³adowania scen

public class GameInitializer : MonoBehaviour
{
    // To jest nazwa Twojej pierwszej sceny, na któr¹ ma przejœæ gra po zainicjowaniu.
    // Ustaw j¹ w Inspektorze Unity (np. "MainMenuScene").
    public string initialSceneName = "Main Menu"; // ZMIEÑ NA NAZWÊ TWOJEJ PIERWSZEJ SCENY GRY

    void Awake()
    {
        // U¿ywamy FindObjectsByType dla nowszej sk³adni Unity
        if (FindObjectsByType<GameInitializer>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Ten obiekt przetrwa ³adowanie scen

        Passport.Init(); // Zainicjuj statyczn¹ klasê Passport. To wczyta dane z JSON.
        Debug.Log("GameInitializer: Passport.Init() wywo³ane.");

        if (Passport.CurrentCountry == null) // Wylosuj kraj tylko, jeœli jeszcze nie jest wybrany
        {
            Passport.CurrentCountry = Passport.ChooseCountry();
            if (Passport.CurrentCountry != null)
            {
                Debug.Log($"GameInitializer: Wylosowano pocz¹tkowy kraj: {Passport.CurrentCountry}");
            }
            else
            {
                Debug.LogError("GameInitializer: Passport.ChooseCountry() zwróci³o null. SprawdŸ, czy s¹ dostêpne kraje w JSON.");
            }
        }

        // PrzejdŸ do pocz¹tkowej sceny gry (np. menu g³ównego)
        SceneManager.LoadScene(initialSceneName);
    }
}