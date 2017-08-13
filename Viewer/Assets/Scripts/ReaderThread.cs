using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;

public class ReaderThread
{
    // Insgesamt sind 12 Grundwasserleiter im Datensatz zu lesen. Jeder Grundwasserleiter besitzt die selbe Anzahl an Knoten.
    // Für jeden Grundwasserleiter wird ein Thread gestartet, der die Höhenlagen aus der Ursprungsdatei, für jeden Knoten des jeweiligen Leiters ausliest.
    // Zuvor wurden alle Knoten mit ihren X und > Koordinaten in einem Array des Typs Vector3[] gespeichert. 
    // Bei 26000 Knoten würde der erste Grundwasserleiter im benannten Array den Speicherplatz 0 - 25999 belegen, der zweite Leiter 26000 - 51999 usw.

    StreamReader reader;
    private Vector3[] leiterOberkantePunktMesh;
    private Vector3[] leiterUnterkantePunktMesh;
    private string[] buffer;
    private int aktuellerLeiter;
    private string geleseneZeile;
    private int anzKnotenProLeiter;
    private int skalierung;
    private string block;
    private Color[] farbe;
    StreamWriter sr;

    private DataSingleton dataSingleton;


    public ReaderThread(string block,int aktuellerLeiter, DataSingleton dataSingleton)
    {

        this.aktuellerLeiter = aktuellerLeiter;

        this.dataSingleton = dataSingleton;
        reader = new StreamReader(dataSingleton.GetDateiPfad());
        anzKnotenProLeiter = dataSingleton.GetAnzKnotenProLeiter();
        leiterOberkantePunktMesh = dataSingleton.GetVectorDerLeiterOberkanten();
        leiterUnterkantePunktMesh = dataSingleton.GetVectorDerLeiterUnterKanten();
        farbe = dataSingleton.GetFarbe();
  
        skalierung = dataSingleton.GetSkalierung();
        this.block = block;

        sr = new StreamWriter("Logger" + block);

    }


	public void read()
    {
        // Erstmaliges lesen der Karte 

        sr.WriteLine("Bin gestartet " + block);
        sr.Flush();

        geleseneZeile = reader.ReadLine();



        while (!geleseneZeile.Contains(block) && !geleseneZeile.Contains("BLOCK  8"))  
        {
            geleseneZeile = reader.ReadLine();
        }

        geleseneZeile = reader.ReadLine();
        buffer = unterteileKarte(geleseneZeile, 10);


        do    
        {
            if (!buffer[0].Contains(block))
            {

                float xOK = leiterOberkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].x;
                float yOK = leiterOberkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].y;
                leiterOberkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].Set(xOK, yOK , (float.Parse(buffer[3]) )*-1);



                float xUK = leiterUnterkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].x;
                float yUK = leiterUnterkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].y;
                leiterUnterkantePunktMesh[(int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter].Set(xUK, yUK , (float.Parse(buffer[2]))*-1);

                setzeFarbe(float.Parse(buffer[5]), ((int.Parse(buffer[1]) - 1) + aktuellerLeiter * anzKnotenProLeiter));
                
            }


            geleseneZeile = reader.ReadLine();
            buffer = unterteileKarte(geleseneZeile, 10);

        } while (!buffer[0].Contains("        -1"));

        sr.WriteLine("Fertig");
        sr.Flush();
    }

    string[] unterteileKarte(string value, int length)
    {
        int strLength = value.Length;
        int strCount = (strLength + length - 1) / length;
        string[] result = new string[strCount];
        for (int i = 0; i < strCount; ++i)
        {
            result[i] = value.Substring(i * length, Mathf.Min(length, strLength));
            strLength -= length;
        }
        return result;
    }

    void setzeFarbe(float kFWert, int knoten)
    {
        if (kFWert < 0.000001 && kFWert > 0.0000001)
        {
            farbe[knoten] = Color.black;
        }
        else if (kFWert < 0.00001 && kFWert > 0.000001)
        {
            farbe[knoten] = Color.gray;
        }
        else if (kFWert < 0.0001 && kFWert > 0.00001)
        {
            farbe[knoten] = Color.cyan;
        }
        else if (kFWert < 0.001 && kFWert > 0.0001)
        {
            farbe[knoten] = Color.green;
        }
        else if (kFWert < 0.01 && kFWert > 0.001)
        {
            farbe[knoten] = Color.blue;
        }

    }


}
