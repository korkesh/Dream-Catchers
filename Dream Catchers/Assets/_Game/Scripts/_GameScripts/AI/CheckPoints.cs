//================================
// Alex
//  checkpoint for players. Save postion to playerPrefs
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CheckPoints : MonoBehaviour
{

    public Vector3 position;
    public bool useCurrentPos;
    public string LevelName;
    public Light lightbulb;

    // Use this for initialization
    void Start()
    {

        if (useCurrentPos == true)
        {
            position = this.transform.position + this.transform.forward;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Level_Manager.Instance.newCheckPoint(position, transform.rotation.eulerAngles, SceneManager.GetActiveScene().name);
            Level_Manager.Instance.checkPointContinue = true;
            lightbulb.enabled = true;
            gameObject.SendMessage("Play", SendMessageOptions.DontRequireReceiver);
        }

    }
}
