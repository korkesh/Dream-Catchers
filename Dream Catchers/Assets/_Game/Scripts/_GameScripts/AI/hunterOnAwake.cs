using UnityEngine;
using System.Collections;

public class hunterOnAwake : MonoBehaviour {

    //sends hunter to check point ... needed causse scene and anim times were mismatched

    void Awake()
    {
       if(Level_Manager.Instance.checkPointContinue == true)
       {
            Character_Manager.instance.GoTocheckPoint();
            //Camera.main.GetComponent<NewCamera>().Reset();
            Level_Manager.Instance.checkPointContinue = false;
       }
    }
}
