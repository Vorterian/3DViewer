using Boo.Lang;
using System;
using UnityEngine;

public class NumpadController : MonoBehaviour
{


    public Transform ZuDrehendesGameObject;
    public DataSingleton _datenAblage;
    public ModellViewer mV;

    private List<GameObject> _gameObjectListe;
    private List<Mesh> _meshListe;
    private Vector3 ursprungsSkaliierung;


    private void Start()
    {
        _gameObjectListe = new List<GameObject>();
        _meshListe = new List<Mesh>();
        ursprungsSkaliierung = ZuDrehendesGameObject.localScale;
    }



    void Update()
    {
   
        // Keypad

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Vector3 scale = ZuDrehendesGameObject.localScale;
            if(scale.z >= ursprungsSkaliierung.z)
            {
                Vector3 scaleNew = new Vector3(scale.x, scale.y, scale.z - 1);
                ZuDrehendesGameObject.localScale = scaleNew;
            }

        }



        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Vector3 scale = ZuDrehendesGameObject.localScale;
            Vector3 scaleNew = new Vector3(scale.x, scale.y, scale.z + 1);
            ZuDrehendesGameObject.localScale = scaleNew;
        }


        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad1))
        {
            manipuliereHoehe(0);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad2))
        {
            manipuliereHoehe(1);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad3))
        {
            manipuliereHoehe(2);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad4))
        {
            manipuliereHoehe(3);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad5))
        {
            manipuliereHoehe(4);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad6))
        {
            manipuliereHoehe(5);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad7))
        {
            manipuliereHoehe(6);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad8))
        {
            manipuliereHoehe(7);
        }

        if (Input.GetKey(KeyCode.KeypadMultiply) && Input.GetKey(KeyCode.Keypad9))
        {
            manipuliereHoehe(8);
        }

        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            setzeCollider();
        }


    }


        public void manipuliereHoehe(int betroffeneLeiter)
        {
            _gameObjectListe.Clear();
            var counterGameobjekte = 0;
            var counterMeshfilter = 0;
            var counterVector3 = 0;
            

            foreach (Transform child in ZuDrehendesGameObject)
            {
                counterGameobjekte++;
            var aufgeteilterDateiName = child.gameObject.name.ToString().Split('-');
            if (Int32.Parse(aufgeteilterDateiName[0]) <= betroffeneLeiter)
                _gameObjectListe.Add(child.gameObject);
            }

            mV.SchreibeLogEintrag(counterGameobjekte.ToString());

            foreach (GameObject gameObject in _gameObjectListe)
            {
                counterMeshfilter++;
                _meshListe.Add(gameObject.GetComponent<MeshFilter>().mesh);

            }
            mV.SchreibeLogEintrag(counterMeshfilter.ToString());

            foreach (Mesh mesh in _meshListe)
            {

                Vector3[] vertices = mesh.vertices;
                counterVector3++;

                for (int index = 0; index < vertices.Length; index++)
                {
                    vertices[index].z = vertices[index].z + 10;
                }

                mesh.vertices = vertices;
            }

        }

    public void setzeCollider()
    {

        foreach (Transform child in ZuDrehendesGameObject)
        {

            child.gameObject.AddComponent<MeshFilter>();
          //  child.gameObject.AddComponent<MeshRenderer>();
            child.gameObject.AddComponent<MeshCollider>();
        }


    }
}