using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject player;

	// Use this for initialization
	void Start ()
    {
        player = GameMaster.Instance.player;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(player.transform); // TEMP
	}
}
