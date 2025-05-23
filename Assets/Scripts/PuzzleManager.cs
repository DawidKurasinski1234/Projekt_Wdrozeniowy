using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla Image, Button (chocia� Image tylko do wy�wietlenia ca�o�ci)
using System.IO; // Potrzebne dla Path.GetFileNameWithoutExtension

public class PuzzleManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4; // Liczba kawa�k�w w kr�tszym wymiarze
    [SerializeField] private Transform gameHolder; // Rodzic dla wszystkich kawa�k�w puzzli
    [SerializeField] private GameObject piecePrefab; // Zmienione na GameObject, bo Instantiate potrzebuje GameObject

    // Usuni�to [Header("UI Elements")] - niepotrzebne, bo obrazek jest wylosowany
    // Usuni�to [SerializeField] private List<Texture2D> imageTextures;
    // Usuni�to [SerializeField] private Transform levelSelectPanel;
    // Usuni�to [SerializeField] private Image levelSelectPrefab;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    // Referencja do obiektu Image, kt�ry wy�wietla� ca�y obrazek przed poci�ciem
    // U�yjemy go do pobrania Texture2D lub po prostu za�adujemy Texture2D bezpo�rednio
    public Image fullPuzzleImageDisplay;

    void Start()
    {
        // 1. Upewnij si�, �e Passport jest zainicjowany i kraj jest wybrany
        // To powinno by� ju� zrobione przez GameInitializer na LoaderScene
        Passport.Init();

        if (Passport.CurrentCountry == null)
        {
            Debug.LogError("PuzzleManager: Brak wylosowanego kraju w Passport. Nie mo�na rozpocz�� gry w puzzle.");
            // Opcjonalnie: wy�wietl komunikat b��du dla u�ytkownika
            return;
        }

        CountryInfo currentCountry = Passport.GetCountry(Passport.CurrentCountry);
        if (currentCountry == null)
        {
            Debug.LogError($"PuzzleManager: Nie znaleziono danych dla kraju '{Passport.CurrentCountry}'. Sprawd� plik JSON.");
            return;
        }

        // 2. Za�aduj Texture2D dla wylosowanego obrazka puzzli
        // U�ywamy nazwy pliku z CountryInfo.obrazekPuzzle
        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(currentCountry.obrazekPuzzle);
        string fullPathInResources = "Puzzle - zdj�cia/" + imagePathWithoutExtension;

        Texture2D jigsawTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (jigsawTexture == null)
        {
            Debug.LogError($"PuzzleManager: NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}'. Upewnij si�, �e plik jest w Assets/Resources/Puzzle - zdj�cia/ i ma poprawne ustawienia (Read/Write Enabled).");
            return;
        }

        Debug.Log($"PuzzleManager: Pomy�lnie za�adowano tekstur� puzzli: {jigsawTexture.name}");

        // Opcjonalnie: Je�li masz komponent Image na scenie, kt�ry wy�wietla� ca�o�� obrazka
        // i chcesz go ukry� przed generowaniem kawa�k�w:
        if (fullPuzzleImageDisplay != null)
        {
            fullPuzzleImageDisplay.gameObject.SetActive(false); // Ukryj pe�ny obrazek przed generowaniem kawa�k�w
        }

        // 3. Rozpocznij gr� w puzzle z za�adowan� tekstur�
        StartGame(jigsawTexture);
    }

    // Publiczna metoda StartGame, kt�ra teraz przyjmuje Texture2D
    public void StartGame(Texture2D jigsawTexture)
    {
        // Zmienili�my to, �eby nie ukrywa� panelu wyboru poziomu, bo go nie ma.
        // Je�li masz jaki� inny panel UI, kt�ry chcesz ukry�, mo�esz go tutaj wy��czy�.
        // np. if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(false); 

        pieces = new List<Transform>();

        dimensions = GetDimensions(jigsawTexture, difficulty);

        CreateJigsawPieces(jigsawTexture);

        // Dodaj tutaj logik� rozrzucania/mieszania kawa�k�w puzzli, je�li jeszcze jej nie masz
        // ShufflePieces(); // Przyk�ad
    }

    Vector2Int GetDimensions(Texture2D jigsawTexture, int difficulty)
    {
        Vector2Int dimensions = Vector2Int.zero;
        if (jigsawTexture.width < jigsawTexture.height)
        {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * jigsawTexture.height) / jigsawTexture.width;
        }
        else
        {
            dimensions.x = (difficulty * jigsawTexture.width) / jigsawTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    void CreateJigsawPieces(Texture2D jigsawTexture)
    {
        height = 1f / dimensions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                // Zmieniono piecePrefab na GameObject, wi�c Instantiate musi by� na GameObject
                Transform piece = Instantiate(piecePrefab, gameHolder).transform; // <- U�yj .transform

                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                // Zapewnij, �e piecePrefab ma komponent MeshFilter i MeshRenderer
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                mesh.uv = new Vector2[4]; // Inicjalizuj UV array

                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;

                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                mesh.uv = uv;

                // Ustaw tekstur� na materiale kawa�ka puzzli
                // Wa�ne: Je�li piecePrefab ma wi�cej ni� jeden materia�, musisz wybra� w�a�ciwy indeks
                // Lub je�li materia� jest wsp�dzielony, u�yj .material, �eby utworzy� instancj�
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    // Mo�esz doda� tutaj metod� do mieszania kawa�k�w, np.
    // void ShufflePieces() { ... }
}