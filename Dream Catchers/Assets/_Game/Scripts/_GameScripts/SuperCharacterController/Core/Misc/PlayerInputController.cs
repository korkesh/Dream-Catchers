using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public PlayerInput Current;

	// Use this for initialization
	void Start () {
        Current = new PlayerInput();
	}
	
	// Update is called once per frame
	void Update () {
        
        // Retrieve our current WASD or Arrow Key input
        // Using GetAxisRaw removes any kind of gravity or filtering being applied to the input
        // Ensuring that we are getting either -1, 0 or 1
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 joy2Input = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));

        bool jumpInput = Input.GetButtonDown("Jump");
        bool lTrigger = Input.GetButtonDown("L");

        Current = new PlayerInput()
        {
            MoveInput = moveInput,
            JumpInput = jumpInput,
            Joy2Input = joy2Input,
            LTrigger = lTrigger
        };
	}
}

public struct PlayerInput
{
    public Vector3 MoveInput;
    public Vector3 Joy2Input;
    public bool JumpInput;
    public bool LTrigger;
}
