using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public AudioClip sound;
    public AudioClip altSound;
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

    void OnCollisionEnter(Collision other)
    {
        if (collide)
        {
            Play();
        }

    }

}
