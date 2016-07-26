using UnityEngine;
using System.Collections;

public class hunterOnAwake : MonoBehaviour {

    void Awake()
    {
       if(Level_Manager.Instance.checkPointContinue == true)
       {
           Character_Manager.instance.GoTocheckPoint();
           Level_Manager.Instance.checkPointContinue = false;
       }
    }
}
