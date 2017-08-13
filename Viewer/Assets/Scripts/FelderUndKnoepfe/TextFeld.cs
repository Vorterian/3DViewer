using UnityEngine;
using UnityEngine.UI;

public class TextFeld : MonoBehaviour
{
    private DataSingleton _datenAblage;
    private InputField eingabe;

    void Start()
    {
        _datenAblage = DataSingleton.GetInstanz(); ;
        eingabe = gameObject.GetComponent<InputField>();
        var eingabeEreignis = new InputField.SubmitEvent();
        eingabeEreignis.AddListener(SubmitName);
        eingabe.onEndEdit = eingabeEreignis;
        StartWert(name);



    }

    private void StartWert(string name)
    {
        if (name.Equals("modelldatei"))
        {
            var pfad = "c:/android/Daten10Leiter";
            eingabe.text = pfad;
            _datenAblage.SetDateiPfad(pfad);
        }
        else
        {
            string datum = "2017";
            eingabe.text = datum;
            _datenAblage.SetAblagePfad(datum);

        }
    }

    private void SubmitName(string text)
    {
        if (name.Equals("modelldatei"))
        {
            _datenAblage.SetDateiPfad(text);
        }
        else
        {
            _datenAblage.SetAblagePfad(text);
        }
    }
}