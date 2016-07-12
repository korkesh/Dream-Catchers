using UnityEngine;
using System.Collections;

// todo: air state locks rotation and moves to keep player at correct viewport pos
public class NewCamera : MonoBehaviour
{
    //===========================================
    // Fields
    //===========================================

    // Modes
    public enum CameraMode
    {
        High,
        Low
    }

    public CameraMode Mode { get; private set;}

    //===========================================
    // Player:
    //===========================================
    public GameObject Player; // focus object
    private PlayerInputController input;
    private PlayerMachine machine;
    private SuperCharacterController controller;

    private Vector3 Target; // actual focus coordinate (offset from player)
    public float targetOffset; // how far above player origin to focus
    private Vector3 vTargetOffset; // vector to add targetOffset to Player.pos
    private Vector3 TargetPos; // position cam wants to be at

    private Vector3 PlayerRoot; // player.x, cam.y, player.z
    private Vector3 TargetDisplacement; // displacement from cam to player target
    private Vector3 BaseDisplacement; // displacement from cam to player transform


    //==========================================
    // Constraints:
    //==========================================
    private float lastGround; // y value of ground either previously stood on or currently slightly above

    // y movement thresholds. approximately height of regular jump in high mode, lower to compensate for low mode
    public float hMaxJumpHeight;   
    public float lMaxJumpHeight;

    public float lookDistance;

    // max planar distance from cam to player
    public float hFollowDistance;
    public float lFollowDistance;

    // Height offset from LastGround position
    public float hHeightGround;
    public float hHeightAir;
    public float lHeightGround;
    public float lHeightAir;

    // Static angles
    public float hAngleGround; // = ~15~
    public float hAngleAir;
    public float lAngleGround; // = ~10~
    public float lAngleAir;
    private float angleOffset = 0; // offset applied to static x angle

    // Jump state variables
    private Vector3 JumpOrigin; // position at which player left the ground
    private Vector3 JumpDisplacement; // displacement vector on frame player left ground


    //=========================================
    // Active Variables
    //=========================================
    private float currentFollowDistance;
    private float currentHeight; // current height above player offset
    private float currentAngle; // current x rotation
    private float currentMaxJumpHeight; // height at which camera will begin raising to follow player y movement

    private Vector3 CurrentTargetPos; // position target is at this frame
    private float currentTargetOffset = 0;


    //==========================================
    // Smoothing Coefficients
    //==========================================
    public float smoothFollow;
    public float smoothVertical;
    public float rotateSpeed;


    // *** Debug ***
    public GameObject test1;
    public GameObject test2;


    void Start ()
    {
        Mode = CameraMode.High;

        Player = GameObject.FindGameObjectWithTag("Player"); // Character_Manager.Instance.Character;

        input = Player.GetComponent<PlayerInputController>();
        machine = Player.GetComponent<PlayerMachine>();
        controller = Player.GetComponent<SuperCharacterController>();

        vTargetOffset = new Vector3(0, targetOffset, 0);

        Target = Player.transform.position + vTargetOffset;
        CurrentTargetPos = Player.transform.position + vTargetOffset;
    }
	

	void LateUpdate ()
    {
        lastGround = controller.currentGround.groundHeight + vTargetOffset.y;

        // change modes if player pressed R
        if (input.Current.RButton)
        {
            if (Mode == CameraMode.High)
            {
                Mode = CameraMode.Low;
            }
            else
            {
                Mode = CameraMode.High;
            }
        }

        // manual x control (rotate around player pivot)
        if (input.Current.Joy2Input.x != 0)
        {
            transform.RotateAround(Player.transform.position, controller.up, Time.deltaTime * rotateSpeed * 10f * input.Current.Joy2Input.x);
        }

        UpdateTarget();
        UpdateActiveVariables();
        UpdateHeight();     
        UpdateVectors();
        ConstrainDistance();
        UpdateRotation();
        UpdateTarget();

        RaycastHit hit = new RaycastHit();
        if (CheckCollision(Player.transform.position + vTargetOffset, transform.position, out hit))
        {
            transform.position = hit.point;
            UpdateRotation();
        }
    }


    void UpdateActiveVariables()
    {
        switch (Mode)
        {
            case CameraMode.High:
                {
                    currentFollowDistance = hFollowDistance;
                    currentMaxJumpHeight = hMaxJumpHeight;

                    if (machine.ground)
                    {
                        currentHeight = hHeightGround;
                        currentAngle = hAngleGround;
                        angleOffset = 0;
                    }
                    else
                    {
                        currentHeight = hHeightAir;
                        currentAngle = hAngleAir;
                    }

                    return;
                }
            case CameraMode.Low:
                {
                    currentFollowDistance = lFollowDistance;
                    currentMaxJumpHeight = lMaxJumpHeight;

                    if (machine.ground)
                    {
                        currentHeight = lHeightGround;
                        currentAngle = lAngleGround;
                        angleOffset = 0;
                    }
                    else
                    {
                        currentHeight = lHeightAir;
                        currentAngle = lAngleAir;
                    }

                    return;
                }
        }
    }


    void UpdateTarget()
    {
        Target = Player.transform.position + vTargetOffset; // base pos

        // determine how aligned player forward is with displacement vector
        BaseDisplacement = (Player.transform.position - transform.position);
        BaseDisplacement.y = 0;

        float align = Vector3.Cross(BaseDisplacement.normalized, Player.transform.forward).y;

        // local right is inconsistent as camera looks ahead of player, so use cross of up/cam-player dir as constant right
        Vector3 right = Vector3.Cross(Vector3.up, BaseDisplacement.normalized);
        
        Target += (right * align * lookDistance);

        // smoothly move target left/right in ground state
        if (machine.ground)
        {     
            currentTargetOffset = Clamp(-lookDistance, lookDistance, currentTargetOffset + (Mathf.Sign(align * lookDistance - currentTargetOffset) * Time.deltaTime * 0.5f * Mathf.Abs(align * lookDistance - currentTargetOffset)));
        }

        CurrentTargetPos = (Player.transform.position + vTargetOffset) + (currentTargetOffset * right);

        test1.transform.position = Target; // debug
        test2.transform.position = CurrentTargetPos;
    }


    void UpdateHeight()
    {
        currentHeight += Clamp(0, float.PositiveInfinity, (Target.y - lastGround) - currentMaxJumpHeight);
        currentHeight -= Clamp(0, float.PositiveInfinity, lastGround - Target.y);

        TargetPos = new Vector3(transform.position.x, lastGround + currentHeight, transform.position.z);

        if (TargetPos.y > transform.position.y)
        {
            // upward movement should be essentially snapped
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, (TargetPos - transform.position).magnitude * smoothVertical * Time.deltaTime);
        }
        else
        {
            // downward movement is slower, rotating downward to keep player in view
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, (TargetPos - transform.position).magnitude * smoothVertical * Time.deltaTime * 0.2f);
        }
    }


    void UpdateVectors()
    {
        PlayerRoot.x = CurrentTargetPos.x;
        PlayerRoot.y = transform.position.y;
        PlayerRoot.z = CurrentTargetPos.z;

        TargetDisplacement = PlayerRoot - transform.position;
    }


    void ConstrainDistance()
    {
        if (!machine.ground)
        {
            // in air follow player via movement instead of rotation, so match player displacement per frame (ignoring y)
            transform.position += new Vector3(machine.transform.position.x - machine.prevPos.x, 0, machine.transform.position.z - machine.prevPos.z);
        }

        // in ground state maintain player distance by travelling along cam-player displacement vector
        else if (!Mathf.Approximately(BaseDisplacement.magnitude, currentFollowDistance))
        {          
            TargetPos = transform.position + (BaseDisplacement.normalized * (BaseDisplacement.magnitude - currentFollowDistance));
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, smoothFollow * Time.deltaTime);
        }
    }


    void UpdateRotation()
    {
        // y-axis rotation to look at player
        if (Vector3.Cross(TargetDisplacement.normalized, transform.right).y < 1)
        {
            Vector3 PlanarForward = transform.forward;
            PlanarForward.y = 0;
            PlanarForward.Normalize();

            float dir = Mathf.Sign(Vector3.Cross(PlanarForward, TargetDisplacement.normalized).y);

            float angle = Vector3.Angle(PlanarForward, TargetDisplacement.normalized);

            // in ground state rotation is locked
            if (machine.ground)
            {
                transform.forward = Quaternion.AngleAxis(angle * dir, Vector3.up) * transform.forward;
            }
        }

        // x - axis rotation: 
        // TODO: fix slight jitter from long falls
        // TODO: look further downward
        // in air states, rotate down further if player goes below viewport threshold
        if (machine.moveDirection.y < -17.5f)
        {
            Vector3 DisplacementDir = (Target - transform.position).normalized;
            if (DisplacementDir.y < transform.forward.y)// - 0.25f)//0.225f)
            {
                if (Vector3.Cross(transform.forward, DisplacementDir).magnitude > 0.2f)
                {
                    Debug.Log("changing angle");
                    angleOffset = Mathf.Max(angleOffset, Vector3.Angle(transform.forward, DisplacementDir));
                }
            }
        }

        // TODO: fix rotation smoothening
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + (currentAngle + angleOffset - transform.eulerAngles.x) * Time.deltaTime * rotateSpeed, transform.eulerAngles.y, transform.eulerAngles.z);
    }


    bool CheckCollision(Vector3 start, Vector3 end, out RaycastHit hit)
    {
        //RaycastHit hit = new RaycastHit();

        Vector3 PlayerRoot = Player.transform.position + vTargetOffset;

        if (Physics.Raycast(PlayerRoot, (transform.position - PlayerRoot).normalized, out hit, (transform.position - PlayerRoot).magnitude))
        {
            Debug.DrawLine(PlayerRoot, hit.point, Color.red);
            if (hit.transform.gameObject.tag == "Wall")
            {
                return true;
            }
        }

        return false;
    }


    // Mathf.Clamp doesn't work?????????????????
    float Clamp(float min, float max, float val)
    {
        if (val < min)
            return min;
        else if (val > max)
            return max;
        return val;
    }
}
