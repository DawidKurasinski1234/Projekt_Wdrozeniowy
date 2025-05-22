// Nazwa pliku: CountryInfo.cs (jeœli tworzysz osobny plik)

using System; // Potrzebne dla atrybutu Serializable

[Serializable] // To jest wa¿ne, aby JsonHelper móg³ j¹ deserializowaæ
public class CountryInfo
{
    public string nazwa;
    public string stolica;
    public string powitanie;
    public string symbol;
    public string symbolPlik;
    public string obrazekPuzzle; // To pole jest kluczowe dla puzzli
}