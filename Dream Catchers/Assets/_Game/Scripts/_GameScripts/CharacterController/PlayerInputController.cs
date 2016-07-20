///===============================================================================
/// Author: Matt & Connor
/// Purpose: Handles controller inputs with regards to Character Actions and 
///          Camera control
///===============================================================================

using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public PlayerInput Current;

    public bool toggleJump;

    public float moveBufferTimer = 0;

	// Use this for initialization
	void Start () {
        Current = new PlayerInput();
	}
	
	// Update is called once per frame
	void Update () {


        if(Game_Manager.instance == null || !Game_Manager.instance.isPaused())
        {
            // Controls set via Unity Input Managre
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 joy2Input = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));

            bool attackInput = Input.GetButtonDown("Attack");

            bool jumpInput = Input.GetButtonDown("Jump") || toggleJump;
            bool jumpHold = Input.GetButton("Jump");
            bool rButton = Input.GetButtonDown("R");

            // TODO: Change the efficiency of this; no lookup everytime
            GameObject dialogBox = GameObject.FindGameObjectWithTag("DialogBox");
            if (dialogBox != null && dialogBox.activeSelf)
            {
                jumpInput = false;
                jumpHold = false;
            }


            Current = new PlayerInput()
            {
                MoveInput = moveInput,
                Joy2Input = joy2Input,
                AttackInput = attackInput,
                JumpInput = jumpInput,
                JumpHold = jumpHold,
                RButton = rButton,
                moveBuffer = false          
            };
        }

        if (Input_Manager.instance.useBuffer)
        {
            // buffer movement for a few frames
            if (Current.MoveInput != Vector3.zero)
            {
                Current.moveBuffer = true;
                moveBufferTimer = 0;
            }
            else
            {
                if (moveBufferTimer > 0.1f)
                {
                    Current.moveBuffer = false; // let go of input long enough to count as no input
                }
                else
                {
                    moveBufferTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            Current.moveBuffer = true;
        }
    }
}

public struct PlayerInput
{
    public Vector3 MoveInput; // Character Movement
    public Vector3 Joy2Input; // Camera Control

    public bool AttackInput; // Attack 

    public bool JumpInput; // Jump
    public bool JumpHold; // Jump Height
    public bool RButton; // Left Trigger

    public bool moveBuffer;
}
