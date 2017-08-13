using System;
using UnityEngine;
using UnityEngine.UI;

public class ToogleKnopf : MonoBehaviour
{
    private DataSingleton _datenAblage;

    void Start()
    {
        _datenAblage = DataSingleton.GetInstanz(); ;

        var toogleKnopf = gameObject.GetComponent<Toggle>();
        toogleKnopf.onValueChanged.AddListener(VeraenderteAuswahl);
        toogleKnopf.isOn = false;
        Initialisiere();
    }

    private void Initialisiere()
    {
        
        _datenAblage.SetAbfrageNeuLaden(false);
        _datenAblage.SetAbfrageDreiecksnetz(false);
        _datenAblage.SetAbfrageWirdKFWertGefordert(false);
    }

    private void VeraenderteAuswahl(bool eingabe)
    {
        if(name.Equals("neuLaden"))
        {
            _datenAblage.SetAbfrageNeuLaden(eingabe);
        }
        else if (name.Equals("dreiecke"))
        {
            _datenAblage.SetAbfrageDreiecksnetz(eingabe);
        }
        else
        {
            _datenAblage.SetAbfrageWirdKFWertGefordert(eingabe);
        }
    }
}