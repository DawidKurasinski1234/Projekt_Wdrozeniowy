using UnityEngine;
using UnityEngine.UI;

public class CityButton : MonoBehaviour
{
    public string cityName;
    private GameManager manager;

    public void Initialize(string name, GameManager gm)
    {
        cityName = name;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
        manager = gm;
    }

    public void OnClick()
    {
        manager.CheckAnswer(cityName);
    }
}
