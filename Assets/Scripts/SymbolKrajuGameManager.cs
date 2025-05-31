using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SymbolKrajuGameManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public SymbolButton[] symbolButtons;

    // Ścieżka w folderze Resources do katalogu z symbolami krajów
    public string symbolsResourcePath = "Resources/Kraje - Symbole/"; // POPRAWIONA ŚCIEŻKA

    private Sprite correctSymbol;
    private string currentCountryName; // Nazwa kraju z Passport.CountryInfo
    private CountryInfo currentCountryInfo; // Pełne informacje o bieżącym kraju

    private int attempts;
    private int maxAttempts = 5;
    private int maxPointsPerQuestion = 5;
    private int currentPointsForThisQuestion;

    private List<Sprite> allUniqueSymbolsInGame;

    void Start()
    {
        Passport.Init(); // Inicjalizuj system Paszportu

        // ROZWIĄZANY KONFLIKT 1
        if (Passport.GetEntriesCount() == 0)
        {
            Debug.LogError("Dane krajów w Paszporcie nie zostały załadowane! Sprawdź Passport.Init() i plik JSON 'countries'.");
            feedbackText.text = "Błąd: Brak danych o krajach.";
            questionText.text = "";
            if (scoreText != null) scoreText.text = "Punkty: 0";
            foreach (SymbolButton button in symbolButtons) button.gameObject.SetActive(false);
            return;
        }

        // Zbierz wszystkie unikalne symbole z danych Paszportu
        allUniqueSymbolsInGame = new List<Sprite>();
        // Potrzebujemy dostępu do wszystkich CountryInfo z Passport.m_entries
        // Jeśli m_entries jest prywatne, Passport musi udostępnić metodę do pobrania wszystkich CountryInfo
        // Na razie zakładam, że możemy iterować po `Passport.GetAllCountryInfos()` (musiałbyś dodać taką metodę do Passport)
        // LUB jeśli Passport.m_entries jest `internal` lub `public` (co nie jest idealne dla `m_entries`):
        /*
        foreach (var entry in Passport.m_entries) // To wymaga, aby m_entries było dostępne
        {
            Sprite symbol = LoadSpriteByResourceName(entry.country.symbolSpriteName); // Zakładamy, że CountryInfo ma pole symbolSpriteName
            if (symbol != null && !allUniqueSymbolsInGame.Contains(symbol))
            {
                allUniqueSymbolsInGame.Add(symbol);
            }
        }
        */
        // Bezpieczniejsza alternatywa: Passport powinien mieć metodę zwracającą wszystkie nazwy zasobów symboli
        // Na potrzeby przykładu, jeśli nie możesz zmienić Passport, a `countrySymbolDatabase` było używane wcześniej
        // do trzymania Sprite'ów, możesz tymczasowo załadować wszystkie Sprite'y z podanej ścieżki
        // To jednak mniej eleganckie niż pobieranie ich na podstawie danych z Passport.
        // Poniżej uproszczone ładowanie WSZYSTKICH sprite'ów z folderu - jeśli nie masz innej metody:
        var loadedSprites = Resources.LoadAll<Sprite>(symbolsResourcePath.TrimEnd('/'));
        allUniqueSymbolsInGame = loadedSprites.Distinct().ToList();


        if (allUniqueSymbolsInGame.Count == 0)
        {
            Debug.LogError($"Nie załadowano żadnych symboli z Resources/{symbolsResourcePath}. Sprawdź ścieżkę i czy obrazki są typu Sprite.");
        }
        else if (allUniqueSymbolsInGame.Count < symbolButtons.Length)
        {
            Debug.LogWarning("Uwaga: Liczba unikalnych symboli w grze jest mniejsza niż liczba przycisków opcji.");
        }

        UpdateScoreDisplay(); // Wyświetl początkowe punkty z Paszportu
        LoadNextSymbolQuestion();
    }

    // Helper do ładowania Sprite na podstawie nazwy (zakładając, że CountryInfo ma pole symbolSpriteName)
    Sprite LoadSpriteByResourceName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        // Pełna ścieżka np. "Games_Data/Kraje - Symbole/flamenco-Hiszpania"
        return Resources.Load<Sprite>(symbolsResourcePath + spriteName);
    }


    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Punkty: {Passport.Points}"; // Użyj Passport.Points
        }
    }

    public void LoadNextSymbolQuestion()
    {
        string chosenCountryNameFromPassport = Passport.ChooseCountry(); // Wybierz kraj używając Paszportu

        if (string.IsNullOrEmpty(chosenCountryNameFromPassport))
        {
            feedbackText.text = "Gratulacje! Odwiedziłeś wszystkie kraje!";
            questionText.text = "Koniec Gry!";
            foreach (SymbolButton button in symbolButtons)
            {
                button.gameObject.SetActive(false);
            }
            return;
        }

        currentCountryInfo = Passport.GetCountry(chosenCountryNameFromPassport);
        if (currentCountryInfo == null)
        {
            Debug.LogError($"Nie można znaleźć danych dla kraju: {chosenCountryNameFromPassport} w Paszporcie.");
            LoadNextSymbolQuestion(); // Spróbuj załadować następny, jeśli jest błąd
            return;
        }

        Passport.CurrentCountry = currentCountryInfo.nazwa; // Ustaw bieżący kraj w Paszporcie
        // NOTE:surusek - i tak tego nie używasz, to po co to
        // Passport.CurrentSelectedCountry = currentCountryInfo; // Ustaw bieżący obiekt CountryInfo w Paszporcie

        currentCountryName = currentCountryInfo.nazwa;
        // Zakładamy, że CountryInfo ma pole np. `symbolSpriteName` lub podobne, które jest nazwą pliku Sprite
        // Jeśli `CountryInfo` przechowuje bezpośrednio Sprite, to super, ale z JSON to trudniejsze.
        // Tutaj użyjemy metody pomocniczej LoadSpriteByResourceName.
        // Musisz upewnić się, że `currentCountryInfo` ma pole, np. `symbolResourceName`
        // które odpowiada nazwie pliku sprite'a w folderze Resources/Games_Data/Kraje - Symbole/
        // np. jeśli dla Włoch symbol to "Pizza-Wlochy.png", to pole powinno zawierać "Pizza-Wlochy"
        // ROZWIĄZANY KONFLIKT 2
        correctSymbol = LoadSpriteByResourceName(currentCountryInfo.symbolPlik);

        if (correctSymbol == null)
        {
            Debug.LogError($"Nie udało się załadować poprawnego symbolu dla kraju: {currentCountryName} (nazwa zasobu: {currentCountryInfo.symbolPlik}). Sprawdź ścieżkę i nazwę pliku w Resources oraz w danych JSON.");
            LoadNextSymbolQuestion(); // Spróbuj załadować następny
            return;
        }

        attempts = 0;
        currentPointsForThisQuestion = maxPointsPerQuestion;

        questionText.text = $"Wybierz symbol kraju: {currentCountryName}";

        List<Sprite> options = new List<Sprite>();
        options.Add(correctSymbol);

        List<Sprite> distractorPool = new List<Sprite>(allUniqueSymbolsInGame);
        distractorPool.Remove(correctSymbol);

        for (int i = 0; i < distractorPool.Count; i++) // Mieszanie
        {
            Sprite temp = distractorPool[i];
            int r = Random.Range(i, distractorPool.Count);
            distractorPool[i] = distractorPool[r];
            distractorPool[r] = temp;
        }

        int optionsNeeded = symbolButtons.Length - 1;
        for (int i = 0; i < optionsNeeded && i < distractorPool.Count; i++)
        {
            options.Add(distractorPool[i]);
        }

        for (int i = 0; i < options.Count; i++) // Mieszanie finalnej listy
        {
            Sprite temp = options[i];
            int r = Random.Range(i, options.Count);
            options[i] = options[r];
            options[r] = temp;
        }

        for (int i = 0; i < symbolButtons.Length; i++)
        {
            if (i < options.Count)
            {
                symbolButtons[i].gameObject.SetActive(true);
                symbolButtons[i].Initialize(options[i], this);
            }
            else
            {
                symbolButtons[i].gameObject.SetActive(false);
            }
        }
        feedbackText.text = "";
    }

    public void CheckAnswer(Sprite chosenSymbol)
    {
        bool correctAnswer = (chosenSymbol == correctSymbol);

        if (correctAnswer)
        {
            feedbackText.text = "Brawo! To poprawny symbol!";
            Passport.Points += currentPointsForThisQuestion; // Użyj Passport.Points
            UpdateScoreDisplay();
        }
        else
        {
            attempts++;
            currentPointsForThisQuestion = Mathf.Max(0, currentPointsForThisQuestion - 1);

            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec prób! Poprawnym symbolem dla {currentCountryName} był ten właściwy.";
            }
            else
            {
                feedbackText.text = $"Niestety, to nie ten symbol. Pozostało prób: {maxAttempts - attempts}. Tracisz 1 punkt.";
            }
        }

        // Kraj jest "odwiedzony" po zakończeniu prób lub po poprawnej odpowiedzi.
        if (correctAnswer || attempts >= maxAttempts)
        {
            Passport.VisitCountry(currentCountryName);
            Invoke("LoadNextSymbolQuestion", correctAnswer ? 2f : 3f);
        }
    }
}