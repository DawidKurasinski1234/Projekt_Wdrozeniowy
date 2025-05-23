using System;
using TMPro;
using UnityEngine;

public class Greeting : MonoBehaviour
{
    public TMP_Text greetingLabel, countryLabel;
    // TUTAJ NIE MA public Image puzzleImageDisplay;
    // TUTAJ NIE MA metody LoadPuzzleImage(string imageName);

    private void Start()
    {
        // Passport.Init() powinno być już wywołane przez GameInitializer na scenie LoaderScene.
        // Passport.CurrentCountry powinno być już ustawione.

        if (Passport.CurrentCountry == null)
        {
            countryLabel.text = "Odwiedziłeś/aś już wszystkie kraje w grze!";
            greetingLabel.text = "";
            Debug.LogWarning("Greeting: Passport.CurrentCountry jest nullem. Możliwe, że wszystkie kraje odwiedzone lub błąd inicjalizacji.");
            return;
        }

        // Ważne: GetCountry zwraca CountryInfo, więc zmienna powinna być CountryInfo
        CountryInfo country = Passport.GetCountry(Passport.CurrentCountry);
        // LUB bezpieczniej: var country = Passport.GetCountry(Passport.CurrentCountry);

        if (country == null)
        {
            Debug.LogError($"Greeting: Nie znaleziono danych dla kraju '{Passport.CurrentCountry}' w bazie.");
            countryLabel.text = "Błąd: Brak danych kraju.";
            greetingLabel.text = "";
            return;
        }

        greetingLabel.text = country.powitanie;
        countryLabel.text = country.nazwa;

        // Tutaj NIE ładujemy żadnego obrazka, to odpowiedzialność PuzzleLoader
    }
}