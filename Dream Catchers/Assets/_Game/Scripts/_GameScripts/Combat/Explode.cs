using UnityEngine;
using System.Collections;

public class Explode : MonoBehaviour {

    public GameObject SplinterPrefab;
    public GameObject sound;

    public void explosion()
    {
        Instantiate(SplinterPrefab, transform.position, transform.rotation);

        if(sound != null)
        {
            GameObject soundObject = Instantiate(sound, transform.position, transform.rotation) as GameObject;
            Destroy(soundObject, soundObject.GetComponent<AudioSource>().clip.length);
        }
    }

}
