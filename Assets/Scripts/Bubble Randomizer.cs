using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleRandomizer : MonoBehaviour {

    protected GameObject _Audio;
    protected AudioManager _audioManager;
    protected float volSFX;

    private AudioSource _speaker;
    public AudioClip bubbleNoise;

    private float waitTime;
    private bool choosenumber;


    // Use this for initialization
    void Start () {
        _Audio = GameObject.Find("AudioManager");
        _audioManager = _Audio.GetComponent<AudioManager>();
        volSFX = _audioManager.volSFX;
        _speaker = this.transform.GetComponent<AudioSource>();
        choosenumber = true;
    }
	
	// Update is called once per frame
	void Update () {
		if(choosenumber == true)
        {
            waitTime = Random.Range(3, 11);
            choosenumber = false;
        }

        else
        {
            StartCoroutine(Bubbling());
        }
	}


    IEnumerator Bubbling()
    {
        yield return new WaitForSeconds(waitTime);
        _speaker.PlayOneShot(bubbleNoise, volSFX);
        choosenumber = true;
    }
}
