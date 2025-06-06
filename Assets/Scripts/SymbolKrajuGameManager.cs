using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO; // WAŻNE: Potrzebne do obsługi ścieżek plików

public class SymbolKrajuGameManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public SymbolButton[] symbolButtons;
    public string symbolsResourcePath = "Kraje - Symbole/"; // Usunięto "Resources/" - Unity robi to automatycznie

    private Sprite correctSymbol;
    private string currentCountryName;
    private CountryInfo currentCountryInfo;

    private int attempts;
    private int maxAttempts = 5;
    private int maxPointsPerQuestion = 5;
    private int currentPointsForThisQuestion;

    private List<Sprite> allUniqueSymbolsInGame;

    void Start()
    {
        Passport.Init();

        if (Passport.GetEntriesCount() == 0)
        {
            Debug.LogError("Dane krajów w Paszporcie nie zostały załadowane!");
            // ... obsługa błędu ...
            return;
        }

        // Ładowanie wszystkich symboli na starcie gry
        var loadedSprites = Resources.LoadAll<Sprite>(symbolsResourcePath.TrimEnd('/'));
        allUniqueSymbolsInGame = loadedSprites.Distinct().ToList();

        if (allUniqueSymbolsInGame.Count == 0)
        {
            Debug.LogError($"Nie załadowano żadnych symboli z Resources/{symbolsResourcePath}. Sprawdź ścieżkę i czy obrazki są typu Sprite.");
        }
        else if (allUniqueSymbolsInGame.Count < symbolButtons.Length)
        {
            Debug.LogWarning("Uwaga: Liczba unikalnych symboli jest mniejsza niż liczba przycisków.");
        }

        UpdateScoreDisplay();
        LoadNextSymbolQuestion();
    }

  
    Sprite LoadSpriteByResourceName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName)) return null;

        // Pobierz nazwę pliku bez rozszerzenia, np. z "Pizza-Wlochy.jpg" robi "Pizza-Wlochy"
        string spriteNameWithoutExtension = Path.GetFileNameWithoutExtension(spriteName);

        // Użyj nazwy bez rozszerzenia do załadowania zasobu
        return Resources.Load<Sprite>(symbolsResourcePath + spriteNameWithoutExtension);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Punkty: {Passport.Points}";
        }
    }

    public void LoadNextSymbolQuestion()
    {
        string chosenCountryNameFromPassport = Passport.ChooseCountry();

        if (string.IsNullOrEmpty(chosenCountryNameFromPassport))
        {
            feedbackText.text = "Gratulacje! Odwiedziłeś wszystkie kraje!";
            questionText.text = "Koniec Gry!";
            foreach (var button in symbolButtons) button.gameObject.SetActive(false);
            return;
        }

        currentCountryInfo = Passport.GetCountry(chosenCountryNameFromPassport);
        currentCountryName = currentCountryInfo.nazwa;
        correctSymbol = LoadSpriteByResourceName(currentCountryInfo.symbolPlik);

        if (correctSymbol == null)
        {
            Debug.LogError($"Krytyczny błąd: Nie można znaleźć symbolu dla {currentCountryName}");
            Invoke("LoadNextSymbolQuestion", 0.1f);
            return;
        }

        attempts = 0;
        currentPointsForThisQuestion = maxPointsPerQuestion;
        questionText.text = $"Wybierz symbol kraju: {currentCountryName}";

        

        // 1. Zacznij listę opcji z poprawnym symbolem.
        List<Sprite> options = new List<Sprite> { correctSymbol };

        // Stwórz pulę "rozpraszaczy" ze wszystkich dostępnych symboli.
        List<Sprite> distractorPool = new List<Sprite>(allUniqueSymbolsInGame);

       //usuniecie z listy niepoprawnych poprawnej odpowiedzi zeby ona zawsze była w wyswietlanych
        distractorPool.Remove(correctSymbol);

       
        for (int i = 0; i < distractorPool.Count; i++)
        {
            Sprite temp = distractorPool[i];
            int randomIndex = Random.Range(i, distractorPool.Count);
            distractorPool[i] = distractorPool[randomIndex];
            distractorPool[randomIndex] = temp;
        }

        
        int distractorsNeeded = symbolButtons.Length - 1;
        for (int i = 0; i < distractorsNeeded && i < distractorPool.Count; i++)
        {
            options.Add(distractorPool[i]);
        }

        
        for (int i = 0; i < options.Count; i++)
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
            Passport.Points += currentPointsForThisQuestion;
            UpdateScoreDisplay();
        }
        else
        {
            attempts++;
            currentPointsForThisQuestion = Mathf.Max(0, currentPointsForThisQuestion - 1);
            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec prób! Poprawny symbol dla {currentCountryName} to ten właściwy.";
            }
            else
            {
                feedbackText.text = $"Niestety, to nie ten symbol. Pozostało prób: {maxAttempts - attempts}.";
            }
        }

        if (correctAnswer || attempts >= maxAttempts)
        {
            Passport.VisitCountry(currentCountryName);
            Invoke("LoadNextSymbolQuestion", correctAnswer ? 2f : 3f);
        }
    }
}