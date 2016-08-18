using UnityEngine;
using System.Collections;

public class BossClown : MonoBehaviour {


    public HandScript RightHand;
    public HandScript LeftHand;

    public int Health;

    public Stages currentStage;

    public bool hunterOnRight;

    public bool inDanger;
    public GameObject inComingBall;


    public GameObject Particles;
    public PlaySound playsound;

	
	
	// Update is called once per frame
	void Update () {

        //plays current stage
        currentStage.Play();

        //checks if there is a ball coming
        if(inComingBall == null)
        {
            inDanger = false;
        }else
        {
            if(inComingBall.GetComponent<ClownAttack>().exploded == true)
            {
                inDanger = false;
            }
        }
	}

    //take damage if hit by bomb
    void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.tag == "Ball" && ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE)
       {
           TakeDamage();
       }
    }
   
    //take damage and play sound
    public void TakeDamage()
    {
        Health -= 1;
        playsound.PlayTheSound();
        if(Health <= 0)
        {
            cueParticles();
            Destroy(this.transform.parent.gameObject);
        }
    }



    public void Block(GameObject ball)
    {
        inDanger = true;
        inComingBall = ball;
    }

    //activates particles

    public void cueParticles()
    {
        if (Particles != null)
        {
            Particles.transform.parent = null;
            Particles.SetActive(true);
        }
    }

    //if dead change scene

    void OnDestroy()
    {
        GameObject lc = GameObject.Find("LevelComplete");
        if (lc != null && Health <= 0)
        {
           
            UI_Manager.instance.ShowMenu(lc.GetComponent<Menu>());
        }
    }

}
