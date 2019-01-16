using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    AudioSource SFXPlayer;

    [SerializeField]
    AudioClip SwingClip;
    [SerializeField]
    AudioClip GateOpenClip;
    [SerializeField]
    AudioClip GateCloseClip;
    [SerializeField]
    AudioClip FireClip;

    public static AudioManager instance = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public virtual void Swing()
    {
        SFXPlayer.PlayOneShot(SwingClip);
    }

    public virtual void GateOpen()
    {
        SFXPlayer.PlayOneShot(GateOpenClip);
    }

    public virtual void GateClose()
    {
        SFXPlayer.PlayOneShot(GateCloseClip);
    }

    public virtual void FireAttack()
    {
        SFXPlayer.PlayOneShot(FireClip);
    }

}
