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
    [SerializeField]
    private Vector3 Displacement; // direction from cam to player
    private Vector3 BaseDisplacement;


    //==========================================
    // Constraints:
    //==========================================
    private float lastGround; // y value of ground either previously stood on or currently slightly above
    public float maxJumpHeight; // max height of a regular jump. camera y does not increase until this is exceeded   

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


    //=========================================
    // Active Variables
    //=========================================
    private float currentFollowDistance;
    private float currentHeight; // current height above player offset
    private float currentAngle; // current x rotation

    private Vector3 CurrentTargetPos; // position target is at this frame
    private float CurrentTargetOffset = 0;


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
	

	void Update ()
    {
        lastGround = controller.currentGround.groundHeight + vTargetOffset.y;

        UpdateTarget();

        UpdateActiveVariables();

        UpdateHeight();
       
        UpdateVectors();  

        // constrain distance
        if (!Mathf.Approximately(BaseDisplacement.magnitude, currentFollowDistance))
        {
            TargetPos = transform.position + (BaseDisplacement.normalized * (BaseDisplacement.magnitude - currentFollowDistance));

            transform.position = Vector3.MoveTowards(transform.position, TargetPos, smoothFollow * Time.deltaTime);
        }

        UpdateRotation();

    }


    void UpdateActiveVariables()
    {
        switch (Mode)
        {
            case CameraMode.High:
                {
                    currentFollowDistance = hFollowDistance;

                    if (machine.ground)
                    {
                        currentHeight = hHeightGround;
                        currentAngle = hAngleGround;
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

                    if (machine.ground)
                    {
                        currentHeight = lHeightGround;
                        currentAngle = lAngleGround;
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
        Target += (Vector3.Cross(Vector3.up, BaseDisplacement.normalized) * align * lookDistance);

        // smoothly move target left/right
        CurrentTargetOffset = Clamp(-lookDistance, lookDistance, CurrentTargetOffset + (Mathf.Sign(align * lookDistance - CurrentTargetOffset) * Time.deltaTime * Mathf.Abs(align * lookDistance - CurrentTargetOffset)));
        CurrentTargetPos = (Player.transform.position + vTargetOffset) + (CurrentTargetOffset * Vector3.Cross(Vector3.up, BaseDisplacement.normalized));
 
        //CurrentTargetPos = Vector3.MoveTowards(CurrentTargetPos, Target, (Target - CurrentTargetPos).magnitude * Time.deltaTime);

        test1.transform.position = Target; // debug
        test2.transform.position = CurrentTargetPos;
    }


    void UpdateHeight()
    {
        currentHeight += Clamp(0, float.PositiveInfinity, (Target.y - lastGround) - maxJumpHeight);
        currentHeight -= Clamp(0, float.PositiveInfinity, lastGround - Target.y);


        TargetPos = new Vector3(transform.position.x, lastGround + currentHeight, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, (TargetPos - transform.position).magnitude * smoothVertical * Time.deltaTime);
    }


    void UpdateVectors()
    {
        PlayerRoot.x = CurrentTargetPos.x;
        PlayerRoot.y = transform.position.y;
        PlayerRoot.z = CurrentTargetPos.z;

        Displacement = PlayerRoot - transform.position;
    }


    void UpdateRotation()
    {
        // y-axis rotation to look at player
        if (Vector3.Cross(Displacement.normalized, transform.right).y < 1)
        {
            Vector3 PlanarForward = transform.forward;
            PlanarForward.y = 0;
            PlanarForward.Normalize();

            float dir = Mathf.Sign(Vector3.Cross(PlanarForward, Displacement.normalized).y);

            float angle = Vector3.Angle(PlanarForward, Displacement.normalized);

            // in ground state rotation is locked
            if (machine.ground)
            {
                //transform.forward = new Vector3(Displacement.normalized.x, transform.forward.y, Displacement.normalized.z);
            }
            //else
            {
                transform.forward = Quaternion.AngleAxis(angle * dir/* * Clamp(0.1f, 1f, (1f - Vector3.Cross(Displacement.normalized, transform.right).y)) * rotateSpeed * Time.deltaTime */, Vector3.up) * transform.forward;
            }
        }


        // x-axis rotation(static)
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + (currentAngle - transform.eulerAngles.x) * Time.deltaTime * rotateSpeed * 0.25f, transform.eulerAngles.y, transform.eulerAngles.z);

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
