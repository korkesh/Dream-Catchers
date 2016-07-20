using UnityEngine;
using System.Collections;

public class BossClown : MonoBehaviour {


    public HandScript RightHand;
    public HandScript LeftHand;

    public int Health;

    public Stages currentStage;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

        currentStage.Play();
	}




    void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ball" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
       {
           TakeDamage();
       }
    }
   

    public void TakeDamage()
    {
        Health -= 1;
        if(Health <= 0)
        {
            Destroy(this.transform.parent.gameObject);
        }
    }
}
