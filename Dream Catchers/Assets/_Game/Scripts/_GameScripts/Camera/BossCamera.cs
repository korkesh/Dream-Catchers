using UnityEngine;
using System.Collections;

public class BossCamera : MonoBehaviour
{
    //===================================
    // Fields
    //===================================

    // Inspector values
    public float zDistance; // amount of distance behind player to maintain
    public float yDistance; // amount of distance above player to maintain
    public Vector3 Forward; // the direction used as constant forward (Vector3.right with the current scene setup)
    public float xRotation = 15f; // amount of rotation around the x-axis
    public float lookDistance = 1f; // how far ahead camera of player camera will look at max (directly L/R rel to cam)
    public bool follow = false; // debug mode (constrain distance from hunter vs static z distance from arena)

    // Private variables
    private GameObject Player;

    private Vector3 CurrentTargetPos;
    private float currentTargetOffset = 0;

    private Transform origin;

	void Start ()
    {
        Forward = Vector3.right;

        Player = GameObject.FindWithTag("Player"); // todo: player reference

        origin = GameObject.FindWithTag("Origin").transform;
	}
	

	void Update ()
    {
        // positioning
        if (!follow)
        {
            transform.position = new Vector3(origin.position.x - zDistance, origin.position.y + yDistance, Player.transform.position.z);
        }
        else
        {
            transform.position = Player.transform.position;
            transform.position += Vector3.up * yDistance; // height
            transform.position -= Forward * zDistance; // distance
        }

        transform.eulerAngles = new Vector3(xRotation, 90, 0);

        LookRotation();
	}


    void LookRotation()
    {
        Vector3 DisplacementDir = Player.transform.position - transform.position;
        DisplacementDir.y = 0;
        DisplacementDir.Normalize();

        float align = Vector3.Cross(DisplacementDir, Player.transform.forward).y;
        Vector3 right = Vector3.Cross(Vector3.up, DisplacementDir);

        // smoothly move target left/right in ground state
        if (Player.GetComponent<PlayerMachine>().ground) // todo: cleanup
        {
            currentTargetOffset = Clamp(-lookDistance, lookDistance, currentTargetOffset + (Mathf.Sign(align * lookDistance - currentTargetOffset) * Time.deltaTime * Mathf.Abs(align * lookDistance - currentTargetOffset)));
        }

        CurrentTargetPos = Player.transform.position + right * currentTargetOffset;

        // rotate to look at target
        Vector3 TargetDisplacement = CurrentTargetPos - transform.position;
        TargetDisplacement.y = 0;
        TargetDisplacement.Normalize();
        
        if (Vector3.Cross(TargetDisplacement, transform.right).y < 1)
        {
            Vector3 PlanarForward = transform.forward;
            PlanarForward.y = 0;
            PlanarForward.Normalize();

            float dir = Mathf.Sign(Vector3.Cross(PlanarForward, TargetDisplacement.normalized).y);

            float angle = Vector3.Angle(PlanarForward, TargetDisplacement.normalized);

            transform.forward = Quaternion.AngleAxis(angle * dir, Vector3.up) * transform.forward;        
        }
    }


    // Mathf.Clamp doesn't work?????????????????
    float Clamp(float min, float max, float val)
    {
        if (val < min)
            return min;
        else if (val > max)
            return max;
        return val;
    }
}
