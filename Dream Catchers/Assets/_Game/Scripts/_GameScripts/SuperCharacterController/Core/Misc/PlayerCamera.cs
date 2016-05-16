using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public enum PlayerCamMode
    {
        high,
        low,
        vantage
    }

    PlayerCamMode mode = PlayerCamMode.high;

    private float Distance;
    private float Height;

    public float DistanceH = 9.0f;
    public float HeightH = 4.0f;
    public float DistanceL = 4.5f;
    public float HeightL = 1.75f;

    public GameObject PlayerTarget;    

    private PlayerInputController input;
    private Transform target;
    private PlayerMachine machine;
    private float yRotation;

    private SuperCharacterController controller;

	// Use this for initialization
	void Start ()
    {
        input = PlayerTarget.GetComponent<PlayerInputController>();
        machine = PlayerTarget.GetComponent<PlayerMachine>();
        controller = PlayerTarget.GetComponent<SuperCharacterController>();
        target = PlayerTarget.transform;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        // toggle mode temp test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (mode == PlayerCamMode.high)
            {
                mode = PlayerCamMode.low;
            }
            else
            {
                mode = PlayerCamMode.high;
            }
        }

        // temp rotation test
        if (Input.GetKey(KeyCode.Z))
        {
            transform.RotateAround(target.position, Vector3.up, Time.deltaTime * 40);
        }
        else if (Input.GetKey(KeyCode.C))
        {
            transform.RotateAround(target.position, Vector3.up, Time.deltaTime * -40);
        }

        if (mode == PlayerCamMode.high || mode == PlayerCamMode.low)
        {
            if (mode == PlayerCamMode.high)
            {
                Distance = DistanceH;
                Height = HeightH;
            }
            else
            {
                Distance = DistanceL;
                Height = HeightL;
            }

            transform.position = target.position;

            Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

            //transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);
            //transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;

            transform.position -= transform.forward * Distance;
            transform.position += controller.up * Height;
        }
	}
}
