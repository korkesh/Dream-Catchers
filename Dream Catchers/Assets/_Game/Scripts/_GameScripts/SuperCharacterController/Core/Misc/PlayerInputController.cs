using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public PlayerInput Current;

    public bool toggleJump;

	// Use this for initialization
	void Start () {
        Current = new PlayerInput();
	}
	
	// Update is called once per frame
	void Update () {
        
        // Retrieve our current WASD or Arrow Key input
        // Using GetAxisRaw removes any kind of gravity or filtering being applied to the input
        // Ensuring that we are getting either -1, 0 or 1

        if(Game_Manager.instance == null || Game_Manager.instance.currentGameState == Game_Manager.GameState.PLAY)
        {
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            Vector2 mouseInput = new Vector2(Input.GetAxis("Joystick X"), Input.GetAxis("Joystick Y"));

            bool attackInput = Input.GetButtonDown("Attack");

            bool jumpInput = Input.GetButtonDown("Jump") || toggleJump;

            Current = new PlayerInput()
            {
                MoveInput = moveInput,
                MouseInput = mouseInput,
                AttackInput = attackInput,
                JumpInput = jumpInput
            };
        }
	}
}

public struct PlayerInput
{
    public Vector3 MoveInput;
    public Vector2 MouseInput;
    public bool AttackInput;
    public bool JumpInput;
}
