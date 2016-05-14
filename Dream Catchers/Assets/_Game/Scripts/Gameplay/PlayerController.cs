using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        idle,
        walk,
        run,
        jump,
        fall
    }

    //===================================
    // Fields
    //===================================

    public PlayerState state;

    public Vector3 pOrigin; // center of collider

    public Vector3 velocity; // movement vector (distance/second)

    public Vector3 camForward; // forward vector relative to camera
    public Vector3 camRight; // right vector relative to camera

    public float hAxis;
    public float vAxis;

    public CapsuleCollider pCollider;

    public float jumpTimer; // controls position over time
    public bool doubleJump = true; // todo


    //===================================
    // Editor Fields
    //===================================

    public float moveSpeed; // walk/run speed
    public float maxSpeed; // max run speed
    public float gravity; // force of gravity
    public float maxFallSpeed;

    public float runThreshold; // at what speed walk transitions to run

    //===================================
    // Functions
    //===================================

    void Start()
    {
        pCollider = GetComponent<CapsuleCollider>();
        pOrigin = transform.position + pCollider.center;

        state = PlayerState.fall; // temp
    }

    void Update()
    {
        CheckInput();

        UpdateState();

        Movement();
    }

    public void CheckInput()
    {
        // get input axes
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");

        // check button events and update states
        if (Input.GetAxis("Fire1") == 1) // is input.getaxis the best approach to button checks?
        {
            // jump
            if (state == PlayerState.idle || state == PlayerState.walk || state == PlayerState.run)
            {
                state = PlayerState.jump;
                jumpTimer = 0;
            }
            else if (state == PlayerState.jump && doubleJump)
            {
                // todo: doublejump
            }
        }
    }

    // checks state-change conditions at the beginning of the frame
    public void UpdateState()
    {
        // transition to falling state
        if (state == PlayerState.idle || state == PlayerState.run || state == PlayerState.walk)
        {
            // ground check (TODO implement optimal solution)
        }
    }

    public void Movement()
    {
        // movement is relative to camera so get relative direction vectors
        camForward = Camera.main.transform.forward;
        camForward.y = 0; // ignore x axis rotation
        camForward.Normalize();

        camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        //=================================
        // Movement States
        //=================================

        // in walking/idle states, directional input is movement vector
        if (state == PlayerState.idle || state == PlayerState.walk)
        {
            if (velocity.magnitude >= runThreshold)
            {
                state = PlayerState.run;
            }
            else
            {
                Vector3 vInput = ((camRight * moveSpeed * hAxis * Time.deltaTime) + (camForward * moveSpeed * vAxis * Time.deltaTime));
                velocity = Vector3.ClampMagnitude(vInput, maxSpeed);
            }
        }

        if (state == PlayerState.run)
        {
            if (velocity.magnitude < runThreshold)
            {
                state = PlayerState.walk;
            }
            else
            {
                // check previous input against this frame's input. if angle is too wide, slide/turnaround
                // temp: copy walk
                Vector3 vInput = ((camRight * moveSpeed * hAxis * Time.deltaTime) + (camForward * moveSpeed * vAxis * Time.deltaTime));
                velocity = Vector3.ClampMagnitude(vInput, maxSpeed);
            }
        }

        // JUMPING:
        if (state == PlayerState.jump)
        {
            jumpTimer += Time.deltaTime;
        }

        // FALLING:
        if (state == PlayerState.fall)
        {
            velocity.y -= gravity * Time.deltaTime; //Mathf.Min(velocity.y + (gravity * Time.deltaTime), maxFallSpeed);
        }

        pOrigin += velocity;

        transform.position = pOrigin - pCollider.center;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggeRed");
        Vector3 oOrigin = other.transform.position + other.bounds.center;

        // hit the floor from above (landing) TODO: account for ceiling case (temp logic)
        if (pOrigin.y - pCollider.bounds.extents.y < oOrigin.y + other.bounds.extents.y)
        {
            // transition states
            float speed = velocity.x + velocity.z;

            if (speed == 0)
            {
                state = PlayerState.idle;
            }
            else if (speed < maxSpeed * 0.5f)
            {
                state = PlayerState.walk;
            }
            else
            {
                state = PlayerState.run;
            }

            //velocity.y = 0;

            //align bottom of character box with top of floor box
            pOrigin.y = (oOrigin.y + other.bounds.extents.y) + GetComponent<BoxCollider>().bounds.extents.y;
        }

        // hit the floor from the side
        //else if (pOrigin.x < oOrigin.x - other.bounds.extents.x || pOrigin.x > oOrigin.x + other.bounds.extents.x)
        //{

        //}

    }

}
