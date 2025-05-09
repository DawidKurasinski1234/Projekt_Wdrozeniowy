using UnityEngine;
using System.Collections.Generic;

public class CountryLoader : MonoBehaviour
{
    public List<CountryInfo> wszystkieKraje;

    void Start()
    {
        WczytajKraje();

        //test czy wszystkie informacje sie poberaja
        //if (wszystkieKraje != null)
        //{
        //    foreach (var kraj in wszystkieKraje)
        //    {
        //        Debug.Log("Kraj: " + kraj.nazwa + ", Stolica: " + kraj.stolica + ", Symbol: " + kraj.symbol);
        //    }
        //}
        //else
        //{
        //    Debug.LogError("Lista kraj�w jest pusta lub nie zosta�a wczytana!");
        //}
    }


    void WczytajKraje()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/countries");
        if (jsonFile != null)
        {
            wszystkieKraje = new List<CountryInfo>(JsonHelper.FromJson<CountryInfo>(jsonFile.text));
           //nastepna linijke mozna usunac zeby nie wyswietla�o sie w konsoli
            Debug.Log("Wczytano " + wszystkieKraje.Count + " kraj�w.");
        }
        else
        {
            //to tez mozna usunac
            Debug.LogError("Nie znaleziono pliku countries.json w Resources/Data");
        }
    }

}
