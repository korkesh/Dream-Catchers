using UnityEngine;
using System.Collections;

public class Stage4 : Stages
{
    public BallSpawner spawner;
    public float timeBetween;
    float time;

	// Use this for initialization
	void Start () {
        time = timeBetween;
	}

    public override void Play()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            spawner.Spawn();
            time = timeBetween;
        }
    }

}
