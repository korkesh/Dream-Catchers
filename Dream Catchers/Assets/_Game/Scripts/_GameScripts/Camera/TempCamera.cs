///===============================================================================
/// Author: Matt
/// Purpose: A basic camera used as a temporary stand in until the proper camera
///          is implemented. This script simply maps the right analog stick
///          to the camera's position
///===============================================================================
using UnityEngine;
using System.Collections;

public class TempCamera : MonoBehaviour {

    public Transform target;
    public float distance = 5.0f;
    public float bufferup = 1.5f;
    public float bufferright = 0.75f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    private float x = 0.0f;
    private float y = 0.0f;

    void Awake()
    {
        AudioSource[] bgm = GetComponents<AudioSource>();

        Audio_Manager.Instance.DreamBGM = bgm[0];
        Audio_Manager.Instance.NightmareBGM = bgm[1];
    }

    // Use this for initialization 
    void Start () {

        //target = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 angles = transform.eulerAngles;
        x = angles.y; y = angles.x;
    }

    // Update is called once per frame 
    void LateUpdate () {

        if (Game_Manager.instance == null || !Game_Manager.instance.isPaused())
        {
            if (target)
            {
                x += Input.GetAxis("Horizontal2") * xSpeed * 0.02f;
                y -= (Input_Manager.instance.invertCamera) ? (-Input.GetAxis("Vertical2") * ySpeed * 0.02f) : (Input.GetAxis("Vertical2") * ySpeed * 0.02f);
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                Vector3 position = rotation * new Vector3(bufferright, 0.0f, -distance) + target.position + new Vector3(0.0f, bufferup, 0.0f);
                transform.rotation = rotation;
                transform.position = position;

                Vector3 cameraPos = transform.position;
                cameraCollision(target.position, ref cameraPos);

                transform.position = cameraPos;
                transform.LookAt(target);
            }
        }
    }

    void cameraCollision(Vector3 fromObject, ref Vector3 toTarget)
    {
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
        RaycastHit objectHit = new RaycastHit();
        if(Physics.Linecast(fromObject, toTarget, out objectHit))
        {
            Debug.DrawRay(objectHit.point, Vector3.left, Color.red);

            toTarget = new Vector3(objectHit.point.x, toTarget.y, objectHit.point.z);
        }
    }

    float ClampAngle(float angle, float min, float max) {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
