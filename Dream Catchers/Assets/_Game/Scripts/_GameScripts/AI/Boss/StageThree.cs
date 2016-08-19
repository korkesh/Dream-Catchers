using UnityEngine;
using System.Collections;

public class StageThree : Stages {

    public bool RightHand;
    int beginHealth;
    public int healthDifference;
    

    public int[] AttackPattern;
    public int index;
    int rotation;


	// Use this for initialization
	void Start () {

        rotation = 0;
        index--;
        if(index < 0)
        {
            index = 0;
        }
	}

    public override void Play()
    {
        //once both hands are gone next stage
        if (Bc.RightHand == null && Bc.LeftHand == null)
        {
            Bc.currentStage = NextStage;
        }
        else if(Bc.RightHand == null)
        {
            //if the right hand is gone, check if left needs to block , if not find an attack
            if(Danger(Bc.LeftHand,false) == false)
            {
                chooseAttack(Bc.LeftHand,false);
            }

        }else if(Bc.LeftHand == null)
        {
            //if the left hand is gone, check if left needs to block , if not find an attack
            if (Danger(Bc.RightHand,false) == false)
            {
                chooseAttack(Bc.RightHand, false);
            }

        }else
        {
            //check for the need to block 
            bool r = Danger(Bc.RightHand, true);
            bool l;
            if(r == false)
            {
                l = Danger(Bc.LeftHand, true);
            }else
            {
                l = false;
            }
            //if no need to block choose an attack
            if(r == false && l == false)
            {
                if (RightHand == true  && Bc.LeftHand.attack == HandScript.Mode.NONE)
                {
                    chooseAttack(Bc.RightHand, true);
                }
                else if(Bc.RightHand.attack == HandScript.Mode.NONE )
                {
                    chooseAttack(Bc.LeftHand, true);
                }
            }
            
        }


    }

    //choose an attack based on number
    public void chooseAttack(HandScript Hand, bool bothHands)
    {


        if(Hand.attack == HandScript.Mode.NONE)
        {
            if (bothHands == true)
            {
                RightHand = !RightHand;
            }

            if(rotation == 3)
            {
                int rand = Random.Range(0, 4);
                index = rand * 3;
                rotation = 0;

            }
            else
            {
                index++;
            }

            if(AttackPattern[index] == 0)
            {
                Hand.ThrowBall();
            }
            else if(AttackPattern[index] == 1)
            {
                Hand.Swipe();
            }
            else
            {
                Hand.ChargeSmackDown();
            }

            rotation++;
           
        }

    }

    //check if theres an incoming ball or if the hand should move back to resting position if blocking
    public bool Danger(HandScript Hand, bool bothHands)
    {
        if(Bc.inDanger == false && Hand.attack == HandScript.Mode.BLOCK)
        {
            Hand.BlockReturn();
            return true;

        }else if(Bc.inDanger == true && ((bothHands == false && (Hand.attack == HandScript.Mode.NONE || Hand.attack == HandScript.Mode.THROW)) || (bothHands == true && Hand.attack == HandScript.Mode.NONE)))
        {
            Hand.Block();
            return true;

        }else if(Hand.attack == HandScript.Mode.BLOCK || Hand.attack == HandScript.Mode.UNBLOCK)
        {
            return true;
        }

        return false;
    }
}
