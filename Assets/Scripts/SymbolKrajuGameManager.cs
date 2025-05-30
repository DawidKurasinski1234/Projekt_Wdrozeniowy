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

    // Œcie¿ka w folderze Resources do katalogu z symbolami krajów
    public string symbolsResourcePath = "Games_Data/Kraje - Symbole/"; // Np. "Games_Data/Kraje - Symbole/"

    private Sprite correctSymbol;
    private string currentCountryName; // Nazwa kraju z Passport.CountryInfo
    private CountryInfo currentCountryInfo; // Pe³ne informacje o bie¿¹cym kraju

    private int attempts;
    private int maxAttempts = 5;
    private int maxPointsPerQuestion = 5;
    private int currentPointsForThisQuestion;

    private List<Sprite> allUniqueSymbolsInGame;

    void Start()
    {
        Passport.Init(); // Inicjalizuj system Paszportu

        if (!Passport.IsInitialized) // Zak³adaj¹c, ¿e m_entries jest publiczne lub mamy dostêp do jego d³ugoœci
        {
            Debug.LogError("Dane krajów w Paszporcie nie zosta³y za³adowane! SprawdŸ Passport.Init() i plik JSON 'countries'.");
            feedbackText.text = "B³¹d: Brak danych o krajach.";
            questionText.text = "";
            if (scoreText != null) scoreText.text = "Punkty: 0";
            foreach (SymbolButton button in symbolButtons) button.gameObject.SetActive(false);
            return;
        }

        // Zbierz wszystkie unikalne symbole z danych Paszportu
        allUniqueSymbolsInGame = new List<Sprite>();
        // Potrzebujemy dostêpu do wszystkich CountryInfo z Passport.m_entries
        // Jeœli m_entries jest prywatne, Passport musi udostêpniæ metodê do pobrania wszystkich CountryInfo
        // Na razie zak³adam, ¿e mo¿emy iterowaæ po `Passport.GetAllCountryInfos()` (musia³byœ dodaæ tak¹ metodê do Passport)
        // LUB jeœli Passport.m_entries jest `internal` lub `public` (co nie jest idealne dla `m_entries`):
        /*
        foreach (var entry in Passport.m_entries) // To wymaga, aby m_entries by³o dostêpne
        {
            Sprite symbol = LoadSpriteByResourceName(entry.country.symbolSpriteName); // Zak³adamy, ¿e CountryInfo ma pole symbolSpriteName
            if (symbol != null && !allUniqueSymbolsInGame.Contains(symbol))
            {
                allUniqueSymbolsInGame.Add(symbol);
            }
        }
        */
        // Bezpieczniejsza alternatywa: Passport powinien mieæ metodê zwracaj¹c¹ wszystkie nazwy zasobów symboli
        // Na potrzeby przyk³adu, jeœli nie mo¿esz zmieniæ Passport, a `countrySymbolDatabase` by³o u¿ywane wczeœniej
        // do trzymania Sprite'ów, mo¿esz tymczasowo za³adowaæ wszystkie Sprite'y z podanej œcie¿ki
        // To jednak mniej eleganckie ni¿ pobieranie ich na podstawie danych z Passport.
        // Poni¿ej uproszczone ³adowanie WSZYSTKICH sprite'ów z folderu - jeœli nie masz innej metody:
        var loadedSprites = Resources.LoadAll<Sprite>(symbolsResourcePath.TrimEnd('/'));
        allUniqueSymbolsInGame = loadedSprites.Distinct().ToList();


        if (allUniqueSymbolsInGame.Count == 0)
        {
            Debug.LogError($"Nie za³adowano ¿adnych symboli z Resources/{symbolsResourcePath}. SprawdŸ œcie¿kê i czy obrazki s¹ typu Sprite.");
        }
        else if (allUniqueSymbolsInGame.Count < symbolButtons.Length)
        {
            Debug.LogWarning("Uwaga: Liczba unikalnych symboli w grze jest mniejsza ni¿ liczba przycisków opcji.");
        }

        UpdateScoreDisplay(); // Wyœwietl pocz¹tkowe punkty z Paszportu
        LoadNextSymbolQuestion();
    }

    // Helper do ³adowania Sprite na podstawie nazwy (zak³adaj¹c, ¿e CountryInfo ma pole symbolSpriteName)
    Sprite LoadSpriteByResourceName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        // Pe³na œcie¿ka np. "Games_Data/Kraje - Symbole/flamenco-Hiszpania"
        return Resources.Load<Sprite>(symbolsResourcePath + spriteName);
    }


    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Punkty: {Passport.Points}"; // U¿yj Passport.Points
        }
    }

    public void LoadNextSymbolQuestion()
    {
        string chosenCountryNameFromPassport = Passport.ChooseCountry(); // Wybierz kraj u¿ywaj¹c Paszportu

        if (string.IsNullOrEmpty(chosenCountryNameFromPassport))
        {
            feedbackText.text = "Gratulacje! Odwiedzi³eœ wszystkie kraje!";
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
            Debug.LogError($"Nie mo¿na znaleŸæ danych dla kraju: {chosenCountryNameFromPassport} w Paszporcie.");
            LoadNextSymbolQuestion(); // Spróbuj za³adowaæ nastêpny, jeœli jest b³¹d
            return;
        }

        Passport.CurrentCountry = currentCountryInfo.nazwa; // Ustaw bie¿¹cy kraj w Paszporcie
        // NOTE:surusek - i tak tego nie u¿ywasz, to po co to
        // Passport.CurrentSelectedCountry = currentCountryInfo; // Ustaw bie¿¹cy obiekt CountryInfo w Paszporcie

        currentCountryName = currentCountryInfo.nazwa;
        // Zak³adamy, ¿e CountryInfo ma pole np. `symbolSpriteName` lub podobne, które jest nazw¹ pliku Sprite
        // Jeœli `CountryInfo` przechowuje bezpoœrednio Sprite, to super, ale z JSON to trudniejsze.
        // Tutaj u¿yjemy metody pomocniczej LoadSpriteByResourceName.
        // Musisz upewniæ siê, ¿e `currentCountryInfo` ma pole, np. `symbolResourceName`
        // które odpowiada nazwie pliku sprite'a w folderze Resources/Games_Data/Kraje - Symbole/
        // np. jeœli dla W³och symbol to "Pizza-Wlochy.png", to pole powinno zawieraæ "Pizza-Wlochy"
        string resName = currentCountryInfo.SymbolResourceName;
        correctSymbol = LoadSpriteByResourceName(resName); // ZMIEÑ "symbolSpriteName" na faktyczne pole w CountryInfo

        if (correctSymbol == null)
        {
            Debug.LogError($"Nie uda³o siê za³adowaæ poprawnego symbolu dla kraju: {currentCountryName} (nazwa zasobu: {resName}). SprawdŸ œcie¿kê i nazwê pliku w Resources oraz w danych JSON.");
            LoadNextSymbolQuestion(); // Spróbuj za³adowaæ nastêpny
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
            Passport.Points += currentPointsForThisQuestion; // U¿yj Passport.Points
            UpdateScoreDisplay();
        }
        else
        {
            attempts++;
            currentPointsForThisQuestion = Mathf.Max(0, currentPointsForThisQuestion - 1);

            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec prób! Poprawnym symbolem dla {currentCountryName} by³ ten w³aœciwy.";
            }
            else
            {
                feedbackText.text = $"Niestety, to nie ten symbol. Pozosta³o prób: {maxAttempts - attempts}. Tracisz 1 punkt.";
            }
        }

        // Kraj jest "odwiedzony" po zakoñczeniu prób lub po poprawnej odpowiedzi.
        if (correctAnswer || attempts >= maxAttempts)
        {
            Passport.VisitCountry(currentCountryName);
            Invoke("LoadNextSymbolQuestion", correctAnswer ? 2f : 3f);
        }
    }
}