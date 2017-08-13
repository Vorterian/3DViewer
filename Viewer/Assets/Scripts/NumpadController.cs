using Boo.Lang;
using UnityEngine;

public class NumpadController : MonoBehaviour
{


    public Transform ZuDrehendesGameObject;
    public DataSingleton _datenAblage;
    public ModellViewer mV;

    private List<GameObject> _gameObjectListe;
    private List<Mesh> _meshListe;


    private void Start()
    {
        _gameObjectListe = new List<GameObject>();
        _meshListe = new List<Mesh>();
    }



    void Update()
    {
   
        // Keypad

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Vector3 scale = ZuDrehendesGameObject.localScale;
            Vector3 scaleNew = new Vector3(scale.x, scale.y, scale.z-1);
            ZuDrehendesGameObject.localScale = scaleNew;
        }



        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Vector3 scale = ZuDrehendesGameObject.localScale;
            Vector3 scaleNew = new Vector3(scale.x, scale.y, scale.z + 1);
            ZuDrehendesGameObject.localScale = scaleNew;
        }


        if (Input.GetKey(KeyCode.KeypadMultiply))
        {
            _gameObjectListe.Clear();
            var counterGameobjekte = 0;
            var counterMeshfilter = 0;
            var counterVector3 = 0;

            foreach (Transform child in ZuDrehendesGameObject)
            {
                counterGameobjekte++;
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
                        vertices[index].z = vertices[index].z - 100 / counterVector3;
                    }

                mesh.vertices = vertices;
            }
        }




    }
}