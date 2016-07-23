using UnityEngine;
using System.Collections;

public class hunterOnAwake : MonoBehaviour {

    void Awake()
    {
       if(Level_Manager.instance.checkPointContinue == true)
       {
           Character_Manager.instance.GoTocheckPoint();
           Level_Manager.instance.checkPointContinue = false;
       }
    }
}
