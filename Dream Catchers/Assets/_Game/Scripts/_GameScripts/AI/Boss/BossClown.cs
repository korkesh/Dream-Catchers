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

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

        currentStage.Play();
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
            cueParticles();
            Destroy(this.transform.parent.gameObject);
        }
    }

    public void Block(GameObject ball)
    {
        inDanger = true;
        inComingBall = ball;
    }

    public void cueParticles()
    {
        if (Particles != null)
        {
            Particles.transform.parent = null;
            Particles.SetActive(true);
        }
    }

    void OnDestroy()
    {
        GameObject lc = GameObject.Find("LevelComplete");
        if (lc != null && Health <= 0)
        {
           
            UI_Manager.instance.ShowMenu(lc.GetComponent<Menu>());
        }
    }

}
