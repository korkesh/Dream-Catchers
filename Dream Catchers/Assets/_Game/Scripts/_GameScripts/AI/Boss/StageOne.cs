using UnityEngine;
using System.Collections;

public class StageOne : Stages {

    public float timeBetweenThrows;
    public bool RightHand;
    int beginHealth;
    float time;

	// Use this for initialization
	void Start () {

        beginHealth = Bc.Health;
        time = 0;
	}

    public override void Play()
    {
        //next stage after one hit
        if(beginHealth != Bc.Health)
        {
            Bc.currentStage = NextStage;
            return;
        }

        //throw ball
        if (Bc.RightHand.attack == HandScript.Mode.NONE && Bc.LeftHand.attack == HandScript.Mode.NONE)
        {
            if(Bc.hunterOnRight == true)
            {
                Bc.RightHand.ThrowBall();
            }
            else
            {
                Bc.LeftHand.ThrowBall();
            }
        }
    }
	
}
