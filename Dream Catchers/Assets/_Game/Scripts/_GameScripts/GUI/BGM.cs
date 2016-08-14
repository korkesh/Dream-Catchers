using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour {

    public AudioSource audio;
    float bgmVolume = 1.0f;

	// Use this for initialization
	void Start () {
        audio = gameObject.GetComponent<AudioSource>();
        audio.volume = bgmVolume;
        audio.ignoreListenerVolume = true;
    }
	
	void Update()
    {
        bgmVolume = Audio_Manager.Instance.bgm;
        audio.volume = bgmVolume;
    }
}
