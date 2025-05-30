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

    // �cie�ka w folderze Resources do katalogu z symbolami kraj�w
    public string symbolsResourcePath = "Games_Data/Kraje - Symbole/"; // Np. "Games_Data/Kraje - Symbole/"

    private Sprite correctSymbol;
    private string currentCountryName; // Nazwa kraju z Passport.CountryInfo
    private CountryInfo currentCountryInfo; // Pe�ne informacje o bie��cym kraju

    private int attempts;
    private int maxAttempts = 5;
    private int maxPointsPerQuestion = 5;
    private int currentPointsForThisQuestion;

    private List<Sprite> allUniqueSymbolsInGame;

    void Start()
    {
        Passport.Init(); // Inicjalizuj system Paszportu

        if (!Passport.IsInitialized) // Zak�adaj�c, �e m_entries jest publiczne lub mamy dost�p do jego d�ugo�ci
        {
            Debug.LogError("Dane kraj�w w Paszporcie nie zosta�y za�adowane! Sprawd� Passport.Init() i plik JSON 'countries'.");
            feedbackText.text = "B��d: Brak danych o krajach.";
            questionText.text = "";
            if (scoreText != null) scoreText.text = "Punkty: 0";
            foreach (SymbolButton button in symbolButtons) button.gameObject.SetActive(false);
            return;
        }

        // Zbierz wszystkie unikalne symbole z danych Paszportu
        allUniqueSymbolsInGame = new List<Sprite>();
        // Potrzebujemy dost�pu do wszystkich CountryInfo z Passport.m_entries
        // Je�li m_entries jest prywatne, Passport musi udost�pni� metod� do pobrania wszystkich CountryInfo
        // Na razie zak�adam, �e mo�emy iterowa� po `Passport.GetAllCountryInfos()` (musia�by� doda� tak� metod� do Passport)
        // LUB je�li Passport.m_entries jest `internal` lub `public` (co nie jest idealne dla `m_entries`):
        /*
        foreach (var entry in Passport.m_entries) // To wymaga, aby m_entries by�o dost�pne
        {
            Sprite symbol = LoadSpriteByResourceName(entry.country.symbolSpriteName); // Zak�adamy, �e CountryInfo ma pole symbolSpriteName
            if (symbol != null && !allUniqueSymbolsInGame.Contains(symbol))
            {
                allUniqueSymbolsInGame.Add(symbol);
            }
        }
        */
        // Bezpieczniejsza alternatywa: Passport powinien mie� metod� zwracaj�c� wszystkie nazwy zasob�w symboli
        // Na potrzeby przyk�adu, je�li nie mo�esz zmieni� Passport, a `countrySymbolDatabase` by�o u�ywane wcze�niej
        // do trzymania Sprite'�w, mo�esz tymczasowo za�adowa� wszystkie Sprite'y z podanej �cie�ki
        // To jednak mniej eleganckie ni� pobieranie ich na podstawie danych z Passport.
        // Poni�ej uproszczone �adowanie WSZYSTKICH sprite'�w z folderu - je�li nie masz innej metody:
        var loadedSprites = Resources.LoadAll<Sprite>(symbolsResourcePath.TrimEnd('/'));
        allUniqueSymbolsInGame = loadedSprites.Distinct().ToList();


        if (allUniqueSymbolsInGame.Count == 0)
        {
            Debug.LogError($"Nie za�adowano �adnych symboli z Resources/{symbolsResourcePath}. Sprawd� �cie�k� i czy obrazki s� typu Sprite.");
        }
        else if (allUniqueSymbolsInGame.Count < symbolButtons.Length)
        {
            Debug.LogWarning("Uwaga: Liczba unikalnych symboli w grze jest mniejsza ni� liczba przycisk�w opcji.");
        }

        UpdateScoreDisplay(); // Wy�wietl pocz�tkowe punkty z Paszportu
        LoadNextSymbolQuestion();
    }

    // Helper do �adowania Sprite na podstawie nazwy (zak�adaj�c, �e CountryInfo ma pole symbolSpriteName)
    Sprite LoadSpriteByResourceName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;
        // Pe�na �cie�ka np. "Games_Data/Kraje - Symbole/flamenco-Hiszpania"
        return Resources.Load<Sprite>(symbolsResourcePath + spriteName);
    }


    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Punkty: {Passport.Points}"; // U�yj Passport.Points
        }
    }

    public void LoadNextSymbolQuestion()
    {
        string chosenCountryNameFromPassport = Passport.ChooseCountry(); // Wybierz kraj u�ywaj�c Paszportu

        if (string.IsNullOrEmpty(chosenCountryNameFromPassport))
        {
            feedbackText.text = "Gratulacje! Odwiedzi�e� wszystkie kraje!";
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
            Debug.LogError($"Nie mo�na znale�� danych dla kraju: {chosenCountryNameFromPassport} w Paszporcie.");
            LoadNextSymbolQuestion(); // Spr�buj za�adowa� nast�pny, je�li jest b��d
            return;
        }

        Passport.CurrentCountry = currentCountryInfo.nazwa; // Ustaw bie��cy kraj w Paszporcie
        // NOTE:surusek - i tak tego nie u�ywasz, to po co to
        // Passport.CurrentSelectedCountry = currentCountryInfo; // Ustaw bie��cy obiekt CountryInfo w Paszporcie

        currentCountryName = currentCountryInfo.nazwa;
        // Zak�adamy, �e CountryInfo ma pole np. `symbolSpriteName` lub podobne, kt�re jest nazw� pliku Sprite
        // Je�li `CountryInfo` przechowuje bezpo�rednio Sprite, to super, ale z JSON to trudniejsze.
        // Tutaj u�yjemy metody pomocniczej LoadSpriteByResourceName.
        // Musisz upewni� si�, �e `currentCountryInfo` ma pole, np. `symbolResourceName`
        // kt�re odpowiada nazwie pliku sprite'a w folderze Resources/Games_Data/Kraje - Symbole/
        // np. je�li dla W�och symbol to "Pizza-Wlochy.png", to pole powinno zawiera� "Pizza-Wlochy"
        string resName = currentCountryInfo.SymbolResourceName;
        correctSymbol = LoadSpriteByResourceName(resName); // ZMIE� "symbolSpriteName" na faktyczne pole w CountryInfo

        if (correctSymbol == null)
        {
            Debug.LogError($"Nie uda�o si� za�adowa� poprawnego symbolu dla kraju: {currentCountryName} (nazwa zasobu: {resName}). Sprawd� �cie�k� i nazw� pliku w Resources oraz w danych JSON.");
            LoadNextSymbolQuestion(); // Spr�buj za�adowa� nast�pny
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
            Passport.Points += currentPointsForThisQuestion; // U�yj Passport.Points
            UpdateScoreDisplay();
        }
        else
        {
            attempts++;
            currentPointsForThisQuestion = Mathf.Max(0, currentPointsForThisQuestion - 1);

            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec pr�b! Poprawnym symbolem dla {currentCountryName} by� ten w�a�ciwy.";
            }
            else
            {
                feedbackText.text = $"Niestety, to nie ten symbol. Pozosta�o pr�b: {maxAttempts - attempts}. Tracisz 1 punkt.";
            }
        }

        // Kraj jest "odwiedzony" po zako�czeniu pr�b lub po poprawnej odpowiedzi.
        if (correctAnswer || attempts >= maxAttempts)
        {
            Passport.VisitCountry(currentCountryName);
            Invoke("LoadNextSymbolQuestion", correctAnswer ? 2f : 3f);
        }
    }
}