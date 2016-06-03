using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Collider weaponCollider;
    public GameObject weaponParentObject;
    public bool attacking;

	// Use this for initialization
	void Start () {
        attacking = false;
        weaponCollider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BeginAttack()
    {
        Debug.Log("attack");

        weaponParentObject.SetActive(true);
        gameObject.GetComponent<Animator>().SetBool("StandAttack", true);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 1);
        attacking = true;
        weaponCollider.enabled = true;
    }

    public void EndAttack()
    {
        Debug.Log("attack end");

        weaponParentObject.SetActive(false);
        attacking = false;
        weaponCollider.enabled = false;
        gameObject.GetComponent<Animator>().SetBool("StandAttack", false);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 0);

    }

    public void BeginPound()
    {
        Debug.Log("pound");

        weaponParentObject.SetActive(true);
        //gameObject.GetComponent<Animator>().applyRootMotion = true;
        gameObject.GetComponent<Animator>().SetBool("GroundPound", true);
        attacking = true;
        weaponCollider.enabled = true;
    }

    public void EndPound()
    {
        Debug.Log("pound end");

        weaponParentObject.SetActive(false);
        attacking = false;
        weaponCollider.enabled = false;
        gameObject.GetComponent<Animator>().SetBool("GroundPound", false);
        //gameObject.GetComponent<Animator>().applyRootMotion = false;
    }

    public void GameOver()
    {
        if (UI_Manager.instance != null && Character_Manager.instance != null)
        {
            UI_Manager.instance.GameOver();
            Character_Manager.instance.revivePlayer();
        }
    }

    public void DamageBegin()
    {
        gameObject.GetComponent<Collider>().enabled = false;
    }

    public void DamageEnd()
    {
        if (Character_Manager.instance != null)
        {
            Character_Manager.instance.invincible = false;
        }
    }
}
