using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TempGUi : MonoBehaviour {

    public Text Health;
    public Text MemoryFrags;
    public Text OtherCollectibles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Health.text = Character_Manager.instance.currentHealth.ToString();
        MemoryFrags.text = Character_Manager.instance.totalMemoryFragmentsCollected.ToString();
        OtherCollectibles.text = Character_Manager.instance.totalOtherCollectsCollected.ToString();

        if(Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("MainMenu");
        }

	}
}
