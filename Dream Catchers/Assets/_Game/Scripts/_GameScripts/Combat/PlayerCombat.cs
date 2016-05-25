using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{

    public bool attacking;

	// Use this for initialization
	void Start () {
        attacking = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BeginAttack()
    {
        Debug.Log("attack");

        gameObject.GetComponent<Animator>().SetBool("StandAttack", true);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 1);
        attacking = true;
    }

    public void EndAttack()
    {
        Debug.Log("attack end");

        attacking = false;
        gameObject.GetComponent<Animator>().SetBool("StandAttack", false);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 0);

    }
}
