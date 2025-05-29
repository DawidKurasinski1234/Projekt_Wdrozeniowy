using UnityEngine;
using UnityEngine.UI; // Dla Image i Button
// using TMPro; // Je�li chcesz wy�wietli� nazw� symbolu (opcjonalnie)

public class SymbolButton : MonoBehaviour
{
    public Image symbolImage; // Przypisz komponent Image tego przycisku w Inspektorze
    // public TextMeshProUGUI symbolNameText; // Opcjonalnie, je�li chcesz wy�wietli� nazw� pod obrazkiem

    private Sprite assignedSymbol;
    private SymbolKrajuGameManager gameManager; 

    void Start()
    {
        // Upewnij si�, �e komponent Image jest przypisany
        if (symbolImage == null)
        {
            symbolImage = GetComponent<Image>();
            if (symbolImage == null)
            {
                Debug.LogError("SymbolButton potrzebuje komponentu Image!", this.gameObject);
                return;
            }
        }
        // Dodaj listener do klikni�cia przycisku
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
            symbolImage.enabled = true; // Poka� obrazek
            // if (symbolNameText != null) symbolNameText.text = assignedSymbol.name; // Opcjonalnie
        }
        else
        {
            // Je�li symbol jest null (np. placeholder lub b��d danych)
            symbolImage.enabled = false; // Ukryj obrazek
            // if (symbolNameText != null) symbolNameText.text = "";
            Debug.LogWarning("Przycisk otrzyma� pusty symbol (null).", this.gameObject);
        }
    }

    void OnClick()
    {
        if (gameManager != null && assignedSymbol != null)
        {
            // Wywo�anie metody CheckAnswer na obiekcie gameManager (typu SymbolKrajuGameManager)
            gameManager.CheckAnswer(assignedSymbol);
        }
        else if (assignedSymbol == null)
        {
            Debug.LogWarning("Klikni�to przycisk bez przypisanego symbolu.", this.gameObject);
        }
    }
}