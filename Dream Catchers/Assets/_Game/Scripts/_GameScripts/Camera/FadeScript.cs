using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class FadeScript : MonoBehaviour {

    //public float fadeTime = 0.5f;

    ScreenOverlay so;
    bool fadeIn = false;
    bool fadeOut = false;

    // Use this for initialization
    void Start()
    {
        so = gameObject.GetComponent<ScreenOverlay>();

        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            so.intensity = Clamp(0f, 1f, so.intensity + Mathf.Lerp(0f, 1f, Time.deltaTime));
        }
        else if (fadeIn)
        {
            if (so.intensity > 0.68f)
            {
                so.intensity = Clamp(0f, 1f, so.intensity - Mathf.Lerp(0f, 1f, Time.deltaTime * 0.5f));
            }
            else if (so.intensity > 0.3f)
            {
                so.intensity = Clamp(0f, 1f, so.intensity - Mathf.Lerp(0f, 1f, Time.deltaTime * 0.7f));
            }
            else
            {
                so.intensity = Clamp(0f, 1f, so.intensity - Mathf.Lerp(0f, 1f, Time.deltaTime * 0.9f));
            }
        }
    }

    public void FadeIn()
    {
        if(fadeIn)
        {
            return;
        }

        so.intensity = 1.0f;

        fadeIn = true;
        fadeOut = false;
    }

    public void FadeOut()
    {
        if (fadeOut)
        {
            return;
        }

        //toggle = t;

        so.intensity = 0f;

        fadeIn = false;
        fadeOut = true;
    }

    float Clamp(float min, float max, float val)
    {
        if (val < min)
            return min;
        else if (val > max)
            return max;
        return val;
    }
}
