using System.Collections.Generic;
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
        StartCapitalQuiz("W�ochy", "Rzym", new List<string> { "Mediolan", "Neapol", "Florencja", "Turyn" });
    }

    public void StartCapitalQuiz(string country, string capital, List<string> others)
    {
        questionText.text = $"Wska� stolic�: {country}";
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
            feedbackText.text = "Brawo! To poprawna odpowied�!";
            // Mo�na doda� punktacj�
        }
        else
        {
            attempts++;
            feedbackText.text = $"To nie stolica. Pr�ba: {attempts}/{maxAttempts}";
            if (attempts >= maxAttempts)
            {
                feedbackText.text = $"Koniec pr�b! Poprawna odpowied� to: {correctCapital}";
                // Automatyczny skip
            }
        }
    }
}
