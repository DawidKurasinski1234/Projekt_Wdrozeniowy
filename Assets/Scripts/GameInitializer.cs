using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do �adowania scen

public class GameInitializer : MonoBehaviour
{
    // To jest nazwa Twojej pierwszej sceny, na kt�r� ma przej�� gra po zainicjowaniu.
    // Ustaw j� w Inspektorze Unity (np. "MainMenuScene").
    public string initialSceneName = "Main Menu"; // ZMIE� NA NAZW� TWOJEJ PIERWSZEJ SCENY GRY

    void Awake()
    {
        // U�ywamy FindObjectsByType dla nowszej sk�adni Unity
        if (FindObjectsByType<GameInitializer>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Ten obiekt przetrwa �adowanie scen

        Passport.Init(); // Zainicjuj statyczn� klas� Passport. To wczyta dane z JSON.
        Debug.Log("GameInitializer: Passport.Init() wywo�ane.");

        if (Passport.CurrentCountry == null) // Wylosuj kraj tylko, je�li jeszcze nie jest wybrany
        {
            Passport.CurrentCountry = Passport.ChooseCountry();
            if (Passport.CurrentCountry != null)
            {
                Debug.Log($"GameInitializer: Wylosowano pocz�tkowy kraj: {Passport.CurrentCountry}");
            }
            else
            {
                Debug.LogError("GameInitializer: Passport.ChooseCountry() zwr�ci�o null. Sprawd�, czy s� dost�pne kraje w JSON.");
            }
        }

        // Przejd� do pocz�tkowej sceny gry (np. menu g��wnego)
        SceneManager.LoadScene(initialSceneName);
    }
}