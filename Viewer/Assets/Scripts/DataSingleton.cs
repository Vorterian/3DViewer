using UnityEngine;
using System.IO;
using System.Collections.Generic;

public sealed class DataSingleton
{

    //TODO :: Logger als eigene Klasse implementieren, doppelter Code ist noch vorhanden

    // Das Singleton wird mit Daten aus der Benutzeroberfläche unter "Einstellungen" mit Daten gefüllt.
    // Mehrere Threads referenzieren ihre lokalen Variablen "_datenAblage" mit dieser Instanz.
    // Wenn auf die Variablen der Klasse schreibend zugegriffen wird, geschieht dies nie durch mehrere Threads gleichzeitig auf einem Speicherplatz
    // Die Datenstruktur ist aus diesem Grund so angelegt, dass keine Locks verwendet werden.
    

    private string _dateiPfad;                              // Speicherort der Modelldatei
    private string _nameDesSpeicherOrtes;                   // Name des Speicherortes
    private int _skalierung;                                // Skalierungsfaktor zur Überhöhung der Geometrie
    private bool _neuLaden;                                 // Wenn ein Ordner namentlich bereits exisitert, kann der Inhalt überschriebenw erden
    private bool _wirdEinePunktwolkeGefordert;              // Abfrage ob eine Punktewolke angefordert wird
    private bool _wirdEinDreiecksNetzGefordert;             // Abfrage ob ein Dreiecksnetz angeforfert wird
    private bool _wirdEineKFWertDarstellungGefordert;       // Abfrage ob eine Darstellung der kF-Werte erfolgen soll

    // Oberkanten der Modellleiter
    private List<GameObject> _gwLeiter1OK = new List<GameObject>();
    private List<GameObject> _gwLeiter2OK = new List<GameObject>();
    private List<GameObject> _gwLeiter3OK = new List<GameObject>();
    private List<GameObject> _gwLeiter4OK = new List<GameObject>();
    private List<GameObject> _gwLeiter5OK = new List<GameObject>();
    private List<GameObject> _gwLeiter6OK = new List<GameObject>();
    private List<GameObject> _gwLeiter7OK = new List<GameObject>();
    private List<GameObject> _gwLeiter8OK = new List<GameObject>();
    private List<GameObject> _gwLeiter9OK = new List<GameObject>();
    private List<GameObject> _gwLeiter10OK = new List<GameObject>();
    private List<GameObject> _gwLeiter11OK = new List<GameObject>();
    private List<GameObject> _gwLeiter12OK = new List<GameObject>();

    //Unterkanten der Modellleiter
    private List<GameObject> _gwLeiter1UK = new List<GameObject>();
    private List<GameObject> _gwLeiter2UK = new List<GameObject>();
    private List<GameObject> _gwLeiter3UK = new List<GameObject>();
    private List<GameObject> _gwLeiter4UK = new List<GameObject>();
    private List<GameObject> _gwLeiter5UK = new List<GameObject>();
    private List<GameObject> _gwLeiter6UK = new List<GameObject>();
    private List<GameObject> _gwLeiter7UK = new List<GameObject>();
    private List<GameObject> _gwLeiter8UK = new List<GameObject>();
    private List<GameObject> _gwLeiter9UK = new List<GameObject>();
    private List<GameObject> _gwLeiter10UK = new List<GameObject>();
    private List<GameObject> _gwLeiter11UK = new List<GameObject>();
    private List<GameObject> _gwLeiter12UK = new List<GameObject>();


    private List<int> _listeAllerDreiecksPunkte;                        // Liste aller Stützstellkombinationen der im Modell vorkommenden Dreiecke. 3 hintereinander folgende
                                                                        // Nummern bilden jeweils das Dreieck.

    private int _anzKnotenProLeiter;                                    // Anzahl der Stützstellen im Modell
    private int _anzLeiter;                                             // Anzahl der im Daten10 aufkommenden Modellleiter
    private Vector3[] _vectorDerLeiterOberkanten;                       // X,Y,Z Vector der Oberkanten
    private Vector3[] _vectorDerLeiterUnterkanten;                      // X,Y,Z Vector der Unterkanten
    private Vector3 _kleinsteKoordinatenUndHoehe;
    private StreamWriter _logger;                                       // Referenz auf den Streamwriter
    private StreamReader _daten10Leser;                                 // Referenz auf den Streamreader
    private Color[] farbe;                                              // Array zur Farbdarstellung des jeweiligen Netzes
    private Material _standardMaterial;                                 // Material mit dem ein Netz als Standard belegt wird
    private Material _netzMaterial;                                     // Material welches zusätzlich hinzugefügt werden kann
    private List<List<int>> harfenEintraege;

    private static DataSingleton Instanzierung = null;                  // Instanzierung des Singeletons
    private static readonly object ReadLock = new object();



    DataSingleton()
    {
        SetStandardMaterial();
        SetNetzMaterial();
    }


    public List<List<int>> HarfenEintraege
    {
        get
        {
            return harfenEintraege;
        }

        set
        {
            harfenEintraege = value;
        }
    }

    public void SetKleinsteKoordinate(Vector3 kleinsteKoordinate)
    {
        _kleinsteKoordinatenUndHoehe = kleinsteKoordinate;
    }

    public Vector3 GetKleinsteKoordinate()
    {
        return _kleinsteKoordinatenUndHoehe;
    }

    public void SetLeiterOberkantenObject(int leiterNummer, GameObject leiter)
    {

        switch (leiterNummer)
        {
            case 0:
                _gwLeiter1OK.Add(leiter);
                break;
            case 1:
                _gwLeiter2OK.Add(leiter);
                break;
            case 2:
                _gwLeiter3OK.Add(leiter);
                break;
            case 3:
                _gwLeiter4OK.Add(leiter);
                break;
            case 4:
                _gwLeiter5OK.Add(leiter);
                break;
            case 5:
                _gwLeiter6OK.Add(leiter);
                break;
            case 6:
                _gwLeiter7OK.Add(leiter);
                break;
            case 7:
                _gwLeiter8OK.Add(leiter);
                break;
            case 8:
                _gwLeiter9OK.Add(leiter);
                break;
            case 9:
                _gwLeiter10OK.Add(leiter);
                break;
            case 10:
                _gwLeiter11OK.Add(leiter);
                break;
            case 11:
                _gwLeiter12OK.Add(leiter);
                break;
            default:
                break;
        }
    }

    public List<GameObject> GetLeiterOberkantenObject(int leiterNummer)
    {
        switch (leiterNummer)
        {
            case 0:
                return _gwLeiter1OK;
            case 1:
                return _gwLeiter2OK;
            case 2:
                return _gwLeiter3OK;
            case 3:
                return _gwLeiter4OK;
            case 4:
                return _gwLeiter5OK;
            case 5:
                return _gwLeiter6OK;
            case 6:
                return _gwLeiter7OK;
            case 7:
                return _gwLeiter8OK;
            case 8:
                return _gwLeiter9OK;
            case 9:
                return _gwLeiter10OK;
            case 10:
                return _gwLeiter11OK;
            case 11:
                return _gwLeiter12OK;
            default:
                return null;
        }
    }

    public void SetLeiterUnterKantenObject(int leiterNummer, GameObject leiter)
    {
        switch (leiterNummer)
        {
            case 0:
                _gwLeiter1UK.Add(leiter);
                break;
            case 1:
                _gwLeiter2UK.Add(leiter);
                break;
            case 2:
                _gwLeiter3UK.Add(leiter);
                break;
            case 3:
                _gwLeiter4UK.Add(leiter);
                break;
            case 4:
                _gwLeiter5UK.Add(leiter);
                break;
            case 5:
                _gwLeiter6UK.Add(leiter);
                break;
            case 6:
                _gwLeiter7UK.Add(leiter);
                break;
            case 7:
                _gwLeiter8UK.Add(leiter);
                break;
            case 8:
                _gwLeiter9UK.Add(leiter);
                break;
            case 9:
                _gwLeiter10UK.Add(leiter);
                break;
            case 10:
                _gwLeiter11UK.Add(leiter);
                break;
            case 11:
                _gwLeiter12UK.Add(leiter);
                break;
            default:
                break;
        }
    }

    public List<GameObject> GetLeiterUnterKantenObject(int leiterNummer)
    {
        switch (leiterNummer)
        {
            case 0:
                return _gwLeiter1UK;
            case 1:
                return _gwLeiter2UK;
            case 2:
                return _gwLeiter3UK;
            case 3:
                return _gwLeiter4UK;
            case 4:
                return _gwLeiter5UK;
            case 5:
                return _gwLeiter6UK;
            case 6:
                return _gwLeiter7UK;
            case 7:
                return _gwLeiter8UK;
            case 8:
                return _gwLeiter9UK;
            case 9:
                return _gwLeiter10UK;
            case 10:
                return _gwLeiter11UK;
            case 11:
                return _gwLeiter12UK;
            default:
                return null;
        }
    }

    public void SetStandardMaterial()
    {
        _standardMaterial = new Material(Shader.Find("Sprites/Default"));

    }

    public void SetNetzMaterial()
    {
        _netzMaterial = new Material(Shader.Find("Netz/Wireframe"));
    }

    public Material GetStandardMaterial()
    {
        return _standardMaterial;
    }

    public Material GetNetzMaterial()
    {
        return _netzMaterial;
    }

    public void SetListeAllerDreiecksPunkte(List<int> listeAllerDreiecksPunkte)
    {
        _listeAllerDreiecksPunkte = listeAllerDreiecksPunkte;
    }

    public List<int> GetListeAllerDreiecksPunkte()
    {
        return _listeAllerDreiecksPunkte;
    }


    public void setAnzKnotenProLeiter(int anzKnotenProLeiter)
    {
        _anzKnotenProLeiter = anzKnotenProLeiter;
    }

    public int GetAnzKnotenProLeiter()
    {
        return _anzKnotenProLeiter;
    }


    public void setAnzModellLeiter(int anzModellLeiter)
    {
        this._anzLeiter = anzModellLeiter;
    }

    public int getAnzModellLeiter()
    {
        return _anzLeiter;
    }



    public void SetAbfrageWirdKFWertGefordert(bool wirdkFWertDarstellungGefordert)
    {
        _wirdEineKFWertDarstellungGefordert = wirdkFWertDarstellungGefordert;

    }

    public bool getAbfrageWirdKFWertGefordert()
    {
        return _wirdEineKFWertDarstellungGefordert;
    }


    public void SetAbfrageNeuLaden(bool neuLaden)
    {
        _neuLaden = neuLaden;

    }

    public bool getAbfrageNeuLaden()
    {
        return _neuLaden;
    }


    public void SetAbfragePunktwolke(bool wirdPunktewolkeGefordert)
    {
        _wirdEinePunktwolkeGefordert = wirdPunktewolkeGefordert;
        _wirdEinDreiecksNetzGefordert = !wirdPunktewolkeGefordert;
    }

    public bool getAbfragePunktewolke()
    {
        return _wirdEinePunktwolkeGefordert;
    }



    public void SetAbfrageDreiecksnetz(bool wirdDreiecksnetzGefordert)
    {
        _wirdEinDreiecksNetzGefordert = wirdDreiecksnetzGefordert;
        _wirdEinePunktwolkeGefordert = !wirdDreiecksnetzGefordert;
    }

    public bool getAbfrageDreiecksnetz()
    {
        return _wirdEinDreiecksNetzGefordert;
    }




    public void SetVectorDerLeiterUnterKanten(Vector3[] vectorDerLeiterUnterKanten)
    {
        _vectorDerLeiterUnterkanten = vectorDerLeiterUnterKanten;
    }

    public Vector3[] GetVectorDerLeiterUnterKanten()
    {
        return _vectorDerLeiterUnterkanten;
    }



    public void SetVectorDerLeiterOberkanten(Vector3[] vectorDerLeiterOberkanten)
    {
        _vectorDerLeiterOberkanten = vectorDerLeiterOberkanten;
    }

    public Vector3[] GetVectorDerLeiterOberkanten()
    {
        return _vectorDerLeiterOberkanten;
    }



    public void SetDaten10Leser(StreamReader daten10Leser)
    {
        _daten10Leser = daten10Leser;
    }

    public StreamReader GetDaten10Leser()
    {
        return _daten10Leser;
    }

    public void closeReader()
    {
        _daten10Leser.Close();
    }


    public void SetDateiPfad(string dataPath)
    {
        _dateiPfad = dataPath;
    }

    public string GetDateiPfad()
    {
        return _dateiPfad;
    }



    public void SetSkalierung(int skalierung)
    {
        _skalierung = skalierung;
    }

    public int GetSkalierung()
    {
        return _skalierung;
    }



    public void SetLogger(StreamWriter logger)
    {
        _logger = logger;
    }

    public void closeLogger()
    {
        _logger.Close();
    }

    public StreamWriter GetLogger()
    {
        return _logger;
    }



    public void setFarbe(Color[] farbe)
    {
        this.farbe = farbe;
    }

    public Color[] GetFarbe()
    {
        return farbe;
    }



    public void SetAblagePfad(string nameDesSpeicherOrtes)
    {
        _nameDesSpeicherOrtes = nameDesSpeicherOrtes;
    }

    public string GetAblagePfad()
    {
        return _nameDesSpeicherOrtes;
    }

   


    public static DataSingleton GetInstanz()
    {
        if (Instanzierung == null)
        {
            lock (ReadLock)
            {
                if (Instanzierung == null)
                {
                    Instanzierung = new DataSingleton();
                }
            }
        }

        return Instanzierung;
    }

 
}