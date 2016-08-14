///===================================================================================
/// Author: Matt (Combat states) & Connor (All Non-combat states; primary contributer)
/// Purpose: Loosely based on a third party script with significant modifications
///          This script handles all movement logic for the player including
///          idle, walking, running, jumping and combat states. 
///==================================================================================
///
using UnityEngine;
using System.Collections;
using System;


[RequireComponent(typeof(SuperCharacterController))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerMachine : SuperStateMachine {

    //================================
    // Variables
    //================================

    //----------------------------------------------
    // Controllers and States
    //----------------------------------------------

    // Add more states by comma separating them
    public enum PlayerStates { Idle = 0, Walk = 1, Run = 2, Jump = 3, DoubleJump = 4, Fall = 5, Damage = 6, Dead = 7, Skid = 8, SkidJump = 9, Dive = 10, Slide = 11, Roll = 12, StandUp = 13 };

    private SuperCharacterController controller;
    private RootCamera cam; // Main Player Follow Camera
    private PlayerInputController input; // Input Controller

    public Vector3 prevPos;
    public Vector3 Displacement; // displacement this frame

    public bool ground;// { get; private set; }

    private float rollSpeed; // planar speed of current roll sequence

    //----------------------------------------------
    // Editor Fields
    //----------------------------------------------

    // Movement
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 10.0f;

    public float maxSpeedTime = 5f; // amount of time it takes to go from idle to max speed (1 = 1s, 10 = 0.1s)
    private float maxAirSpeedTime = 1f; // amount of time it takes to go from 0 x/z speed to max in the air
    [SerializeField]
    public float speed = 0f; // current run speed
    [SerializeField]
    private float xSpeed = 0f; // current left/right speed
    public float RunSpeed = 0.65f;
    public float RunTurnSpeed = 10.0f;
    public float MaxRunSpeed = 8.0f;
    public float TurnRadius = 4.0f;
    private float skidTimer = 0f;
    public float skidTime = 0.5f; // time in seconds skid state lasts for if not broken by jump/fall
    private float runTimer = 0f;

    // Diving
    public float MaxDiveSpeed;
    public float DiveJumpForce;

    private float slideTimer = 0f;
    public float slideFriction; // time in seconds for bellyside to end
    public float slideTurnSpeed = 500f;

    private bool diving = false; // controls dive state (prevents double jumping out of dive-fall)

    // Jumping
    public float VerticalSpeedCap = 10.0f;
    public float VerticalSpeedCapDown = -30.0f;
    public float AirTurnSpeed = 10.0f;
    public float AirAcceleration = 3.0f;
    private float maxAirSpeed = 8.5f;
    public float JumpAcceleration = 5.0f;
    public float JumpHoldAcceleration = 10.0f;
    public float JumpTime = 0.3f; // amount of time holding jump button extends height after initial press
    public float MinJumpHeight = 1.75f;
    public float MaxJumpHeight = 3.0f;
    public float DoubleJumpHeight = 2.0f;
    public float JumpTimer = 0f;
    public Vector3 LastGroundPos { get; private set; } // position character was at last frame they were grounded

    public bool jumping = false; // set to true in active jump states (not fall/dive etc)
    private bool finishDoubleJump = false;
    public bool hasDoubleJump = true;

    // Physics
    public float Gravity = 60.0f;
    public float DiveGravity = 39.0f;
    public float GroundFriction = 10.0f;

    //----------------------------------------------
    // Functional Parameters
    //----------------------------------------------

    public float idleTimer { get; private set; } // how long the player has been idling

    public Vector3 moveDirection; // player movement direction vector

    public Vector3 lookDirection { get; private set; } // current direction camera is facing

    public Vector3 localMovement { get; private set; } // call localMovement at beginning of frame once


    //================================
    // Methods
    //================================

    //----------------------------------------------
    // Initialization
    //----------------------------------------------

    void Start ()
    {
        cam = Camera.main.GetComponent<RootCamera>();

        input = gameObject.GetComponent<PlayerInputController>();

        // Grab the controller object from our object
        controller = gameObject.GetComponent<SuperCharacterController>();
		
		// Our character's current facing direction, planar to the ground
        lookDirection = transform.forward;

        // Set our currentState to idle on startup unless gameover is set
        gameObject.GetComponent<Animator>().SetBool("Dead", false);
        currentState = PlayerStates.Idle;
    }

    //----------------------------------------------
    // Helper Functions
    //----------------------------------------------

    private bool AcquiringGround()
    {
        ground = controller.currentGround.IsGrounded(false, 0.01f);
        return ground;//controller.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        ground = controller.currentGround.IsGrounded(true, 0.5f);

        return ground;
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

    //================================
    // State Machine
    //================================

    //----------------------------------------------
    // Global Update
    //----------------------------------------------

    protected override void EarlyGlobalSuperUpdate()
    {
        localMovement = LocalMovement();

        Displacement = transform.position - prevPos;

        if (ground)
        {
            LastGroundPos = transform.position;
        }

        // Rotate out facing direction horizontally based on mouse input
        //lookDirection = Quaternion.AngleAxis(input.Current.MouseInput.x, controller.up) * lookDirection;
        lookDirection = Math3d.ProjectVectorOnPlane(controller.up, Camera.main.transform.forward);

        // Put any code in here you want to run BEFORE the state's update function.
        // This is run regardless of what state you're in

        // Allow Attacks only when on ground and upon attack input
        if (input.Current.AttackInput && (Game_Manager.instance != null && Game_Manager.instance.currentGameState != Game_Manager.GameState.GAMEOVER) && Character_Manager.instance.toggleHammer && !currentState.Equals(PlayerStates.Dive) && !currentState.Equals(PlayerStates.Slide))
        {
            gameObject.GetComponent<PlayerCombat>().BeginAttack();
            idleTimer = 0;
        }

        // Prevent all movement once dead
        if ((Character_Manager.instance != null && Character_Manager.instance.isDead) 
            && (Game_Manager.instance != null && Game_Manager.instance.currentGameState == Game_Manager.GameState.GAMEOVER)
            && !currentState.Equals(PlayerStates.Dead))
        {
            currentState = PlayerStates.Dead;
        }

        // Taking Damage from enemy
        if ((Character_Manager.instance != null && Character_Manager.instance.invincible && !Character_Manager.instance.isDead)
            && (Game_Manager.instance != null && Game_Manager.instance.currentGameState != Game_Manager.GameState.GAMEOVER)
            && !currentState.Equals(PlayerStates.Damage))
        {
            currentState = PlayerStates.Damage;
        }
    }

    protected override void LateGlobalSuperUpdate()
    {
        // Put any code in here you want to run AFTER the state's update function.
        // This is run regardless of what state you're in

        // Move the player by our velocity every frame
        prevPos = transform.position;

        if (moveDirection.y < VerticalSpeedCapDown)
        {
            moveDirection.y = VerticalSpeedCapDown;
        }

        transform.position += moveDirection * Time.deltaTime;    
    }

    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    // 
    // MOVEMENT STATES
    //
    // Author/modifier: Conor MacKeigan
    //
    // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

    //----------------------------------------------
    // Idle
    //----------------------------------------------

    // Below are the three state functions. Each one is called based on the name of the state,
    // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
    // Jump_SuperUpdate()
    void Idle_EnterState()
    {
        diving = false;
        hasDoubleJump = true;

        controller.EnableSlopeLimit();
        controller.EnableClamping();

        xSpeed = 0;

        if (input.Current.MoveInput == Vector3.zero)
        {
            speed = 0;
            gameObject.GetComponent<Animator>().SetBool("Walking", false);
            gameObject.GetComponent<Animator>().SetBool("Running", false);
            gameObject.GetComponent<Animator>().SetBool("Falling", false);
        }
    }

    void Idle_SuperUpdate()
    {
        float prevIdle = idleTimer;
        idleTimer += Time.deltaTime;

        // Run every frame we are in the idle state
        // Dont allow attack and then jump
        if (input.Current.JumpInput && !gameObject.GetComponent<PlayerCombat>().damage)
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
            currentState = PlayerStates.Run;
            return;
        }

        // Apply friction to slow us to a halt
        float new_ratio = 0.9f * Time.deltaTime * maxSpeedTime;
        float old_ratio = 1 - new_ratio;

        speed = clampF(0f, float.PositiveInfinity, (speed * old_ratio) - (speed * new_ratio));

        moveDirection = transform.forward * speed;

        if(idleTimer >= 5f && prevIdle < 5f)
        {
            gameObject.GetComponent<Animator>().SetBool("Idle1", true);
        }
        else if (idleTimer >= 5f)
        {
            gameObject.GetComponent<Animator>().SetBool("Idle1", false);

            if (idleTimer >= 10f && prevIdle < 10f)
            {
                gameObject.GetComponent<Animator>().SetBool("Idle2", true);
            }
            else if (idleTimer >= 10f)
            {
                gameObject.GetComponent<Animator>().SetBool("Idle2", false);
                idleTimer = 0; // reset timer, completed full cycle
            }
        }
        


        // ANIMATION:
        if (speed / MaxRunSpeed > 0.5f)
        {
            gameObject.GetComponent<Animator>().SetBool("Running", true);
            gameObject.GetComponent<Animator>().SetBool("Walking", false);
        }
        else if (speed / MaxRunSpeed > 0.01f)
        {
            gameObject.GetComponent<Animator>().SetBool("Running", false);
            gameObject.GetComponent<Animator>().SetBool("Walking", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("Running", false);
            gameObject.GetComponent<Animator>().SetBool("Walking", false);
        }
    }

    void Idle_ExitState()
    {
        idleTimer = 0;
        gameObject.GetComponent<Animator>().SetBool("IdleTimeOut", false);
        gameObject.GetComponent<Animator>().SetBool("Idle1", false);
        gameObject.GetComponent<Animator>().SetBool("Idle2", false);
    }

  
    //----------------------------------------------
    // Running
    //----------------------------------------------

    void Run_EnterState()
    {
        runTimer = 0f;
    }

    void Run_SuperUpdate()
    {
        runTimer += Time.deltaTime;
        // Dont allow attack and then jump
        if (input.Current.JumpInput && !gameObject.GetComponent<PlayerCombat>().damage)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        // dive condition
        if (input.Current.DiveInput && speed / MaxRunSpeed > 0.5f)
        {
            currentState = PlayerStates.Dive;
            return;
        }

        // MOVEMENT:
        float new_ratio;
        float old_ratio;

        if (speed / MaxRunSpeed > 0.5f) // run speeds must turn gradually
        {
            // ROTATION:
            new_ratio = 0.9f * Time.deltaTime * RunTurnSpeed;
            old_ratio = 1 - new_ratio;

            transform.forward = ((moveDirection.normalized * old_ratio) + (localMovement * new_ratio)).normalized;

            // skid if input is >90 degrees of current facing direction
            if (Vector3.Cross(Math3d.ProjectVectorOnPlane(controller.up, transform.right).normalized, Math3d.ProjectVectorOnPlane(controller.up, localMovement).normalized).y > 0.49f)
            {
                currentState = PlayerStates.Skid;
                transform.forward = Math3d.ProjectVectorOnPlane(Vector3.up, localMovement);
                return;
            }
        }
        else if (localMovement != Vector3.zero)
        {
            transform.forward = localMovement;
        }

        // SPEED:
        // get desired speed
        float desiredSpeed = (float)Math.Round(clampF(0f, 1f, input.Current.MoveInput.magnitude) * MaxRunSpeed, 2);

        new_ratio = 0.9f * Time.deltaTime * maxSpeedTime;
        old_ratio = 1 - new_ratio;

        speed = (speed * old_ratio) + (desiredSpeed * new_ratio);

        if (speed > MaxRunSpeed)
        {
            speed = MaxRunSpeed;
        }

        moveDirection = transform.forward * speed;


        // ANIMATION:
        if (speed / MaxRunSpeed > 0.5f)
        {
            gameObject.GetComponent<Animator>().SetBool("Running", true);
            gameObject.GetComponent<Animator>().SetBool("Walking", false);
        }
        else if (speed / MaxRunSpeed > 0.01f)
        {
            gameObject.GetComponent<Animator>().SetBool("Running", false);
            gameObject.GetComponent<Animator>().SetBool("Walking", true);
        }
        else
        {
            currentState = PlayerStates.Idle;
        }
        
    }

    void Run_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Running", false);
        gameObject.GetComponent<Animator>().SetBool("Walking", false);

        runTimer = 0f;
    }

    void Skid_EnterState()
    {
        skidTimer = 0;

        transform.forward = localMovement;

        // immediate slowing effect
        moveDirection = moveDirection.normalized * 0.2f;
    }

    void Skid_SuperUpdate()
    {
        skidTimer += Time.deltaTime;

        //transform.forward = localMovement;

        // when in skid state slow to a stop
        float new_ratio = 0.9f * Time.deltaTime * maxSpeedTime;
        float old_ratio = 1 - new_ratio;

        speed = (speed * old_ratio) - (speed * new_ratio);
        if (speed < 0)
            speed = 0;

        moveDirection = moveDirection.normalized * speed;

        if (skidTimer >= skidTime)
        {
            currentState = PlayerStates.Idle;
            moveDirection = Vector3.zero;
            return;
        }

        if (input.Current.JumpInput)
        {
            currentState = PlayerStates.SkidJump;
            return;
        }
    }

    void Skid_ExitState()
    {

    }

    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    // 
    // AIR CONTROL STATES
    //
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

    

    //----------------------------------------------
    // Jumping
    //----------------------------------------------
    void SkidJump_EnterState()
    {
        jumping = true;
        JumpTimer = 0;

        hasDoubleJump = false;

        ground = false;

        gameObject.GetComponent<Animator>().SetBool("DoubleJump", true);
    
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        if (localMovement != Vector3.zero)
        {
            moveDirection = new Vector3(localMovement.x, 0, localMovement.z);
            transform.forward = moveDirection;
        }
        
        float magnitude = input.Current.MoveInput.magnitude;
        if (magnitude > 0.9f)
        {
            magnitude = 1;
        }

        speed = magnitude * 4;

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight * 1.8f, Gravity);
    }

    void SkidJump_SuperUpdate()
    {
        // dive condition
        if (input.Current.DiveInput)
        {
            currentState = PlayerStates.Dive;
            return;
        }

        // fall condition
        if (finishDoubleJump)
        {
            finishDoubleJump = false;
            currentState = PlayerStates.Fall;
            return;
        }

        // move player up against gravity for 0.5 seconds
        if (JumpTimer + Time.deltaTime < JumpTime)
        {
            moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer += Time.deltaTime;
        }
        else if (JumpTimer < JumpTime)
        {
            moveDirection += controller.up * (JumpTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer = JumpTime;
        }


        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        // landing condition
        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        // X/Z movement
        float new_ratio;
        float old_ratio;

        if (input.Current.MoveInput != Vector3.zero)
        {
            // ROTATION:
            new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, localMovement).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (localMovement * new_ratio)).normalized;

            // SPEED:
            float cross = Vector3.Cross(localMovement, transform.right).y; // speed is a function of how aligned the input direction is with the player forward vector
            float speedCoefficient = (cross - -1) / (1 - -1); // normalize cross 0..1

            // get desired speed
            float magnitude = input.Current.MoveInput.magnitude;
            if (magnitude > 0.9f)
            {
                magnitude = 1f;
            }

            float desiredSpeed = magnitude * speedCoefficient * MaxRunSpeed;

            new_ratio = 0.9f * Time.deltaTime * 6.75f;
            old_ratio = 1.0f - new_ratio;

            speed = (speed * old_ratio) + (desiredSpeed * new_ratio);
            moveDirection = transform.forward * speed;
        }
        else
        {
            // slow to stop
            new_ratio = 0.9f * Time.deltaTime * 2.75f;
            old_ratio = 1.0f - new_ratio;

            speed = clampF(0f, float.PositiveInfinity, (speed * old_ratio) - (speed * new_ratio));

            moveDirection = transform.forward * speed;
        }


        // Y Movement
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;
        moveDirection += verticalMoveDirection;

        planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        speed = planarMoveDirection.magnitude;
    }

    void SkidJump_ExitState()
    {
        jumping = false;
        gameObject.GetComponent<Animator>().SetBool("DoubleJump", false);
    }


    void Jump_EnterState()
    {
        gameObject.SendMessage("Play", SendMessageOptions.DontRequireReceiver);

        jumping = true;
        ground = false;

        JumpTimer = 0;

        gameObject.GetComponent<Animator>().SetBool("Jumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        if (moveDirection.y < 0)
        {
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        }

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight, Gravity);

        // add external y velocity for elevators
        float externalY = clampF(0f, float.PositiveInfinity, ((transform.position - prevPos) / Time.deltaTime).y) * 0.5f;
        moveDirection += controller.up * externalY;

        // cap jump speed
        if (moveDirection.y > VerticalSpeedCap)
        {
            moveDirection = new Vector3(moveDirection.x, VerticalSpeedCap, moveDirection.z);
        }
        else if (moveDirection.y < -VerticalSpeedCap)
        {
            moveDirection = new Vector3(moveDirection.x, -VerticalSpeedCap, moveDirection.z);
        }
    }

    void Jump_SuperUpdate()
    {
        // dive condition
        if (input.Current.DiveInput)
        {
            currentState = PlayerStates.Dive;
            return;
        }

        // double jump condition
        if (input.Current.JumpInput && hasDoubleJump)
        {
            currentState = PlayerStates.DoubleJump;
            return;
        }


        // move player up against gravity for 0.5 seconds
        if (input.Current.JumpHold)
        {
            if (JumpTimer + Time.deltaTime < JumpTime)
            {
                moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer += Time.deltaTime;
            }
            else if (JumpTimer < JumpTime)
            {
                moveDirection += controller.up * (JumpTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer = JumpTime;
            }
        }
        else
        {
            JumpTimer = JumpTime;
        }


        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        // landing condition
        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        // X/Z movement
        float new_ratio;
        float old_ratio;

        if (input.Current.MoveInput != Vector3.zero)
        {
            // ROTATION:
            new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, localMovement).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (localMovement * new_ratio)).normalized;
         
            // SPEED:
            float cross = Vector3.Cross(localMovement, transform.right).y; // speed is a function of how aligned the input direction is with the player forward vector
            float speedCoefficient = (cross - -1) / (1 - -1); // normalize cross 0..1

            // get desired speed
            float magnitude = input.Current.MoveInput.magnitude;
            if (magnitude > 0.9f)
            {
                magnitude = 1f;
            }

            float desiredSpeed = magnitude * speedCoefficient * MaxRunSpeed;

            new_ratio = 0.9f * Time.deltaTime * 6.75f;
            old_ratio = 1.0f - new_ratio;

            speed = (speed * old_ratio) + (desiredSpeed * new_ratio);
            moveDirection = transform.forward * speed;
        }
        else
        {
            // slow to stop
            new_ratio = 0.9f * Time.deltaTime * 2.75f;
            old_ratio = 1.0f - new_ratio;

            speed = clampF(0f, float.PositiveInfinity, (speed * old_ratio) - (speed * new_ratio));

            moveDirection = transform.forward * speed;
        }
        

        // Y Movement
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;
        moveDirection += verticalMoveDirection;

        planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        speed = planarMoveDirection.magnitude;
    }

    void Jump_ExitState()
    {
        jumping = false;

        gameObject.GetComponent<Animator>().SetBool("Jumping", false);
    }

    //----------------------------------------------
    // Double Jump
    //----------------------------------------------

    void DoubleJump_EnterState()
    {

        gameObject.SendMessage("PlayAlt", SendMessageOptions.DontRequireReceiver);

        hasDoubleJump = false;
        jumping = true;

        gameObject.GetComponent<Animator>().SetBool("DoubleJump", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        moveDirection += controller.up * CalculateJumpSpeed(DoubleJumpHeight, Gravity);
    }

    void DoubleJump_SuperUpdate()
    {
        // dive condition
        if (input.Current.DiveInput)
        {
            currentState = PlayerStates.Dive;
            return;
        }

        // fall condition
        if (finishDoubleJump)
        {
            finishDoubleJump = false;
            currentState = PlayerStates.Fall;
            return;
        }

        // move player up against gravity for 0.5 seconds
        if (JumpTimer + Time.deltaTime < JumpTime)
        {
            moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer += Time.deltaTime;
        }
        else if (JumpTimer < JumpTime)
        {
            moveDirection += controller.up * (JumpTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer = JumpTime;
        }


        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        // landing condition
        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        // X/Z movement
        float new_ratio;
        float old_ratio;

        if (input.Current.MoveInput != Vector3.zero)
        {
            // ROTATION:
            new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, localMovement).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (localMovement * new_ratio)).normalized;

            // SPEED:
            float cross = Vector3.Cross(localMovement, transform.right).y; // speed is a function of how aligned the input direction is with the player forward vector
            float speedCoefficient = (cross - -1) / (1 - -1); // normalize cross 0..1

            // get desired speed
            float magnitude = input.Current.MoveInput.magnitude;
            if (magnitude > 0.9f)
            {
                magnitude = 1f;
            }

            float desiredSpeed = magnitude * speedCoefficient * MaxRunSpeed;

            new_ratio = 0.9f * Time.deltaTime * 6.75f;
            old_ratio = 1.0f - new_ratio;

            speed = (speed * old_ratio) + (desiredSpeed * new_ratio);
            moveDirection = transform.forward * speed;
        }
        else
        {
            // slow to stop
            new_ratio = 0.9f * Time.deltaTime * 2.75f;
            old_ratio = 1.0f - new_ratio;

            speed = clampF(0f, float.PositiveInfinity, (speed * old_ratio) - (speed * new_ratio));

            moveDirection = transform.forward * speed;
        }


        // Y Movement
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;
        moveDirection += verticalMoveDirection;

        planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        speed = planarMoveDirection.magnitude;
    }

    void DoubleJump_ExitState()
    {
        jumping = false;
        gameObject.GetComponent<Animator>().SetBool("DoubleJump", false);
    }

    void FinishDoubleJump()
    {
        finishDoubleJump = true;
    }

    //----------------------------------------------
    // Falling
    //----------------------------------------------

    void Fall_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Falling", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        // moveDirection = trueVelocity;
    }

    void Fall_SuperUpdate()
    {
        // dive condition
        if (input.Current.DiveInput && !diving)
        {
            currentState = PlayerStates.Dive;
            return;
        }

        // double jump condition
        if (input.Current.JumpInput && hasDoubleJump)
        {
            currentState = PlayerStates.DoubleJump;
            return;
        }


        // move player up against gravity for 0.5 seconds
        if (JumpTimer + Time.deltaTime < JumpTime)
        {
            moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer += Time.deltaTime;
        }
        else if (JumpTimer < JumpTime)
        {
            moveDirection += controller.up * (JumpTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
            JumpTimer = JumpTime;
        }


        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        // landing condition
        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        // X/Z movement
        float new_ratio;
        float old_ratio;

        if (input.Current.MoveInput != Vector3.zero)
        {
            // ROTATION:
            new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, localMovement).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (localMovement * new_ratio)).normalized;

            // SPEED:
            float cross = Vector3.Cross(localMovement, transform.right).y; // speed is a function of how aligned the input direction is with the player forward vector
            float speedCoefficient = (cross - -1) / (1 - -1); // normalize cross 0..1

            // get desired speed
            float magnitude = input.Current.MoveInput.magnitude;
            if (magnitude > 0.9f)
            {
                magnitude = 1f;
            }

            float desiredSpeed = magnitude * speedCoefficient * MaxRunSpeed;

            new_ratio = 0.9f * Time.deltaTime * 6.75f;
            old_ratio = 1.0f - new_ratio;

            speed = (speed * old_ratio) + (desiredSpeed * new_ratio);
            moveDirection = transform.forward * speed;
        }
        else
        {
            // slow to stop
            new_ratio = 0.9f * Time.deltaTime * 2.75f;
            old_ratio = 1.0f - new_ratio;

            speed = clampF(0f, float.PositiveInfinity, (speed * old_ratio) - (speed * new_ratio));

            moveDirection = transform.forward * speed;
        }


        // Y Movement
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;
        moveDirection += verticalMoveDirection;

        planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        speed = planarMoveDirection.magnitude;
    }

    void Fall_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Falling", false);
    }


    //--------------------------------------------------
    //  Dive Related States
    //--------------------------------------------------

    void Dive_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        hasDoubleJump = false;

        if (!input.Current.JumpHold)
        {
            JumpTimer = JumpTime;
            moveDirection.y = clampF(float.NegativeInfinity, 0f, moveDirection.y);
        }

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

        if (ground)
        {
            JumpTimer = JumpTime; // prevent jump timer from ticking in ground dive (raises character upward)     
            moveDirection = transform.forward * MaxDiveSpeed;
            moveDirection.y += DiveJumpForce;
        }
        else
        {
            // entering dive from airstate provides additive forward speed, not static
            moveDirection += transform.forward * clampF(0f, MaxDiveSpeed - planarMoveDirection.magnitude, (MaxDiveSpeed - maxAirSpeed) * 1.25f);

            planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
            if (planarMoveDirection.magnitude < (MaxDiveSpeed - maxAirSpeed) * 2f)
            {
                moveDirection -= planarMoveDirection;
                planarMoveDirection = planarMoveDirection.normalized * (MaxDiveSpeed - maxAirSpeed) * 2.2f; // minimum dive planar speed
                moveDirection += planarMoveDirection;
            }
        }

        ground = false;
        diving = true;

        gameObject.GetComponent<Animator>().SetBool("Diving", true);

        planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        speed = planarMoveDirection.magnitude;
    }

    void Dive_SuperUpdate()
    {
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        // landing on ground transition
        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Slide;
            return;
        }

        moveDirection = transform.forward * MaxDiveSpeed;

        // slight control
        float new_ratio = 0.9f * Time.deltaTime * maxAirSpeedTime;
        float old_ratio = 1.0f - new_ratio;

        // SPEED:
        float magnitude = input.Current.MoveInput.magnitude;
        if (magnitude > 0.9f)
        {
            magnitude = 1f;
        }

        float desiredRightSpeed = Vector3.Cross(transform.forward, localMovement).y * MaxDiveSpeed * magnitude * 0.85f;
        float desiredForwardSpeed = Vector3.Cross(localMovement, transform.right).y * MaxDiveSpeed * magnitude * 0.75f;

        xSpeed = (xSpeed * old_ratio) + (desiredRightSpeed * new_ratio);
        speed = clampF(-MaxDiveSpeed, MaxDiveSpeed, (speed * old_ratio) + (desiredForwardSpeed * new_ratio));

        moveDirection = transform.forward * speed;
        moveDirection += transform.right * xSpeed;

        // gravity
        verticalMoveDirection -= controller.up * (Gravity + 1.75f) * Time.deltaTime;
        moveDirection += verticalMoveDirection;

        if (input.Current.JumpHold)
        {
            // upward movement if transitioned from dive state
            if (JumpTimer + Time.deltaTime < JumpTime)
            {
                moveDirection += controller.up * Time.deltaTime * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer += Time.deltaTime;
            }
            else if (JumpTimer < JumpTime)
            {
                moveDirection += controller.up * (JumpTime - JumpTimer) * (JumpHoldAcceleration * (JumpTimer - 1) * -1);
                JumpTimer = JumpTime;
            }
        }
        else
        {
            JumpTimer = JumpTime;
        }
    }

    void Dive_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Diving", false);

        controller.feet.offset = 0.5f;
    }


    void Slide_EnterState()
    {
        diving = false; // if the player falls off an edge in slide state they can dive again, but not out of roll

        gameObject.GetComponent<Animator>().SetBool("Sliding", true);

        slideTimer = 0f;

        controller.feet.offset = 0.15f;
    }

    void Slide_SuperUpdate()
    {
        slideTimer += Time.deltaTime;

        // transition to fall condition
        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        // transition to roll condition
        if (input.Current.DiveInput || input.Current.JumpInput)
        {           
            rollSpeed = Math3d.ProjectVectorOnPlane(controller.up, moveDirection).magnitude;

            currentState = PlayerStates.Roll;
            return;
        }

        // friction
        speed = clampF(0f, float.PositiveInfinity, speed - (MaxDiveSpeed * slideFriction * Time.deltaTime));
        moveDirection = transform.forward * speed;

        // stand up condition
        if (moveDirection.magnitude < 0.8f && slideTimer > 0.25f)
        {
            moveDirection = Vector3.zero;

            // todo: fix standup animation
            //currentState = PlayerStates.StandUp;
            currentState = PlayerStates.Idle;

            return;
        }

        // steering (fixed rotation amount)
        if (localMovement != Vector3.zero)
        {
            Vector3 Cross = Vector3.Cross(transform.forward, localMovement);

            if (Cross.magnitude > 0.02f)
            {
                transform.forward = Quaternion.AngleAxis(Mathf.Sign(Cross.y) * slideTurnSpeed * Time.deltaTime, controller.up) * transform.forward;
            }
            else if (localMovement != Vector3.zero)
            {
                // if it's a behind input manually turn a bit for deadlock. otherwise, snap dir
                if (Vector3.Cross(transform.right, localMovement).y < 0.98f)
                {
                    transform.forward = localMovement;
                }
            }
        }
    }

    void Slide_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Sliding", false);

        controller.feet.offset = 0.5f;
    }


    void StandUp_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("StandUp", true);
    }

    void StandUp_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("StandUp", false);
    }


    void Roll_EnterState()
    {
        diving = true;
        speed = rollSpeed;

        gameObject.GetComponent<Animator>().SetBool("Rolling", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection = transform.forward * speed;
        moveDirection.y += DiveJumpForce;
    }

    void Roll_SuperUpdate()
    {
        ground = false;

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        moveDirection = transform.forward * rollSpeed;

        // slight control
        float new_ratio = 0.9f * Time.deltaTime * maxAirSpeedTime;
        float old_ratio = 1.0f - new_ratio;

        // SPEED:
        float magnitude = input.Current.MoveInput.magnitude;
        if (magnitude > 0.9f)
        {
            magnitude = 1f;
        }

        float desiredForwardSpeed = Vector3.Cross(localMovement, transform.right).y * MaxDiveSpeed * magnitude;
        float desiredRightSpeed = Vector3.Cross(transform.forward, localMovement).y * MaxDiveSpeed * magnitude * 0.5f;

        speed = (speed * old_ratio) + (desiredForwardSpeed * new_ratio);
        xSpeed = (xSpeed * old_ratio) + (desiredRightSpeed * new_ratio);

        moveDirection = transform.forward * speed;
        moveDirection += transform.right * xSpeed;

        // gravity
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;
        moveDirection += verticalMoveDirection;
    }

    void Roll_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Rolling", false);
    }

    // animation event function
    public void FinishRoll()
    {
        currentState = PlayerStates.Fall;
    }


    //----------------------------------------------
    // Take Damage
    //----------------------------------------------

    void Damage_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();

        gameObject.GetComponent<Animator>().SetBool("Damage", true);
        //gameObject.GetComponent<Animator>().applyRootMotion = true;

    }

    void Damage_SuperUpdate()
    {
        if(Character_Manager.Instance != null && !Character_Manager.Instance.invincible)
        {
            currentState = PlayerStates.Idle;
            return;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
    }

    void Damage_ExitState()
    {
        //gameObject.GetComponent<Animator>().applyRootMotion = false;
        gameObject.GetComponent<Animator>().SetBool("Damage", false);
        gameObject.GetComponent<Collider>().enabled = true;
    }

    void FinishDamage()
    {
        currentState = PlayerStates.Idle;

        Character_Manager.Instance.invincible = false;
    }


    //----------------------------------------------
    // Death
    //----------------------------------------------

    void Dead_EnterState()
    {
        controller.EnableSlopeLimit();
        controller.EnableClamping();

        gameObject.GetComponent<Animator>().SetBool("Dead", true);

    }

    void Dead_SuperUpdate()
    {
        if(!Character_Manager.instance.isDead)
        {
            UI_Manager.instance.GameOver();
            currentState = PlayerStates.Idle;
            return;
        }

        // Apply friction to slow us to a halt
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, GroundFriction * Time.deltaTime);
    }

    void Dead_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Dead", false);
    }

    float clampF(float min, float max, float val)
    {
        if (val < min)
            return min;
        else if (val > max)
            return max;
        return val;
    }
}
