using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do �adowania scen

public class GameInitializer : MonoBehaviour
{
    // To jest nazwa Twojej pierwszej sceny, na kt�r� ma przej�� gra po zainicjowaniu.
    // Ustaw j� w Inspektorze Unity (np. "MainMenuScene").
    public string initialSceneName = "MainMenuScene"; // ZMIE� NA NAZW� TWOJEJ PIERWSZEJ SCENY GRY

    void Awake()
    {
        // Sprawd�, czy GameInitializer ju� istnieje, aby zapobiec duplikacji
        // Dzieje si� tak, je�li ten obiekt mia�by przetrwa� sceny (DontDestroyOnLoad)
        // W tym przypadku lepiej, �eby by� tylko jeden.
        if (FindObjectsOfType<GameInitializer>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Upewnij si�, �e ten obiekt przetrwa �adowanie scen,
        // aby Passport zosta� zainicjowany tylko raz.
        DontDestroyOnLoad(gameObject);

        // Zainicjuj statyczn� klas� Passport.
        // To wczyta dane z JSON.
        Passport.Init();
        Debug.Log("GameInitializer: Passport.Init() wywo�ane.");

        // Wylosuj pierwszy kraj TERAZ.
        // To ustawia Passport.CurrentCountry dla ca�ej sesji gry.
        if (Passport.CurrentCountry == null) // Sprawd�, czy kraj jeszcze nie jest wybrany
        {
            Passport.CurrentCountry = Passport.ChooseCountry();
            if (Passport.CurrentCountry != null)
            {
                Debug.Log($"GameInitializer: Wylosowano pocz�tkowy kraj: {Passport.CurrentCountry}");
            }
            else
            {
                Debug.LogError("GameInitializer: Passport.ChooseCountry() zwr�ci�o null. Sprawd�, czy s� dost�pne kraje.");
            }
        }

        // Przejd� do pocz�tkowej sceny gry (np. menu g��wnego).
        // Robimy to w Awake(), aby scena g��wna mia�a ju� zainicjowane dane.
        SceneManager.LoadScene(initialSceneName);
    }
}