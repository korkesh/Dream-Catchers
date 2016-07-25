using UnityEngine;
using System.Collections;

public class StageThree : Stages {

    public bool RightHand;
    int beginHealth;
    public int healthDifference;

	// Use this for initialization
	void Start () {
	
	}

    public override void Play()
    {
        if (Bc.Health <= (beginHealth - healthDifference))
        {
            Bc.currentStage = NextStage;
        }

        if(Bc.RightHand.attack == HandScript.Mode.NONE && Bc.LeftHand.attack == HandScript.Mode.NONE)
        {
            float rand = Random.Range(0, 30.0f);
            if(rand < 10.0f)
            {
                if(RightHand == false)
                {
                    Bc.RightHand.ThrowBall();
                }
                else
                {
                    Bc.LeftHand.ThrowBall();
                }

            }else if(rand < 20)
            {
                if (RightHand == false)
                {
                    Bc.RightHand.Swipe();
                }
                else
                {
                    Bc.LeftHand.Swipe();
                }
            }
            else
            {
                if (RightHand == false)
                {
                    Bc.RightHand.ChargeSmackDown();
                }
                else
                {
                    Bc.RightHand.ChargeSmackDown();
                }
            }

            RightHand = !RightHand;
        }

       
    }
}
