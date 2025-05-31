using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Passport {
    private struct Entry {
        public bool        visited;
        public CountryInfo country;   
    }
    
    public static int      Points;
    public static string   CurrentCountry;
    
    private static Entry[] m_entries;
    private static int     m_visited;


    //dodane na potrzeby innych skryptów
    public static int GetEntriesCount()
    {
        if (m_entries == null)
        {
            return 0;
        }
        return m_entries.Length;
    }
    //

    public static CountryInfo CurrentSelectedCountry { get; internal set; }

    private static int
    GetEntryIndex(string countryName)
    {
        for (int i = 0; i < m_entries.Length; i++) {
            if (m_entries[i].country.nazwa == countryName)
                return i;
        }
        return -1;
    }

    public static void
    VisitCountry(string countryName)
    {
        int idx = GetEntryIndex(countryName);
        if (idx != -1) {
            m_entries[idx].visited = true;
            m_visited++;
        }
    }

    public static List<CountryInfo>
    VisitedCountries()
    {
        var infos = new List<CountryInfo>();
        foreach (Entry entry in m_entries) {
            if (entry.visited)
                infos.Add(entry.country);
        }
        return infos;
    }

    public static CountryInfo
    GetCountry(string countryName)
    {
        int idx = GetEntryIndex(countryName);
        if (idx != -1)
            return m_entries[idx].country;
        return null;
    }
  
    public static string
    ChooseCountry() 
    {
        if (m_visited == m_entries.Length)
            return null;
        
        Entry? entry = null;
        var rng = new Random();
        do {
            int index = rng.Next(0, m_entries.Length);
            if (!m_entries[index].visited)
                entry = m_entries[index];
        } while (entry == null);
        return entry.Value.country.nazwa;
    }
    
    public static void
    Init()
    {
        if (m_entries != null)
            return; // Already initialized
        
        TextAsset asset  = Resources.Load<TextAsset>("countries");
        var    countries = JsonHelper.FromJson<CountryInfo>(asset.text);
        
        Points    = 0;
        m_entries = new Entry[countries.Length];
        for (int i = 0; i < countries.Length; i++) {
            m_entries[i] = new Entry {
                visited = false,
                country = countries[i]
            };
        }
    }
}
