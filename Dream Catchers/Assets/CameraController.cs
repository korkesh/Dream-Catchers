using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public enum CameraMode
    {
        follow,
        vantage
    }

    public CameraMode mode;

    public GameObject player;

    // vantage fields
    public Vector3 vantagePoint;

    // follow fields
    public Vector3 followOffset;

	// Use this for initialization
	void Start ()
    {
        player = GameMaster.Instance.player;

        mode = CameraMode.vantage;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // mode toggle
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwitchMode();
        }

        switch(mode)
        {
            case CameraMode.follow:
                {
                    FollowUpdate();

                    break;
                }
            case CameraMode.vantage:
                {
                    VantageUpdate();

                    break;
                }
        }
	}


    void FollowUpdate()
    {
        // temp
        transform.position = player.transform.position - followOffset;
        transform.LookAt(player.transform); 
    }

    void VantageUpdate()
    {
        transform.position = vantagePoint;
        transform.LookAt(player.transform); // temp
    }


    void SwitchMode()
    {
        mode++;
        if ((int)mode > 1)
        {
            mode = 0;
        }
    }
}
