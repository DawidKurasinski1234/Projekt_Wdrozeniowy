using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla Image, Button (chocia¿ Image tylko do wyœwietlenia ca³oœci)
using System.IO; // Potrzebne dla Path.GetFileNameWithoutExtension

public class PuzzleManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4; // Liczba kawa³ków w krótszym wymiarze
    [SerializeField] private Transform gameHolder; // Rodzic dla wszystkich kawa³ków puzzli
    [SerializeField] private GameObject piecePrefab; // Zmienione na GameObject, bo Instantiate potrzebuje GameObject

    // Usuniêto [Header("UI Elements")] - niepotrzebne, bo obrazek jest wylosowany
    // Usuniêto [SerializeField] private List<Texture2D> imageTextures;
    // Usuniêto [SerializeField] private Transform levelSelectPanel;
    // Usuniêto [SerializeField] private Image levelSelectPrefab;

    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    // Referencja do obiektu Image, który wyœwietla³ ca³y obrazek przed pociêciem
    // U¿yjemy go do pobrania Texture2D lub po prostu za³adujemy Texture2D bezpoœrednio
    public Image fullPuzzleImageDisplay;

    void Start()
    {
        // 1. Upewnij siê, ¿e Passport jest zainicjowany i kraj jest wybrany
        // To powinno byæ ju¿ zrobione przez GameInitializer na LoaderScene
        Passport.Init();

        if (Passport.CurrentCountry == null)
        {
            Debug.LogError("PuzzleManager: Brak wylosowanego kraju w Passport. Nie mo¿na rozpocz¹æ gry w puzzle.");
            // Opcjonalnie: wyœwietl komunikat b³êdu dla u¿ytkownika
            return;
        }

        CountryInfo currentCountry = Passport.GetCountry(Passport.CurrentCountry);
        if (currentCountry == null)
        {
            Debug.LogError($"PuzzleManager: Nie znaleziono danych dla kraju '{Passport.CurrentCountry}'. SprawdŸ plik JSON.");
            return;
        }

        // 2. Za³aduj Texture2D dla wylosowanego obrazka puzzli
        // U¿ywamy nazwy pliku z CountryInfo.obrazekPuzzle
        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(currentCountry.obrazekPuzzle);
        string fullPathInResources = "Puzzle - zdjêcia/" + imagePathWithoutExtension;

        Texture2D jigsawTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (jigsawTexture == null)
        {
            Debug.LogError($"PuzzleManager: NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}'. Upewnij siê, ¿e plik jest w Assets/Resources/Puzzle - zdjêcia/ i ma poprawne ustawienia (Read/Write Enabled).");
            return;
        }

        Debug.Log($"PuzzleManager: Pomyœlnie za³adowano teksturê puzzli: {jigsawTexture.name}");

        // Opcjonalnie: Jeœli masz komponent Image na scenie, który wyœwietla³ ca³oœæ obrazka
        // i chcesz go ukryæ przed generowaniem kawa³ków:
        if (fullPuzzleImageDisplay != null)
        {
            fullPuzzleImageDisplay.gameObject.SetActive(false); // Ukryj pe³ny obrazek przed generowaniem kawa³ków
        }

        // 3. Rozpocznij grê w puzzle z za³adowan¹ tekstur¹
        StartGame(jigsawTexture);
    }

    // Publiczna metoda StartGame, która teraz przyjmuje Texture2D
    public void StartGame(Texture2D jigsawTexture)
    {
        // Zmieniliœmy to, ¿eby nie ukrywaæ panelu wyboru poziomu, bo go nie ma.
        // Jeœli masz jakiœ inny panel UI, który chcesz ukryæ, mo¿esz go tutaj wy³¹czyæ.
        // np. if (levelSelectPanel != null) levelSelectPanel.gameObject.SetActive(false); 

        pieces = new List<Transform>();

        dimensions = GetDimensions(jigsawTexture, difficulty);

        CreateJigsawPieces(jigsawTexture);

        // Dodaj tutaj logikê rozrzucania/mieszania kawa³ków puzzli, jeœli jeszcze jej nie masz
        // ShufflePieces(); // Przyk³ad
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
                // Zmieniono piecePrefab na GameObject, wiêc Instantiate musi byæ na GameObject
                Transform piece = Instantiate(piecePrefab, gameHolder).transform; // <- U¿yj .transform

                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                // Zapewnij, ¿e piecePrefab ma komponent MeshFilter i MeshRenderer
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

                // Ustaw teksturê na materiale kawa³ka puzzli
                // Wa¿ne: Jeœli piecePrefab ma wiêcej ni¿ jeden materia³, musisz wybraæ w³aœciwy indeks
                // Lub jeœli materia³ jest wspó³dzielony, u¿yj .material, ¿eby utworzyæ instancjê
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    // Mo¿esz dodaæ tutaj metodê do mieszania kawa³ków, np.
    // void ShufflePieces() { ... }
}