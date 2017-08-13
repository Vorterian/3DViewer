using UnityEngine;

public class KeyboardController : MonoBehaviour
{

    /* 
     *      Der KeyboardController regelt die Belegung der Pfeiltasten, QWEASD und der Leertaste 
     * */

    public Transform ZuDrehendesGameObject;                 // Zuweisung des Controllers zu dem Gameobject
    public DataSingleton datenAblage;                       // Verweis auf das DataSingelton
    public ModellViewer modellViewer;                       // Verweis auf den ModellViewer              

    private int _bewegungsGeschwindigkeit = 5000;           // Faktor der horizontalen und vertikalen Bewegungsgeschwindigkeit
    private int _rotationsGeschwindigkeit = 30;             // Faktor der Euler Transformation



    private void Start()
    {

    }



    void Update()
    {
   
        // Pfeiltasten

        if (Input.GetKey(KeyCode.RightArrow))
        {

            transform.Translate(new Vector3(_bewegungsGeschwindigkeit * Time.deltaTime, 0, 0));

        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {

            transform.Translate(new Vector3(-_bewegungsGeschwindigkeit * Time.deltaTime, 0, 0));

        }


        if (Input.GetKey(KeyCode.DownArrow))
        {

            transform.Translate(new Vector3(0, -_bewegungsGeschwindigkeit * Time.deltaTime, 0));

        }


        if (Input.GetKey(KeyCode.UpArrow))
        {

            transform.Translate(new Vector3(0, _bewegungsGeschwindigkeit * Time.deltaTime, 0));

        }


        // QWEASD
        if (Input.GetKey(KeyCode.W))
        {

            transform.Rotate(-Vector3.right * _rotationsGeschwindigkeit * Time.deltaTime);
            transform.Translate(new Vector3(0, -_bewegungsGeschwindigkeit * Time.deltaTime, 0));

        }

        if (Input.GetKey(KeyCode.S))
        {

            transform.Rotate(-Vector3.left * _rotationsGeschwindigkeit * Time.deltaTime);
            transform.Translate(new Vector3(0, _bewegungsGeschwindigkeit * Time.deltaTime, 0));

        }

        if (Input.GetKey(KeyCode.Q))
        {

            transform.Rotate(Vector3.forward * _rotationsGeschwindigkeit * 2 * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.E))
        {

            transform.Rotate(Vector3.back * _rotationsGeschwindigkeit * 2 * Time.deltaTime);

        }


        if (Input.GetKey(KeyCode.A))
        {

            transform.RotateAround(ZuDrehendesGameObject.position, Vector3.forward, _rotationsGeschwindigkeit * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.D))
        {

            transform.RotateAround(ZuDrehendesGameObject.position, Vector3.back, _rotationsGeschwindigkeit * Time.deltaTime);

        }

        // Leertaste
        if (Input.GetKey(KeyCode.Space))
        {


            transform.LookAt(ZuDrehendesGameObject.position);
        }
    }
}