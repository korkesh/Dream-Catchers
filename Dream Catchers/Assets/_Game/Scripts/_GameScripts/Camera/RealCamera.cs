///===============================================================================
/// Author: Connor
/// Purpose: The main camera manager
///===============================================================================

using UnityEngine;
using System.Collections;

public class RealCamera : MonoBehaviour
{
    public RootCamera RootCam;
    public GameObject PlayerTarget;
    public GameObject Player;
    public float lookDistance;

	// Use this for initialization
	void Start ()
    {
        //transform.position = RootCam.transform.position;
        //transform.rotation = RootCam.transform.rotation;

        Player = GameObject.FindGameObjectWithTag("Player"); // Character_Manager.Instance.Character;
        PlayerTarget = Player.GetComponent<HunterChildren>().camTarget;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // always same position as the invisible camera, only rotation differs
        //transform.position = RootCam.transform.position;

        // set track object position
        UpdateTarget();

        // rotation:
        Vector3 angle = (PlayerTarget.transform.position - transform.position).normalized;

        Vector3 cross = Vector3.Cross(transform.forward, angle);

        if (cross.magnitude > 0.04f)
        {
            transform.forward += Vector3.Slerp(transform.forward, PlayerTarget.transform.position - transform.position, 0.025f) * Time.deltaTime * RootCam.rotateSpeed;
        }
    }

    // updates track object coordinates
    void UpdateTarget()
    {
        PlayerTarget.transform.position = RootCam.PlayerTarget.transform.position;

        // apply look offset
        PlayerTarget.transform.position += transform.right * (1 - RootCam.playerCamCross.magnitude) * Mathf.Sign(Vector3.Cross(Math3d.ProjectVectorOnPlane(Vector3.up, RootCam.transform.forward).normalized, Math3d.ProjectVectorOnPlane(Vector3.up, Player.transform.forward).normalized).y) * lookDistance;
    }
}
