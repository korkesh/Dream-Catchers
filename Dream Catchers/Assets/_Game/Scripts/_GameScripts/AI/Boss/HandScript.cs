using UnityEngine;
using System.Collections;

public class HandScript : MonoBehaviour {

    public int Health;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}


    public void ThrowBall()
    {

    }

    public void Swipe()
    {

    }

    public void SmackDown()
    {

    }

    void OnDestroy()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
       
    }


    //cannonball launch code again from unity answers
    public Vector3 Jump(Vector3 target, float angle, Transform current)
    {
        Vector3 dir = target - current.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
        // calculate the velocity magnitude
        float sin = Mathf.Sin(2 * a);
        float div = dist * Physics.gravity.magnitude / sin;
        if (sin == 0 || div < 0)
        {
            return current.transform.forward * 2;
        }
        float vel = Mathf.Sqrt(div);
        return vel * dir.normalized;
    }
}
