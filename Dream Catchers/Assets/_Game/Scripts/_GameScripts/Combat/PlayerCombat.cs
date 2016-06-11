using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public GameObject weaponCollider;
    public GameObject groundPoundCollider;
    public GameObject weaponParentObject;
    public bool attacking;

    public float attackCooldown;

    // Use this for initialization
    void Start()
    {
        attacking = false;
        weaponCollider.SetActive(false);
        groundPoundCollider.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginAttack()
    {
        if(attacking)
        {
            return;
        }

        Debug.Log("Attacking");

        weaponParentObject.SetActive(true); // Turn hammer on

        // Attack animation
        gameObject.GetComponent<Animator>().SetBool("StandAttack", true);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 1);

        // Allow attacks to register damage
        attacking = true;
        weaponCollider.SetActive(true);

        StartCoroutine(EndAttack());

    }

    public IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackCooldown);

        Debug.Log("Attacking End");

        weaponParentObject.SetActive(false); // Turn hammer off

        // DisAllow attacks to register damage
        attacking = false;
        weaponCollider.SetActive(false);

        // Attack animation End
        gameObject.GetComponent<Animator>().SetBool("StandAttack", false);
        gameObject.GetComponent<Animator>().SetLayerWeight(1, 0);

    }

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
