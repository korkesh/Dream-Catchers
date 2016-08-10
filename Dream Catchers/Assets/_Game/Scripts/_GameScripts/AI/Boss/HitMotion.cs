using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HitMotion : MonoBehaviour {

    public GameObject Head;
    public float up;
    public float down;

	// Use this for initialization
	void Start () {

        UP();
        
	}
	
	// Update is called once per frame
	void Update () {

        //if (DOTween.IsTweening(Head.transform) == false)
        //{
        //    Idle();
        //}

	}

    //public void Idle()
    //{
    //    Sequence idle = DOTween.Sequence();
    //    idle.Append(Head.transform.DOShakePosition(10,new Vector3(0,0.5f,0),1,5,false));
    //}


    public void UP()
    {
        Sequence goingUp = DOTween.Sequence();
        goingUp.Append(Head.transform.DOMoveY(up, 5, false)).OnComplete(() =>
        {
            DOWN();
        }); 
    }

    public void DOWN()
    {
        Sequence goingUp = DOTween.Sequence();
        goingUp.Append(Head.transform.DOMoveY(down, 5, false)).OnComplete(() =>
        {
            UP();
        });
    }

    public void Hit()
    {
        Sequence hit = DOTween.Sequence();
        hit.Append(Head.transform.DOShakeRotation(1.5f, 10, 10, 10));
    }



    void OnCollisionEnter(Collision collision)
    {
        if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.NIGHTMARE && collision.gameObject.tag == "Ball")
        {
            Hit();
        }
    }
}
