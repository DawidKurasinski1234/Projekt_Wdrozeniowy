using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do ³adowania scen

public class GameInitializer : MonoBehaviour
{
    // To jest nazwa Twojej pierwszej sceny, na któr¹ ma przejœæ gra po zainicjowaniu.
    // Ustaw j¹ w Inspektorze Unity (np. "MainMenuScene").
    public string initialSceneName = "MainMenuScene"; // ZMIEÑ NA NAZWÊ TWOJEJ PIERWSZEJ SCENY GRY

    void Awake()
    {
        // SprawdŸ, czy GameInitializer ju¿ istnieje, aby zapobiec duplikacji
        // Dzieje siê tak, jeœli ten obiekt mia³by przetrwaæ sceny (DontDestroyOnLoad)
        // W tym przypadku lepiej, ¿eby by³ tylko jeden.
        if (FindObjectsOfType<GameInitializer>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Upewnij siê, ¿e ten obiekt przetrwa ³adowanie scen,
        // aby Passport zosta³ zainicjowany tylko raz.
        DontDestroyOnLoad(gameObject);

        // Zainicjuj statyczn¹ klasê Passport.
        // To wczyta dane z JSON.
        Passport.Init();
        Debug.Log("GameInitializer: Passport.Init() wywo³ane.");

        // Wylosuj pierwszy kraj TERAZ.
        // To ustawia Passport.CurrentCountry dla ca³ej sesji gry.
        if (Passport.CurrentCountry == null) // SprawdŸ, czy kraj jeszcze nie jest wybrany
        {
            Passport.CurrentCountry = Passport.ChooseCountry();
            if (Passport.CurrentCountry != null)
            {
                Debug.Log($"GameInitializer: Wylosowano pocz¹tkowy kraj: {Passport.CurrentCountry}");
            }
            else
            {
                Debug.LogError("GameInitializer: Passport.ChooseCountry() zwróci³o null. SprawdŸ, czy s¹ dostêpne kraje.");
            }
        }

        // PrzejdŸ do pocz¹tkowej sceny gry (np. menu g³ównego).
        // Robimy to w Awake(), aby scena g³ówna mia³a ju¿ zainicjowane dane.
        SceneManager.LoadScene(initialSceneName);
    }
}