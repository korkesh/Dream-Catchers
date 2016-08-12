using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject SplinterPrefab;
    public HealthManager HM;

    public void explosion()
    {
        if(HM == null)
        {
           HM = this.GetComponent<HealthManager>();
        }

        if(HM != null)
        {
            if(HM.currentHealth <= 0)
            {
                Instantiate(SplinterPrefab, transform.position, transform.rotation);
            }
        }
        
    }

}
