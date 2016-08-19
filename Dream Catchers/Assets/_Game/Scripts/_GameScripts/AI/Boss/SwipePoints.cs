using UnityEngine;
using System.Collections;

//not used anymore
public class SwipePoints : MonoBehaviour {

    public enum Direction
    {
        X,
        Y,
        Z
    }

    public Direction dir;
    public bool negative;
    public GameObject tracking;
    public float step;
    public float theY;
    public bool setY;

	// Use this for initialization
	void Start () {
	
        if(tracking == null)
        {
            tracking = GameObject.FindGameObjectWithTag("Player");
        }
        if(negative == false)
        {
            step = step * -1;
        }

	}
	
	// Update is called once per frame
	void Update () {
	
        //take out later

        moveToPosition();
	}

    public void moveToPosition()
    {
        float addition = 0;
        if (setY == true)
        {
            addition = theY - tracking.transform.position.y;
        } 
        Vector3 vec = new Vector3(0, addition, 0);

        switch (dir)
        {
            case Direction.X:
                this.transform.position = tracking.transform.position + step * Vector3.right + vec;
                break;
            case Direction.Y:
                this.transform.position = tracking.transform.position + step * Vector3.up + vec;
                break;
            case Direction.Z:
                this.transform.position = tracking.transform.position + step * Vector3.forward + vec;
                break;
        }
    }
}
