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
        time -= Time.deltaTime;
        if(beginHealth != Bc.Health)
        {
            Bc.currentStage = NextStage;
        }

        if(time <= 0)
        {
            time = timeBetweenThrows;
            if(RightHand == false)
            {
                Bc.RightHand.spawner.Spawn();
                RightHand = true;
            }else
            {
                Bc.LeftHand.spawner.Spawn();
                RightHand = false;
            }
        }
    }
	
}
