using UnityEngine;
using System.Collections;

public class StageTwo : Stages {

    public int healthDifference;
    int beginHealth;
    public float waitOnSwipe;
    public float waitOnThrow;
    public bool RightHand;
    bool throwBall;
    //float timeSwipe;
    //float timeThrow;
    int AttackTime;

	// Use this for initialization
	void Start () {

        beginHealth = Bc.Health;
        //timeThrow = waitOnThrow;
        //timeSwipe = 0;
        AttackTime = 0;
        throwBall = false;
	}
	
	public override void Play()
    {
        //if hit once go to next stage
        if(Bc.Health <= (beginHealth-healthDifference))
        {
            Bc.currentStage = NextStage;
        }

        //if not throwing ball swipe
        if(Bc.RightHand.attack == HandScript.Mode.NONE && Bc.LeftHand.attack == HandScript.Mode.NONE)
        {
            if(throwBall == false)
            {
                if(AttackTime == 0)
                {
                    Bc.RightHand.Swipe();
                    AttackTime++;
                }
                else
                {
                    Bc.LeftHand.Swipe();
                    AttackTime = 0;
                    throwBall = true;
                }
            }
            else
            {
               if(Bc.hunterOnRight == true)
               {
                   Bc.RightHand.ThrowBall();
                   throwBall = false;
               }
               else
               {

                   Bc.LeftHand.ThrowBall();
                   throwBall = false;
               }
            }
        }
    }
}
