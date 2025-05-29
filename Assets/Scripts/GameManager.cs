/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public CityButton[] cityButtons;

    private string correctCapital;
    private int attempts = 0;
    private int maxAttempts = 5;

    void Start()
    {
        StartCapitalQuiz("W³ochy", "Rzym", new List<string> { "Mediolan", "Neapol", "Florencja", "Turyn" });
    }

    public void StartCapitalQuiz(string country, string capital, List<string> others)
    {
        questionText.text = $"Wska¿ stolicê: {country}";
        correctCapital = capital;
        attempts = 0;

        List<string> allCities = new List<string>(others);
        allCities.Add(capital);
        allCities.Sort((a, b) => Random.value.CompareTo(0.5f)); // shuffle

        for (int i = 0; i < cityButtons.Length; i++)
        {
            cityButtons[i].Initialize(allCities[i], this);
        }

        feedbackText.text = "";
    }

    public void CheckAnswer(string chosen)
    {
        if (chosen == correctCapital)
        {
            feedbackText.text = "Brawo! To poprawna odpowiedŸ!";
            // Mo¿na dodaæ punktacjê
        }
        else
        {
            attempts++;
            feedbackText.text = $"To nie stolica. Próba: {attempts}/{maxAttempts}";
            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec prób! Poprawna odpowiedŸ to: {correctCapital}";
                // Automatyczny skip
            }
        }
    }
}
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public CityButton[] cityButtons;

    private string correctCapital;
    private int attempts = 0;
    private int maxAttempts = 5;

    void Start()
    {
        Passport.Init();

        string countryName = Passport.CurrentCountry;
        if (countryName != null)
        {
            CountryInfo info = Passport.GetCountry(countryName);
            if (info != null)
            {
                Passport.VisitCountry(countryName);
                StartCapitalQuiz(info);
            }
            else
            {
                Debug.LogError("Nie znaleziono danych kraju: " + countryName);
            }
        }
        else
        {
            questionText.text = "Wszystkie kraje odwiedzone!";
        }
    }

    public void StartCapitalQuiz(CountryInfo country)
    {
        questionText.text = $"Wska¿ stolicê: {country.nazwa}";
        correctCapital = country.stolica;
        attempts = 0;

        // Skopiuj inne miasta i upewnij siê, ¿e nie zawieraj¹ stolicy
        List<string> allCities = new List<string>(country.miasta);
        allCities.Remove(country.stolica); // na wszelki wypadek

        // Dodaj stolicê do listy i przetasuj
        allCities.Add(country.stolica);
        ShuffleList(allCities);

        // Przypisz do przycisków
        for (int i = 0; i < cityButtons.Length; i++)
        {
            if (i < allCities.Count)
                cityButtons[i].Initialize(allCities[i], this);
            else
                cityButtons[i].gameObject.SetActive(false); // ukryj nadmiarowe przyciski
        }

        feedbackText.text = "";
    }

    public void CheckAnswer(string chosen)
    {
        if (chosen == correctCapital)
        {
            feedbackText.text = "Brawo! To poprawna odpowiedŸ!";
            Passport.Points++;
            // Tu mo¿na np. przejœæ do nastêpnego kraju lub ekranu
        }
        else
        {
            attempts++;
            feedbackText.text = $"To nie stolica. Próba: {attempts}/{maxAttempts}";
            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec prób! Poprawna odpowiedŸ to: {correctCapital}";
                // Tu te¿ mo¿na przejœæ dalej
            }
        }
    }

    // Proste mieszanie listy (Fisher-Yates shuffle)
    private void ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            string temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}