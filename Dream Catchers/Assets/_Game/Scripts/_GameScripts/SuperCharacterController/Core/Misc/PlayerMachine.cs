using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

    //==============================================
    // Controllers and States
    //==============================================

    // Add more states by comma separating them
    enum PlayerStates { Idle = 0, Walk = 1, Run = 2, Jump = 3, DoubleJump = 4, Fall = 5, Damage = 6, Dead = 7 }

    private SuperCharacterController controller;
    private PlayerCamera cam; // Main Player Follow Camera
    private PlayerInputController input; // Input Controller

    //==============================================
    // Editor Fields
    //==============================================

    // Movement
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 10.0f;

    public float WalkSpeed = 0.1f;
    public float WalkspeedThreshold = 0.5f;

    public float RunSpeed = 0.65f;
    public float MaxRunSpeed = 6.0f;
    public float RunTurnSpeed = 4.0f;

    // Jumping
    public float JumpAcceleration = 5.0f;
    public float JumpHoldAcceleration = 10.0f;
    public float JumpHoldTime = 0.5f; // amount of time holding jump button extends height after initial press
    public float MinJumpHeight = 1.75f;
    public float MaxJumpHeight = 3.0f;
    public float DoubleJumpHeight = 2.0f;
    public float JumpTimer = 0;

    // Physics
    public float Gravity = 25.0f;
    public float GroundFriction = 10.0f;

    //==============================================
    // Functional Parameters
    //==============================================

    public float idleTimer { get; private set; } // how long the player has been idling

    public Vector3 moveDirection { get; private set; } // player movement direction vector

    public Vector3 facing; // direction player is facing

    public Vector3 lookDirection { get; private set; } // current direction camera is facing

    public float xRotation { get; private set; } // TODO: Wat is this?

    private bool jumpHold = false; // controls variable jump heights by holding jump button
    public float jumpTravelled = 0; // keeps track of 

    //============================================
    // Debug Inspector Fields:
    //============================================

    /*public float moveSpeed;
    public float hAxis;
    public float vAxis;
    public float inputMagnitude;
    public float inputPlayerCross;*/

	void Start ()
    {
        cam = Camera.main.GetComponent<PlayerCamera>();

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
        // DEBUG:
        /*hAxis = input.Current.MoveInput.x;
        vAxis = input.Current.MoveInput.z;
        inputMagnitude = input.Current.MoveInput.magnitude;*/

        // Rotate out facing direction horizontally based on mouse input
        //lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x, controller.up) * lookDirection;
        lookDirection = Math3d.ProjectVectorOnPlane(controller.up, Camera.main.transform.forward);

        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in

        // Allow Attacks only when on ground and upon attack input
        if(input.Current.AttackInput && !currentState.Equals(PlayerStates.Jump))
        {
            gameObject.GetComponent<PlayerCombat>().BeginAttack();
        }

        // Prevent all movement once dead
        if((Character_Manager.instance != null && Character_Manager.instance.isDead))
        {
            currentState = PlayerStates.Dead;
        }

        // Taking Damage from enemy
        if ((Character_Manager.instance != null && Character_Manager.instance.invincible) && !currentState.Equals(PlayerStates.Damage))
        {
            currentState = PlayerStates.Damage;
        }
    }

    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        transform.position += moveDirection * Time.deltaTime;

        // Rotate mesh to face correct direction (temp if implementing min turn radius)
        if(Math3d.ProjectVectorOnPlane(controller.up, facing) != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Math3d.ProjectVectorOnPlane(controller.up, facing), controller.up);
        }
    }

    private bool AcquiringGround()
    {
        bool ground = controller.currentGround.IsGrounded(false, 0.01f);

        if (ground && cam != null)
        {
            cam.setLastGround = transform.position.y;
        }

        return ground;//controller.currentGround.IsGrounded(false, 0.01f);
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

        // update camera target in case landing from the air
        if(cam != null)
        {
            cam.targetPos = cam.targetPosHigh;
        }
    }

    void Idle_SuperUpdate()
    {
        idleTimer += Time.deltaTime;

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
            if (Mathf.Abs(input.Current.MoveInput.x) + Mathf.Abs(input.Current.MoveInput.z) >= WalkspeedThreshold)
            {
                currentState = PlayerStates.Run;
                return;
            }
            else
            {
                currentState = PlayerStates.Walk;
                return;
            }
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);

        // check camera for obstructions
        if (idleTimer > 0.6f && cam != null)
        {
            cam.CheckOcclusion();
        }
    }

    void Idle_ExitState()
    {
        idleTimer = 0;
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
            if (Mathf.Abs(input.Current.MoveInput.x) + Mathf.Abs(input.Current.MoveInput.z) >= WalkspeedThreshold)
            {
                currentState = PlayerStates.Run;
                return;
            }
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * WalkSpeed, WalkAcceleration * Time.deltaTime);
            facing = LocalMovement(); // when walking always facing in direction moving todo: account for external forces
        }
        else 
        {
            currentState = PlayerStates.Idle;
            return; 
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
            // transition to walk condition
            if (Mathf.Abs(input.Current.MoveInput.x) + Mathf.Abs(input.Current.MoveInput.z) < WalkspeedThreshold)
            {
                currentState = PlayerStates.Walk;
                return;
            }

            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed, RunAcceleration * Time.deltaTime);
            facing = LocalMovement(); // when walking always facing in direction moving todo: account for external forces
        }
        else
        {
            currentState = PlayerStates.Idle;
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
        JumpTimer = 0;

        gameObject.GetComponent<Animator>().SetBool("Jumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        if (moveDirection.y < 0)
        {
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        }

        jumpTravelled = 0;

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight, Gravity);

        // update camera target to middle
        if(cam != null)
        {
            cam.targetPos.y = (cam.targetPosHigh.y + cam.targetPosLow.y) * 0.5f;
        }
    }

    void Jump_SuperUpdate()
    {
        // if holding jump button and not at max jump height, raise movement vector
        if (!input.Current.JumpHold)
        {
            jumpHold = false;
        }

        if (jumpHold)
        {
            if (JumpTimer + Time.deltaTime < JumpHoldTime)
            {
                moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer += Time.deltaTime;
            }
            else if (JumpTimer < JumpHoldTime)
            {
                moveDirection += controller.up * (JumpHoldTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer = JumpHoldTime;
            }

            //Vector3 initialV = moveDirection;
            //moveDirection += controller.up * Time.deltaTime * 50;

            //jumpTravelled += (moveDirection - initialV).magnitude; 
            //Math3d.ProjectVectorOnPlane(controller.up, (moveDirection - initialV)).magnitude;
        }

        // transition to double jump
        if (input.Current.JumpInput && input.toggleJump != true)
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

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * MaxRunSpeed, JumpAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;

        // update camera target
        if (moveDirection.y < -0.2f && cam != null)
        {
            cam.targetPos.y = cam.targetPosLow.y; // move target to lowest height when falling downward
        }
    }

    void Jump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Jumping", false);
        input.toggleJump = false;
        jumpHold = false;
    }

    void DoubleJump_EnterState()
    {

        gameObject.GetComponent<Animator>().SetBool("DoubleJump", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        moveDirection += controller.up * CalculateJumpSpeed(DoubleJumpHeight, Gravity);

        // update camera target
        if(cam != null)
        {
            cam.targetPos.y = (cam.targetPosHigh.y + cam.targetPosLow.y) * 0.5f;
        }
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

        planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * MaxRunSpeed, JumpAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection = planarMoveDirection + verticalMoveDirection;

        // update camera target
        if (moveDirection.y < -0.2f && cam != null)
        {
            cam.targetPos.y = cam.targetPosLow.y; // move target to lowest height when falling downward
        }
    }

    void DoubleJump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("DoubleJump", false);
        input.toggleJump = false;
    }

    void Fall_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Falling", true);

        Debug.Log("Entering Fall State");

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

    void Fall_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Falling", false);

    }

    void Damage_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();

        gameObject.GetComponent<Animator>().SetBool("Damage", true);
        gameObject.GetComponent<Animator>().applyRootMotion = true;

    }

    void Damage_SuperUpdate()
    {
        if(Character_Manager.instance != null && !Character_Manager.instance.invincible)
        {
            currentState = PlayerStates.Idle;
            return;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
    }

    void Damage_ExitState()
    {
        gameObject.GetComponent<Animator>().applyRootMotion = false;
        gameObject.GetComponent<Animator>().SetBool("Damage", false);
        gameObject.GetComponent<Collider>().enabled = true;
    }

    void Dead_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();

        gameObject.GetComponent<Animator>().SetBool("Dead", true);
    }

    void Dead_SuperUpdate()
    {
        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
    }

    void Dead_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Dead", false);
    }
}
