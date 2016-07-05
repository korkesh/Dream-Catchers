///=====================================================================================
/// Author: Connor
/// Purpose: This is the "invisible" camera that keeps track of the base forward vector
///          The "real" camera uses this as a reference for look ahead rotations so it 
///          does not affect calculations
///======================================================================================

using UnityEngine;
using System.Collections;


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
    public float MaxHeightDelta; // maximum difference between cam and player y before currentground gets overridden

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
    //private Transform lookUpTarget; Un-Used Variable
    //private Transform lookDownTarget; Un-Used Variable

    //private float CamY = 0; // amount of y manipulation camera has applied to it - Currentyl Un-Used

    // Target:
    public SteeringBehaviourComponent steering;

    public Vector3 playerCamCross;

    public float targetHeightHigh = 0.8f; // above character (standard)
    public float targetHeightLow = 0f; // below character (for falling)
    public float targetHeight; // current target height

    public Vector3 targetPos; // actual target coords in world space    

    // private Vector3 lookOffset; // the vector that moves the target to "look ahead" of the player - Currently Un-Used
    public float lookDistance = 2.5f; // controls how far along cam right the target will move when the player is facing that direction

    private SuperCharacterController controller;

    private float yOffset; // offset from default pos by player input

    public float lastGround;// { get; private set; } // y value of last ground player was on
    public float setLastGround { set { lastGround = value; } }

    // EDITOR DEBUG:
    public float crossLength;

	// Use this for initialization
	void Start ()
    {
        currentMode = CameraMode.Field;

        Player = GameObject.FindGameObjectWithTag("Player"); //Character_Manager.Instance.Character;
        PlayerTarget = Player.GetComponent<HunterChildren>().camTarget;
        LookUp = Player.GetComponent<HunterChildren>().lookUp;
        LookDown = Player.GetComponent<HunterChildren>().lookDown;

        input = Player.GetComponent<PlayerInputController>();
        machine = Player.GetComponent<PlayerMachine>();
        controller = Player.GetComponent<SuperCharacterController>();

        steering = gameObject.GetComponent<SteeringBehaviourComponent>();
        target = PlayerTarget.transform;
        //lookUpTarget = LookUp.transform;
        //lookDownTarget = LookDown.transform;

        lastGround = machine.transform.position.y;

        targetHeight = targetHeightHigh;
        targetPos = Player.transform.position;
        targetPos.y += targetHeight;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        prevPos = transform.position;
        //lastGround = controller.lastGroundPosition.y;

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

        CheckCollision();

        lastGround = controller.currentGround.groundHeight;

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
        //Vector3 planarCamForward = Math3d.ProjectVectorOnPlane(controller.up, transform.forward).normalized; Un-Used Variable

        playerCamCross = Vector3.Cross(transform.right, planarPlayerForward);

        // closer to 0 means close to looking straight left/right, close to 1 means looking almost straight ahead
        //lookOffset = transform.right * (1 - playerCamCross.magnitude) * Mathf.Sign(Vector3.Cross(planarCamForward, planarPlayerForward).y) * lookDistance;


        // set target position in world coords
        targetPos = new Vector3();//Player.transform.position;
        targetPos.y += targetHeight;
        // move ahead a little bit
        //targetPos += Math3d.ProjectVectorOnPlane(controller.up, transform.forward).normalized;

        // TEMPORARY MOVEMENT
        if (PlayerTarget.transform.position != targetPos)
        {
            Vector3 lerp = Vector3.Lerp(PlayerTarget.transform.localPosition, targetPos, Vector3.Distance(PlayerTarget.transform.position, targetPos));

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

        else if ((PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Run && machine.runTimer > 0.2f)
        {
            Vector3 playerCamCross = Vector3.Cross(Math3d.ProjectVectorOnPlane(Vector3.up, Player.transform.forward), Math3d.ProjectVectorOnPlane(Vector3.up, transform.forward));

            if (playerCamCross.magnitude > 0.02f)
            {
                float turnDirection = Mathf.Sign(playerCamCross.y) * -1;
                transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * 0.25f * turnDirection);
            }
        }
        else if ((PlayerMachine.PlayerStates)machine.currentState == PlayerMachine.PlayerStates.Walk)
        {
            Vector3 playerCamCross = Vector3.Cross(Math3d.ProjectVectorOnPlane(Vector3.up, Player.transform.forward), Math3d.ProjectVectorOnPlane(Vector3.up, transform.forward));

            if (playerCamCross.magnitude > 0.02f)
            {
                float turnDirection = Mathf.Sign(playerCamCross.y) * -1;
                transform.RotateAround(target.position, controller.up, Time.deltaTime * rotateSpeed * 0.08f * turnDirection);
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
        if (Mathf.Abs(transform.position.y - Player.transform.position.y) > 20)
        {
            // maximum vertical distance exceeded
            lastGround = Player.transform.position.y + Height;
        }

        if (!Mathf.Approximately(transform.position.y, lastGround + Height))
        {
            float verticalMovement = (lastGround + Height) - transform.position.y;
            transform.position += new Vector3(0, verticalMovement * Time.deltaTime * 2, 0);
            //steering.Target = new Vector3(transform.position.x, lastGround + Height, transform.position.z);
        }


 
    }

    public void CheckCollision()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(PlayerTarget.transform.position, (transform.position - PlayerTarget.transform.position).normalized, out hit, (transform.position - PlayerTarget.transform.position).magnitude))
        {
            transform.position = hit.point;
        }
    }
}
