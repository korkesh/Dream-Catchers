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

	}

    //moving boss head up and down
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
