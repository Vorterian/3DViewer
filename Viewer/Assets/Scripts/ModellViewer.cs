using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading;


/* Zuerst wird der Datensatz eingelesen und die dynamischen (Vom Nutzer vorgegbene) Variablen belegt.
 * Im Anschluss wird je nach Anforderung ein Dreiecksnetz, oder eine Punktewolke aus den zur Verfügung stehenden Daten erzeugt.
 * In Abhänigkeit der Modellgröße (Knotenanzahl) werden mehrere Netze je Grundwasserleiter konstruiert. 
 * Liegt die Knotenanzahl unter 65000, ist die Anzahl der Netze je Grundwasserleiter gleich eins 
 * 
 * Zeitlicher Aufruf der Funktionen
 * 
 * 1) ModellViewer
 * 
 * 2) LeseDaten10
 * 2.1) leseBlock6
 * 2.2) InitialisierungDynamischeVariablen
 * 2.3) BearbeiteDreiecke
 * 2.3.1) LoescheDoppelteDreiecke
 * 2.3.2) DupliziereUndRotiereDreiecke

 * 3) KorrekturDerRaumlage

 * 4) KonstruiereAngefordertesGameObject
 * 4.1) Falls Punktwolke gefordert: BauePunktMesh 
 * 4.2) Falls Dreiecksnetz gefordert:  KonstruiereOeberUndUnterkantenGameObjekte

 * 7) 
        
 * */

public class ModellViewer
{

	// Datenablage
	private string _dateiPfad;
    private string _nameDesSpeicherOrtes;
    
    private StreamWriter _logger;

    // Fortschrittsanzeige
    private string _anzeigeText = "";
    private float _fortschrittsAnzeige = 0;
    static object Lock = new object();

    private int[] _bearbeiteteDreiecke;
    private int _anzKnotenImModell;                     // Anzahl der Knoten im Modell (Anzahl der Knoten * Anzahl der Leiter)
    private int _anzDerNetze;                          
    private int _anzKnotenProLeiter;                    // Wieviele Knoten besitzt das beschriebene Modell
    private int _anzModellLeiter;                       // Wieviele Modellleiter werden im Datensatz beschrieben
    private GameObject _punkteWolke;                    // Übergeordnetes Gameobject zur Ablage der Punktwolke
    private GameObject _dreiecksNetz;                   // Übergeordnetes Gameobject zur Ablage des Dreiecknetzes
    private Vector3[] _vectorDerLeiterOberkanten;       // Speicherung der Grundwasserleiter - Oberkanten mit ihren Werten x,y,z
    private Vector3[] _vectorDerLeiterUnterKanten;      // Speicherung der Grundwasserleiter - Unterkanten mit ihren Werten x,y,z
    private Vector3[] VerticeListe;                     // Liste der Vertices des Dreiecksnetzes. Vertices : 
    private Vector3 _kleinsteKoordinatenUndHoehe;       // Kleinster Koordinatenwert der Modellpunkte. Wird genutzt um das Modell in den Ursprung zu transformieren
    private StreamReader _daten10Leser;
    private Material _standardMaterial;
    private Color[] _farbwerteDerKnoten;                // Speicherung von Farbwerten
    private List<int> _listeAllerDreiecksPunkte;

    private volatile bool _istDasDreieckEinmalig = true;
    private DataSingleton _datenAblage;                 // Das Singleton enthält allte notwendigen Informationen über das Netz


    private int _skalierung;
    Boolean NeuLaden = false;


    // Konstuktor der Klasse greift auf das Singleton zu und initalisiert alle notwendigen Variablen
    public ModellViewer(Boolean nurFunktionenLaden)
    {

        _datenAblage = DataSingleton.GetInstanz();

        InitialisierungDerNutzerEingaben();

        SchreibeLogEintrag("Modell-Klasse initialisiert");

        if (!nurFunktionenLaden)
            LeseDaten10();

	}

    void InitialisierungDerNutzerEingaben()
    {
        _kleinsteKoordinatenUndHoehe = new Vector3();

        _datenAblage.SetListeAllerDreiecksPunkte(new List<int>());

        _skalierung = _datenAblage.GetSkalierung();
        _logger = _datenAblage.GetLogger();
        _daten10Leser = _datenAblage.GetDaten10Leser();
        _dateiPfad = _datenAblage.GetDateiPfad();
        _nameDesSpeicherOrtes = _datenAblage.GetAblagePfad();
        _standardMaterial = _datenAblage.GetStandardMaterial();

    }

    void InitialisierungDynamischeVariablen()
    {

        _anzModellLeiter = _datenAblage.getAnzModellLeiter();
        _anzKnotenProLeiter = _datenAblage.GetAnzKnotenProLeiter();
        _anzKnotenImModell = _anzKnotenProLeiter * _anzModellLeiter;

        _listeAllerDreiecksPunkte = _datenAblage.GetListeAllerDreiecksPunkte();
        _vectorDerLeiterOberkanten = _datenAblage.GetVectorDerLeiterOberkanten();
        _vectorDerLeiterUnterKanten = _datenAblage.GetVectorDerLeiterUnterKanten();
        _farbwerteDerKnoten = _datenAblage.GetFarbe();

    }

    IEnumerator LeseDaten10()
    {
        /*
         * Die Blöcke 6 und 7-1 bis 7-x (x.Max = 12) werden ausgelesenen und auf Grundlage dieser Daten
         * entstehen die Punkt und Dreiecksnetze
         */

        // List<Thread> threadList = new List<Thread>();
        var listeDerThreads = new List<Thread>();
        var eingeleseneZeile = "";

        do
        {
            eingeleseneZeile = _daten10Leser.ReadLine();

            if (eingeleseneZeile != null)
            {
                if (eingeleseneZeile.Contains("BLOCK  6"))
                {

                    SchreibeLogEintrag("Beginne mit der Bearbeitung vom Block 6");

                    var leseBlock6 = new ReaderGeometrie(_datenAblage);

                    SchreibeLogEintrag("Bearbeitung vom Block 6 beendet");

                    InitialisierungDynamischeVariablen();

                    BearbeiteDreiecke();

                    SchreibeLogEintrag("Dreiecke sortiert");


                    for (int indexLeiter = 1; indexLeiter < 10; indexLeiter++)
                    {
                        SchreibeLogEintrag("Starte Thread " + indexLeiter);

                        var readerThread = new ReaderThread("BLOCK  7-" + indexLeiter, indexLeiter-1, _datenAblage);
                        var readThread = new Thread(new ThreadStart(readerThread.read));
                        readThread.Start();
                        listeDerThreads.Add(readThread);
                    }


                    for (int indexLeiter = 10; indexLeiter < 13; indexLeiter++)
                    {
                        SchreibeLogEintrag("Starte Thread " + indexLeiter);

                        var readerThread = new ReaderThread("BLOCK  7" + indexLeiter, indexLeiter - 1, _datenAblage);
                        var readThread = new Thread(new ThreadStart(readerThread.read));
                        readThread.Start();
                        listeDerThreads.Add(readThread);
                    }

                    SchreibeLogEintrag(" Anzahl der gestarteten Threads " + listeDerThreads.Count);

                    // _anzeigeText = (" Anzahl der gestarteten Threads " + listeDerThreads.Count);

                    for (int zeigerAufDenThreadIndex = 0; zeigerAufDenThreadIndex < listeDerThreads.Count; zeigerAufDenThreadIndex++)
                    {
                        listeDerThreads[zeigerAufDenThreadIndex].Join();
                        _anzeigeText = (" Anzahl der beendeten Threads " + zeigerAufDenThreadIndex + 1);
                        SchreibeLogEintrag("Thread" + zeigerAufDenThreadIndex + " fertig");
                    }
                }
            }

        } while (! eingeleseneZeile.Contains("BLOCK  8"));

        SchreibeLogEintrag("Korrektur der Raumlage ");

        KorrekturDerRaumlage(_vectorDerLeiterOberkanten);

        SchreibeLogEintrag("Beginne mit der Netzgenerierung ");

        KonstruiereAngefordertesGameObject();

        return null;
    }

     void BearbeiteDreiecke()
     {

        SchreibeLogEintrag("Beginne mit der Bearbeitung der DreiecksInformationen");

        LoescheDoppelteDreiecke();

        DupliziereUndRotiereDreiecke();

    }


    public void setEinmaligesDreieck()
    {
        _istDasDreieckEinmalig = false;
    }

    public bool readEinmaligesDreieck()
    {
         return _istDasDreieckEinmalig;
    }





    void LoescheDoppelteDreiecke()
    {


        if(_anzKnotenProLeiter > 30000)
        {
            var zwischenAblageAllerDreiecksPunkte = _listeAllerDreiecksPunkte.ToArray();
            var startWert = 0;
            var endWert = 0;
            var anzThreads = 5;

            _listeAllerDreiecksPunkte.Clear();
            SchreibeLogEintrag("Anzahl der Dreiecke vor der Vereinzelung " + ((zwischenAblageAllerDreiecksPunkte.Length) / 3).ToString());


            for (int indexDesDreieckStartpunktes = 0; indexDesDreieckStartpunktes < zwischenAblageAllerDreiecksPunkte.Length - 3; indexDesDreieckStartpunktes = indexDesDreieckStartpunktes + 3)
            {

                SchreibeLogEintrag(indexDesDreieckStartpunktes.ToString() + " " + zwischenAblageAllerDreiecksPunkte.Length);
                _istDasDreieckEinmalig = true;

                var listeDerThreads = new List<Thread>();
                var listeDerThreadObjecte = new List<VereinzelungsThread>();

                var ersterDreiecksPunkt = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes];
                var zweiterDreiecksPunkt = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes + 1];
                var dritterDreiecksPunkt = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes + 2];

                var anzDerZuBearbeitendenDreiecke = (zwischenAblageAllerDreiecksPunkte.Length - indexDesDreieckStartpunktes) / 3;
                var elementeProThread = (int)Math.Floor((double)anzDerZuBearbeitendenDreiecke / anzThreads) * 3;


                for (int anzDerThreads = 0; anzDerThreads < anzThreads - 1; anzDerThreads++)
                {
                    //    SchreibeLogEintrag("Starte den Loop Thread");
                    startWert = indexDesDreieckStartpunktes + 3 + (elementeProThread * (anzDerThreads));
                    endWert = startWert + elementeProThread - 1;
                    var vereinzelungsThread = new VereinzelungsThread(startWert, endWert, ersterDreiecksPunkt, zweiterDreiecksPunkt, dritterDreiecksPunkt, zwischenAblageAllerDreiecksPunkte, this, _logger);
                    var startThread = new Thread(new ThreadStart(vereinzelungsThread.read));
                    startThread.Start();
                    listeDerThreads.Add(startThread);
                }



                startWert = indexDesDreieckStartpunktes + 3 + (elementeProThread * (anzThreads - 1));

                var vereinzelungsThreadLast = new VereinzelungsThread(startWert, zwischenAblageAllerDreiecksPunkte.Length - 1, ersterDreiecksPunkt, zweiterDreiecksPunkt, dritterDreiecksPunkt, zwischenAblageAllerDreiecksPunkte, this, _logger);
                var startThreadLast = new Thread(new ThreadStart(vereinzelungsThreadLast.read));
                startThreadLast.Start();
                listeDerThreadObjecte.Add(vereinzelungsThreadLast);
                listeDerThreads.Add(startThreadLast);



                for (int zeigerAufDenThreadIndex = 0; zeigerAufDenThreadIndex < listeDerThreads.Count; zeigerAufDenThreadIndex++)
                {
                    listeDerThreads[zeigerAufDenThreadIndex].Join();
                }



                for (int zeigerAufDenThreadIndex = 0; zeigerAufDenThreadIndex < listeDerThreads.Count; zeigerAufDenThreadIndex++)
                {
                    if (_istDasDreieckEinmalig)
                    {
                        listeDerThreads[zeigerAufDenThreadIndex].Join();
                    }
                    else
                    {

                        for (int zeigerZumAnhalten = 0; zeigerZumAnhalten < listeDerThreadObjecte.Count; zeigerZumAnhalten++)
                        {
                            listeDerThreadObjecte[zeigerZumAnhalten].RequestStop();
                        }

                        break;
                    }

                }

                for (int zeigerZumAnhalten = 0; zeigerZumAnhalten < listeDerThreadObjecte.Count; zeigerZumAnhalten++)
                {
                    listeDerThreads[zeigerZumAnhalten].Join();
                }


                if (_istDasDreieckEinmalig)
                {
                    try
                    {
                        _listeAllerDreiecksPunkte.Add(ersterDreiecksPunkt);
                        _listeAllerDreiecksPunkte.Add(zweiterDreiecksPunkt);
                        _listeAllerDreiecksPunkte.Add(dritterDreiecksPunkt);
                    }
                    catch (Exception e)
                    {

                        SchreibeLogEintrag(e.ToString());
                    }


                }
            }
        }
        else
        {
            var zwischenAblageAllerDreiecksPunkte = _listeAllerDreiecksPunkte.ToArray();

            _listeAllerDreiecksPunkte.Clear();

            SchreibeLogEintrag("Anzahl der Dreiecke vor der Vereinzelung " + ((zwischenAblageAllerDreiecksPunkte.Length) / 3).ToString());

            for (int indexDesDreieckStartpunktes = 0; indexDesDreieckStartpunktes < zwischenAblageAllerDreiecksPunkte.Length - 3; indexDesDreieckStartpunktes = indexDesDreieckStartpunktes + 3)
            {
                var istDasDreieckEinmalig = true;

                var startpunkt = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes];
                var stützpunktZwei = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes + 1];
                var stützpunktDrei = zwischenAblageAllerDreiecksPunkte[indexDesDreieckStartpunktes + 2];


                for (int indexDesListenDurchlaufes = indexDesDreieckStartpunktes + 3; indexDesListenDurchlaufes < zwischenAblageAllerDreiecksPunkte.Length - 3; indexDesListenDurchlaufes = indexDesListenDurchlaufes + 3)
                {
                    if (zwischenAblageAllerDreiecksPunkte[indexDesListenDurchlaufes] == startpunkt && zwischenAblageAllerDreiecksPunkte[indexDesListenDurchlaufes + 1] == stützpunktZwei && zwischenAblageAllerDreiecksPunkte[indexDesListenDurchlaufes + 2] == stützpunktDrei)
                    {
                        istDasDreieckEinmalig = false;
                    }

                }

                if (istDasDreieckEinmalig)
                {
                    try
                    {
                        _listeAllerDreiecksPunkte.Add(startpunkt);
                        _listeAllerDreiecksPunkte.Add(stützpunktZwei);
                        _listeAllerDreiecksPunkte.Add(stützpunktDrei);
                    }
                    catch (Exception e)
                    {

                        SchreibeLogEintrag(e.ToString());
                    }


                }
            }
        }
        

        SchreibeLogEintrag("Anzahl der Dreiecke nach der Vereinzelung " + _listeAllerDreiecksPunkte.Count + " " + ((_listeAllerDreiecksPunkte.Count) / 3).ToString());

    }



    void DupliziereUndRotiereDreiecke()
    {
        /*
         * Damit das Netz von allen Seiten (Oben und Unten) sichtbar ist, muss das jeweilige Dreieck in beide "Richtungen" 
         * (Im Uhrzeigersinn und gegen den Uhrzeigersinn) aufgespannt werden.
         * Die bereits sortieren Dreiecke werden in dieser Funktion gedreht und nach _bearbeiteteDreiecke übertragen,
         */

        SchreibeLogEintrag("Anzahl der Dreiecke in einer Drehrichtung " + _listeAllerDreiecksPunkte.Count / 3);

        var anzAllerDreiecksPunkte = _listeAllerDreiecksPunkte.Count;

        for (int dreiecksPunkt = 0; dreiecksPunkt < anzAllerDreiecksPunkte; dreiecksPunkt = dreiecksPunkt + 3)
        {

            _listeAllerDreiecksPunkte.Add(_listeAllerDreiecksPunkte[dreiecksPunkt+2]);
            _listeAllerDreiecksPunkte.Add(_listeAllerDreiecksPunkte[dreiecksPunkt + 1]);
            _listeAllerDreiecksPunkte.Add(_listeAllerDreiecksPunkte[dreiecksPunkt]);

        }

        _bearbeiteteDreiecke = _listeAllerDreiecksPunkte.ToArray();

        SchreibeLogEintrag("Anzahl der Dreiecke in Beide Richtungen " + _listeAllerDreiecksPunkte.Count / 3);

        _listeAllerDreiecksPunkte.Clear();

        

    }

    void KonstruiereAngefordertesGameObject()
    {


        var anzDerErlaubtenKnoten = 65000;

        _anzDerNetze = _anzKnotenImModell / anzDerErlaubtenKnoten;
        _punkteWolke = new GameObject("Punktewolke_" + _nameDesSpeicherOrtes);
        _dreiecksNetz = new GameObject("Dreiecksnetz_" + _nameDesSpeicherOrtes);
        _datenAblage.HarfenEintraege = KonstruiereHarfe();


        SchreibeLogEintrag(" Harfe erstellt " + _datenAblage.HarfenEintraege.Count);

        setCameraParameters(Camera.main, _dreiecksNetz);

        if (_datenAblage.getAbfragePunktewolke())
        {
            for (int aktNetz = 0; aktNetz < _anzDerNetze - 1; aktNetz++)
            {
                BauePunktMesh(aktNetz, anzDerErlaubtenKnoten);
            }
        }


        if (_datenAblage.getAbfrageDreiecksnetz())
        {
            KonstruiereOeberUndUnterkantenGameObjekte();
        }

        // _datenAblage.closeLogger();
    }


    List<List<int>> KonstruiereHarfe()
    {
        var listeAllerHarfen = new List<List<int>>();

        SchreibeLogEintrag("Erstelle Harfe");

        var kritischeKnoten = 20000;

        if (_anzKnotenProLeiter > kritischeKnoten)
        {

            var harfe = new List<int>();
            var stuetzpunkteDerDreiecke = new List<int>();

            for (int i = 0; i < _anzKnotenProLeiter; i++)
            {
                harfe.Add(-9999);
            }


            var counter65K = 0;

            for (int aktuellerDreiecksStuetzpunkt = 0; aktuellerDreiecksStuetzpunkt < _bearbeiteteDreiecke.Length - 3; aktuellerDreiecksStuetzpunkt = aktuellerDreiecksStuetzpunkt + 3)
            {
                var stuetzpunkt1 = _bearbeiteteDreiecke[aktuellerDreiecksStuetzpunkt];
                var stuetzpunkt2 = _bearbeiteteDreiecke[aktuellerDreiecksStuetzpunkt + 1];
                var stuetzpunkt3 = _bearbeiteteDreiecke[aktuellerDreiecksStuetzpunkt + 2];

                if (counter65K <= kritischeKnoten && aktuellerDreiecksStuetzpunkt < _bearbeiteteDreiecke.Length - 6)
                {

                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt1))
                        counter65K++;

                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt2))
                        counter65K++;

                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt3))
                        counter65K++;


                    if (counter65K <= kritischeKnoten)
                    {

                        harfe.RemoveAt(stuetzpunkt1);
                        harfe.Insert(stuetzpunkt1, stuetzpunkt1);

                        harfe.RemoveAt(stuetzpunkt2);
                        harfe.Insert(stuetzpunkt2, stuetzpunkt2);

                        harfe.RemoveAt(stuetzpunkt3);
                        harfe.Insert(stuetzpunkt3, stuetzpunkt3);

                        stuetzpunkteDerDreiecke.Add(stuetzpunkt1);
                        stuetzpunkteDerDreiecke.Add(stuetzpunkt2);
                        stuetzpunkteDerDreiecke.Add(stuetzpunkt3);

                    }

                }
                else
                {

                    for (int KnotenUeber65K = kritischeKnoten; KnotenUeber65K < harfe.Count; KnotenUeber65K++)
                    {
                        if (harfe[KnotenUeber65K] != -9999)
                        {
                            for (int KnotenUnter65K = 0; KnotenUnter65K <= kritischeKnoten; KnotenUnter65K++)
                            {
                                if (harfe[KnotenUnter65K] == -9999)
                                {

                                    harfe[KnotenUnter65K] = KnotenUeber65K;
                                    harfe[KnotenUeber65K] = -9999;

                                    for (int indexDesDurchlaufes = 0; indexDesDurchlaufes < stuetzpunkteDerDreiecke.Count; indexDesDurchlaufes++)
                                    {

                                        if (stuetzpunkteDerDreiecke[indexDesDurchlaufes] == KnotenUeber65K)
                                        {
                                            stuetzpunkteDerDreiecke[indexDesDurchlaufes] = KnotenUnter65K;
                                        }

                                    }

                                    break;
                                }
                            }
                        }
                    }


                    var tmpHarfe = -9999;
                    harfe.RemoveAll(item => item == tmpHarfe);

                    var  tmpHarfe2 = new List<int>(harfe);
                    var tmptmpDreiecke = new List<int>(stuetzpunkteDerDreiecke); ;

                    listeAllerHarfen.Add(tmptmpDreiecke);
                    listeAllerHarfen.Add(tmpHarfe2);

                    SchreibeLogEintrag("Länge der Harfe " + harfe.Count);
                    SchreibeLogEintrag("Länge der Dreiecke " + stuetzpunkteDerDreiecke.Count);
                    SchreibeLogEintrag("Aktueller Index " + aktuellerDreiecksStuetzpunkt + "von " + _bearbeiteteDreiecke.Length);
                    harfe.Clear();
                    stuetzpunkteDerDreiecke.Clear();


                    for (int indexKnotenProLeiter = 0; indexKnotenProLeiter < _anzKnotenProLeiter; indexKnotenProLeiter++)
                    {
                        harfe.Add(-9999);
                    }

                    counter65K = 0;

                    
                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt1))
                        counter65K++;

                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt2))
                        counter65K++;

                    if (!stuetzpunkteDerDreiecke.Contains(stuetzpunkt3))
                        counter65K++;
                       

                    if (counter65K <= kritischeKnoten)
                    {

                        harfe.RemoveAt(stuetzpunkt1);
                        harfe.Insert(stuetzpunkt1, stuetzpunkt1);

                        harfe.RemoveAt(stuetzpunkt2);
                        harfe.Insert(stuetzpunkt2, stuetzpunkt2);

                        harfe.RemoveAt(stuetzpunkt3);
                        harfe.Insert(stuetzpunkt3, stuetzpunkt3);

                        stuetzpunkteDerDreiecke.Add(stuetzpunkt1);
                        stuetzpunkteDerDreiecke.Add(stuetzpunkt2);
                        stuetzpunkteDerDreiecke.Add(stuetzpunkt3);

                    }
                     
                }

            }

            for (int indexDerHarfe = 0; indexDerHarfe < listeAllerHarfen.Count; indexDerHarfe++)
            {
               SchreibeLogEintrag(listeAllerHarfen[indexDerHarfe].Count.ToString());
            }

            return listeAllerHarfen;
        }
        else
        {
            return listeAllerHarfen;
        }

    }


    void KonstruiereOeberUndUnterkantenGameObjekte()
    {
        SchreibeLogEintrag("Beginne mit der Meshfüllung");

        var listeDerThreads = new List<Thread>();
        var listeDerThreadObjecte = new List<ThreadKonstruiereUnternetze>();

        for (int aktModellLeiter = 0; aktModellLeiter < _anzModellLeiter; aktModellLeiter++)
        {
            SchreibeLogEintrag("Bearbeite folgenden Leiter " + aktModellLeiter);

            var farbeDerOberkante = SucheNegativeGeologieUndKopplungen("Oberkante", aktModellLeiter);
            var farbeDerUnterkante = SucheNegativeGeologieUndKopplungen("Unterkante", aktModellLeiter);

            SchreibeLogEintrag("Farben erzeugt");

            for (int indexHarfe = 0; indexHarfe < _datenAblage.HarfenEintraege.Count ; indexHarfe = indexHarfe + 2)
            {
                SchreibeLogEintrag("Und folgendes Unternetz " + indexHarfe);

                var threadUnterNetzKonstruktionOK = new ThreadKonstruiereUnternetze (indexHarfe, aktModellLeiter, "Oberkante", _datenAblage, farbeDerOberkante);
                var startThreadLastOK = new Thread(new ThreadStart(threadUnterNetzKonstruktionOK.read));
                listeDerThreadObjecte.Add(threadUnterNetzKonstruktionOK);
                listeDerThreads.Add(startThreadLastOK);
                startThreadLastOK.Start();

                var threadUnterNetzKonstruktionUK = new ThreadKonstruiereUnternetze(indexHarfe, aktModellLeiter, "Unterkante", _datenAblage, farbeDerUnterkante);
                var startThreadLastUK = new Thread(new ThreadStart(threadUnterNetzKonstruktionUK.read));
                listeDerThreadObjecte.Add(threadUnterNetzKonstruktionUK);
                listeDerThreads.Add(startThreadLastUK);
                startThreadLastUK.Start();
            }


        }

        SchreibeLogEintrag("Warte auf Threads");

        // Wenn ein Thread fertig ist, wird dessen Ergebnis ausgelesen und das GameObject gebaut 
        for (int zeigerAufDenThreadIndex = 0; zeigerAufDenThreadIndex < listeDerThreads.Count; zeigerAufDenThreadIndex++)
        {
            listeDerThreads[zeigerAufDenThreadIndex].Join(); // Warten auf den Thread

            SchreibeLogEintrag("Folgender Thread ist fertig " + zeigerAufDenThreadIndex);


            GameObject unterNetz;
            Mesh mesh = new Mesh();

            // Auslesen der Daten
            var modellLeiter = listeDerThreadObjecte[zeigerAufDenThreadIndex].getErgebnis().ModellLeiter;
            var oberOderUnterkante = listeDerThreadObjecte[zeigerAufDenThreadIndex].getErgebnis().OberOderUnterkante;
            var dreiecksPunkte = listeDerThreadObjecte[zeigerAufDenThreadIndex].getErgebnis().DreiecksPunkte;
            var vertices = listeDerThreadObjecte[zeigerAufDenThreadIndex].getErgebnis().TmpVertices;
            var farben = listeDerThreadObjecte[zeigerAufDenThreadIndex].getErgebnis().FarbenDerStuetzpunkte;

            SchreibeLogEintrag("" + modellLeiter + " " + oberOderUnterkante + " " + vertices.Count + " "+ farben .Count+ " " + dreiecksPunkte.Count + " ");

            if (oberOderUnterkante.Equals("Oberkante"))
            {
                unterNetz = new GameObject(modellLeiter + oberOderUnterkante + zeigerAufDenThreadIndex);
                
            }
            else
            {
                unterNetz = new GameObject(modellLeiter + oberOderUnterkante + zeigerAufDenThreadIndex);


            }

            unterNetz.AddComponent<MeshFilter>();
            unterNetz.AddComponent<MeshRenderer>();
            unterNetz.GetComponent<Renderer>().material = _standardMaterial;
            mesh.vertices = vertices.ToArray();
            mesh.colors = farben.ToArray();
            mesh.triangles = dreiecksPunkte.ToArray();
            unterNetz.GetComponent<MeshFilter>().mesh = mesh;
            unterNetz.AddComponent<MeshCollider>();


            if (oberOderUnterkante.Equals("Oberkante"))
            {
                unterNetz.name = modellLeiter.ToString() + "-OK";
                UnityEditor.AssetDatabase.CreateAsset(unterNetz.GetComponent<MeshFilter>().mesh, "Assets/Resources/Dreiecksnetze/" + _nameDesSpeicherOrtes + @"/" + _nameDesSpeicherOrtes + "_" + modellLeiter + "_OK" + zeigerAufDenThreadIndex + ".asset");
                _datenAblage.SetLeiterOberkantenObject(modellLeiter, unterNetz);
            }
            else
            {
                unterNetz.name = modellLeiter.ToString() + "-UK";
                UnityEditor.AssetDatabase.CreateAsset(unterNetz.GetComponent<MeshFilter>().mesh, "Assets/Resources/Dreiecksnetze/" + _nameDesSpeicherOrtes + @"/" + _nameDesSpeicherOrtes + "_" + modellLeiter + "_UK" + zeigerAufDenThreadIndex + ".asset");
                _datenAblage.SetLeiterUnterKantenObject(modellLeiter, unterNetz);
            }

            unterNetz.transform.parent = _dreiecksNetz.transform;
        }

    }

    string[] UnterteileKarte(string geleseneZeile, int laenge)
    {
        var laengeDerZeile = geleseneZeile.Length;
        int zuLesendeEintraege = (laengeDerZeile + laenge - 1) / laenge;
        string[] result = new string[zuLesendeEintraege];
        for (int index = 0; index < zuLesendeEintraege; ++index)
        {
            result[index] = geleseneZeile.Substring(index * laenge, Mathf.Min(laenge, laengeDerZeile));
            laengeDerZeile -= laenge;
        }
        return result;
    }



    void SaveAndRefresh()
    {
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }


    Mesh FuelleDreiecksMesh(string oberOderUnterKante, int aktGruppe, Vector3[] oberOderUnterKanteGeometrie)
    {

        Mesh mesh = new Mesh();
        Vector3[] verticesSortiert = BaueVertices(aktGruppe, oberOderUnterKanteGeometrie);
        int[] indecies = new int[_anzKnotenProLeiter];

        //Color[] myColors = SucheNegativeGeologieUndKopplungen(oberOderUnterKante, aktGruppe, indecies);
        Color[] myColors = SucheNegativeGeologieUndKopplungen(oberOderUnterKante, aktGruppe);

        mesh.vertices = verticesSortiert;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
        mesh.colors = myColors;

        SchreibeLogEintrag("Anzahl von Indizes " + indecies.Length);

        mesh.triangles = _bearbeiteteDreiecke;



        return mesh;
    }





    Vector3[] BaueVertices(int gruppe, Vector3[] oberOderUnterKante)
    {

        // Vector3[] baueVertices(int gruppe,  Vector3[] oberOderUnterKante)
        // --- in --> 
        // int gruppe :: Je Grundwasserleiter sind die Knoten des Netzes zu einer Gruppe zusammengefasst. 12 Grundwasserleiter ergeben 12 Gruppen
        // Vector3[] oberOderUnterKante :: Array mit den Höhen der Ober-, oder Unterkante
        // <-- out --
        // Vector3[] :: Belegung der Knoten des Netztes mit den jeweiligen Höhen
        // ----------------------------------------------------------------------------------------------------------------------------------------
        // Innerhalb des Arrays "arrayDreiecke" sind die Stützpunkte aller Dreiecke gespeichert  [1 2 3] [2 3 4]
        // Die Dreiecke sind bereits einmalig in jeder Drehrichhtung vorhanden - Dadurch wird das back culling umgangen [1 2 3] [3 2 1] [2 3 4] [4 3 2]
        // Da das Daten10 nicht immer sortiert ist, erfolgt eine numerische Sortierung innerhalb der Funktionen 
        // Die erzeugten Vertices umfassen einen Array mit allen in den mesh.triangle genannten Stützpunkten. Je nach Leiter
        // erhalten diese neue Werte. 


        var arrayListVertices = new List<Vector3>();

        for (int i = 0; i < _anzKnotenProLeiter; i++)
            arrayListVertices.Add(new Vector3(0, 0, 0));



        for (int i = 0; i < _bearbeiteteDreiecke.Length - 1; i++)
        {

            arrayListVertices.RemoveAt(_bearbeiteteDreiecke[i]);
            arrayListVertices.Insert(_bearbeiteteDreiecke[i], oberOderUnterKante[_bearbeiteteDreiecke[i] + gruppe * _anzKnotenProLeiter] - _kleinsteKoordinatenUndHoehe);

        }

        VerticeListe = arrayListVertices.ToArray();

        SchreibeLogEintrag("Anzahl der Vertices " + VerticeListe.Length.ToString());
        return VerticeListe;
    }

   


   // Color[] SucheNegativeGeologieUndKopplungen(string oberOderUnterkante, int aktLeiter, int[] indecies)
    Color[] SucheNegativeGeologieUndKopplungen(string oberOderUnterkante, int aktLeiter)
    {
        Color[] farbenDerStuetzpunkte = new Color[_anzKnotenProLeiter];


        if (oberOderUnterkante.Equals("Unterkante") && aktLeiter < _anzModellLeiter-1)
        {
            for (int knoten = 0; knoten < _anzKnotenProLeiter; knoten++)
            {
                if (_vectorDerLeiterUnterKanten[aktLeiter * _anzKnotenProLeiter + knoten].z == _vectorDerLeiterOberkanten[(aktLeiter + 1) * _anzKnotenProLeiter + knoten].z)
                {
     //               schreibeLogeintrag(farbenDerStuetzpunkte[knoten].ToString() + " Clear " + LeiterUnterkante[AktLeiter * AnzKnotenProLeiter + knoten].z + " " + LeiterOberkante[(AktLeiter + 1) * AnzKnotenProLeiter + knoten].z + " " + AktLeiter + " "  + knoten);
                     farbenDerStuetzpunkte[knoten] = Color.clear;
      
                }
                else if (_vectorDerLeiterUnterKanten[aktLeiter * _anzKnotenProLeiter + knoten].z < _vectorDerLeiterOberkanten[(aktLeiter + 1) * _anzKnotenProLeiter + knoten].z)
                {
    //                schreibeLogeintrag(farbenDerStuetzpunkte[knoten].ToString() + " RED " + LeiterUnterkante[AktLeiter * AnzKnotenProLeiter + knoten].z + " " + LeiterOberkante[(AktLeiter + 1) * AnzKnotenProLeiter + knoten].z + " " + AktLeiter + " " + knoten);
                    farbenDerStuetzpunkte[knoten] = Color.red;
                    SchreibeLogEintrag(_vectorDerLeiterUnterKanten[aktLeiter * _anzKnotenProLeiter + knoten].z.ToString() + " " + _vectorDerLeiterOberkanten[(aktLeiter + 1) * _anzKnotenProLeiter + knoten].z.ToString());
                }
                else
                {
                    SetzteFarbeNachAuswahlkritierium(aktLeiter, knoten, farbenDerStuetzpunkte);
                    
                }

     //           indecies[knoten] = knoten;
                
            }
        }
        else if (oberOderUnterkante.Equals("Oberkante") && aktLeiter != 0)
        {
            for (int knoten = 0; knoten < _anzKnotenProLeiter; ++knoten)
            {
                if (_vectorDerLeiterOberkanten[aktLeiter * _anzKnotenProLeiter + knoten].z == _vectorDerLeiterUnterKanten[(aktLeiter - 1) * _anzKnotenProLeiter + knoten].z)
                {
                    farbenDerStuetzpunkte[knoten] = Color.clear;
            
                }
                else if (_vectorDerLeiterOberkanten[aktLeiter * _anzKnotenProLeiter + knoten].z > _vectorDerLeiterUnterKanten[(aktLeiter - 1) * _anzKnotenProLeiter + knoten].z)
                {
                    farbenDerStuetzpunkte[knoten] = Color.red;
                }
                else
                {
                    SetzteFarbeNachAuswahlkritierium(aktLeiter, knoten, farbenDerStuetzpunkte);
                   // farbenDerStuetzpunkte[Knoten] = FarbwerteDerKnoten[AktLeiter * AnzKnotenProLeiter + Knoten];
                }

         //       indecies[knoten] = knoten;
            }
        }
        else
        {
            for (int knoten = 0; knoten < _anzKnotenProLeiter; ++knoten)
            {
                // farbenDerStuetzpunkte[knoten] = FarbwerteDerKnoten[AktLeiter * AnzKnotenProLeiter + knoten];
                SetzteFarbeNachAuswahlkritierium(aktLeiter, knoten, farbenDerStuetzpunkte);
         //       indecies[knoten] = knoten;
            }

        }
        
        return farbenDerStuetzpunkte;
    }
    
    void SetzteFarbeNachAuswahlkritierium(int aktLeiter, int index, Color[] farbenDerStuetztpunkte)
    {
        if(_datenAblage.getAbfrageWirdKFWertGefordert())
        {
            farbenDerStuetztpunkte[index] = _farbwerteDerKnoten[aktLeiter * _anzKnotenProLeiter + index];
        }
        else
        {
            switch (aktLeiter)
            {
                case 0:
                    farbenDerStuetztpunkte[index] = Color.blue;
                    break;
                case 1:
                    farbenDerStuetztpunkte[index] = Color.cyan;
                    break;
                case 2:
                    farbenDerStuetztpunkte[index] = Color.gray;
                    break;
                case 3:
                    farbenDerStuetztpunkte[index] = Color.green;
                    break;
                case 4:
                    farbenDerStuetztpunkte[index] = Color.magenta;
                    break;
                case 5:
                    farbenDerStuetztpunkte[index] = Color.black;
                    break;
                case 6:
                    farbenDerStuetztpunkte[index] = Color.white;
                    break;
                case 7:
                    farbenDerStuetztpunkte[index] = Color.yellow;
                    break;
                case 8:
                    farbenDerStuetztpunkte[index] = Color.blue;
                    break;
                case 9:
                    farbenDerStuetztpunkte[index] = Color.cyan;
                    break;
                case 10:
                    farbenDerStuetztpunkte[index] = Color.gray;
                    break;
                case 11:
                    farbenDerStuetztpunkte[index] = Color.green;
                    break;
                default:
             
                    break;
            }
        }
        
    }
 

   

    void BauePunktMesh(int aktGruppe, int maxGruppenKnoten)
    {
		// Bause das Mesh
		GameObject punkteMesh = new GameObject (_nameDesSpeicherOrtes + aktGruppe);
		punkteMesh.AddComponent<MeshFilter> ();
		punkteMesh.AddComponent<MeshRenderer> ();


        //punkteMesh.GetComponent<Renderer>().material = _standardMaterial;

        // Das Mesh wird gefüllt
        punkteMesh.GetComponent<MeshFilter>().mesh = FuellePunktMesh(aktGruppe, maxGruppenKnoten);

        // Ordnerhierarchy 
        punkteMesh.transform.parent = _punkteWolke.transform;


		// Das erzeugte Mesh wird gespeichert
		UnityEditor.AssetDatabase.CreateAsset(punkteMesh.GetComponent<MeshFilter> ().mesh, "Assets/Resources/Punktwolken/" + _nameDesSpeicherOrtes + @"/" + _nameDesSpeicherOrtes + aktGruppe + ".asset");
		UnityEditor.AssetDatabase.SaveAssets ();
		UnityEditor.AssetDatabase.Refresh();
	}

    Mesh FuellePunktMesh(int aktGruppe, int maxGruppenKnoten)
    {
		
		Mesh mesh = new Mesh ();
		
		Vector3[] myPoints = new Vector3[maxGruppenKnoten]; 
		int[] indecies = new int[maxGruppenKnoten];
		Color[] myColors = new Color[maxGruppenKnoten];

		for(int i=0; i< maxGruppenKnoten; ++i)
        {
            myPoints[i] = _vectorDerLeiterOberkanten[aktGruppe * maxGruppenKnoten + i] - _kleinsteKoordinatenUndHoehe;
            indecies[i] = i;
            myColors[i] = _farbwerteDerKnoten[aktGruppe * maxGruppenKnoten + i ];
   //         logger.WriteLine("farbe " + myColors[i] + " " + i);
        }


		mesh.vertices = myPoints;
		mesh.colors = myColors;
		mesh.SetIndices(indecies, MeshTopology.Points,0);
	//	mesh.uv = new Vector2[maxGruppenKnoten];
		mesh.normals = new Vector3[maxGruppenKnoten];


		return mesh;
	}

    // void setzeKoordinatenUrsprung(Vector3[] knotenPunktMesh)
    // knotenPunktMesh Vector3[] :: Jeder Knoten des Netzes wird eingelesen um den Punkt mit den kleinsten Koordinaten zu finden
    // Durch die Subtraktion dieser Koordinaten wird das Mesh in den Ursprung (0/0) gelegt
	void KorrekturDerRaumlage(Vector3[] vectorDerLeiterOberOderUnterKante)
    {
        for (int index = 0; index < vectorDerLeiterOberOderUnterKante.Length; index++)
        {


            if (_kleinsteKoordinatenUndHoehe.magnitude == 0)
                _kleinsteKoordinatenUndHoehe = vectorDerLeiterOberOderUnterKante[index];

            if (vectorDerLeiterOberOderUnterKante[index].x < _kleinsteKoordinatenUndHoehe.x)
                _kleinsteKoordinatenUndHoehe.x = vectorDerLeiterOberOderUnterKante[index].x;
              
            if (vectorDerLeiterOberOderUnterKante[index].y < _kleinsteKoordinatenUndHoehe.y)
                _kleinsteKoordinatenUndHoehe.y = vectorDerLeiterOberOderUnterKante[index].y;
        }

        _kleinsteKoordinatenUndHoehe.z = 0;

        _datenAblage.SetKleinsteKoordinate(_kleinsteKoordinatenUndHoehe);
        SchreibeLogEintrag("Kleinste Koordinate "  + (_kleinsteKoordinatenUndHoehe.x).ToString() + " " + (_kleinsteKoordinatenUndHoehe.y).ToString() + " " + (_kleinsteKoordinatenUndHoehe.z).ToString() );

	}


    public void SchreibeLogEintrag(string zuSchreibenderText)
    {


        

        lock (Lock)
        {
            _logger.WriteLine("-----------------------------------------------");
            _logger.WriteLine(System.DateTime.Now + " " + zuSchreibenderText);
            _logger.WriteLine("");
            _logger.Flush();
        }




    }


    void OnGUI()
    {
		if (!NeuLaden){
			GUI.BeginGroup (new Rect(Screen.width/2-100, Screen.height/2, 400.0f, 20));
			GUI.Box (new Rect (0, 0, 400.0f, 20.0f), _anzeigeText);
            GUI.Box (new Rect (0, 0, _fortschrittsAnzeige*400.0f, 20), "");
			GUI.EndGroup ();
		}
	}

    public Material[] getNetzMaterialOnOff(GameObject netz)
    {

        Material[] materialBeschreibung = netz.GetComponent<Renderer>().materials;

        try
        {
            if (materialBeschreibung.Length < 2)
            {
                materialBeschreibung = new Material[2];
                materialBeschreibung[0] = _standardMaterial;
                materialBeschreibung[1] = _datenAblage.GetNetzMaterial();
                materialBeschreibung[1].renderQueue = 3000;
            }
            else
            {
                materialBeschreibung = new Material[1];
                materialBeschreibung[0] = _standardMaterial;
            }

        }
        catch
        {
            
        }
        finally
        {

        }


        return materialBeschreibung;
    }

    public Material[] getEinfaerbungOnOff(GameObject netz)
    {

        Material[] materialBeschreibung = netz.GetComponent<Renderer>().materials;


        if (materialBeschreibung.Length < 2)
        {
            materialBeschreibung = new Material[2];
            materialBeschreibung[0] = _standardMaterial;
            materialBeschreibung[1] = _datenAblage.GetNetzMaterial();
            materialBeschreibung[1].renderQueue = 3000;
        }
        else
        {
            materialBeschreibung = new Material[1];
            materialBeschreibung[0] = _datenAblage.GetNetzMaterial();
            materialBeschreibung[0].renderQueue = 3000;
        }


        return materialBeschreibung;
    }

    public Camera setCameraParameters(Camera cam, GameObject uebergeordnetesGameobject)
    {


        var positionsVektor = new Vector3(uebergeordnetesGameobject.transform.position.x + 9365, uebergeordnetesGameobject.transform.position.y - 23373, uebergeordnetesGameobject.transform.position.z + 6745);
        cam.transform.position = positionsVektor;

        cam.transform.rotation = Quaternion.Euler(-75, 198, 0);

        //cam.fieldOfView = 10;
        // cam.farClipPlane = 100000F;

        cam.fieldOfView = 30;
        cam.farClipPlane = 500000F;

        cam.gameObject.AddComponent(typeof(FRowController));
        cam.gameObject.GetComponent<FRowController>()._datenAblage = _datenAblage;
        cam.gameObject.GetComponent<FRowController>().enabled = true;


        cam.gameObject.AddComponent(typeof(KeyboardController));
        cam.gameObject.GetComponent<KeyboardController>().ZuDrehendesGameObject = uebergeordnetesGameobject.transform;
        cam.gameObject.GetComponent<KeyboardController>().modellViewer = this;
        cam.gameObject.GetComponent<KeyboardController>().datenAblage = _datenAblage;
        cam.gameObject.GetComponent<KeyboardController>().enabled = true;


        cam.gameObject.AddComponent(typeof(NumpadController));
        cam.gameObject.GetComponent<NumpadController>().ZuDrehendesGameObject = uebergeordnetesGameobject.transform;
        cam.gameObject.GetComponent<NumpadController>().mV = this;
        cam.gameObject.GetComponent<NumpadController>()._datenAblage = _datenAblage;
        cam.gameObject.GetComponent<NumpadController>().enabled = true;


        cam.gameObject.AddComponent(typeof(MouseController));
        cam.gameObject.GetComponent<MouseController>().ZuDrehendesGameObject = uebergeordnetesGameobject.transform;
        cam.gameObject.GetComponent<MouseController>().modellViewer = this;
        cam.gameObject.GetComponent<MouseController>()._datenAblage = _datenAblage;
        cam.gameObject.GetComponent<MouseController>().enabled = true;


        return cam;
    }

}

