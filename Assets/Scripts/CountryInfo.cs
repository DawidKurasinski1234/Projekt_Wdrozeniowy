// Nazwa pliku: CountryInfo.cs (je�li tworzysz osobny plik)

using System; // Potrzebne dla atrybutu Serializable
using System.Collections.Generic; //dla listy
using Path = System.IO.Path;

[Serializable] // To jest wa�ne, aby JsonHelper m�g� j� deserializowa�
public class CountryInfo
{
    public string nazwa;
    public string stolica;
    public List<string> miasta;
    public string powitanie;
    public string symbol;
    public string symbolPlik;
    public string obrazekPuzzle; // To pole jest kluczowe dla puzzli
    
    private string
    GetResourceName(string file, string directory)
    {
        return directory + "/" + Path.GetFileNameWithoutExtension(file);
    }
    
    public string SymbolResourceName => GetResourceName(symbolPlik,    "Kraje - Symbole");
    public string PuzzleResourceName => GetResourceName(obrazekPuzzle, "PuzzleImages");
}