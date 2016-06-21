//================================
// Alex
//  Deals Damage to character
//================================
using UnityEngine;
using System.Collections;

public class DamageDealer : MonoBehaviour {

    public int Damage;

	public void DealDamage()
    {
        Character_Manager.instance.takeDamage(Damage);
    }

   
}
