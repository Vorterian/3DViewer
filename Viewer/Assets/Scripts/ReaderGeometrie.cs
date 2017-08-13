using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;


public class ReaderGeometrie
{

    private StreamWriter _logger;

    private int _anzKnotenProLeiter;                 // Headerinformation aus Block 6. Anzahl der horizontalen Knoten
    private int _anzLeiter;                          // Headerinformation aus Block 6. Anzahl der Modellgrundwasserleiter
    private int _anzKnotenImModell;                  // Anzahl allter Knoten im Modell. Ergibt sich aus Horizontale Knoten * Grundwasserleiter
    private int _skalierung;                         // Initialier Z Transformationswert des Objektes

    private Vector3[] _vectorDerLeiterOberkanten;    // Speicherung der Oberkanten aller Knoten im Modell mit x,y,z
    private Vector3[] _vectorDerLeiterUnterkanten;   // Speicherung der Unterkanten aller Knoten im Model mit x,y,z

    private List<int> _listeAllerDreiecksPunkte;     // Auf Grundlage der im Daten10 angegebenen Nachbarschaften werden die Stützpunkte jedes Dreieckes berechnet und abgespeichert

    private DataSingleton _datenAblage;              // Zugriff auf das Singleton



    // Konstuktor der Klasse greift auf das Singleton zu und initalisiert alle notwendigen Variablen
    public ReaderGeometrie(DataSingleton dataSingleton)
    {
        
        _datenAblage = dataSingleton;

        Initialisiere();

        SchreibeLogeintrag("Modell-Klasse initialisiert");

        LeseBlock6(dataSingleton.GetDaten10Leser());

    }

    void Initialisiere()
    {
        _listeAllerDreiecksPunkte = _datenAblage.GetListeAllerDreiecksPunkte();
        _skalierung = _datenAblage.GetSkalierung();
        _logger = _datenAblage.GetLogger();
    }



    /*
        * Aus dem Datensatz werden die Rechts- und Hochwerte jedes einzelnen Punktes ausgelesen.
        * Im Header der Geometrie (BLOCK  6) werden die Anzahl der Grundwasserleiter und die Anzahl der Knoten im Modell benannt
        * 
        * 
        * */
    void LeseBlock6(StreamReader daten10Leser)
    {
      //  string[] buffer;
      //  string karte;
      //  int counter = 0;

        SchreibeLogeintrag("Lese Block 6");

        //var counter = 0;
        var geleseneZeile = daten10Leser.ReadLine();
        var buffer = UnterteileKarte(geleseneZeile, 10);

        // Auslesen der Headerinformationen aus dem Block  6

        _anzKnotenProLeiter = int.Parse(buffer[0]);
        _datenAblage.setAnzKnotenProLeiter(_anzKnotenProLeiter);

        _anzLeiter = int.Parse(buffer[2]);
        _datenAblage.setAnzModellLeiter(_anzLeiter);

        // Initialisierung der von den Headerinformationen abhängigen Variablen
        _anzKnotenImModell = _anzKnotenProLeiter * _anzLeiter;
        _datenAblage.SetVectorDerLeiterOberkanten(new Vector3[_anzKnotenImModell]);
        _datenAblage.SetVectorDerLeiterUnterKanten(new Vector3[_anzKnotenImModell]);
        _datenAblage.setFarbe(new Color[_anzKnotenImModell]);

        _vectorDerLeiterOberkanten = _datenAblage.GetVectorDerLeiterOberkanten();
        _vectorDerLeiterUnterkanten = _datenAblage.GetVectorDerLeiterUnterKanten();



        for (int i = 0; i < _anzKnotenProLeiter; i++)
        {

            geleseneZeile = daten10Leser.ReadLine();


            // Sollte eine Zeile auskommentiert sein, dann wird diese übersprungen
            if (!geleseneZeile.Contains("c"))
            {

                

                // In der Karte werden über eine feste Länge (i10) die ersten drei Werte ausgelesen
                // Diese werte sind (1) die Knotennummer (2) der Rechtswert (3) der Hochwert

                buffer = UnterteileKarte(geleseneZeile, 10);


                //      Bauen der Vertices. Dabei wird der Zentralknoten immer mit zwei Nachbarknoten zu einem Dreieck vereint
                //      Die Daten10 Struktur ist so aufgebaut, dass im Uhrzeigersinn die korrekte Kombination der Eckpunkte angeben ist
                //
                //      0          1              2              3      4          5         6          7        8         9         10        11       
                //      2248    2517121.51    5672097.83      12099   14734      14732     14731      2247     14746     14747     14748     12092
                //
                //      Zuletzt wird die Datenstruktur gesäubert und Doppelnennung von Dreiecken eliminiert.

                for (int zeigerPositionImBuffer = 3; zeigerPositionImBuffer < buffer.Length - 1; zeigerPositionImBuffer++)
                {
                    
                    try
                    {
              
                        var dreiecksPunkt01 = int.Parse(buffer[0]) - 1;
                        var dreiecksPunkt02 = int.Parse(buffer[zeigerPositionImBuffer]) - 1;
                        var dreiecksPunkt03 = int.Parse(buffer[zeigerPositionImBuffer + 1]) - 1;

                        

                        if (dreiecksPunkt01 > dreiecksPunkt03)
                        {
                            var zwischenSpeicher = dreiecksPunkt01;
                            dreiecksPunkt01 = dreiecksPunkt03;
                            dreiecksPunkt03 = zwischenSpeicher;
                        }

                        if (dreiecksPunkt01 > dreiecksPunkt02)
                        {
                            var zwischenSpeicher = dreiecksPunkt01;
                            dreiecksPunkt01 = dreiecksPunkt02;
                            dreiecksPunkt02 = zwischenSpeicher;
                        }

                        if (dreiecksPunkt02 > dreiecksPunkt03)
                        {
                            var zwischenSpeicher = dreiecksPunkt02;
                            dreiecksPunkt02 = dreiecksPunkt03;
                            dreiecksPunkt03 = zwischenSpeicher;
                        }

                        _listeAllerDreiecksPunkte.Add(dreiecksPunkt01);
                        _listeAllerDreiecksPunkte.Add(dreiecksPunkt02);
                        _listeAllerDreiecksPunkte.Add(dreiecksPunkt03);
                    }
                    catch (Exception e)
                    {
                      //  schreibeLogeintrag(buffer.ToString());
                      //  schreibeLogeintrag(e + " " + buffer[0] + " " + buffer[indexDreiecksBildung] + " " + buffer[indexDreiecksBildung + 1]);
                    }


                }

                try
                {
                    // Letztes Dreieck 

                    var dreiecksPunkt01 = int.Parse(buffer[0]) - 1;
                    var dreiecksPunkt02 = int.Parse(buffer[buffer.Length - 1]) - 1;
                    var dreiecksPunkt03 = int.Parse(buffer[3]) - 1;
                    
                  

                    if (dreiecksPunkt01 > dreiecksPunkt03)
                    {
                        var zwischenSpeicher = dreiecksPunkt01;
                        dreiecksPunkt01 = dreiecksPunkt03;
                        dreiecksPunkt03 = zwischenSpeicher;
                    }

                    if (dreiecksPunkt01 > dreiecksPunkt02)
                    {
                        var zwischenSpeicher = dreiecksPunkt01;
                        dreiecksPunkt01 = dreiecksPunkt02;
                        dreiecksPunkt02 = zwischenSpeicher;
                    }

                    if (dreiecksPunkt02 > dreiecksPunkt03)
                    {
                        var zwischenSpeicher = dreiecksPunkt02;
                        dreiecksPunkt02 = dreiecksPunkt03;
                        dreiecksPunkt03 = zwischenSpeicher;
                    }

                    _listeAllerDreiecksPunkte.Add(dreiecksPunkt01);
                    _listeAllerDreiecksPunkte.Add(dreiecksPunkt02);
                    _listeAllerDreiecksPunkte.Add(dreiecksPunkt03);

                }
                catch (Exception e)
                {
                    _logger.WriteLine(buffer.ToString());
                    _logger.WriteLine(e + " " + buffer[0]);
                    _logger.Flush();

                }

               

                // Bis auf die Höheninformationen sind die x,y Koordinaten der Knoten in der vertikalen für jeden Modellleiter identisch
                // Daher findet die Übertragung dieser Informationen direkt statt

                for (int indexGrundwasserLeiter = 0; indexGrundwasserLeiter <= _anzLeiter - 1; indexGrundwasserLeiter++)
                {
                    _vectorDerLeiterOberkanten[(int.Parse(buffer[0]) - 1) + indexGrundwasserLeiter * _anzKnotenProLeiter] = new Vector3(float.Parse(buffer[1]), float.Parse(buffer[2]), 0);
                    _vectorDerLeiterUnterkanten[(int.Parse(buffer[0]) - 1) + indexGrundwasserLeiter * _anzKnotenProLeiter] = new Vector3(float.Parse(buffer[1]), float.Parse(buffer[2]), 0);
                }
            }
        }
    }


    void SchreibeLogeintrag(string loggerText)
    {
        _logger.WriteLine("-----------------------------------------------");
        _logger.WriteLine(System.DateTime.Now + " " + loggerText);
        _logger.WriteLine("");
        _logger.Flush();
    }

    string[] UnterteileKarte(string value, int length)
    {
        var strLength = value.Length;
        int strCount = (strLength + length - 1) / length;
        string[] result = new string[strCount];
        for (int i = 0; i < strCount; ++i)
        {
            result[i] = value.Substring(i * length, Mathf.Min(length, strLength));
            strLength -= length;
        }
        return result;
    }

}


