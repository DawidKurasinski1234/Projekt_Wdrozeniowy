using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla Image
using System.IO; // Potrzebne dla Path.GetFileNameWithoutExtension
using TMPro; // Dodane, jeœli u¿ywasz TextMeshPro dla puzzleText

public class PuzzleManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4; // Okreœla liczbê kawa³ków w krótszym wymiarze
    [SerializeField] private Transform gameHolder; // Rodzic dla wszystkich kawa³ków puzzli
    [SerializeField] private GameObject piecePrefab; // Prefab pojedynczego kawa³ka puzzli (powinien mieæ Quad, MeshFilter, MeshRenderer)

    [Tooltip("Mno¿nik skali dla ka¿dego kawa³ka puzzli. U¿yj, aby dostosowaæ widoczny rozmiar.")]
    [SerializeField] private float pieceScaleMultiplier = 100f; // Domyœlna wartoœæ, mo¿esz dostosowaæ w Inspektorze

    [Header("UI Elements")]
    [SerializeField] private Image fullPuzzleImageDisplay; // Obrazek UI wyœwietlaj¹cy ca³y obrazek przed pociêciem
    [SerializeField] private TextMeshProUGUI puzzleText; // Tekst "U³ó¿ puzzle" lub podobny (zak³adam TextMeshPro)
    [SerializeField] private GameObject levelSelectPanel; // Panel, który chcesz ukryæ po starcie gry (np. panel z wyborem poziomu/krajów)

    private List<Transform> pieces;
    private Vector2Int dimensions; // Wymiary siatki puzzli (np. 4x4, 5x5)
    private float pieceWidthWorld; // Szerokoœæ pojedynczego kawa³ka w jednostkach Unity
    private float pieceHeightWorld; // Wysokoœæ pojedynczego kawa³ka w jednostkach Unity

    void Start()
    {
        // SprawdŸ, czy GameHolder i PiecePrefab s¹ przypisane. Jeœli nie, zg³oœ b³¹d i przerwij.
        if (gameHolder == null)
        {
            Debug.LogError("PuzzleManager: Game Holder nie jest przypisany w Inspektorze! Przypisz obiekt nadrzêdny dla kawa³ków puzzli.");
            return;
        }
        if (piecePrefab == null)
        {
            // Ten b³¹d by³ widoczny na screenie
            Debug.LogError("PuzzleManager: Piece Prefab nie jest przypisany w Inspektorze! Przypisz prefab pojedynczego kawa³ka puzzli.");
            return;
        }

        // Upewnij siê, ¿e Passport jest zainicjowany.
        Passport.Init();

        // Rozpoczynamy proces ³adowania i startu puzzli
        LoadAndStartPuzzle();
    }

    private void LoadAndStartPuzzle()
    {
        Texture2D jigsawTexture = null;
        CountryInfo currentCountryInfo = null; // Zmienna do przechowywania obiektu CountryInfo

        // SprawdŸ, czy Passport.CurrentCountry (string) jest ustawiony
        if (string.IsNullOrEmpty(Passport.CurrentCountry))
        {
            Debug.LogError("PuzzleManager: Brak wylosowanej nazwy kraju w Passport.CurrentCountry (string). Nie mo¿na rozpocz¹æ gry w puzzle.");
            return;
        }

        // Pobierz obiekt CountryInfo na podstawie nazwy kraju
        currentCountryInfo = Passport.GetCountry(Passport.CurrentCountry);

        if (currentCountryInfo == null)
        {
            Debug.LogError($"PuzzleManager: Nie znaleziono danych dla kraju o nazwie '{Passport.CurrentCountry}'. SprawdŸ plik JSON i implementacjê GetCountry.");
            return;
        }

        // --- WA¯NA ZMIANA TUTAJ ---
        // Pamiêtaj, aby ZAST¥PIÆ 'TwojaNazwaPolaKraju' na rzeczywist¹ nazwê pola
        // w Twojej klasie CountryInfo, które przechowuje nazwê kraju (np. 'name', 'countryName', 'country').
        string countryNameForLog = "BrakNazwyKraju";
        // if (!string.IsNullOrEmpty(currentCountryInfo.TwojaNazwaPolaKraju)) // Odkomentuj i wstaw poprawn¹ nazwê pola
        // {
        //     countryNameForLog = currentCountryInfo.TwojaNazwaPolaKraju; // Odkomentuj i wstaw poprawn¹ nazwê pola
        // }
        // Przyk³adowo, jeœli masz 'public string name;' w CountryInfo:
        if (currentCountryInfo.nazwa != null) // ZMIEN TO NA TWOJ¥ W£AŒCIW¥ NAZWÊ POLA W CountryInfo
        {
            countryNameForLog = currentCountryInfo.nazwa; // ZMIEN TO NA TWOJ¥ W£AŒCIW¥ NAZWÊ POLA W CountryInfo
        }


        if (string.IsNullOrEmpty(currentCountryInfo.obrazekPuzzle)) // Sprawdzamy pole z obrazkiem puzzli
        {
            Debug.LogError($"PuzzleManager: W danych kraju '{countryNameForLog}' brakuje nazwy pliku obrazka puzzli ('obrazekPuzzle').");
            return;
        }

        // U¿ywamy nazwy pliku z currentCountryInfo.obrazekPuzzle
        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(currentCountryInfo.obrazekPuzzle);
        string fullPathInResources = "Puzzle - zdjêcia/" + imagePathWithoutExtension;

        jigsawTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (jigsawTexture == null)
        {
            Debug.LogError($"PuzzleManager: NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}'. " +
                           $"Upewnij siê, ¿e plik jest w 'Assets/Resources/Puzzle - zdjêcia/' " +
                           $"i ma poprawne ustawienia ('Texture Type: Sprite (2D and UI)', 'Read/Write Enabled' zaznaczone w zaawansowanych).");
        }
        else
        {
            Debug.Log($"PuzzleManager: Pomyœlnie za³adowano teksturê puzzli: {jigsawTexture.name}");
            StartGame(jigsawTexture); // Rozpocznij grê z za³adowan¹ tekstur¹
        }
    }

    public void StartGame(Texture2D jigsawTexture)
    {
        // Ukryj UI, które nie jest ju¿ potrzebne
        if (fullPuzzleImageDisplay != null)
        {
            fullPuzzleImageDisplay.gameObject.SetActive(false); // Ukryj ca³y obrazek (UI Image)
            Debug.Log("PuzzleManager: Ukryto fullPuzzleImageDisplay.");
        }
        if (levelSelectPanel != null)
        {
            levelSelectPanel.gameObject.SetActive(false); // Ukryj panel wyboru poziomu (jeœli istnieje)
            Debug.Log("PuzzleManager: Ukryto levelSelectPanel.");
        }
        if (puzzleText != null)
        {
            puzzleText.gameObject.SetActive(false); // Ukryj tekst "U³ó¿ puzzle" (jeœli istnieje)
            Debug.Log("PuzzleManager: Ukryto puzzleText.");
        }

        pieces = new List<Transform>();

        // U¿ywamy sta³ych wymiarów siatki puzzli.
        dimensions = new Vector2Int(difficulty, difficulty); // Np. 4x4, 5x5 itp.

        Debug.Log($"[PuzzleManager] Wymiary siatki puzzli (dimensions): {dimensions.x}x{dimensions.y}");

        // Oblicz rozmiar pojedynczego kawa³ka w jednostkach Unity.
        // Ka¿dy kawa³ek bêdzie mia³ 1x1 jednostkê Unity, a nastêpnie zostanie przeskalowany przez pieceScaleMultiplier.
        pieceWidthWorld = 1f;
        pieceHeightWorld = 1f;

        CreateJigsawPieces(jigsawTexture);

        // Dodaj tutaj logikê rozrzucania/mieszania kawa³ków puzzli, jeœli jeszcze jej nie masz
        // ShufflePieces(); 
    }

    void CreateJigsawPieces(Texture2D jigsawTexture)
    {
        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                // Instantiate zwraca GameObject, potrzebujemy jego Transform
                // Upewnij siê, ¿e piecePrefab jest przypisany w Inspektorze!
                Transform piece = Instantiate(piecePrefab, gameHolder).transform;

                // Obliczanie pozycji kawa³ka tak, aby ca³oœæ by³a wyœrodkowana wokó³ (0,0)
                // Offset (przesuniêcie) dla wyœrodkowania ca³ego obrazka
                // dimensions.x i dimensions.y to liczba kawa³ków.
                float offsetX = -(dimensions.x / 2f - 0.5f) * pieceWidthWorld;
                float offsetY = -(dimensions.y / 2f - 0.5f) * pieceHeightWorld;

                piece.localPosition = new Vector3(
                    (col * pieceWidthWorld) + offsetX,      // Pozycja X
                    (row * pieceHeightWorld) + offsetY,     // Pozycja Y
                    -1f); // Pozycja Z (bli¿ej kamery ni¿ 0,0,0)

                // Skalowanie kawa³ka - u¿ywamy pieceWidthWorld i pieceHeightWorld
                // plus globalny mno¿nik skali, aby dostosowaæ widoczny rozmiar
                piece.localScale = new Vector3(
                    pieceWidthWorld * pieceScaleMultiplier,
                    pieceHeightWorld * pieceScaleMultiplier,
                    1f); // Skala Z powinna byæ 1 dla Quad'a

                piece.name = $"Piece {col}x{row}";
                pieces.Add(piece);

                Debug.Log($"[PuzzleManager] Utworzono {piece.name} - Pozycja lokalna: {piece.localPosition}, Skala lokalna: {piece.localScale}");

                // --- UV Mapping ---
                // Kluczowe: upewnij siê, ¿e Read/Write Enabled jest zaznaczone dla tekstury!
                // Obliczanie wspó³rzêdnych UV dla tego fragmentu tekstury
                float uvWidth = 1f / dimensions.x; // Szerokoœæ fragmentu UV (np. 1/4 = 0.25 dla 4 kolumn)
                float uvHeight = 1f / dimensions.y; // Wysokoœæ fragmentu UV (np. 1/4 = 0.25 dla 4 rzêdów)

                // Wspó³rzêdne UV dla czterech wierzcho³ków Quad'a
                Vector2[] uv = new Vector2[4];
                // UV dla dolnego-lewego wierzcho³ka (uv[0] wierzcho³ek 0)
                uv[0] = new Vector2(uvWidth * col, uvHeight * row);
                // UV dla dolnego-prawego wierzcho³ka (uv[1] wierzcho³ek 1)
                uv[1] = new Vector2(uvWidth * (col + 1), uvHeight * row);
                // UV dla górnego-lewego wierzcho³ka (uv[2] wierzcho³ek 2)
                uv[2] = new Vector2(uvWidth * col, uvHeight * (row + 1));
                // UV dla górnego-prawego wierzcho³ka (uv[3] wierzcho³ek 3)
                uv[3] = new Vector2(uvWidth * (col + 1), uvHeight * (row + 1));

                // Przypisz nasze nowe UV do siatki.
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                // Upewnij siê, ¿e MeshFilter i jego mesh nie s¹ null
                if (mesh != null)
                {
                    mesh.uv = uv;
                }
                else
                {
                    Debug.LogError($"[PuzzleManager] B³¹d: Brak MeshFilter lub jego mesh na Piece {piece.name}. SprawdŸ prefab!");
                }

                // Ustaw teksturê na materiale kawa³ka
                // Upewnij siê, ¿e PiecePrefab u¿ywa materia³u z shaderem Unlit/Texture, a nie Sprites-Default!
                // Wa¿ne: u¿ywamy .material, aby uzyskaæ instancjê materia³u dla tego konkretnego obiektu,
                // co zapobiega modyfikowaniu wspólnego materia³u dla wszystkich prefabów.
                MeshRenderer meshRenderer = piece.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.material != null)
                {
                    meshRenderer.material.SetTexture("_MainTex", jigsawTexture);
                    Debug.Log($"[PuzzleManager] Przypisano teksturê do {piece.name}. Materia³: {meshRenderer.material.name}");
                }
                else
                {
                    Debug.LogError($"[PuzzleManager] B³¹d: Brak MeshRenderer lub Material na Piece {piece.name}. SprawdŸ prefab PiecePrefab!");
                }
            }
        }
    }

    // Mo¿esz dodaæ tutaj metodê do mieszania kawa³ków, np.
    // void ShufflePieces() { ... }
}