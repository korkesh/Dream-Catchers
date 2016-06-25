///=====================================================================================
/// Author: Matt
/// Purpose: Handles health on enemies and player
///======================================================================================

using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

    public int maxHealth;
    public int currentHealth;
    public bool spawns;

    bool isVulnerable = true;
	
    void Start()
     {
         spawns = false;
     }

	// Update is called once per frame
	void Update () {
	
        if(currentHealth <= 0)
        {
            spawns = true;
            Destroy(gameObject);
        }
	}

    public void TakeDamage(int dmg)
    {
        if(isVulnerable)
        {
            currentHealth -= dmg;
        }
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
