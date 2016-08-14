using UnityEngine;
using System.Collections;


// Author: Conor MacKeigan
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

    public CameraMode Mode { get; private set; }

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
    private Vector3 PrevDispDir; // displacement from cam to player on previous frame

    //==========================================
    // Constraints:
    //==========================================
    [SerializeField]
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

    public float maxDownSpeed;

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

    private float heightOffset; // how far current height is from target height

    private Vector3 CurrentTargetPos; // position target is at this frame
    private float currentTargetOffset = 0;

    private bool rotate = false; // set to true for the frame if player manipulated rotation
    [SerializeField]
    private float xRotationOffset = 0; // player manipulation value to x axis rotation

    private float currentRotateSpeed = 0; // speed at which camera is rotating around y axis (accelerates)
    [SerializeField]
    private float floorArc = 0; // amount of arc rotation applied to see above floors

    bool collision = false; // set to true each frame if any collision occurred (prevents target updating/jitter)

    //==========================================
    // Smoothing Coefficients
    //==========================================
    public float smoothFollow;
    public float smoothVertical;
    public float rotateSpeed;
    public float lastGroundSpeed; // speed at which camera moves up/down when lastGround is changed


    void Start()
    {
        transform.eulerAngles = Vector3.zero;

        Mode = CameraMode.Low;

        Player = GameObject.FindGameObjectWithTag("Player"); // Character_Manager.Instance.Character;

        input = Player.GetComponent<PlayerInputController>();
        machine = Player.GetComponent<PlayerMachine>();
        controller = Player.GetComponent<SuperCharacterController>();

        vTargetOffset = new Vector3(0, targetOffset, 0);
        Target = Player.transform.position + vTargetOffset;
        CurrentTargetPos = Player.transform.position + vTargetOffset;

        BaseDisplacement = (Player.transform.position - transform.position);
        BaseDisplacement.y = 0;

        lastGround = controller.currentGround.groundHeight + vTargetOffset.y;

        UpdateActiveVariables();
        transform.eulerAngles = new Vector3(currentAngle + angleOffset, transform.eulerAngles.y, transform.eulerAngles.z);
    }


    void LateUpdate()
    {
        CheckLock(); // checks if the camera is "locked" (directly above player)
        UpdateMode(); // updates between high/low mode

        ManualRotation(); // right stick-driven arc around player

        UpdateActiveVariables(); // updates variables such as follow distance, height and angle based on mode and other interactions

        // do not update distance/height during collision unless planar forward distance threshold is exceeded
        if ((!collision && floorArc == 0f) || Math3d.ProjectVectorOnPlane(controller.up, (Player.transform.position + vTargetOffset) - transform.position).magnitude > currentFollowDistance)
        {
            UpdateHeight(); // updates height variables based on player position
            ConstrainDistance(); // planar forward player follow
        }

        UpdateTarget(); // updates the view target, telling the camera what to focus on and facilitating look ahead features

        UpdateVectors(); // update displacement vectors for calculations
        UpdateRotation(); // update automatic camera rotation to keep Target in center of X-axis and modifies X-axis rotation for fall tracking

        ManualLook(); // right-stick driven up/down rotation

        // Occlusion avoidance for floors and walls
        collision = false;
        CheckOcclusion();

        UpdateTarget(); // update target again to smoothen changes since previous update
    }


    void UpdateActiveVariables()
    {
        switch (Mode)
        {
            case CameraMode.High:
                {
                    currentFollowDistance = hFollowDistance;
                    currentMaxJumpHeight = hMaxJumpHeight;

                    if (!machine.jumping)
                    {
                        currentHeight = hHeightGround;
                        currentAngle = hAngleGround + xRotationOffset + floorArc;
                        angleOffset = 0;
                    }
                    else
                    {
                        currentHeight = hHeightGround;//Air;
                        currentAngle = hAngleGround + floorArc;//Air + floorArc;
                        xRotationOffset = 0;
                    }

                    return;
                }
            case CameraMode.Low:
                {
                    currentFollowDistance = lFollowDistance;
                    currentMaxJumpHeight = lMaxJumpHeight;

                    if (!machine.jumping)
                    {
                        currentHeight = lHeightGround;
                        currentAngle = lAngleGround + xRotationOffset + floorArc;
                        angleOffset = 0;
                    }
                    else
                    {
                        currentHeight = lHeightGround;//Air;
                        currentAngle = lAngleGround + floorArc;//Air;
                        xRotationOffset = 0;
                    }

                    return;
                }
        }
    }


    // R-button mode swap
    void UpdateMode()
    {
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
    }


    // Arcs camera position around player driven by right-stick input
    void ManualRotation()
    {
        // manual x control (rotate around player pivot)
        rotate = false;
        RaycastHit hit;

        if (input.Current.LTrigger > 0.2f || input.Current.RTrigger > 0.2f)
        {
            rotate = true;
            if (currentRotateSpeed < 6f)
            {
                currentRotateSpeed = 6f;
            }

            transform.RotateAround(Player.transform.position, controller.up, Time.deltaTime * rotateSpeed * Mathf.Max(Mathf.Min(18, currentRotateSpeed += Time.deltaTime * 32f), 8f) * (input.Current.LTrigger * -1f + input.Current.RTrigger));
        }
        else if (Mathf.Abs(input.Current.Joy2Input.x) > 0.25f)//&& machine.ground)
        {
            rotate = true;
            transform.RotateAround(Player.transform.position, controller.up, Time.deltaTime * rotateSpeed * Mathf.Max(Mathf.Min(18, currentRotateSpeed += Time.deltaTime * 32f), 8f) * input.Current.Joy2Input.x);

            // if rotation caused a collision revert
            if (CheckCollision(Player.transform.position + vTargetOffset, transform.position, out hit))
            {
                if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Floor")
                {
                    //transform.RotateAround(Player.transform.position, controller.up, Time.deltaTime * -rotateSpeed * Mathf.Min(16, currentRotateSpeed += Time.deltaTime * 32) * input.Current.Joy2Input.x);
                    //rotate = false;
                }
            }
        }
        else
        {
            currentRotateSpeed = 0;// Clamp(0, float.PositiveInfinity, currentRotateSpeed + (0f - currentRotateSpeed) * Time.deltaTime * 4);
        }
        
    }


    // updates position of tracking target
    void UpdateTarget()
    {
        // store previous frame's displacement before updating
        PrevDispDir = BaseDisplacement.normalized;

        // apply offset for varying player collision sphere heights
        Vector3 sphereOffset = new Vector3(0f, controller.feet.offset - 0.5f, 0f);

        Target = Player.transform.position + vTargetOffset + sphereOffset; // base pos

        // determine how aligned player forward is with displacement vector

        BaseDisplacement = (Player.transform.position - transform.position);
        BaseDisplacement.y = 0;

        float align = Vector3.Cross(BaseDisplacement.normalized, Player.transform.forward).y;

        // local right is inconsistent as camera looks ahead of player, so use cross of up/cam-player dir as constant right
        Vector3 right = Vector3.Cross(Vector3.up, BaseDisplacement.normalized);

        Target += (right * align * lookDistance);

        if (collision || floorArc > 0f)
        { // focus on character origin when in a collision state
            align = 0f;
        }

        // smoothly move target left/right in ground state or with occlusion avoidance active
        if (machine.ground && !machine.jumping)
        {
            currentTargetOffset = Clamp(-lookDistance, lookDistance, currentTargetOffset + (Mathf.Sign(align * lookDistance - currentTargetOffset) * Time.deltaTime /* 0.5f */* Mathf.Abs(align * lookDistance - currentTargetOffset)));
        }

        CurrentTargetPos = (Player.transform.position + vTargetOffset + sphereOffset) + (currentTargetOffset * right);
    }


    // updates currentHeight and lastGround variables
    void UpdateHeight()
    {
        // update anchor height based on most recent ground height
        float newGround = controller.currentGround.groundHeight + vTargetOffset.y;

        if (newGround > lastGround)
        {
            lastGround += (newGround - lastGround) * Time.deltaTime * lastGroundSpeed;
        }
        else
        {
            lastGround += (newGround - lastGround) * Time.deltaTime * lastGroundSpeed * 0.2f;
        }

        currentHeight += Clamp(0f, float.PositiveInfinity, ((Target.y - lastGround) - currentMaxJumpHeight)); // move up by amount player exceeds threshold

        // apply rotation offset to keep player in view
        if (newGround < lastGround)
        {
            currentAngle += Clamp(0f, 60f, Mathf.Max((lastGround - newGround) * 4f, (lastGround - Target.y) * 4f));
        }
        else if (Target.y < lastGround)
        {
            currentAngle += Clamp(0f, 60f, (lastGround - Target.y) * 4f);
        }
    }


    // updates displacement between the tracking target and the camera
    void UpdateVectors()
    {
        PlayerRoot.x = CurrentTargetPos.x;
        PlayerRoot.y = transform.position.y;
        PlayerRoot.z = CurrentTargetPos.z;

        TargetDisplacement = PlayerRoot - transform.position;
    }


    // follows the player across forward plane
    void ConstrainDistance()
    {
        // follow player via their movement direction if rotation hasn't been manipulated this frame
        if (!rotate)
        {
            TargetPos = Player.transform.position - (PrevDispDir * currentFollowDistance);
            TargetPos.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, smoothFollow * Time.deltaTime);
        }
        // manual rotation logic messes with movement following, so simply close gap via displacement dir
        else if (!Mathf.Approximately(BaseDisplacement.magnitude, currentFollowDistance))
        {
            TargetPos = transform.position + (BaseDisplacement.normalized * (BaseDisplacement.magnitude - currentFollowDistance));
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, smoothFollow * Time.deltaTime);
        }
    }


    // automatic rotation updating to keep player on screen at all times
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
            //if (machine.ground)
            {
                transform.forward = Quaternion.AngleAxis(angle * dir, Vector3.up) * transform.forward;
            }
        }

        // x-axis rotation
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + (currentAngle + angleOffset - transform.eulerAngles.x) * Time.deltaTime * rotateSpeed, transform.eulerAngles.y, transform.eulerAngles.z);
    }


    // right stick-driven up/down look (gradually reverted by player movement if not held)
    void ManualLook()
    {
        // get root position (pre look interpolation)
        Vector3 Root = Player.transform.position;
        Root -= (BaseDisplacement.normalized * currentFollowDistance);
        Root.y = lastGround + currentHeight;

        // manual look rotation around x-axis
        if (Mathf.Abs(input.Current.Joy2Input.z) > 0.45f && machine.ground)
        {
            // rotation
            xRotationOffset = Clamp(-15f, 25f, xRotationOffset + input.Current.Joy2Input.z * Time.deltaTime * rotateSpeed * 10f);
        }
        else
        { // move toward default if moving and no manipulation input
            xRotationOffset += (0f - xRotationOffset) * Time.deltaTime * machine.moveDirection.magnitude * 0.25f;
        }
    }


    // avoid wall and floor obstructions
    void CheckOcclusion()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 Disp;
        Vector3 Root;

        // get root position (pre look interpolation)
        Root = Player.transform.position;
        Root -= (BaseDisplacement.normalized * currentFollowDistance);
        Root.y = lastGround + currentHeight;

        // Floor Occlusion
        if (!machine.ground || machine.jumping)
        {
            // smoothly reduce floorArc toward 0 in air states
            floorArc -= floorArc * Time.deltaTime * 0.15f;

            Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
            Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
            transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;
        }
        else
        {
            // rotate
            Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
            Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;

            transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

            float prevFloorArc = floorArc;

            for (int i = 0; i < 24; i++)
            {
                if (CheckCollision(transform.position, Player.transform.position + vTargetOffset * 0.2f, out hit))
                {
                    if (hit.transform.gameObject.tag == "Floor")
                    {
                        collision = true;
                        xRotationOffset = 0f;
                        floorArc = Clamp(0f, 60f, floorArc + Time.deltaTime * 2f);

                        Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
                        Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
                        transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // if no collision, try to rotate back
            if (!collision)
            {
                for (int i = 0; i < 24; i++)
                {
                    prevFloorArc = floorArc;
                    floorArc = Clamp(0f, 60f, floorArc - Time.deltaTime * 2f);

                    Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
                    Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
                    transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

                    if (CheckCollision(transform.position, Player.transform.position + vTargetOffset * 0.2f, out hit))
                    {
                        if (hit.transform.gameObject.tag == "Floor")
                        {
                            floorArc = prevFloorArc; // stops alternating jitter

                            Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
                            Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
                            transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

                            break;
                        }
                    }
                }
            }
        }

        // Wall Occlusion   
        if (CheckCollision(Player.transform.position + vTargetOffset * 0.2f, transform.position, out hit))
        {
            if (hit.transform.gameObject.tag == "Wall")
            {
                collision = true;
                transform.position = new Vector3(hit.point.x, Mathf.Min(Target.y + currentHeight, hit.point.y), hit.point.z);

                //currentHeight = 
            }
        }
    }


    // resets camera to behind player instantly if it's stuck directly above them
    void CheckLock()
    {
        if (Mathf.Abs(transform.position.x - Player.transform.position.x) < 0.1f && Mathf.Abs(transform.position.z - Player.transform.position.z) < 0.1f)
        {
            transform.position = Player.transform.position;
            transform.position -= Player.transform.forward * currentFollowDistance;
            transform.position += Vector3.up * currentHeight;
        }
    }


    // checks for first collision point along ray between start/end TODO: raycast ignore layer to get wall hits thru obstacles
    bool CheckCollision(Vector3 Start, Vector3 End, out RaycastHit hit)
    {
        if (Physics.Raycast(Start, (End - Start).normalized, out hit, (End - Start).magnitude))
        {
            return true;
        }

        return false;
    }

    // resets camera to behind hunter immediately
    public void Reset()
    {
        transform.position = Player.transform.position;
        transform.position -= (BaseDisplacement.normalized * currentFollowDistance);
        transform.position = new Vector3(transform.position.x, lastGround + currentHeight, transform.position.z);

        xRotationOffset = 0;
        UpdateActiveVariables();
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