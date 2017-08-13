using UnityEngine;
using UnityEngine.SceneManagement;

public class WechselSzene : MonoBehaviour
{
    private DataSingleton _datenAblage;

    private void Start()
    {
        _datenAblage = DataSingleton.GetInstanz();
    }

    private bool Kontrolle()
    { 

        if (!(_datenAblage.GetAblagePfad() == (null) && _datenAblage.GetDateiPfad() == (null)))
            return true;

        return false;
    }

    public void LoadByIndex(int szenenIndex)
    {
        if(Kontrolle() || szenenIndex != 1)
        {
            SceneManager.LoadScene(szenenIndex);
        }
        

    }
}
