using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RechargeAnimation : MonoBehaviour {

    public Image rechargeGraphic;
    public float timer = 60;

    void Start()
    {
        rechargeGraphic.fillAmount = 1.0f;      // Initally progress bar is full
    }

    void Update()
    {
        
            rechargeGraphic.fillAmount += Time.deltaTime / timer;
        
    }

    public void StartRecharge(float s)
    {
        rechargeGraphic.fillAmount = 0.0f;      // Initally progress bar is empty
        timer = s;
    }
}
