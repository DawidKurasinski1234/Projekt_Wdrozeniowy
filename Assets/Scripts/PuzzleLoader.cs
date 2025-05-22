using System;
using System.IO; // Potrzebne dla Path
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla komponentu Image
using TMPro; // Jeœli chcesz wyœwietlaæ nazwê kraju na scenie puzzli

public class PuzzleLoader : MonoBehaviour
{
    public Image puzzleImageDisplay; // To pole BÊDZIESZ PRZECI¥GAÆ na scenie puzzli
    public TMP_Text countryNameLabel; // Opcjonalnie: do wyœwietlenia nazwy kraju na scenie puzzli

    private void Start()
    {
        // Upewnij siê, ¿e Passport jest zainicjowany (powinno byæ przez GameInitializer)
        Passport.Init();

        // Pobierz wylosowany kraj z Passport
        CountryInfo country = Passport.CurrentSelectedCountry;

        if (country == null)
        {
            Debug.LogError("PuzzleLoader: Brak wylosowanego kraju w Passport. CurrentSelectedCountry jest nullem.");
            if (countryNameLabel != null)
            {
                countryNameLabel.text = "B³¹d: Brak danych kraju.";
            }
            if (puzzleImageDisplay != null)
            {
                puzzleImageDisplay.gameObject.SetActive(false);
            }
            return;
        }

        // Opcjonalnie: wyœwietl nazwê kraju na scenie puzzli
        if (countryNameLabel != null)
        {
            countryNameLabel.text = $"U³ó¿ puzzle z kraju: {country.nazwa}";
        }

        // Za³aduj i ustaw obrazek puzzli
        LoadPuzzleImage(country.obrazekPuzzle);
    }

    /// <summary>
    /// £aduje obrazek puzzli z folderu Resources i przypisuje go do komponentu UI Image.
    /// </summary>
    /// <param name="imageName">Nazwa pliku obrazka (np. "Gora_Fuji.jpg") z bazy danych.</param>
    private void LoadPuzzleImage(string imageName)
    {
        if (puzzleImageDisplay == null)
        {
            Debug.LogError("Brak referencji do obiektu UI Image (puzzleImageDisplay) w skrypcie PuzzleLoader. Przeci¹gnij obiekt Image z hierarchii do pola w Inspektorze.");
            return;
        }

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("Brak nazwy pliku obrazka puzzli w bazie danych dla wybranego kraju. Obrazek nie zostanie za³adowany.");
            puzzleImageDisplay.gameObject.SetActive(false);
            return;
        }

        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
        string fullPathInResources = "Puzzle - zdjêcia/" + imagePathWithoutExtension;

        Texture2D loadedTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (loadedTexture != null)
        {
            Sprite puzzleSprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
            puzzleImageDisplay.sprite = puzzleSprite;
            puzzleImageDisplay.SetNativeSize();
            puzzleImageDisplay.gameObject.SetActive(true);

            Debug.Log($"[PuzzleLoader.cs] Za³adowano obrazek puzzli: '{fullPathInResources}' dla kraju '{Passport.CurrentSelectedCountry.nazwa}'.");
        }
        else
        {
            Debug.LogError($"[PuzzleLoader.cs] NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}' w folderze 'Assets/Resources/Puzzle - zdjêcia/'. Upewnij siê, ¿e nazwa pliku jest poprawna i znajduje siê w odpowiednim miejscu.");
            puzzleImageDisplay.gameObject.SetActive(false);
        }
    }
}