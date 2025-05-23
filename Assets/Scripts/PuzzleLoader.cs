using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla komponentu Image
using TMPro; // Jeœli chcesz wyœwietlaæ nazwê kraju na scenie puzzli

public class PuzzleLoader : MonoBehaviour
{
    public Image puzzleImageDisplay; // PRZECI¥GNIJ tutaj komponent Image ze sceny puzzli
    public TMP_Text countryNameLabel; // Opcjonalnie: do wyœwietlenia nazwy kraju na scenie puzzli

    private void Start()
    {
        // Passport.Init() zosta³o ju¿ wywo³ane przez GameInitializer.
        // Passport.CurrentCountry powinno byæ ju¿ ustawione i zawieraæ nazwê wylosowanego kraju.

        if (Passport.CurrentCountry == null) // Nadal sprawdzamy, czy nazwa kraju jest ustawiona
        {
            Debug.LogError("PuzzleLoader: Passport.CurrentCountry jest nullem! B³¹d w GameInitializer lub brak dostêpnych krajów.");
            if (countryNameLabel != null) { countryNameLabel.text = "B³¹d: Brak danych kraju."; }
            if (puzzleImageDisplay != null) { puzzleImageDisplay.gameObject.SetActive(false); }
            return;
        }

        // Pobierz pe³ny obiekt CountryInfo na podstawie nazwy kraju
        CountryInfo country = Passport.GetCountry(Passport.CurrentCountry);

        if (country == null) // Sprawdzamy, czy faktycznie uda³o siê pobraæ dane
        {
            Debug.LogError($"PuzzleLoader: Nie znaleziono danych dla kraju '{Passport.CurrentCountry}' w bazie. SprawdŸ JSON.");
            if (countryNameLabel != null) { countryNameLabel.text = "B³¹d: Brak danych kraju."; }
            if (puzzleImageDisplay != null) { puzzleImageDisplay.gameObject.SetActive(false); }
            return;
        }

        if (countryNameLabel != null)
        {
            countryNameLabel.text = $"U³ó¿ puzzle z kraju: {country.nazwa}";
        }

        LoadPuzzleImage(country.obrazekPuzzle); // Wywo³aj ³adowanie obrazka
    }

    private void LoadPuzzleImage(string imageName)
    {
        if (puzzleImageDisplay == null)
        {
            Debug.LogError("Brak referencji do obiektu UI Image (puzzleImageDisplay) w skrypcie PuzzleLoader. Przeci¹gnij obiekt Image z hierarchii do pola w Inspektorze.");
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
        string fullPathInResources = "Puzzle - zdjêcia/" + imagePathWithoutExtension;

        Texture2D loadedTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (loadedTexture != null)
        {
            Sprite puzzleSprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
            puzzleImageDisplay.sprite = puzzleSprite;
            puzzleImageDisplay.SetNativeSize(); // Dopasuj rozmiar Image do rozmiaru obrazka
            puzzleImageDisplay.gameObject.SetActive(true);

            Debug.Log($"[PuzzleLoader.cs] Za³adowano obrazek puzzli: '{fullPathInResources}' dla kraju '{Passport.CurrentCountry}'.");
        }
        else
        {
            Debug.LogError($"[PuzzleLoader.cs] NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}' w folderze 'Assets/Resources/Puzzle - zdjêcia/'. Upewnij siê, ¿e nazwa pliku jest poprawna i znajduje siê w odpowiednim miejscu.");
            puzzleImageDisplay.gameObject.SetActive(false);
        }
    }
}