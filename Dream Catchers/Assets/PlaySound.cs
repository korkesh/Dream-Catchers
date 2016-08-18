using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public AudioClip sound;
    public AudioClip altSound;
    public AudioClip altSound2;
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

    void PlayAlt2()
    {
        GetComponent<AudioSource>().PlayOneShot(altSound2);
    }

    void PlayDream()
    {
        GetComponent<AudioSource>().PlayOneShot(dreamSound);

    }

    void PlayNightmare()
    {
        GetComponent<AudioSource>().PlayOneShot(nightmareSound);

    }

    public void PlayTheSound()
    {
        Play();
    }

    void OnCollisionEnter(Collision other)
    {
        if (collide)
        {
            Play();
        }

    }

}
