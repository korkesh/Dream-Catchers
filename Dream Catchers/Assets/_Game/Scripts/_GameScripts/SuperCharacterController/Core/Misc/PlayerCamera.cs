using UnityEngine;
using System.Collections;


// default camera
public class PlayerCamera : MonoBehaviour
{
    public float Distance = 9.0f;
    public float Height = 5.0f;

    public float rotateSpeed = 75;

    public GameObject PlayerTarget;    

    private PlayerInputController input;
    private Transform target;
    private PlayerMachine machine;

    private SuperCharacterController controller;

    private float yOffset; // offset from default pos by player input

    public float lastGround { get; private set; } // y value of last ground player was on
    public float setLastGround { set { lastGround = value; } }


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

        // temp rotation test
        if (input.Current.Joy2Input.x != 0)
        {
            transform.RotateAround(target.position, Vector3.up, Time.deltaTime * rotateSpeed * input.Current.Joy2Input.x);
        }

        Vector3 targetPos = target.position;
        targetPos.y = lastGround;

        transform.position = targetPos; // target.position;

        Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

        //horizontal rotation
        //transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);

        //todo: y rotation transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;

        transform.position -= transform.forward * Distance;
        transform.position += controller.up * Height;
    }

}
