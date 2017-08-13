using System.IO;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{
    private int _zoomGeschwindigkeit = 100000;
    private int _bewegungsGeschwindigkeit = 5000;
    private int _rotationsGeschwindigkeit = 10;

    public Transform ZuDrehendesGameObject;
    public DataSingleton _datenAblage;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public ModellViewer modellViewer;
    private List<GameObject> aktiveGameobjectListe;

    float x = 0.0f;
    float y = 0.0f;



    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        x = angles.y;
        y = angles.x;


     
    }



    void Update()
    {
  
        // Mausrad

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
           transform.Translate(new Vector3(0, 0, _zoomGeschwindigkeit * Time.deltaTime));
        }


        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
           transform.Translate(new Vector3(0, 0, -_zoomGeschwindigkeit * Time.deltaTime));
        }


        // Mausknöpfe


        if (Input.GetMouseButtonDown(0))
        {

            modellViewer.SchreibeLogEintrag("Maustaste 0 gedrückt ");

            RaycastHit hitInfo = new RaycastHit();

            modellViewer.SchreibeLogEintrag("Maustaste 0 gedrückt " + hitInfo.ToString());

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {

                var grundwasserleiter = hitInfo.collider.gameObject;
                

                MeshCollider meshCollider = hitInfo.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                    return;

                Mesh mesh = meshCollider.sharedMesh;
                Vector3[] alleStuetzpunkteDesLeiters = mesh.vertices;
                int[] alleDreieckeDesLeiters = mesh.triangles;

                Vector3 punkt1 = alleStuetzpunkteDesLeiters[alleDreieckeDesLeiters[hitInfo.triangleIndex * 3 + 0]];
                Vector3 punkt2 = alleStuetzpunkteDesLeiters[alleDreieckeDesLeiters[hitInfo.triangleIndex * 3 + 1]];
                Vector3 punkt3 = alleStuetzpunkteDesLeiters[alleDreieckeDesLeiters[hitInfo.triangleIndex * 3 + 2]];

                modellViewer.SchreibeLogEintrag("Folgende KnotenWerte sind für den Knoten 1 hinterlegt " + (alleDreieckeDesLeiters[hitInfo.triangleIndex * 3] + 1).ToString() + " " + (2512658 + punkt1.x).ToString() + " " + (5663052 + punkt1.y).ToString() + " " + punkt1.z.ToString());
                modellViewer.SchreibeLogEintrag("Folgende KnotenWerte sind für den Knoten 2  hinterlegt " + (alleDreieckeDesLeiters[hitInfo.triangleIndex * 3 + 1] + 1).ToString() + " " + (2512658 + punkt2.x).ToString() + " " + (5663052 + punkt2.y).ToString() + " " + punkt2.z.ToString());
                modellViewer.SchreibeLogEintrag("Folgende KnotenWerte sind für den Knoten 3  hinterlegt " + (alleDreieckeDesLeiters[hitInfo.triangleIndex * 3 + 2] + 1).ToString() + " " + (2512658 + punkt3.x).ToString() + " " + (5663052 + punkt3.y).ToString() + " " + punkt3.z.ToString());
            }
        }
    

        if (Input.GetMouseButtonDown(2))
        {
            modellViewer.SchreibeLogEintrag("Maustaste 2 gedrückt ");
            RaycastHit hitInfo = new RaycastHit();

            modellViewer.SchreibeLogEintrag("Maustaste 2 gedrückt " + hitInfo.ToString());

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {

                var grundwasserleiter = hitInfo.collider.gameObject;
                var material = modellViewer.getNetzMaterialOnOff(grundwasserleiter);
                var leiterInformation = UnterteileKarte(grundwasserleiter.name);

                modellViewer.SchreibeLogEintrag("Finde folgender Leiter " + leiterInformation[0] + leiterInformation[1]);

                if(leiterInformation[1].Equals("OK"))
                {
                    aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(Int32.Parse(leiterInformation[0]));
                }
                else
                {
                    aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(Int32.Parse(leiterInformation[0]));
                }
                

                for (int index = 0; index < aktiveGameobjectListe.Count; index++)
                {
                    modellViewer.SchreibeLogEintrag("Anzahl der Unterzetze " + index);

                    aktiveGameobjectListe[index].GetComponent<Renderer>().materials = material;
                }

            }
        }

        if (Input.GetMouseButtonDown(1))
        {

            modellViewer.SchreibeLogEintrag("Maustaste 3 gedrückt ");
            RaycastHit hitInfo = new RaycastHit();



            modellViewer.SchreibeLogEintrag("Maustaste 3 gedrückt " + hitInfo.ToString());

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
            {


                var grundwasserleiter = hitInfo.collider.gameObject;
                var material = modellViewer.getEinfaerbungOnOff(grundwasserleiter);
                var leiterInformation = UnterteileKarte(grundwasserleiter.name);

                if (leiterInformation[1].Equals("OK"))
                {
                    aktiveGameobjectListe = _datenAblage.GetLeiterOberkantenObject(Int32.Parse(leiterInformation[0]));
                }
                else
                {
                    aktiveGameobjectListe = _datenAblage.GetLeiterUnterKantenObject(Int32.Parse(leiterInformation[0]));
                }

                for (int index = 0; index < aktiveGameobjectListe.Count; index++)
                {
                    aktiveGameobjectListe[index].GetComponent<Renderer>().materials = material;
                }

               
            }
        }


        // Mausachsen

        if (ZuDrehendesGameObject && Input.GetKey("left ctrl") && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {

            // Make the rigid body not change rotation


            //  x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            //   y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

    //        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            /*      RaycastHit hit;
                  if (Physics.Linecast(target.position, transform.position, out hit))
                  {
                      distance -= hit.distance;
                  }
             */

      //      Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
     //       Vector3 position = rotation * negDistance - ZuDrehendesGameObject.position;
            //   Vector3 position = rotation * new Vector3(0,0,0) + target.position;

        //    transform.position = transform.position;
            transform.rotation = rotation;
      //      transform.position = position;
        }


    }

    string[] UnterteileKarte(string geleseneZeile)
    {
        var aufgeteilterDateiName = geleseneZeile.Split('-');


        return aufgeteilterDateiName;

    }



    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}