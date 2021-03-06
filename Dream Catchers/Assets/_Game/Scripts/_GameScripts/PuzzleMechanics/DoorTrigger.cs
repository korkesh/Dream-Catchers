﻿///=====================================================================================
/// Author: Matt
/// Purpose: Lower a door upon a certain criteria being met; i.e. certain enemies killed
///======================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorTrigger : MonoBehaviour {

    public enum TRIGGER_TYPE
    {
        ENEMY = 0
    }

    public TRIGGER_TYPE triggerType = TRIGGER_TYPE.ENEMY;

    public List<GameObject> enemiesToKill;

    public LowerDoor lowerDoorRef;

    bool opened = false;
	
	// Update is called once per frame
	void Update () {
	
        if(opened)
        {
            return;
        }

        foreach(GameObject go in enemiesToKill)
        {
            if(go != null)
            {
                return;
            }
        }

        lowerDoorRef.ActivateDoor();
        opened = true;
	}
}
