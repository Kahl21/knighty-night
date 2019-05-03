using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    AudioSource SFXPlayer;

    [SerializeField]
    AudioSource MusicPlayer;

    //Original SFX
    [Header("Original SFX")]
    [SerializeField]
    AudioClip SwingClip;
    [SerializeField]
    AudioClip GateOpenClip;
    [SerializeField]
    AudioClip GateCloseClip;
    [SerializeField]
    AudioClip FireClip;

    //Player SFX
    [Header("Player SFX")]
    [SerializeField]
    AudioClip PlayerDamageClip;

    [SerializeField]
    AudioClip PlayerDead;


    //Sewer SFX
    [Header("Sewer SFX")]
    [SerializeField]
    AudioClip BubblesClip;




    //General SFX
    [SerializeField]
    AudioClip RoomCompleteClip;

    [SerializeField]
    AudioClip ButtonPressedClip;

    [SerializeField]
    AudioClip ButtonMovedClip;


    //Ghost SFX
    [Header("Ghost SFX")]

    [SerializeField]
    AudioClip ColorGhostCorrectClip;




    //BackGround Music
    [Header("Music")]
    [SerializeField]
    AudioClip BGMDungeon;
    [SerializeField]
    AudioClip BGMSewer;
    [SerializeField]
    AudioClip BGMGraveyard;
    [SerializeField]
    AudioClip BGMCathedral;
    [SerializeField]
    AudioClip BGMMenu;
    [SerializeField]
    AudioClip BGMBoss;
    [SerializeField]
    AudioClip BGMChase;
    [SerializeField]
    AudioClip BGMSecretBoss;

    //master volume value
    public float volMaster = 10;
    //music volume value
    public float volMusic = 10;
    //sfx music volume
    public float volSFX = 10;
    //function that changes the music volume
    //function that changes the sfx volume

    //audio play format
    //SFXPlayer.PlayOneShot(clip, volume);
    //Put This In code To Make Noise
    //AudioManager.instance.Swing();

    bool boss;
    bool chase;
    bool secretBoss;

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



        MusicPlayer = this.transform.GetChild(0).GetComponent<AudioSource>();


        RestartMusic();

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if(volMaster<=volMusic)
        {
            volMusic = volMaster;
        }
        if(volMaster<=volSFX)
        {
            volSFX = volMaster;
        }

        ChooseMusic();
    }

    //Volume Control
    public virtual void MasterVolume(float value) //NOTE: This may not be in the final menu depending how lazy I feel ¯\_(ツ)_/¯
    {
        volMaster = value;
        volMusic = value;
        volSFX = value;
        RestartMusic();
    }

    public virtual void MusicVolume(float value)
    {
        volMusic = value;
        RestartMusic();
    }

    public virtual void SFXVolume(float value)
    {
        volSFX = value;
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    //Original SFX Functions 
    public virtual void Swing()
    {
        SFXPlayer.PlayOneShot(SwingClip, volSFX);
    }

    public virtual void GateOpen()
    {
            SFXPlayer.PlayOneShot(GateOpenClip, volSFX);
    }

    public virtual void GateClose()
    {
            SFXPlayer.PlayOneShot(GateCloseClip, volSFX);
    }

    public virtual void FireAttack()
    {
            SFXPlayer.PlayOneShot(FireClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    //Player SFX Functions
    public virtual void PlayerDamaged()
    {
            SFXPlayer.PlayOneShot(PlayerDamageClip, volSFX);
    }

    public virtual void PlayerDied()
    {
        SFXPlayer.PlayOneShot(PlayerDead, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Sewer SFX Functions
    public virtual void Bubbling()
    {
            SFXPlayer.PlayOneShot(BubblesClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //General SFX


    public virtual void RoomComplete()
    {
        if (!SFXPlayer.isPlaying)
            SFXPlayer.PlayOneShot(RoomCompleteClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Ghost SFX Functions
    public virtual void GhostColorCorrect()
    {
            SFXPlayer.PlayOneShot(ColorGhostCorrectClip, volSFX);
    }

    public virtual void ButtonPressed()
    {
        SFXPlayer.PlayOneShot(ButtonPressedClip, volSFX);
    }

    public virtual void ButtonMoving()
    {
        SFXPlayer.PlayOneShot(ButtonMovedClip, volSFX);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    public virtual void ChooseMusic()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (MusicPlayer.enabled)
        {


            if (!boss && !chase && !secretBoss)
            {

                if (currentScene > 0 && currentScene <= 4)
                {
                    if (!MusicPlayer.isPlaying)
                        MusicPlayer.PlayOneShot(BGMDungeon, volMusic / 10);
                }

                else if (currentScene > 4 && currentScene <= 7)
                {
                    if (!MusicPlayer.isPlaying)
                        MusicPlayer.PlayOneShot(BGMSewer, volMusic / 10);
                }

                else if (currentScene > 7 && currentScene <= 10)
                {
                    if (!MusicPlayer.isPlaying)
                        MusicPlayer.PlayOneShot(BGMGraveyard, volMusic / 10);
                }

                else if (currentScene > 10 && currentScene <= 13)
                {
                    if (!MusicPlayer.isPlaying)
                        MusicPlayer.PlayOneShot(BGMCathedral, volMusic / 10);
                }

                else if (currentScene == 0)
                    if (!MusicPlayer.isPlaying)
                        MusicPlayer.PlayOneShot(BGMMenu, volMusic / 10);
            }

            else if (boss && !chase && !secretBoss)
            {
                if (!MusicPlayer.isPlaying)
                    MusicPlayer.PlayOneShot(BGMBoss, volMusic / 10);
            }

            else if (!boss && chase && !secretBoss)
            {
                if (!MusicPlayer.isPlaying)
                    MusicPlayer.PlayOneShot(BGMChase, volMusic / 10);
            }

            else if (!boss && !chase && secretBoss)
            {
                if (!MusicPlayer.isPlaying)
                    MusicPlayer.PlayOneShot(BGMSecretBoss, volMusic / 10);
            }


        }


    }

    public virtual void StartMusic()
    {
        StartCoroutine(MusicStart());
    }

    public virtual void RestartMusic()
    {
        MusicPlayer.Stop();
        if(volMusic>0)
            ChooseMusic();
    }



    IEnumerator MusicStart()
    {
        float fadeTime = 0;
        RestartMusic();
        MusicPlayer.volume = 0;
        yield return new WaitForSeconds(1.8f);
        MusicPlayer.volume = volSFX;

    }


    public virtual void ChaseStart() { chase = true; RestartMusic(); }
    public virtual void ChaseStop() { chase = false; RestartMusic(); }
    public virtual void BossStart() { boss = true; RestartMusic(); }
    public virtual void SecretBossStart() { secretBoss = true; RestartMusic(); }
    public virtual void BossStop() { boss = false; secretBoss = false; RestartMusic(); }
}
