using System;
using TMPro;
using UnityEngine;

public class Greeting: MonoBehaviour {
    public TMP_Text greetingLabel, countryLabel;

    private void Start()
    {
        if (Passport.CurrentCountry == null) {
            countryLabel.text  = "Odwiedziłeś/aś już wszystkie kraje w grze!";
            greetingLabel.text = "";
            return;
        }
        
        var country = Passport.GetCountry(Passport.CurrentCountry);
        greetingLabel.text = country.powitanie;
        countryLabel.text  = country.nazwa;
    }
}
