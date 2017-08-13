using UnityEngine;
using System.IO;
using System;

/* Die Klasse Starter erhält seine Initialwerte aus dem Singleton, welches zuvor über das Hauptmenü - Einstellung mit allen relevanten Daten
 * initialisiert wurde. Wichtige Parameter sind hierbei der Pfadname zum Modellsatz (DATEN10), der Pfad des Speicherortes, die Art des Darstellung (Punktwolke, DreiecksNetz).
 * 
 * Das Programm prüft zuerst, ob der angegebene Ablagename bereits existiert. Ist dies der Fall, dann werden die abgelegten Dateien
 * eingeladen und mit einem Material belegt. Dadurch kann eine Neugenerierung bereits vorhandener Daten umgangen werden.
 * 
 * Sollte kein Ordner und somit keine Dateien existieren, wird dieser angelegt und die Netzgenerierung gestartet
 */


public class Starter : MonoBehaviour
{
    /*
        public string DateiPfad;
        public string AblageName;
        public bool DreiecksNetz;
        public bool Punktwolke;
        public int Skalierung;
        public bool NeuLadenErzwingen;
        public bool KFWerte;
       */

    private string _dateiPfad;
    private string _ablageName;
    private bool _dreiecksNetz;
    private bool _punktWolke;
    private int _skalierung;
    private bool _neuLadenErzwingen;
    private bool _kFWerte;

    private StreamWriter _logger;
    private DataSingleton _datenAblage;


    void Start ()
    {
        /*
         * Die Methode wird direkt bei der Initialisierung des Scriptes aufgerufen.
         * Der Logger ( StreamWriter) dokumentiert die wichtigsten Arbeitsschritte
         * und beinhaltet auch die Fehlermeldungen
         * */

        _datenAblage =  DataSingleton.GetInstanz(); ;
        _logger = new StreamWriter("Log.txt");

        SchreibeLogEintrag("Ist die korrekte Ordnerstrukt vorhanden?");
        PruefeOrdnerstruktur();

        SchreibeLogEintrag("Lese das Singleton (TODO)");
        FuelleDasSingleton();

        SchreibeLogEintrag("Pruefe ob neue Daten erzeugt werden müssen");
        PruefeAufVorhandeDaten();
	}

    void FuelleDasSingleton()
    {

        //_datenAblage.SetAblagePfad(AblageName);
        _ablageName = _datenAblage.GetAblagePfad();

        //  _datenAblage.SetDateiPfad(DateiPfad);
        _dateiPfad = _datenAblage.GetDateiPfad();

        //_datenAblage.SetSkalierung(Skalierung);
        _skalierung = _datenAblage.GetSkalierung();

        // _datenAblage.SetAbfrageDreiecksnetz(true);
        _dreiecksNetz = _datenAblage.getAbfrageDreiecksnetz();

        //_datenAblage.SetAbfragePunktwolke(Punktwolke);
        _punktWolke = _datenAblage.getAbfragePunktewolke();

        _neuLadenErzwingen = _datenAblage.getAbfrageNeuLaden();

        _datenAblage.SetLogger(_logger);
        _datenAblage.SetDaten10Leser(new StreamReader(_datenAblage.GetDateiPfad()));
        _datenAblage.SetStandardMaterial();
        //_datenAblage.SetAbfrageWirdKFWertGefordert(KFWerte);
        
    }

    void PruefeOrdnerstruktur()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

        if (!Directory.Exists(Application.dataPath + "/Resources/Punktwolken/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "Punktwolken");

        if (!Directory.Exists(Application.dataPath + "/Resources/Dreiecksnetze/"))
            UnityEditor.AssetDatabase.CreateFolder("Assets/Resources", "Dreiecksnetze");
    }


    

    void PruefeAufVorhandeDaten()
    {

        /*
         * Ist der Speicherort bereits vorhanden und nicht leer, wird dessen Inhalt geladen.
         * Hierbei wird unterschieden, ob eine Punktwolke, oder ein Dreiecksnetz angefordert wurde.
         * Sollten keine alten Daten existieren, wird ein neues Netz, oder eine neue Punktwolke generiert
         */

        if (_datenAblage.getAbfragePunktewolke())
        {
            if (!Directory.Exists(Application.dataPath + "/Resources/Punktwolken/" + _ablageName) || _neuLadenErzwingen)
            {

                SchreibeLogEintrag("Keine alten Daten vorhanden - Erzeuge neue");

                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Punktwolken", _ablageName);
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Dreiecksnetze", _ablageName);
                StartCoroutine("starteModellViewer");

            }
            else
            {

                SchreibeLogEintrag("Daten sind bereits vorhanden und werden geladen");
                LadeVorhandeneDaten();

            }
        }
        else if (_datenAblage.getAbfrageDreiecksnetz())
        {
           
            if (!Directory.Exists(Application.dataPath + "/Resources/Dreiecksnetze/" + _ablageName))
            {

                SchreibeLogEintrag("Keine alten Daten vorhanden - Erzeuge neue");

                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Punktwolken", _ablageName);
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Dreiecksnetze", _ablageName);
                StarteModellViewer();

            }
            else if(_neuLadenErzwingen)
            {
                UnityEditor.FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/Dreiecksnetze/" + _ablageName);
                UnityEditor.FileUtil.DeleteFileOrDirectory(Application.dataPath + "/Resources/Punktwolken/" + _ablageName);

                SchreibeLogEintrag("Überschreibe alte Daten");

                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Punktwolken", _ablageName);
                UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Dreiecksnetze", _ablageName);
                StarteModellViewer();
            }
            else
            {

                SchreibeLogEintrag("Daten sind bereits vorhanden und werden geladen");
                LadeVorhandeneDaten();

            }
        }

	}
	
	
	void LadeVorhandeneDaten()
    {
        /*
         * Die angeforderten Daten sind bereits vorhanden und werden eingeladen.
         * Dabei wird das Singleton mit den ausgelesenen Daten belegt. Die Zuordnung
         * erfolgt über die Namensgebung des Dreiecksnetzes
         * */

        SchreibeLogEintrag("Altdaten werden geladen");

        if (_datenAblage.getAbfragePunktewolke())
        {

           // TODO Einladen der Punktwolke

        }


        if (_datenAblage.getAbfrageDreiecksnetz())
        {
            var funktionsModell = new ModellViewer(true);

            SchreibeLogEintrag("Altes Netz gefunden und wird geladen");

            var ordnerInformation = new DirectoryInfo(Application.dataPath + "/Resources/Dreiecksnetze/" + _ablageName);
            var ordnerInhalt = ordnerInformation.GetFiles();
            var count = -1;
            var dreiecksNetzOrdner = new GameObject("Dreiecksnetz_" + _ablageName);

            funktionsModell.setCameraParameters(Camera.main, dreiecksNetzOrdner);


            foreach (var datei in ordnerInhalt) 
            {

                if(!datei.Name.Contains("meta"))
                {
                    count++;

                    var aufgeteilterDateiName = datei.Name.ToString().Split('_');
                    var vollstaendigerDateiName = datei.Name.ToString();
                    var zuKuerzen = ".asset";
                    var dateiName = vollstaendigerDateiName.Replace(zuKuerzen, "");
                    var dreiecksNetz = new GameObject(dateiName);

                    dreiecksNetz.AddComponent<MeshFilter>();
                    dreiecksNetz.AddComponent<MeshRenderer>();
                    
                    dreiecksNetz.GetComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Dreiecksnetze/" + _ablageName + "/" + dateiName, typeof(Mesh));
                    dreiecksNetz.GetComponent<Renderer>().material = _datenAblage.GetStandardMaterial();
                    dreiecksNetz.AddComponent<MeshCollider>();

                    dreiecksNetz.transform.parent = dreiecksNetzOrdner.transform;

                    SchreibeLogEintrag("Füge hinzu " + aufgeteilterDateiName[2]);

                    if (datei.Name.Contains("OK"))
                    {
                        dreiecksNetz.name = (aufgeteilterDateiName[1])+"-OK";
                        SchreibeLogEintrag("Füge  hinzu " + dreiecksNetz.name);
                         _datenAblage.SetLeiterOberkantenObject(Int32.Parse(aufgeteilterDateiName[1]), dreiecksNetz);
             
                    }

                    else
                    {
                        dreiecksNetz.name = (aufgeteilterDateiName[1]) + "-UK";
                        SchreibeLogEintrag("Füge  hinzu " + dreiecksNetz.name);
                        _datenAblage.SetLeiterUnterKantenObject(Int32.Parse(aufgeteilterDateiName[1]), dreiecksNetz);

                    }
                   
                }
                
            }

        }

	}


    void StarteModellViewer()
    {

        var netzModell = new ModellViewer(false);

    }


    void SchreibeLogEintrag(string zuSchreibenderText)
    {

        _logger.WriteLine("-----------------------------------------------");
        _logger.WriteLine(zuSchreibenderText);
        _logger.WriteLine("");
        _logger.Flush();

    }
}
