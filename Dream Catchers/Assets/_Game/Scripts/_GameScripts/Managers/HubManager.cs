using UnityEngine;
using System.Collections;

public class HubManager : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject tutorialCamera;
    public GameObject level1Camera;
    public GameObject level2Camera;

    public GameObject TutorialDoor;
    public GameObject Level1Door;
    public GameObject Level2Door;

    // Use this for initialization
    void Start () {
        Level_Manager.Instance.FurthestLevelProgressed = (Level_Manager.Levels)PlayerPrefs.GetInt("FurthestLevelProgressed");

        HubDoorToOpen();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //-----------------
    // Hub Logic
    //-----------------
    public void HubDoorToOpen()
    {
        switch (Level_Manager.Instance.FurthestLevelProgressed)
        {
            case Level_Manager.Levels.TUTORIAL:
                {
                    // Open Level 1 Door && close Tutorial Door

                    tutorialCamera.SetActive(true);
                    //mainCamera.SetActive(false);
                    TutorialDoor.GetComponent<LowerDoor>().CloseDoor();

                    StartCoroutine(CameraSwap("Level1"));

                    break;
                }
            case Level_Manager.Levels.CANNON:
                {
                    // Open Level 2 Door && close Level 1 Door

                    level1Camera.SetActive(true);
                    //mainCamera.SetActive(false);
                    Level1Door.GetComponent<LowerDoor>().CloseDoor();

                    StartCoroutine(CameraSwap("Level2"));

                    break;
                }
            case Level_Manager.Levels.DAMAGE:
                {
                    // Allow Boss Door Activation && close Level 2 Door
                    break;
                }
        }
    }

    public IEnumerator CameraSwap(string door)
    {
        yield return new WaitForSeconds(2.5f);

        switch (door)
        {
            case "Level1":
                {
                    // Open Level 1 Door && close Tutorial Door

                    level1Camera.SetActive(true);
                    tutorialCamera.SetActive(false);
                    Level1Door.GetComponent<LowerDoor>().OpenDoor();

                    StartCoroutine(CameraSwap("Default"));

                    break;
                }
            case "Level2":
                {
                    // Open Level 2 Door && close Level 1 Door

                    level2Camera.SetActive(true);
                    level1Camera.SetActive(false);
                    Level2Door.GetComponent<LowerDoor>().OpenDoor();

                    StartCoroutine(CameraSwap("Default"));

                    break;
                }
            case "Boss":
                {
                    // Allow Boss Door Activation && close Level 2 Door
                    break;
                }
            default:
                {
                    tutorialCamera.SetActive(false);
                    level1Camera.SetActive(false);
                    level2Camera.SetActive(false);

                    break;
                }
        }

;
    }
}
