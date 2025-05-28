using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Potrzebne dla Image
using System.IO; // Potrzebne dla Path.GetFileNameWithoutExtension
using TMPro; // Dodane, je�li u�ywasz TextMeshPro dla puzzleText

public class PuzzleManager : MonoBehaviour
{
    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4; // Okre�la liczb� kawa�k�w w kr�tszym wymiarze
    [SerializeField] private Transform gameHolder; // Rodzic dla wszystkich kawa�k�w puzzli
    [SerializeField] private GameObject piecePrefab; // Prefab pojedynczego kawa�ka puzzli (powinien mie� Quad, MeshFilter, MeshRenderer)

    [Tooltip("Mno�nik skali dla ka�dego kawa�ka puzzli. U�yj, aby dostosowa� widoczny rozmiar.")]
    [SerializeField] private float pieceScaleMultiplier = 100f; // Domy�lna warto��, mo�esz dostosowa� w Inspektorze

    [Header("UI Elements")]
    [SerializeField] private Image fullPuzzleImageDisplay; // Obrazek UI wy�wietlaj�cy ca�y obrazek przed poci�ciem
    [SerializeField] private TextMeshProUGUI puzzleText; // Tekst "U�� puzzle" lub podobny (zak�adam TextMeshPro)
    [SerializeField] private GameObject levelSelectPanel; // Panel, kt�ry chcesz ukry� po starcie gry (np. panel z wyborem poziomu/kraj�w)

    private List<Transform> pieces;
    private Vector2Int dimensions; // Wymiary siatki puzzli (np. 4x4, 5x5)
    private float pieceWidthWorld; // Szeroko�� pojedynczego kawa�ka w jednostkach Unity
    private float pieceHeightWorld; // Wysoko�� pojedynczego kawa�ka w jednostkach Unity

    void Start()
    {
        // Sprawd�, czy GameHolder i PiecePrefab s� przypisane. Je�li nie, zg�o� b��d i przerwij.
        if (gameHolder == null)
        {
            Debug.LogError("PuzzleManager: Game Holder nie jest przypisany w Inspektorze! Przypisz obiekt nadrz�dny dla kawa�k�w puzzli.");
            return;
        }
        if (piecePrefab == null)
        {
            // Ten b��d by� widoczny na screenie
            Debug.LogError("PuzzleManager: Piece Prefab nie jest przypisany w Inspektorze! Przypisz prefab pojedynczego kawa�ka puzzli.");
            return;
        }

        // Upewnij si�, �e Passport jest zainicjowany.
        Passport.Init();

        // Rozpoczynamy proces �adowania i startu puzzli
        LoadAndStartPuzzle();
    }

    private void LoadAndStartPuzzle()
    {
        Texture2D jigsawTexture = null;
        CountryInfo currentCountryInfo = null; // Zmienna do przechowywania obiektu CountryInfo

        // Sprawd�, czy Passport.CurrentCountry (string) jest ustawiony
        if (string.IsNullOrEmpty(Passport.CurrentCountry))
        {
            Debug.LogError("PuzzleManager: Brak wylosowanej nazwy kraju w Passport.CurrentCountry (string). Nie mo�na rozpocz�� gry w puzzle.");
            return;
        }

        // Pobierz obiekt CountryInfo na podstawie nazwy kraju
        currentCountryInfo = Passport.GetCountry(Passport.CurrentCountry);

        if (currentCountryInfo == null)
        {
            Debug.LogError($"PuzzleManager: Nie znaleziono danych dla kraju o nazwie '{Passport.CurrentCountry}'. Sprawd� plik JSON i implementacj� GetCountry.");
            return;
        }

        // --- WA�NA ZMIANA TUTAJ ---
        // Pami�taj, aby ZAST�PI� 'TwojaNazwaPolaKraju' na rzeczywist� nazw� pola
        // w Twojej klasie CountryInfo, kt�re przechowuje nazw� kraju (np. 'name', 'countryName', 'country').
        string countryNameForLog = "BrakNazwyKraju";
        // if (!string.IsNullOrEmpty(currentCountryInfo.TwojaNazwaPolaKraju)) // Odkomentuj i wstaw poprawn� nazw� pola
        // {
        //     countryNameForLog = currentCountryInfo.TwojaNazwaPolaKraju; // Odkomentuj i wstaw poprawn� nazw� pola
        // }
        // Przyk�adowo, je�li masz 'public string name;' w CountryInfo:
        if (currentCountryInfo.nazwa != null) // ZMIEN TO NA TWOJ� W�A�CIW� NAZW� POLA W CountryInfo
        {
            countryNameForLog = currentCountryInfo.nazwa; // ZMIEN TO NA TWOJ� W�A�CIW� NAZW� POLA W CountryInfo
        }


        if (string.IsNullOrEmpty(currentCountryInfo.obrazekPuzzle)) // Sprawdzamy pole z obrazkiem puzzli
        {
            Debug.LogError($"PuzzleManager: W danych kraju '{countryNameForLog}' brakuje nazwy pliku obrazka puzzli ('obrazekPuzzle').");
            return;
        }

        // U�ywamy nazwy pliku z currentCountryInfo.obrazekPuzzle
        string imagePathWithoutExtension = Path.GetFileNameWithoutExtension(currentCountryInfo.obrazekPuzzle);
        string fullPathInResources = "Puzzle - zdj�cia/" + imagePathWithoutExtension;

        jigsawTexture = Resources.Load<Texture2D>(fullPathInResources);

        if (jigsawTexture == null)
        {
            Debug.LogError($"PuzzleManager: NIE ZNALEZIONO obrazka puzzli: '{fullPathInResources}'. " +
                           $"Upewnij si�, �e plik jest w 'Assets/Resources/Puzzle - zdj�cia/' " +
                           $"i ma poprawne ustawienia ('Texture Type: Sprite (2D and UI)', 'Read/Write Enabled' zaznaczone w zaawansowanych).");
        }
        else
        {
            Debug.Log($"PuzzleManager: Pomy�lnie za�adowano tekstur� puzzli: {jigsawTexture.name}");
            StartGame(jigsawTexture); // Rozpocznij gr� z za�adowan� tekstur�
        }
    }

    public void StartGame(Texture2D jigsawTexture)
    {
        // Ukryj UI, kt�re nie jest ju� potrzebne
        if (fullPuzzleImageDisplay != null)
        {
            fullPuzzleImageDisplay.gameObject.SetActive(false); // Ukryj ca�y obrazek (UI Image)
            Debug.Log("PuzzleManager: Ukryto fullPuzzleImageDisplay.");
        }
        if (levelSelectPanel != null)
        {
            levelSelectPanel.gameObject.SetActive(false); // Ukryj panel wyboru poziomu (je�li istnieje)
            Debug.Log("PuzzleManager: Ukryto levelSelectPanel.");
        }
        if (puzzleText != null)
        {
            puzzleText.gameObject.SetActive(false); // Ukryj tekst "U�� puzzle" (je�li istnieje)
            Debug.Log("PuzzleManager: Ukryto puzzleText.");
        }

        pieces = new List<Transform>();

        // U�ywamy sta�ych wymiar�w siatki puzzli.
        dimensions = new Vector2Int(difficulty, difficulty); // Np. 4x4, 5x5 itp.

        Debug.Log($"[PuzzleManager] Wymiary siatki puzzli (dimensions): {dimensions.x}x{dimensions.y}");

        // Oblicz rozmiar pojedynczego kawa�ka w jednostkach Unity.
        // Ka�dy kawa�ek b�dzie mia� 1x1 jednostk� Unity, a nast�pnie zostanie przeskalowany przez pieceScaleMultiplier.
        pieceWidthWorld = 1f;
        pieceHeightWorld = 1f;

        CreateJigsawPieces(jigsawTexture);

        // Dodaj tutaj logik� rozrzucania/mieszania kawa�k�w puzzli, je�li jeszcze jej nie masz
        // ShufflePieces(); 
    }

    void CreateJigsawPieces(Texture2D jigsawTexture)
    {
        for (int row = 0; row < dimensions.y; row++)
        {
            for (int col = 0; col < dimensions.x; col++)
            {
                // Instantiate zwraca GameObject, potrzebujemy jego Transform
                // Upewnij si�, �e piecePrefab jest przypisany w Inspektorze!
                Transform piece = Instantiate(piecePrefab, gameHolder).transform;

                // Obliczanie pozycji kawa�ka tak, aby ca�o�� by�a wy�rodkowana wok� (0,0)
                // Offset (przesuni�cie) dla wy�rodkowania ca�ego obrazka
                // dimensions.x i dimensions.y to liczba kawa�k�w.
                float offsetX = -(dimensions.x / 2f - 0.5f) * pieceWidthWorld;
                float offsetY = -(dimensions.y / 2f - 0.5f) * pieceHeightWorld;

                piece.localPosition = new Vector3(
                    (col * pieceWidthWorld) + offsetX,      // Pozycja X
                    (row * pieceHeightWorld) + offsetY,     // Pozycja Y
                    -1f); // Pozycja Z (bli�ej kamery ni� 0,0,0)

                // Skalowanie kawa�ka - u�ywamy pieceWidthWorld i pieceHeightWorld
                // plus globalny mno�nik skali, aby dostosowa� widoczny rozmiar
                piece.localScale = new Vector3(
                    pieceWidthWorld * pieceScaleMultiplier,
                    pieceHeightWorld * pieceScaleMultiplier,
                    1f); // Skala Z powinna by� 1 dla Quad'a

                piece.name = $"Piece {col}x{row}";
                pieces.Add(piece);

                Debug.Log($"[PuzzleManager] Utworzono {piece.name} - Pozycja lokalna: {piece.localPosition}, Skala lokalna: {piece.localScale}");

                // --- UV Mapping ---
                // Kluczowe: upewnij si�, �e Read/Write Enabled jest zaznaczone dla tekstury!
                // Obliczanie wsp�rz�dnych UV dla tego fragmentu tekstury
                float uvWidth = 1f / dimensions.x; // Szeroko�� fragmentu UV (np. 1/4 = 0.25 dla 4 kolumn)
                float uvHeight = 1f / dimensions.y; // Wysoko�� fragmentu UV (np. 1/4 = 0.25 dla 4 rz�d�w)

                // Wsp�rz�dne UV dla czterech wierzcho�k�w Quad'a
                Vector2[] uv = new Vector2[4];
                // UV dla dolnego-lewego wierzcho�ka (uv[0] wierzcho�ek 0)
                uv[0] = new Vector2(uvWidth * col, uvHeight * row);
                // UV dla dolnego-prawego wierzcho�ka (uv[1] wierzcho�ek 1)
                uv[1] = new Vector2(uvWidth * (col + 1), uvHeight * row);
                // UV dla g�rnego-lewego wierzcho�ka (uv[2] wierzcho�ek 2)
                uv[2] = new Vector2(uvWidth * col, uvHeight * (row + 1));
                // UV dla g�rnego-prawego wierzcho�ka (uv[3] wierzcho�ek 3)
                uv[3] = new Vector2(uvWidth * (col + 1), uvHeight * (row + 1));

                // Przypisz nasze nowe UV do siatki.
                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                // Upewnij si�, �e MeshFilter i jego mesh nie s� null
                if (mesh != null)
                {
                    mesh.uv = uv;
                }
                else
                {
                    Debug.LogError($"[PuzzleManager] B��d: Brak MeshFilter lub jego mesh na Piece {piece.name}. Sprawd� prefab!");
                }

                // Ustaw tekstur� na materiale kawa�ka
                // Upewnij si�, �e PiecePrefab u�ywa materia�u z shaderem Unlit/Texture, a nie Sprites-Default!
                // Wa�ne: u�ywamy .material, aby uzyska� instancj� materia�u dla tego konkretnego obiektu,
                // co zapobiega modyfikowaniu wsp�lnego materia�u dla wszystkich prefab�w.
                MeshRenderer meshRenderer = piece.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.material != null)
                {
                    meshRenderer.material.SetTexture("_MainTex", jigsawTexture);
                    Debug.Log($"[PuzzleManager] Przypisano tekstur� do {piece.name}. Materia�: {meshRenderer.material.name}");
                }
                else
                {
                    Debug.LogError($"[PuzzleManager] B��d: Brak MeshRenderer lub Material na Piece {piece.name}. Sprawd� prefab PiecePrefab!");
                }
            }
        }
    }

    // Mo�esz doda� tutaj metod� do mieszania kawa�k�w, np.
    // void ShufflePieces() { ... }
}