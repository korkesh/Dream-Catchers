using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class FadeScript : MonoBehaviour {

    public float fadeTime = 0.5f;

    ScreenOverlay so;
    bool fadeIn = false;
    bool fadeOut = false;

    bool toggle;

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
            Debug.Log("fading out");
            so.intensity = Mathf.Lerp(so.intensity, 1.0f, (Time.deltaTime * 3f) / fadeTime);

            if(so.intensity >= 0.97f)
            {
                if(toggle)
                {
                    ManipulationManager.instance.currentWorldState = (ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
                                                                        ? ManipulationManager.WORLD_STATE.NIGHTMARE
                                                                        : ManipulationManager.WORLD_STATE.DREAM;
                }
            }
        }
        else if (fadeIn)
        {
            so.intensity = Mathf.Lerp(so.intensity, 0.0f, Time.deltaTime / fadeTime);
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
}
