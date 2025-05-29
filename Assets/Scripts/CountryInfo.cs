// Nazwa pliku: CountryInfo.cs (je�li tworzysz osobny plik)

using System; // Potrzebne dla atrybutu Serializable
using System.Collections.Generic; //dla listy

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
    
    public string
    SymbolResourceName()
    {
        string ret = $"Kraje - Symbole/{symbolPlik}";
        return ret.Substring(0, ret.LastIndexOf('.') - 1);
    }
}