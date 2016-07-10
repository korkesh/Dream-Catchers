using UnityEngine;
using System.Collections;

public class Confetti : MonoBehaviour {

    public GameObject Confet;
    public HealthManager hM;
    void OnDestroy()
    {
        if(Confet != null && hM != null)
        {
            if(hM.currentHealth <=0 )
            {
                GameObject.Instantiate(Confet, this.transform.position, this.transform.rotation);
            }

        }
        else
        {
            Debug.Log("no HM");
        }  
    }
}
