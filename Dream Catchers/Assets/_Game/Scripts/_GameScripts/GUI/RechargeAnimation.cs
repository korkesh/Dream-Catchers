///=====================================================================================
/// Author: Matt
/// Purpose: Circle swipe animation on manipulation UI element
///======================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RechargeAnimation : MonoBehaviour {

    public Image rechargeGraphic;
    float timer = 60;

    void Start()
    {
        rechargeGraphic.fillAmount = 1.0f;
    }

    void Update()
    {     
       rechargeGraphic.fillAmount += Time.deltaTime / timer;      
    }

    public void StartRecharge(float s)
    {
        rechargeGraphic.fillAmount = 0.0f;
        timer = s;
    }
}
