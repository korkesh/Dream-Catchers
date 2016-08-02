///=====================================================================================
/// Author: Matt
/// Purpose: Handles the combat flow of the player; includes attacking, invicibility,
///          damage and death
///======================================================================================

using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public GameObject weaponCollider;
    //public GameObject groundPoundCollider;
    public GameObject weaponParentObject;
    public bool attacking;

    public float attackStart;
    public float attackLength;
    public float iframes;

    private PlayerMachine machine;
    private PlayerInputController input;

    // Use this for initialization
    void Start()
    {
        attacking = false;
        weaponCollider.SetActive(false);
        //groundPoundCollider.SetActive(false);

        machine = GameObject.FindWithTag("Player").GetComponent<PlayerMachine>();
        input = GameObject.FindWithTag("Player").GetComponent<PlayerInputController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginAttack()
    {
        if(attacking || !Character_Manager.instance.toggleHammer)
        {
            return;
        }

//        Debug.Log("Attacking");
        StartCoroutine(EnableCollider());
        StartCoroutine(EndAttack());

        weaponParentObject.SetActive(true); // Turn hammer on

        // Attack animation
        // sideswing on horizontal or 0 input
        if (Vector3.Cross(machine.transform.forward, machine.localMovement).magnitude > 0.5f || input.Current.MoveInput.magnitude < 0.25f)
        {
            gameObject.GetComponent<Animator>().SetBool("SideSwing", true);
        }
        // down swing
        else
        {
            gameObject.GetComponent<Animator>().SetBool("SideSwing", true);
        }

        gameObject.GetComponent<Animator>().SetLayerWeight(1, 1);
    }

    public IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(attackStart);

        weaponCollider.SetActive(true);

        // Allow attacks to register damage
        attacking = true;
    }

    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackLength);

//        Debug.Log("Attacking End");

        weaponParentObject.SetActive(false); // Turn hammer off

        // DisAllow attacks to register damage
        attacking = false;
        weaponCollider.SetActive(false);

        // Attack animation End
        gameObject.GetComponent<Animator>().SetBool("SideSwing", false);
        gameObject.GetComponent<Animator>().SetBool("DownSwing", false);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 0);

    }

    /* [DISABLED]
    public void BeginPound()
    {
        weaponParentObject.SetActive(true);
        gameObject.GetComponent<Animator>().SetBool("GroundPound", true);
        attacking = true;
        groundPoundCollider.SetActive(true);
    }

    public void EndPound()
    {
        weaponParentObject.SetActive(false);
        attacking = false;
        groundPoundCollider.SetActive(false);
        gameObject.GetComponent<Animator>().SetBool("GroundPound", false);
    }
    */

    // Turn on invincibilty
    public void DamageBegin()
    {
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(DamageEnd());
    }

    // Turn off invincibility
    public IEnumerator DamageEnd()
    {
        yield return new WaitForSeconds(iframes);

        if (Character_Manager.instance != null)
        {
            //Debug.Log("Damage End");
            Character_Manager.instance.invincible = false;
        }
    }

    // Handle Death state
    public void GameOver()
    {
        if (UI_Manager.instance != null && Character_Manager.instance != null)
        {
            Character_Manager.instance.revivePlayer();
        }
    }
}
