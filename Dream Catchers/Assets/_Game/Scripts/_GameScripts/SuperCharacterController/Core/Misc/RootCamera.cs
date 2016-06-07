using UnityEngine;
using System.Collections;


// This is the "invisible" camera that keeps track of the base forward vector
// The "real" camera uses this as a reference for look ahead rotations so it does not affect calculations
public class RootCamera : MonoBehaviour
{
    public enum CameraMode
    {
        Field,
        Close,
        Look
    }

    public CameraMode currentMode;

    public GameObject Player; // player GO reference

    public float MaxDistance = 11.0f;
    public float MinDistance = 9.0f;
    public float Height = 4.5f; // height offset from player

    public float moveSpeed = 10.0f;
    public float rotateSpeed = 100;

    public GameObject PlayerTarget;
    public GameObject LookUp;
    public GameObject LookDown;

    public float idleTimer; // counts how long in seconds the camera transform has been idling
    private Vector3 prevPos;

    private PlayerInputController input;
    private PlayerMachine machine;

    private Transform target;
    private Transform lookUpTarget;
    private Transform lookDownTarget;

    private float CamY = 0; // amount of y manipulation camera has applied to it

    // Target:
    public SteeringBehaviourComponent steering;

    public Vector3 playerCamCross;

    public float targetHeightHigh = 1.5f; // above character (standard)
    public float targetHeightLow = -0.5f; // below character (for falling)
    public float targetHeight; // current target height

    public Vector3 targetPos; // actual target coords in world space    

    private Vector3 lookOffset; // the vector that moves the target to "look ahead" of the player
    public float lookDistance = 2.5f; // controls how far along cam right the target will move when the player is facing that direction

    private SuperCharacterController controller;

    public bool aligning = false; // controls timer for camera align
    private float alignTimer;
    public float alignTime = 1.5f;

    private float yOffset; // offset from default pos by player input

    public float lastGround;// { get; private set; } // y value of last ground player was on
    public float setLastGround { set { lastGround = value; } }

    // EDITOR DEBUG:
    public float crossLength;

	// Use this for initialization
	void Start ()
    {
        currentMode = CameraMode.Field;

        input = Player.GetComponent<PlayerInputController>();
        machine = Player.GetComponent<PlayerMachine>();
        controller = Player.GetComponent<SuperCharacterController>();

        steering = gameObject.GetComponent<SteeringBehaviourComponent>();
        target = PlayerTarget.transform;
        lookUpTarget = LookUp.transform;
        lookDownTarget = LookDown.transform;

        lastGround = machine.transform.position.y;

        targetHeight = targetHeightHigh;
        targetPos = Player.transform.position;
        targetPos.y += targetHeight;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        prevPos = transform.position;
        lastGround = controller.lastGroundPosition.y;

        UpdateTarget();

        FollowPlayer();
        
        if (transform.position == prevPos)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0;
        }

        //Vector3 targetPos = target.position;
        //targetPos.y = lastGround;

        //transform.position = targetPos; // target.position;

        //Vector3 left = Vector3.Cross(machine.lookDirection, controller.up);

        ////horizontal rotation
        ////transform.rotation = Quaternion.LookRotation(machine.lookDirection, controller.up);

        ////todo: y rotation transform.rotation = Quaternion.AngleAxis(yRotation, left) * transform.rotation;

        //transform.position -= transform.forward * Distance;
        //transform.position += controller.up * Height;
    }


    // moves the player child that the camera follows
    public void UpdateTarget()
    {
        // process target height
        if ((PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Idle ||
            (PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Walk ||
            (PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Run)
        {
            targetHeight = targetHeightHigh;
        }
        else
        {
            if (machine.moveDirection.y < -0.35f)
            {
                targetHeight = targetHeightLow;
            }
            else
            {
                targetHeight = (targetHeightHigh + targetHeightLow) * 0.5f;
            }
        }

        // process cam right offset for look ahead
        // move target "in front" of player to give forward vantage
        Vector3 planarPlayerForward = Math3d.ProjectVectorOnPlane(controller.up, controller.transform.forward).normalized;
        Vector3 planarCamForward = Math3d.ProjectVectorOnPlane(controller.up, transform.forward).normalized;

        playerCamCross = Vector3.Cross(transform.right, planarPlayerForward);

        // closer to 0 means close to looking straight left/right, close to 1 means looking almost straight ahead
        lookOffset = transform.right * (1 - playerCamCross.magnitude) * Mathf.Sign(Vector3.Cross(planarCamForward, planarPlayerForward).y) * lookDistance;


        // set target position in world coords
        targetPos = Player.transform.position;
        targetPos.y += targetHeight;

        // TEMPORARY MOVEMENT
        if (PlayerTarget.transform.position != targetPos)
        {
            Vector3 lerp = Vector3.Lerp(PlayerTarget.transform.localPosition, targetPos, 0.2f);

            if (!Mathf.Approximately(lerp.x, targetPos.x) || !Mathf.Approximately(lerp.y, targetPos.y) || !Mathf.Approximately(lerp.z, targetPos.z))
            {
                PlayerTarget.transform.localPosition = lerp;
            }
            else
            {
                PlayerTarget.transform.localPosition = targetPos;
            }
        }


        
    }


    public void FollowPlayer()
    {
        // min/max distance
        if (currentMode == CameraMode.Field)
        {
            Vector3 displacement = Math3d.ProjectVectorOnPlane(controller.up, PlayerTarget.transform.position - transform.position);

            if (displacement.magnitude > MaxDistance)
            {
                transform.position += (displacement.magnitude - MaxDistance) * displacement.normalized;
            }
            else if (displacement.magnitude < MinDistance)
            {
                transform.position -= (MinDistance - displacement.magnitude) * displacement.normalized;
            }
        }


        // rotation
        Vector3 angle = (PlayerTarget.transform.position - transform.position).normalized;

        Vector3 cross = Vector3.Cross(transform.forward, angle);

        crossLength = cross.magnitude; // DEBUG

        if (cross.magnitude > 0.04f)
        {
            transform.forward += Vector3.Slerp(transform.forward, PlayerTarget.transform.position - transform.position, 0.025f) * Time.deltaTime * rotateSpeed;
        }


        // ROTATE AROUND PLAYER AS PIVOT POINT:
        // temp rotation test
        if (input.Current.Joy2Input.x != 0)
        {
            transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * input.Current.Joy2Input.x);
        }

        else if ((PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Idle)
        {
            Vector3 playerCamCross = Vector3.Cross(Math3d.ProjectVectorOnPlane(Vector3.up, Player.transform.forward), Math3d.ProjectVectorOnPlane(Vector3.up, transform.forward));

            if (machine.idleTimer > 1)
            {
                if (playerCamCross.magnitude > 0.02f)
                {
                    float turnDirection = Mathf.Sign(playerCamCross.y) * -1;
                    transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * 0.25f * turnDirection);
                }
                // facing straight toward camera is a special case
                else
                {
                    if (Vector3.Cross(transform.right, controller.transform.forward).y > 0.97)
                    {
                        transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * 0.25f);
                    }
                }
            }
        }
            
        


        // y rotation
        //if (input.Current.Joy2Input.z < 0) // look up
        //{
        //    currentMode = CameraMode.Look;

        //    targetPos.y = targetPosHigh.y;

        //    // move toward upward vantage point
        //    if ((transform.position - lookUpTarget.position).magnitude > 0.05f)
        //    {
        //        transform.position = Vector3.Lerp(transform.position, lookUpTarget.position, Mathf.Abs(input.Current.Joy2Input.z) * Time.deltaTime);
        //    }

        //}
        //// look down
        //else if (input.Current.Joy2Input.z > 0)
        //{
        //    currentMode = CameraMode.Look;

        //    targetPos.y = targetPosLow.y;
        //    targetPos.z = 4;

        //    if ((transform.position - lookDownTarget.position).magnitude > 0.05f)
        //    {
        //        transform.position = Vector3.Lerp(transform.position, lookDownTarget.position, input.Current.Joy2Input.z * Time.deltaTime);
        //    }
        //}
        //else
        //{ // TODO: store old field cam coords for seamless reversion
        //    targetPos.y = targetPosHigh.y;
        //    targetPos.z = 0;
        //    currentMode = CameraMode.Field;
        //}

        // up/down movement
        if (!Mathf.Approximately(transform.position.y, lastGround + Height))
        {
            steering.Target = new Vector3(transform.position.x, lastGround + Height, transform.position.z);
        }


        // align with player forward if holding straight up or idle(?)
        if (aligning)
        {
            alignTimer += Time.deltaTime;

            if (alignTimer >= alignTime)
            {

            }
        }
        else
        {
            alignTimer = 0;
        }
    }

    // if something is obstructing the character from camera view, rotate until visible todo: alpha
    public void CheckOcclusion()
    {
        //if (idleTimer < 0.6f)
        //{
        //    return; // must be stationary for at least one second to apply automated occlusion rotation
        //}

        //if (Physics.Raycast(transform.position, (PlayerTarget.transform.parent.position - transform.position).normalized, (PlayerTarget.transform.parent.position - transform.position).magnitude - 1))
        //{
        //    // find out which side to rotate
        //    Transform left = occlusionCheckL.transform;
        //    Transform right = occlusionCheckL.transform;

        //    left.position = transform.position;
        //    right.position = transform.position;

        //    bool r = true; // default dir is right

        //    float distance = 0; // counts how much distance much be travelled to avoid obstruction

        //    while (true)
        //    {
        //        distance += 0.2f;

        //        left.position = transform.position;
        //        right.position = transform.position;

        //        left.RotateAround(PlayerTarget.transform.position, controller.up, -distance);
        //        right.RotateAround(PlayerTarget.transform.position, controller.up, distance);

        //        if (!Physics.Raycast(left.position, (PlayerTarget.transform.parent.position - transform.position).normalized, (PlayerTarget.transform.parent.position - transform.position).magnitude - 1))
        //        {
        //            r = false;
        //            break;
        //        }
        //        else if (!Physics.Raycast(right.position, (PlayerTarget.transform.parent.position - transform.position).normalized, (PlayerTarget.transform.parent.position - transform.position).magnitude - 1))
        //        {
        //            r = true;
        //            break; // r is true by default
        //        }

        //        if (distance > 20)
        //        {
        //            Debug.Log("escape");
        //            break; // safety net escape (large geometry not dealt with yet anyway)
        //        }
        //    }

        //    // rotate camera away from obstruction
        //    if (!r)
        //    {
        //        distance *= -1;
        //    }

            //transform.RotateAround(PlayerTarget.transform.position, controller.up, Time.deltaTime * distance * 0.3f);
        //}
    }
}
