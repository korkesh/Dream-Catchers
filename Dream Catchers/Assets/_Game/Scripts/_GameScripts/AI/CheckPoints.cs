//================================
// Alex
//  checkpoint for players. Save postion to playerPrefs
//================================

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CheckPoints : MonoBehaviour
{

   // public Vector3 position;
   // public bool useCurrentPos;
    public string LevelName;
    public Light lightbulb;
    public GameObject SpawnSpot;
    public BoxCollider BC;

    // Use this for initialization
    void Start()
    {

    }

    //save chcke point
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
          
            Level_Manager.Instance.newCheckPoint(SpawnSpot.transform.position, SpawnSpot.transform.rotation.eulerAngles, SceneManager.GetActiveScene().name);
            Level_Manager.Instance.checkPointContinue = true;
            lightbulb.enabled = true;
            if(BC != null)
            {
                BC.enabled = false;
            }
            gameObject.SendMessage("Play", SendMessageOptions.DontRequireReceiver);
        }

    }
}
