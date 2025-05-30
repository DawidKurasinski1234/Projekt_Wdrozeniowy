using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla komponentu Image
using TMPro; // Je�li chcesz wy�wietla� nazw� kraju na scenie puzzli

public class PuzzleLoader : MonoBehaviour
{
    public Image puzzleImageDisplay; // PRZECI�GNIJ tutaj komponent Image ze sceny puzzli
    public TMP_Text countryNameLabel; // Opcjonalnie: do wy�wietlenia nazwy kraju na scenie puzzli

    private void Start()
    {
        // Passport.Init() zosta�o ju� wywo�ane przez GameInitializer.
        // Passport.CurrentCountry powinno by� ju� ustawione i zawiera� nazw� wylosowanego kraju.

        if (Passport.CurrentCountry == null) // Nadal sprawdzamy, czy nazwa kraju jest ustawiona
        {
            Debug.LogError("PuzzleLoader: Passport.CurrentCountry jest nullem! B��d w GameInitializer lub brak dost�pnych kraj�w.");
            if (countryNameLabel != null) { countryNameLabel.text = "B��d: Brak danych kraju."; }
            if (puzzleImageDisplay != null) { puzzleImageDisplay.gameObject.SetActive(false); }
            return;
        }

        // Pobierz pe�ny obiekt CountryInfo na podstawie nazwy kraju
        CountryInfo country = Passport.GetCountry(Passport.CurrentCountry);

        if (country == null) // Sprawdzamy, czy faktycznie uda�o si� pobra� dane
        {
            Debug.LogError($"PuzzleLoader: Nie znaleziono danych dla kraju '{Passport.CurrentCountry}' w bazie. Sprawd� JSON.");
            if (countryNameLabel != null) { countryNameLabel.text = "B��d: Brak danych kraju."; }
            if (puzzleImageDisplay != null) { puzzleImageDisplay.gameObject.SetActive(false); }
            return;
        }

        if (countryNameLabel != null)
        {
            countryNameLabel.text = $"U�� puzzle z kraju: {country.nazwa}";
        }

        LoadPuzzleImage(country.obrazekPuzzle); // Wywo�aj �adowanie obrazka
    }

    private void LoadPuzzleImage(string imageName)
    {
        if (puzzleImageDisplay == null)
        {
            Debug.LogError("Brak referencji do obiektu UI Image (puzzleImageDisplay) w skrypcie PuzzleLoader. Przeci�gnij obiekt Image z hierarchii do pola w Inspektorze.");
            return;
        }

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("Brak nazwy pliku obrazka puzzli w bazie danych dla wybranego kraju.");
            puzzleImageDisplay.gameObject.SetActive(false);
            return;
        }

        // Resources.Load nie wymaga podawania rozszerzenia pliku
        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
        string fullPathInResources = "PuzzleImages/" + imagePathWithoutExtension;

        Texture2D loadedTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (loadedTexture != null)
        {
            Sprite puzzleSprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
            puzzleImageDisplay.sprite = puzzleSprite;
            puzzleImageDisplay.SetNativeSize(); // Dopasuj rozmiar Image do rozmiaru obrazka
            puzzleImageDisplay.gameObject.SetActive(true);

            Debug.Log($"[PuzzleLoader.cs] Za�adowano obrazek puzzli: '{fullPathInResources}' dla kraju '{Passport.CurrentCountry}'.");
        }
        else
        {
            Debug.LogError($"[PuzzleLoader.cs] NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}' w folderze 'Assets/Resources/Puzzle - zdj�cia/'. Upewnij si�, �e nazwa pliku jest poprawna i znajduje si� w odpowiednim miejscu.");
            puzzleImageDisplay.gameObject.SetActive(false);
        }
    }
}