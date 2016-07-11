﻿///===================================================================================
/// Author: Matt (Combat states) & Connor (All Non-combat states; primary contributer)
/// Purpose: Loosely based on a third party script with significant modifications
///          This script handles all movement logic for the player including
///          idle, walking, running, jumping and combat states. 
///==================================================================================
///
using UnityEngine;
using System.Collections;
using System;

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
    public Vector3 Displacement; // displacement this frame

    public bool ground { get; private set; }

    //----------------------------------------------
    // Editor Fields
    //----------------------------------------------

    // Movement
    public float WalkAcceleration = 30.0f;
    public float RunAcceleration = 10.0f;

    //public float WalkSpeed = 0.1f;
    //public float WalkspeedThreshold = 0.5f;

    public float maxSpeedTime = 5; // amount of time it takes to go from idle to max speed (1 = 1s, 10 = 0.1s)
    public float maxAirSpeedTime = 1; // amount of time it takes to go from 0 x/z speed to max in the air
    private float speed = 0; // current run speed
    public float RunSpeed = 0.65f;
    public float RunTurnSpeed = 10.0f;
    public float MaxRunSpeed = 6.0f;
    public float TurnRadius = 4.0f;
    public float runTimer = 0; // how long player has maintained runstate
    private float skidTimer = 0;
    public float skidTime = 0.5f; // time in seconds skid state lasts for if not broken by jump/fall

    // Jumping
    public float VerticalSpeedCap = 10.0f;
    public float VerticalSpeedCapDown = -30.0f;
    public float AirTurnSpeed = 10.0f;
    public float AirAcceleration = 3.0f;
    public float JumpAcceleration = 5.0f;
    public float JumpHoldAcceleration = 10.0f;
    public float JumpHoldTime = 0.5f; // amount of time holding jump button extends height after initial press
    public float MinJumpHeight = 1.75f;
    public float MaxJumpHeight = 3.0f;
    public float DoubleJumpHeight = 2.0f;
    public float JumpTimer = 0;
    public Vector3 LastGroundPos { get; private set; } // position character was at last frame they were grounded

    // Physics
    public float Gravity = 25.0f;
    public float GroundFriction = 10.0f;

    //----------------------------------------------
    // Functional Parameters
    //----------------------------------------------

    public float idleTimer { get; private set; } // how long the player has been idling

    public Vector3 moveDirection; // player movement direction vector

    public Vector3 lookDirection { get; private set; } // current direction camera is facing

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
        if (input.Current.AttackInput && (Game_Manager.instance != null && Game_Manager.instance.currentGameState != Game_Manager.GameState.GAMEOVER) && Character_Manager.instance.toggleHammer)
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
            currentState = PlayerStates.Run;
            return;
        }

        // Apply friction to slow us to a halt
        float new_ratio = 0.9f * Time.deltaTime * maxSpeedTime;
        float old_ratio = 1 - new_ratio;

        speed = (speed * old_ratio) - (speed * new_ratio);
        if (speed < 0)
            speed = 0;

        moveDirection = transform.forward * speed;

        if(idleTimer >= 5)
        {
            gameObject.GetComponent<Animator>().SetBool("IdleTimeOut", true);
        }

        // ANIMATION:
        if (speed / MaxRunSpeed > 0.5f)
        {
            gameObject.GetComponent<Animator>().SetBool("Running", true);
            gameObject.GetComponent<Animator>().SetBool("Walking", false);
        }
        else if (speed / MaxRunSpeed > 0.1f)
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
        // Run once when we exit the idle state
    }

  
    //----------------------------------------------
    // Running
    //----------------------------------------------

    void Run_EnterState()
    {
        //gameObject.GetComponent<Animator>().SetBool("Running", true);
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

        if (input.Current.MoveInput != Vector3.zero)
        {
            // ROTATION:
            float new_ratio = 0.9f * Time.deltaTime * RunTurnSpeed;
            float old_ratio = 1 - new_ratio;

            transform.forward = ((moveDirection.normalized * old_ratio) + (LocalMovement() * new_ratio)).normalized;

            // skid if input is >90 degrees of current facing direction
            if (Vector3.Cross(Math3d.ProjectVectorOnPlane(controller.up, transform.right).normalized, Math3d.ProjectVectorOnPlane(controller.up, LocalMovement()).normalized).y > 0.4f)
            {
                currentState = PlayerStates.Skid;
                transform.forward = Math3d.ProjectVectorOnPlane(Vector3.up, LocalMovement());
                return;
            }
            

            // SPEED:
            // get desired speed
            float desiredSpeed = (float)Math.Round(clampF(0f, 1f, input.Current.MoveInput.magnitude) * MaxRunSpeed, 2);

            new_ratio = 0.9f * Time.deltaTime * maxSpeedTime;
            old_ratio = 1 - new_ratio;

            speed = (speed * old_ratio) + (desiredSpeed * new_ratio);

            moveDirection = transform.forward * speed;


            // ANIMATION:
            if (speed / MaxRunSpeed > 0.5f)
            {
                gameObject.GetComponent<Animator>().SetBool("Running", true);
                gameObject.GetComponent<Animator>().SetBool("Walking", false);
            }
            else if (speed / MaxRunSpeed > 0.1f)
            {
                gameObject.GetComponent<Animator>().SetBool("Running", false);
                gameObject.GetComponent<Animator>().SetBool("Walking", true);
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
        skidTimer = 0;

        transform.forward = LocalMovement();

        // immediate slowing effect
        moveDirection = moveDirection.normalized * 0.25f;
    }

    void Skid_SuperUpdate()
    {
        skidTimer += Time.deltaTime;

        transform.forward = LocalMovement();

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

        transform.forward = Math3d.ProjectVectorOnPlane(Vector3.up, LocalMovement()).normalized;
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

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            speed = MaxRunSpeed * speedCoefficient;
            moveDirection = transform.forward * speed;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
            speed = 0;
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

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            speed = MaxRunSpeed * speedCoefficient;
            moveDirection = transform.forward * speed;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
            speed = 0;
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

            // speed is a function of how aligned the input direction is with the player forward vector
            float cross = Vector3.Cross(LocalMovement(), transform.right).y;

            // normalize cross
            float speedCoefficient = (cross - -1) / (1 - -1);

            speed = MaxRunSpeed * speedCoefficient;
            moveDirection = transform.forward * speed;
        }
        else
        {
            moveDirection = Vector3.zero; // todo: add slight buffer?
            speed = 0;
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
        //gameObject.GetComponent<Animator>().applyRootMotion = true;

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
        //gameObject.GetComponent<Animator>().applyRootMotion = false;
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

    float clampF(float min, float max, float val)
    {
        if (val < min)
            return min;
        else if (val > max)
            return max;
        return val;
    }
}