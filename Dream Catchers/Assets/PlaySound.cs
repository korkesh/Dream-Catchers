using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public AudioClip sound;
    public AudioClip altSound;
    public AudioClip dreamSound;
    public AudioClip nightmareSound;

    public GameObject correspondingSound;
    public GameObject soundCollider;
    public bool collide;

    void Start()
    {

    }

    void Play()
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }

    void PlayAlt()
    {
        GetComponent<AudioSource>().PlayOneShot(altSound);
    }

    void PlayDream()
    {
        GetComponent<AudioSource>().PlayOneShot(dreamSound);

    }

    void PlayNightmare()
    {
        GetComponent<AudioSource>().PlayOneShot(nightmareSound);

    }

    void OnCollisionEnter(Collision other)
    {
        if (collide)
        {
            Play();
        }

    }

}
