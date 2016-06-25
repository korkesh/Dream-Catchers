using UnityEngine;
using System.Collections;

/*
 * Example implementation of the SuperStateMachine and SuperCharacterController
 */
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
    public enum PlayerStates { Idle = 0, Walk = 1, Run = 2, Jump = 3, DoubleJump = 4, Fall = 5, Damage = 6, Dead = 7, Skid = 8, SkidJump = 9 };

    private SuperCharacterController controller;
    private RootCamera cam; // Main Player Follow Camera
    private PlayerInputController input; // Input Controller

    public Vector3 prevPos;

    //----------------------------------------------
    // Editor Fields
    //----------------------------------------------

    // Movement
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 10.0f;

    public float WalkSpeed = 0.1f;
    public float WalkspeedThreshold = 0.5f;

    public float RunSpeed = 0.65f;
    public float RunTurnSpeed = 10.0f;
    public float MaxRunSpeed = 6.0f;
    public float TurnRadius = 4.0f;
    public float runTimer = 0; // how long player has maintained runstate

    // Jumping
    public float VerticalSpeedCap = 10.0f;
    public float AirTurnSpeed = 10.0f;
    public float AirAcceleration = 3.0f;
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

    //----------------------------------------------
    // Functional Parameters
    //----------------------------------------------

    public float idleTimer { get; private set; } // how long the player has been idling

    public Vector3 moveDirection; // player movement direction vector

    public Vector3 facing; // direction player is facing

    public Vector3 lookDirection { get; private set; } // current direction camera is facing

    public float xRotation { get; private set; } // TODO: Wat is this?

    //private bool jumpHold = false; // controls variable jump heights by holding jump button - Currently Un-Used
    public float jumpTravelled = 0; // keeps track of 

    //----------------------------------------------
    // Debug Inspector Fields:
    //----------------------------------------------

    public float crossY;
    /*public float moveSpeed;
    public float hAxis;
    public float vAxis;
    public float inputMagnitude;
    public float inputPlayerCross;*/

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
        bool ground = controller.currentGround.IsGrounded(false, 0.01f);

        if (ground && cam)
        {
            //cam.setLastGround = transform.position.y;
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

    //================================
    // State Machine
    //================================

    //----------------------------------------------
    // Global Update
    //----------------------------------------------

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
        if (input.Current.AttackInput && (Game_Manager.instance != null && Game_Manager.instance.currentGameState != Game_Manager.GameState.GAMEOVER))
        {
            gameObject.GetComponent<PlayerCombat>().BeginAttack();
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
        transform.position += moveDirection * Time.deltaTime;

        // Rotate mesh to face correct direction (temp if implementing min turn radius)
        if (Math3d.ProjectVectorOnPlane(controller.up, facing) != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Math3d.ProjectVectorOnPlane(controller.up, facing), controller.up);
        }

        prevPos = transform.position;
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
        controller.EnableSlopeLimit();
        controller.EnableClamping();
    }

    void Idle_SuperUpdate()
    {
        idleTimer += Time.deltaTime;

        // Run every frame we are in the idle state
        // Dont allow attack and then jump
        if (input.Current.JumpInput && !gameObject.GetComponent<PlayerCombat>().attacking)
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

        if(idleTimer >= 5)
        {
            gameObject.GetComponent<Animator>().SetBool("IdleTimeOut", true);
        }
    }

    void Idle_ExitState()
    {
        idleTimer = 0;
        gameObject.GetComponent<Animator>().SetBool("IdleTimeOut", false);
        // Run once when we exit the idle state
    }

    //----------------------------------------------
    // Walking
    //----------------------------------------------

    void Walk_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Walking", true);
    }

    void Walk_SuperUpdate()
    {
        // Dont allow attack and then jump
        if (input.Current.JumpInput && !gameObject.GetComponent<PlayerCombat>().attacking)
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

    //----------------------------------------------
    // Running
    //----------------------------------------------

    // todo: > 180 degree turn case
    void Run_EnterState()
    {
        gameObject.GetComponent<Animator>().SetBool("Running", true);
    }

    void Run_SuperUpdate()
    {
        runTimer += Time.deltaTime;

        // Dont allow attack and then jump
        if (input.Current.JumpInput && !gameObject.GetComponent<PlayerCombat>().attacking)
        {
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if ((Input_Manager.instance.useBuffer && input.Current.moveBuffer) || (!Input_Manager.instance.useBuffer && input.Current.MoveInput != Vector3.zero))
        {
            // transition to walk condition
            if (Mathf.Abs(input.Current.MoveInput.x) + Mathf.Abs(input.Current.MoveInput.z) < WalkspeedThreshold)
            {
                currentState = PlayerStates.Walk;
                return;
            }

            if (!Input_Manager.instance.useBuffer)
            {
                moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * RunSpeed, RunAcceleration * Time.deltaTime);
                facing = LocalMovement(); // when walking always facing in direction moving todo: account for external forces
            }
            else
            {
                float new_ratio = 0.9f * Time.deltaTime * RunTurnSpeed;
                float old_ratio = 1 - new_ratio;

                transform.forward = ((moveDirection.normalized * old_ratio) + (LocalMovement() * new_ratio)).normalized;
                facing = transform.forward;

                moveDirection = transform.forward * MaxRunSpeed;

                if (Input_Manager.instance.useBuffer)
                {
                    // skid if input is >90 degrees of current facing direction
                    if (input.Current.MoveInput != Vector3.zero)
                    {
                        if (Vector3.Cross(Math3d.ProjectVectorOnPlane(controller.up, transform.right).normalized, Math3d.ProjectVectorOnPlane(controller.up, LocalMovement()).normalized).y > 0.4f)
                        {
                            currentState = PlayerStates.Skid;
                            transform.forward = Math3d.ProjectVectorOnPlane(Vector3.up, LocalMovement());
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            currentState = PlayerStates.Idle;
            runTimer = 0;

            return;
        }
    }

    void Run_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Running", false);
    }

    void Skid_EnterState()
    {
        transform.forward = LocalMovement();

        // immediate slowing effect
        moveDirection.Normalize();
        moveDirection *= 0.4f;
    }

    void Skid_SuperUpdate()
    {
        // when in skid state slow to a stop
        moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, Time.deltaTime * 10);

        if (moveDirection.magnitude < 0.1f)
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

        // input overrides rotation
        //float new_ratio = 0.9f * Time.deltaTime * RunTurnSpeed; Un-Used Variable
        //float old_ratio = 1 - new_ratio; Un-Used Variable

        transform.forward = Math3d.ProjectVectorOnPlane(Vector3.up, LocalMovement()).normalized;
        facing = transform.forward;
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
        JumpTimer = 0;

        gameObject.GetComponent<Animator>().SetBool("Jumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection = new Vector3(LocalMovement().x, 0, LocalMovement().z);
        transform.forward = moveDirection;
        facing = moveDirection;
        

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight * 1.8f, Gravity);
    }

    void SkidJump_SuperUpdate()
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


        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            float new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            float old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, LocalMovement()).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (LocalMovement() * new_ratio)).normalized;
            facing = transform.forward;

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            moveDirection = transform.forward * MaxRunSpeed * speedCoefficient;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
        }

        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection += verticalMoveDirection;
    }

    void SkidJump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Jumping", false);
        input.toggleJump = false;
        //jumpHold = false;
    }


    void Jump_EnterState()
    {
        //jumpHold = true;
        JumpTimer = 0;

        gameObject.GetComponent<Animator>().SetBool("Jumping", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        if (moveDirection.y < 0)
        {
            moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        }

        moveDirection += controller.up * CalculateJumpSpeed(MinJumpHeight, Gravity);

        // calculate external vertical movement
        float externalVerticalVelocity = ((transform.position - prevPos) / Time.deltaTime).y;
        moveDirection += new Vector3(0, externalVerticalVelocity, 0);

        // cap jump speed
        if (moveDirection.y > VerticalSpeedCap)
        {
            moveDirection = new Vector3(moveDirection.x, VerticalSpeedCap, moveDirection.z);
        }
    }

    void Jump_SuperUpdate()
    {
        // if holding jump button and not at max jump height, raise movement vector
        if (!input.Current.JumpHold)
        {
            //jumpHold = false;
        }

        //if (jumpHold)
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
        }

        // transition to double jump
        if (input.Current.JumpInput && input.toggleJump != true)
        {
            currentState = PlayerStates.DoubleJump;
            return;
        }

        // Allow ground pound only after a double jump
        /*if (input.Current.AttackInput)
        {
            currentState = PlayerStates.GroundPound;
            return;
        }*/

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;            
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            float new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            float old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, LocalMovement()).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (LocalMovement() * new_ratio)).normalized;
            facing = transform.forward;

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            moveDirection = transform.forward * MaxRunSpeed * speedCoefficient;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
        }

        //planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * MaxRunSpeed, AirAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection += verticalMoveDirection;
    }

    void Jump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("Jumping", false);
        input.toggleJump = false;
        //jumpHold = false;
    }

    //----------------------------------------------
    // Double Jump
    //----------------------------------------------

    void DoubleJump_EnterState()
    {

        gameObject.GetComponent<Animator>().SetBool("DoubleJump", true);

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        moveDirection += controller.up * CalculateJumpSpeed(DoubleJumpHeight, Gravity);
    }

    void DoubleJump_SuperUpdate()
    {
        /*if (input.Current.AttackInput)
        {
            currentState = PlayerStates.GroundPound;
            return;
        }*/

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
        Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

        if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
        {
            moveDirection = planarMoveDirection;
            currentState = PlayerStates.Idle;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero)
        {
            float new_ratio = 0.9f * Time.deltaTime * AirTurnSpeed;
            float old_ratio = 1.0f - new_ratio;

            // hack: turn a tiny bit manually to avoid 180 degree lock
            if (Vector3.Cross(transform.right, LocalMovement()).y > 0.988f)
            {
                transform.forward = Quaternion.AngleAxis(1, controller.up) * transform.forward;
            }

            transform.forward = ((transform.forward * old_ratio).normalized + (LocalMovement() * new_ratio)).normalized;
            facing = transform.forward;

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            moveDirection = transform.forward * MaxRunSpeed * speedCoefficient;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
        }

        //planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * MaxRunSpeed, AirAcceleration * Time.deltaTime);
        verticalMoveDirection -= controller.up * Gravity * Time.deltaTime;

        moveDirection += verticalMoveDirection;
    }

    void DoubleJump_ExitState()
    {
        gameObject.GetComponent<Animator>().SetBool("DoubleJump", false);
        input.toggleJump = false;
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
        /*if (input.Current.AttackInput)
        {
            currentState = PlayerStates.GroundPound;
            return;
        }*/

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

    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    // 
    // COMBAT STATES
    //
    //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

    //----------------------------------------------
    // Ground Pound [DISABLED]
    //----------------------------------------------

    /*void GroundPound_EnterState()
    {
        Debug.Log("Entering Pound State");

        gameObject.GetComponent<PlayerCombat>().BeginPound();
    }

    void GroundPound_SuperUpdate()
    {
        if (AcquiringGround())
        {
            moveDirection = Math3d.ProjectVectorOnPlane(controller.up, Vector3.zero);
            if(!gameObject.GetComponent<Animator>().GetBool("GroundPound"))
            {
                currentState = PlayerStates.Idle;
            }
            return;
        }

        moveDirection -= controller.up * GroundPoundSpeed * Time.deltaTime;
    }

    void GroundPound_ExitState()
    {
        Debug.Log("Leaving Pound State");
    }*/

    //----------------------------------------------
    // Take Damage
    //----------------------------------------------

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

}
