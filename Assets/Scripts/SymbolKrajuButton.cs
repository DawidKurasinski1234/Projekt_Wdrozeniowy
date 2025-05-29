using UnityEngine;
using UnityEngine.UI; // Dla Image i Button
// using TMPro; // Jeœli chcesz wyœwietliæ nazwê symbolu (opcjonalnie)

public class SymbolButton : MonoBehaviour
{
    public Image symbolImage; // Przypisz komponent Image tego przycisku w Inspektorze
    // public TextMeshProUGUI symbolNameText; // Opcjonalnie, jeœli chcesz wyœwietliæ nazwê pod obrazkiem

    private Sprite assignedSymbol;
    private SymbolKrajuGameManager gameManager; 

    void Start()
    {
        // Upewnij siê, ¿e komponent Image jest przypisany
        if (symbolImage == null)
        {
            symbolImage = GetComponent<Image>();
            if (symbolImage == null)
            {
                Debug.LogError("SymbolButton potrzebuje komponentu Image!", this.gameObject);
                return;
            }
        }
        // Dodaj listener do klikniêcia przycisku
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Metoda Initialize teraz przyjmuje SymbolKrajuGameManager
    public void Initialize(Sprite symbol, SymbolKrajuGameManager manager) // <--- ZMIANA TUTAJ
    {
        assignedSymbol = symbol;
        gameManager = manager;

        if (symbol != null)
        {
            symbolImage.sprite = assignedSymbol;
            symbolImage.enabled = true; // Poka¿ obrazek
            // if (symbolNameText != null) symbolNameText.text = assignedSymbol.name; // Opcjonalnie
        }
        else
        {
            // Jeœli symbol jest null (np. placeholder lub b³¹d danych)
            symbolImage.enabled = false; // Ukryj obrazek
            // if (symbolNameText != null) symbolNameText.text = "";
            Debug.LogWarning("Przycisk otrzyma³ pusty symbol (null).", this.gameObject);
        }
    }

    void OnClick()
    {
        if (gameManager != null && assignedSymbol != null)
        {
            // Wywo³anie metody CheckAnswer na obiekcie gameManager (typu SymbolKrajuGameManager)
            gameManager.CheckAnswer(assignedSymbol);
        }
        else if (assignedSymbol == null)
        {
            Debug.LogWarning("Klikniêto przycisk bez przypisanego symbolu.", this.gameObject);
        }
    }
}