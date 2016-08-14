using UnityEngine;
using System.Collections;

public class SpawnSound : MonoBehaviour {

    public GameObject sound;

    void OnDestroy()
    {
        GameObject soundObject = Instantiate(sound, transform.position, transform.rotation) as GameObject;
        Destroy(soundObject, soundObject.GetComponent<AudioSource>().clip.length);
    }
}
