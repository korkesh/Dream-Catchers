using UnityEngine;
using System.Collections;

public abstract class Stages : MonoBehaviour {

    public BossClown Bc;
    public Stages NextStage;

    public abstract void Play();
}
