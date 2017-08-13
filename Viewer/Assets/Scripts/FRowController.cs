using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FRowController : MonoBehaviour
{

    public DataSingleton _datenAblage;
    private List<GameObject> aktiveGameobjectListe; 

    private void Start()
    {

    }



    void Update()
    {

        if (Input.GetKey(KeyCode.F1) && Input.GetKey(KeyCode.LeftShift))
        {
            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(0);

            for(int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F2) && Input.GetKey(KeyCode.LeftShift))
        {
            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(1);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F3) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(2);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F4) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(3);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F5) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(4);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F6) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(5);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F7) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(6);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F8) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(7);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }


        if (Input.GetKey(KeyCode.F9) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(8);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }

        if (Input.GetKey(KeyCode.F10) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(9);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }

        if (Input.GetKey(KeyCode.F11) && Input.GetKey(KeyCode.LeftShift))
        {
            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(10);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }

        if (Input.GetKey(KeyCode.F12) && Input.GetKey(KeyCode.LeftShift))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(11);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }
        }

        /// Unterkanten


        if (Input.GetKey(KeyCode.F1) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(0);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F2) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(1);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F3) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(2);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F4) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(3);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F5) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(4);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F6) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(5);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F7) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(6);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F8) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(7);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }


        if (Input.GetKey(KeyCode.F9) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(8);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }

        if (Input.GetKey(KeyCode.F10) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(9);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }

        if (Input.GetKey(KeyCode.F11) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(10);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }

        if (Input.GetKey(KeyCode.F12) && Input.GetKey("left ctrl"))
        {

            aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(11);

            for (int index = 0; index < aktiveGameobjectListe.Count; index++)
            {
                aktiveGameobjectListe[index].SetActive(!aktiveGameobjectListe[index].activeSelf);
            }

        }

    }

}