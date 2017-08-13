using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{

    // Wechsel der Szenen der grafischen Benutzeroberfläche. Der Index wird in den Buildsettings umgesetzt.
	public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
