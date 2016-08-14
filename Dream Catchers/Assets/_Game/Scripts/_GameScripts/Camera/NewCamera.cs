using UnityEngine;
using System.Collections;


// Author: Conor MacKeigan
// Description: Camera behaviour catered to a 3D Platformer.
//              Focuses on keeping the view ahead of the player clear to minimize manual input and avoid blind jumps on whims.
//              Emphasizes landing vantage and a sense of vertical spacial awareness by rising quickly and lowering slowly, rotating to keep the player in view.
//              Behaviour is consistent and the camera will always correct itself out if it ever gets confused or stuck in an odd position (which shouldn't happen naturally).
//              Wall collisions are handled by quickly moving to the point of intersection from the player to the camera, which are very common due to boxy levels + 360 control.
//              Obstructions such as floors between the player and camera are not rendered.

[RequireComponent(typeof(DefaultMode))]
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

    // Jump state variables
    private Vector3 JumpOrigin; // position at which player left the ground
    private Vector3 JumpDisplacement; // displacement vector on frame player left ground

    //=========================================
    // Active Variables
    //=========================================
    private Vector3 Root;

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

    private bool collision = false; // set to true each frame if any collision occurred (prevents target updating/jitter)
    Vector3 CollisionTarget;
    private float collisionDistance = 0; // amount of distance required to cover to reach collision point (normalized 0..1)

    private MeshRenderer CurrentObstruction = null; // store current wall we are hitting to turn off / on its mesh

    //==========================================
    // Smoothing Coefficients
    //==========================================
    public float smoothFollow;
    public float smoothVertical;
    public float rotateSpeed;
    public float lastGroundSpeed; // speed at which camera moves up/down when lastGround is changed


    void Start()
    {
        UpdateMode(GetComponent<DefaultMode>().mode);

        transform.eulerAngles = Vector3.zero;

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
        transform.eulerAngles = new Vector3(currentAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void Awake()
    {
        UpdateMode(GetComponent<DefaultMode>().mode); // camera starts in high/low mode depending on level
    }

    void LateUpdate()
    {
        CheckLock(); // checks if the camera is "locked" (directly above player)
        UpdateMode(); // updates between high/low mode

        ManualRotation(); // right stick-driven arc around player

        UpdateActiveVariables(); // updates variables such as follow distance, height and angle based on mode and other interactions

        // do not apply follow distance during collision unless planar forward distance threshold is exceeded
        if (!collision || Math3d.ProjectVectorOnPlane(controller.up, (Player.transform.position + vTargetOffset) - transform.position).magnitude > currentFollowDistance)
        {  
            ConstrainDistance(); // planar forward player follow
        }

        UpdateHeight(); // updates height variables based on player position
        UpdateTarget(); // updates the view target, telling the camera what to focus on and facilitating look ahead features

        UpdateVectors(); // update displacement vectors for calculations
        UpdateRotation(); // update automatic camera rotation to keep Target in center of X-axis and modifies X-axis rotation for fall tracking

        ManualLook(); // right-stick driven up/down rotation

        UpdateRoot(); // updates root position

        // Occlusion avoidance for floors and walls
        collision = false;
        CheckOcclusion();

        CollisionMovement();

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
                    }
                    else
                    {
                        currentHeight = hHeightGround;//Air;
                        currentAngle = hAngleGround + floorArc;//Air + floorArc;
                        xRotationOffset = 0f;
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
                    }
                    else
                    {
                        currentHeight = lHeightGround;//Air;
                        currentAngle = lAngleGround + floorArc;//Air;
                        xRotationOffset = 0f;
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
    
    // Automatic mode set
    public void UpdateMode(string mode)
    {
        if (mode == "High")
        {
            Mode = CameraMode.High;
        }
        else if (mode == "Low")
        {
            Mode = CameraMode.Low;
        }
        else
        {
            Mode = CameraMode.Low;
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
            //currentAngle += Clamp(0f, 60f, (lastGround - Target.y) * 4f);
            currentAngle += Clamp(0f, 60f, Mathf.Max((lastGround - newGround) * 4f, (lastGround - Target.y) * 4f));
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
        transform.eulerAngles = new Vector3(transform.eulerAngles.x + (currentAngle - transform.eulerAngles.x) * Time.deltaTime * rotateSpeed, transform.eulerAngles.y, transform.eulerAngles.z);
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


    void UpdateRoot()
    {
        // get root position (pre look interpolation)
        Root = Player.transform.position;
        Root -= (BaseDisplacement.normalized * currentFollowDistance);
        Root.y = lastGround + currentHeight;

        // update pos to root
        Vector3 Disp = Root - (Player.transform.position + vTargetOffset);
        transform.position = (Player.transform.position + vTargetOffset) + Disp;
    }


    // avoid wall obstructions
    void CheckOcclusion()
    {
        RaycastHit hit = new RaycastHit();

        // Wall Occlusion   
        if (CheckCollision(Player.transform.position + vTargetOffset, transform.position, out hit))
        {
            if (hit.transform.gameObject.tag == "Wall")
            {
                collision = true;
                CollisionTarget = hit.point;

                if (CollisionTarget.y - (Player.transform.position.y + vTargetOffset.y) > currentHeight)
                { // prevents camera height from getting stuck on the wall while the player moves down on an elevator
                    CollisionTarget.y = (Player.transform.position.y + vTargetOffset.y) + currentHeight;
                }

                // Turn mesh renderer on/off
                if (CurrentObstruction == null)
                {
                    CurrentObstruction = hit.transform.gameObject.GetComponent<MeshRenderer>();

                    if (CurrentObstruction == null)
                    {
                        // some mesh renderers are parents of their colliders so iterate through all parents
                        Transform t = hit.transform;
                        while (t.parent != null)
                        {
                            t = t.parent;
                            CurrentObstruction = t.GetComponent<MeshRenderer>();

                            if (CurrentObstruction != null)
                            {
                                break;
                            }
                        }
                    }
                }
                if (CurrentObstruction != null)
                {
                    CurrentObstruction.enabled = false;
                }
            }
            else if (hit.transform.gameObject.tag == "Floor" || hit.transform.gameObject.tag == "Platform")
            {
                // Turn mesh renderer on/off
                if (CurrentObstruction == null)
                {
                    CurrentObstruction = hit.transform.gameObject.GetComponent<MeshRenderer>();

                    if (CurrentObstruction == null)
                    {
                        // some mesh renderers are parents of their colliders so iterate through all parents
                        Transform t = hit.transform;
                        while (t.parent != null)
                        {
                            t = t.parent;
                            CurrentObstruction = t.GetComponent<MeshRenderer>();

                            if (CurrentObstruction != null)
                            {
                                break;
                            }
                        }
                    }
                }
                if (CurrentObstruction != null)
                {
                    CurrentObstruction.enabled = false;
                }
            }
            else if (CurrentObstruction != null)
            {
                CurrentObstruction.enabled = true;
                CurrentObstruction = null;
            }
        }
        else if (CurrentObstruction != null)
        {
            CurrentObstruction.enabled = true;
            CurrentObstruction = null;
        }

        //Vector3 Disp;
        //Vector3 Root;

        //// get root position (pre look interpolation)
        //Root = Player.transform.position;
        //Root -= (BaseDisplacement.normalized * currentFollowDistance);
        //Root.y = lastGround + currentHeight;

        //// Floor Occlusion
        //if (!machine.ground || machine.jumping)
        //{
        //    // smoothly reduce floorArc toward 0 in air states
        //    floorArc -= floorArc * Time.deltaTime * 0.15f;

        //    Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
        //    Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
        //    transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;
        //}
        //else
        //{
        //    // rotate
        //    Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
        //    Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;

        //    transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

        //    float prevFloorArc = floorArc;

        //    for (int i = 0; i < 24; i++)
        //    {
        //        if (CheckCollision(transform.position, Player.transform.position + vTargetOffset * 0.2f, out hit))
        //        {
        //            if (hit.transform.gameObject.tag == "Floor")
        //            {
        //                collision = true;
        //                xRotationOffset = 0f;
        //                floorArc = Clamp(0f, 60f, floorArc + Time.deltaTime * 2f);

        //                Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
        //                Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
        //                transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    // if no collision, try to rotate back
        //    if (!collision)
        //    {
        //        for (int i = 0; i < 24; i++)
        //        {
        //            prevFloorArc = floorArc;
        //            floorArc = Clamp(0f, 60f, floorArc - Time.deltaTime * 2f);

        //            Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
        //            Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
        //            //transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

        //            if (CheckCollision(transform.position, Player.transform.position + vTargetOffset * 0.2f, out hit))
        //            {
        //                if (hit.transform.gameObject.tag == "Floor")
        //                {
        //                    floorArc = prevFloorArc; // stops alternating jitter

        //                    Disp = Root - (Player.transform.position + vTargetOffset * 0.2f);
        //                    Disp = Quaternion.AngleAxis(floorArc, Vector3.Cross(Vector3.up, BaseDisplacement.normalized)) * Disp;
        //                    transform.position = (Player.transform.position + vTargetOffset * 0.2f) + Disp;

        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
    }


    void CollisionMovement()
    {
        if (collision)
        {
            if (collisionDistance < 0.5f)
            {
                collisionDistance = Clamp(0f, 1f, collisionDistance + Time.deltaTime * 4f);
            }
            else
            {
                collisionDistance = Clamp(0f, 1f, collisionDistance + ((1f - collisionDistance) * (Time.deltaTime * 16f)));
            }

            transform.position += (CollisionTarget - transform.position) * collisionDistance;
        }
        else
        {
            if (collisionDistance > 0.5f)
            {
                collisionDistance = Clamp(0f, 1f, collisionDistance - Time.deltaTime * 4f);
            }
            else
            {
                collisionDistance = Clamp(0f, 1f, collisionDistance - (collisionDistance * Time.deltaTime * 16f));
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