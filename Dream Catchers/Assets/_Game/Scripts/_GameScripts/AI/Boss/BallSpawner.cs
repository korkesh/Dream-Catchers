﻿using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour {

    public GameObject TargetLaunch;
    public GameObject TargetHitBack;
    public GameObject Ballprefab;
    public float LaunchAngle;
    public bool FindObjectTOLuanchAt;
    public string launchTargetTag;
    public GameObject currentAttack;
    public Type type;
    public float LocalScale;
    public string HammerTargetMessage;
    public string NightmareTargetMessage;
    public bool SpawnIfExits;

    public enum Type
    {
        Always,
        Timer,
        waitForCall
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(type == Type.Always)
        {
            if(currentAttack == null)
            {
                Spawn();
            }

        }
	
	}

    //spawn ball
    public void Spawn()
    {
        gameObject.SendMessage("Play", SendMessageOptions.DontRequireReceiver);
        if (currentAttack != null && SpawnIfExits == false)
        {
            return;
        }else if(currentAttack != null)
        {
            ClownAttack CA = currentAttack.GetComponent<ClownAttack>();
            if(CA != null)
            {
                CA.detroyBall();
            }
        }
            
        GameObject rocket = (GameObject)Instantiate(Ballprefab, this.transform.position, this.transform.rotation);
        currentAttack = rocket;
        rocket.transform.localScale = rocket.transform.localScale * LocalScale;
        ClownAttack cA = rocket.GetComponent<ClownAttack>();
        cA.clown = TargetHitBack;
        cA.LaunchAngle = LaunchAngle;
        cA.Hammermessagetosend = HammerTargetMessage;
        cA.Nightmaremessagetosend = NightmareTargetMessage;
        if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
        {
            cA.NightmareBall.SetActive(true);
            cA.spCollider = null;
        }
        Rigidbody rocketClone = rocket.GetComponent<Rigidbody>();
        if(FindObjectTOLuanchAt == true)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(launchTargetTag);
            rocketClone.velocity = Jump(obj.transform.position, LaunchAngle, this.transform);
        }
        else
        {
            if(TargetLaunch != null)
            {
                 rocketClone.velocity = Jump(TargetLaunch.transform.position, LaunchAngle, this.transform);
            }
           
        }
        
    }

    //gotten from online http://answers.unity3d.com/questions/148399/shooting-a-cannonball.html
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
