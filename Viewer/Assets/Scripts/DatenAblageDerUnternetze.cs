using UnityEngine;
using System.Collections.Generic;
using System;


public class DatenAblageDerUnternetze
{

	private String _oberOderUnterkante;
    private int _modellLeiter;
    private List<int> _dreiecksPunkte;
    private List<Vector3> _tmpVertices;
    private new List<Color>_farbenDerStuetzpunkte;
	
	public DatenAblageDerUnternetze()
	{
		_oberOderUnterkante = "";
		_modellLeiter = 0;
		_dreiecksPunkte = new List<int>();
		_tmpVertices = new List<Vector3>();
		_farbenDerStuetzpunkte = new List<Color>();
	}

    public string OberOderUnterkante
    {
        get
        {
            return _oberOderUnterkante;
        }

        set
        {
            _oberOderUnterkante = value;
        }
    }

    public int ModellLeiter
    {
        get
        {
            return _modellLeiter;
        }

        set
        {
            _modellLeiter = value;
        }
    }

    public List<int> DreiecksPunkte
    {
        get
        {
            return _dreiecksPunkte;
        }

        set
        {
            _dreiecksPunkte = value;
        }
    }

    public List<Vector3> TmpVertices
    {
        get
        {
            return _tmpVertices;
        }

        set
        {
            _tmpVertices = value;
        }
    }

    public List<Color> FarbenDerStuetzpunkte
    {
        get
        {
            return _farbenDerStuetzpunkte;
        }

        set
        {
            _farbenDerStuetzpunkte = value;
        }
    }
}