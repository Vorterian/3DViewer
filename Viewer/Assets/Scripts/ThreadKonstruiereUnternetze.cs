using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class ThreadKonstruiereUnternetze
{
    private Color[] _myColors;
    private List<List<int>> _harfenEintraege;
    private int _unternetz;
    private int _modellLeiter;
    private string _oberOderUnterkante;
    private int _anzKnotenProLeiter;
    private DatenAblageDerUnternetze antwort;
    private Vector3 _kleinsteKoordinatenUndHoehe;
    private Vector3[] _vectorDerLeiterOberkanten;
    private Vector3[] _vectorDerLeiterUnterKanten;
    private StreamWriter _logger;


    public ThreadKonstruiereUnternetze(int unternetz,  int modellLeiter, string oberOderUnterkante, DataSingleton datenAblage, Color[] farbe)
    {


        _harfenEintraege = datenAblage.HarfenEintraege;
        _kleinsteKoordinatenUndHoehe = datenAblage.GetKleinsteKoordinate();
        _logger = new StreamWriter("LogTest"+unternetz+ modellLeiter+ oberOderUnterkante+".txt");
        _myColors = farbe;
        _unternetz = unternetz;
        _modellLeiter = modellLeiter;
        _oberOderUnterkante = oberOderUnterkante;
        _anzKnotenProLeiter = datenAblage.GetAnzKnotenProLeiter();
        _vectorDerLeiterUnterKanten = datenAblage.GetVectorDerLeiterUnterKanten();
        _vectorDerLeiterOberkanten = datenAblage.GetVectorDerLeiterOberkanten();
        antwort = new DatenAblageDerUnternetze();

        SchreibeLogEintrag("Thread initialisiert");
    }


			
	public void read()
	{

        SchreibeLogEintrag("Thread gestartet " + _harfenEintraege.Count);

        var meineDreiecksPunkte = _harfenEintraege[_unternetz];
        SchreibeLogEintrag("Dreiecksgroesse " + meineDreiecksPunkte.Count);
        var meineHarfe = _harfenEintraege[_unternetz+1];
        SchreibeLogEintrag("Harfen Groesse  " + meineHarfe.Count);
        var tmpVertices = new List<Vector3>();  
		var farbenDerStuetzpunkte = new List<Color>();


        for (int i = 0; i < meineHarfe.Count; i++)
        {
            farbenDerStuetzpunkte.Add(new Color(0.27f, 0.27f, 0.27f, 0));
            tmpVertices.Add(new Vector3(-9999, -9999, -9999));
        }

        try
        {
            for (int indexFuerDasUnternetz = 0; indexFuerDasUnternetz < meineDreiecksPunkte.Count; indexFuerDasUnternetz++)
            {

               
                farbenDerStuetzpunkte.RemoveAt(meineDreiecksPunkte[indexFuerDasUnternetz]);
                farbenDerStuetzpunkte.Insert(meineDreiecksPunkte[indexFuerDasUnternetz], _myColors[meineDreiecksPunkte[indexFuerDasUnternetz]]);


                if (_oberOderUnterkante.Equals("Oberkante"))
                {
                    tmpVertices.RemoveAt(meineDreiecksPunkte[indexFuerDasUnternetz]);
                    tmpVertices.Insert(meineDreiecksPunkte[indexFuerDasUnternetz],_vectorDerLeiterOberkanten[meineHarfe[meineDreiecksPunkte[indexFuerDasUnternetz]] + _modellLeiter * _anzKnotenProLeiter] - _kleinsteKoordinatenUndHoehe);

                }
                else
                {
                    tmpVertices.RemoveAt(meineDreiecksPunkte[indexFuerDasUnternetz]);
                    tmpVertices.Insert(meineDreiecksPunkte[indexFuerDasUnternetz],_vectorDerLeiterUnterKanten[meineHarfe[meineDreiecksPunkte[indexFuerDasUnternetz]] + _modellLeiter * _anzKnotenProLeiter] - _kleinsteKoordinatenUndHoehe);

                }

                SchreibeLogEintrag(meineDreiecksPunkte[indexFuerDasUnternetz] + " " + indexFuerDasUnternetz);
            }
            
        }
        catch(Exception e)
        {
            SchreibeLogEintrag("Folgender Fehler "+ e.ToString());
        }

        var tmpVector = new Vector3(-9999, -9999, -9999);
        tmpVertices.RemoveAll(item => item == tmpVector);

        var tmpColor = new Color(0.27f, 0.27f, 0.27f, 0);
        farbenDerStuetzpunkte.RemoveAll(item => item == tmpColor);


        SchreibeLogEintrag("Modellleiter " + _modellLeiter + " " + _unternetz + " " + meineDreiecksPunkte.Count + " " + farbenDerStuetzpunkte.Count + " " + tmpVertices.Count);
            
            antwort.DreiecksPunkte = meineDreiecksPunkte;
            antwort.FarbenDerStuetzpunkte = farbenDerStuetzpunkte;
            antwort.TmpVertices = tmpVertices;
            antwort.OberOderUnterkante = _oberOderUnterkante;



    }
			
			
	public DatenAblageDerUnternetze getErgebnis()
	{
		return antwort;
	}

    void SchreibeLogEintrag(string zuSchreibenderText)
    {

        _logger.WriteLine("-----------------------------------------------");
        _logger.WriteLine(zuSchreibenderText);
        _logger.WriteLine("");
        _logger.Flush();

    }
}