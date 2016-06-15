﻿using UnityEngine;
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
        

        // Retrieve our current WASD or Arrow Key input
        // Using GetAxisRaw removes any kind of gravity or filtering being applied to the input
        // Ensuring that we are getting either -1, 0 or 1
        if(Game_Manager.instance == null || !Game_Manager.instance.isPaused())
        {
            // Controls set via Unity Input Managre
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 joy2Input = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));

            bool attackInput = Input.GetButtonDown("Attack");

            bool jumpInput = Input.GetButtonDown("Jump") || toggleJump;
            bool jumpHold = Input.GetButton("Jump");
            bool lTrigger = Input.GetButtonDown("L");

            Current = new PlayerInput()
            {
                MoveInput = moveInput,
                Joy2Input = joy2Input,
                AttackInput = attackInput,
                JumpInput = jumpInput,
                JumpHold = jumpHold,
                LTrigger = lTrigger,
                moveBuffer = false          
            };
        }

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
}

public struct PlayerInput
{
    public Vector3 MoveInput; // Character Movement
    public Vector3 Joy2Input; // Camera Control

    public bool AttackInput; // Attack 

    public bool JumpInput; // Jump
    public bool JumpHold; // Jump Height
    public bool LTrigger; // Left Trigger?

    public bool moveBuffer;
}
