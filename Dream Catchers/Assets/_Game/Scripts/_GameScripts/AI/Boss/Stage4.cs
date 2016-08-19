using UnityEngine;
using System.Collections;

public class Stage4 : Stages
{
    public BallSpawner spawnerR;
    public BallSpawner spawnerL;
    public float timeBetween;
    float time;

	// Use this for initialization
	void Start () {
        time = timeBetween;
	}

    //throw ball
    public override void Play()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            if (Bc.hunterOnRight == true)
            {
                spawnerR.Spawn();
            }
            else
            {
                spawnerL.Spawn();
            }
            time = timeBetween;
        }
    }

}
