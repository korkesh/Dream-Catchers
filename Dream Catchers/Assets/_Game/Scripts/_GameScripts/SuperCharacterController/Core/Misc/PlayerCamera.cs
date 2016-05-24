using UnityEngine;
using System.Collections;


// default camera
public class PlayerCamera : MonoBehaviour
{
    public float MaxDistance = 11.0f;
    public float MinDistance = 9.0f;
    public float Height = 4.5f;

    public float moveSpeed = 10.0f;
    public float rotateSpeed = 75;

    public GameObject PlayerTarget;    

    private PlayerInputController input;
    private Transform target;
    private PlayerMachine machine;

    private SuperCharacterController controller;

    private float yOffset; // offset from default pos by player input

    public float lastGround { get; private set; } // y value of last ground player was on
    public float setLastGround { set { lastGround = value; } }

    // EDITOR DEBUG:
    public float crossLength;

	// Use this for initialization
	void Start ()
    {
        input = PlayerTarget.GetComponent<PlayerInputController>();
        machine = PlayerTarget.GetComponent<PlayerMachine>();
        controller = PlayerTarget.GetComponent<SuperCharacterController>();
        target = PlayerTarget.transform;

        lastGround = machine.transform.position.y;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        FollowPlayer();

        // temp rotation test
        if (input.Current.Joy2Input.x != 0)
        {
            transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * input.Current.Joy2Input.x);
        }

        //Vector3 targetPos = target.position;
        //targetPos.y = lastGround;

        //transform.position = targetPos; // target.position;

        //Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

        ////horizontal rotation
        ////transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);

        ////todo: y rotation transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;

        //transform.position -= transform.forward * Distance;
        //transform.position += controller.up * Height;
    }

    public void FollowPlayer()
    {
        // min/max distance
        Vector3 displacement = Math3d.ProjectVectorOnPlane(controller.up, PlayerTarget.transform.position - transform.position);

        if (displacement.magnitude > MaxDistance)
        {

            transform.position += (displacement.magnitude - MaxDistance) * displacement.normalized; 
        }
        else if (displacement.magnitude < MinDistance)
        {
            transform.position -= (MinDistance - displacement.magnitude) * displacement.normalized;
        }


        // rotation
        //Vector3 angle = (Math3d.ProjectVectorOnPlane(controller.up, PlayerTarget.transform.position - transform.position)).normalized;

        //Vector3 cross = Vector3.Cross(transform.forward, angle);

        //crossLength = cross.magnitude; // debug

        //if (cross.magnitude > 0.1f)
        //{
        //    transform.forward += Vector3.Slerp(transform.forward, PlayerTarget.transform.position - transform.position, 0.1f) * Time.deltaTime * rotateSpeed;
        //}

        Vector3 angle = (PlayerTarget.transform.position - transform.position).normalized;

        Vector3 cross = Vector3.Cross(transform.forward, angle);

        crossLength = cross.magnitude; // DEBUG

        if (cross.magnitude > 0.1f)
        {
            transform.forward += Vector3.Slerp(transform.forward, PlayerTarget.transform.position - transform.position, 0.1f) * Time.deltaTime * rotateSpeed;
        }
    }

}
