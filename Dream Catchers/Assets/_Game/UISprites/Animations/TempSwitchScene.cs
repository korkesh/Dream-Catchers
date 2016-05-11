using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TempSwitchScene : MonoBehaviour {

    public string NextScene;

	public void loadNext()
    {
        SceneManager.LoadScene(NextScene);
    }
}
