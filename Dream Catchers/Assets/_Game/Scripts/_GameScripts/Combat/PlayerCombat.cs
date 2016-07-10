﻿///=====================================================================================
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

    // Use this for initialization
    void Start()
    {
        attacking = false;
        weaponCollider.SetActive(false);
        //groundPoundCollider.SetActive(false);

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
        gameObject.GetComponent<Animator>().SetBool("StandAttack", true);
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
        gameObject.GetComponent<Animator>().SetBool("StandAttack", false);
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
    }

    // Turn off invincibility
    public void DamageEnd()
    {
        if (Character_Manager.instance != null)
        {
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
