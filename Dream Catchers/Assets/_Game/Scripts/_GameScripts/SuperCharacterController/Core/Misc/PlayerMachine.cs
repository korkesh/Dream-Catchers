using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

    public Transform AnimatedMesh;

    //==============================================
    // Editor Fields
    //==============================================

    public float RunSpeed = 4.0f;
    public float WalkAcceleration = 30.0f;
    public float JumpAcceleration = 5.0f;
    public float MinJumpHeight = 1.75f;
    public float MaxJumpHeight = 3.0f;
    public float DoubleJumpHeight = 2.0f;
    public float Gravity = 25.0f;
    public float GroundFriction = 10.0f;
    public float RunThreshold = 0.5f; // % of max run speed required to transition between run/walk

    // Add more states by comma separating them
    enum PlayerStates { Idle, Walk, Run, Jump, DoubleJump, Fall, Attack }

    private SuperCharacterController controller;

    // current velocity
    private Vector3 moveDirection; // player movement direction vector

    private Vector3 facing; // direction player is facing

    // current direction camera is facing
    public Vector3 lookDirection { get; private set; }

    public float xRotation { get; private set; }

    private PlayerInputController input;

    private bool jumpHold = false; // controls variable jump heights by holding jump button
    public float jumpTravelled = 0; // keeps track of 

    //============================================
    // Debug Inspector Fields:
    //============================================

    public float moveSpeed;

	void Start ()
    {

        input = gameObject.GetComponent<PlayerInputController>();

        // Grab the controller object from our object
        controller = gameObject.GetComponent<SuperCharacterController>();
		
		// Our character's current facing direction, planar to the ground
        lookDirection = transform.forward;

        // Set our currentState to idle on startup
        currentState = PlayerStates.Idle;
	}

    protected override void EarlyGlobalSuperUpdate()
    {
        // Rotate out facing direction horizontally based on mouse input
        //lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x, controller.up) * lookDirection;
        lookDirection = Math3d.ProjectVectorOnPlane(controller.up, Camera.main.transform.forward);

        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in
    }

    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        transform.position += moveDirection * Time.deltaTime;

        // Rotate mesh to face correct direction (temp if implementing min turn radius)
        AnimatedMesh.rotation = Quaternion.LookRotation(Math3d.ProjectVectorOnPlane(controller.up, facing), controller.up);
    }

    private bool AcquiringGround()
    {
        return controller.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        return controller.currentGround.IsGrounded(true, 0.5f);
    }

    public void RotateGravity(Vector3 up)
    {
        lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
    }

    /// <summary>
    /// Constructs a vector representing our movement local to our lookDirection, which is
    /// controlled by the camera
    /// </summary>
    private Vector3 LocalMovement()
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.Current.MoveInput.x != 0)
        {
            local += right * input.Current.MoveInput.x;
        }

        if (input.Current.MoveInput.z != 0)
        {
            local += lookDirection * input.Current.MoveInput.z;
        }

        return local.normalized;
    }

    // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

	/*void Update () {
	 * Update is normally run once on every frame update. We won't be using it
     * in this case, since the SuperCharacterController component sends a callback Update 
     * called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires
     * further callbacks depending on the state
	}*/

    // Below are the three state functions. Each one is called based on the name of the state,
    // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
    // Jump_SuperUpdate()
    void Idle_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }

    void Idle_SuperUpdate()
    {
        // Run every frame we are in the idle state

        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            currentState = PlayerStates.Walk;
            return;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
    }

    void Idle_ExitState()
    {
        // Run once when we exit the idle state
    }

    void Walk_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Walking", true);
    }

    void Walk_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed * input.Current.MoveInput.magnitude, WalkAcceleration * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(moveDirection.normalized);
            facing = input.Current.MoveInput; // when walking always facing in direction moving

            if (moveDirection.magnitude > RunSpeed * RunThreshold)
            {
                currentState = PlayerStates.Run;
                return;
            }
        }
        else 
        {
            if (moveDirection.magnitude == 0)
            {
                currentState = PlayerStates.Idle;
                return;
            }

            moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
        }
    }

    void Walk_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Walking", false);
    }

    // todo: > 180 degree turn case
    void Run_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Running", true);
    }

    void Run_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed * input.Current.MoveInput.magnitude, WalkAcceleration * Time.deltaTime);
            //transform.rotation = Quaternion.LookRotation(moveDirection.normalized);
            if (input.Current.MoveInput.magnitude > 0.1f)
            {
                facing = input.Current.MoveInput;
            }

            if (moveDirection.magnitude <= RunSpeed * RunThreshold)
            {
                currentState = PlayerStates.Walk;
                return;
            }
        }
        else
        {
            currentState = PlayerStates.Walk;
            return;
        }
    }

    void Run_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Running", false);
    }

    void Jump_EnterState()
    {
        jumpHold = true;

        gameObject.GetComponent<Animator>().SetBool("Jumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        if (moveDirection.y < 0)
        {
            moveDirection.y = 0;
        }

        jumpTravelled = 0;

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight, Gravity);

        //jumpTravelled = (moveDirection - initialJumpVelocity).magnitude;       
    }

    void Jump_SuperUpdate()
    {
        // if holding jump button and not at max jump height, raise movement vector
        if (!input.Current.JumpHold)
        {
            jumpHold = false;
        }

        if (jumpHold && jumpTravelled < MaxJumpHeight)
        {
            Vector3 initialV = moveDirection;
            moveDirection += controller.up * Time.deltaTime * 50;

            jumpTravelled += (moveDirection - initialV).magnitude;//Math3d.ProjectVectorOnPlane(controller.up, (moveDirection - initialV)).magnitude;
        }

        // transition to double jump
        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.DoubleJump;
            return;
        }

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;            
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, JumpAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
    }

    void Jump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Jumping", false);
        jumpHold = false;
    }

    void DoubleJump_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("DoubleJumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection.y = 0;
        moveDirection += controller.up * CalculateJumpSpeed(DoubleJumpHeight, Gravity);
    }

    void DoubleJump_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, JumpAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;
    }

    void DoubleJump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("DoubleJumping", false);
    }

    void Fall_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        // moveDirection = trueVelocity;
    }

    void Fall_SuperUpdate()
    {
        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.DoubleJump;
            return;
        }

        if (AcquiringGround())
        {
            moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
            currentState = PlayerStates.Idle;
            return;
        }

        moveDirection -= controller.up * Gravity * Time.deltaTime;
    }

    // todo: spawn detection triangle mesh, check for link into second attack during last bit of anim, if not transition to idle on anim finish
    void Attack_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Attack1", true);
    }

    void Attack_SuperUpdate()
    {

    }

    void Attack_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Attack1", false);
    }
}
