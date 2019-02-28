using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    AudioSource SFXPlayer;

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


    //Sewer SFX
    [Header("Sewer SFX")]
    [SerializeField]
    AudioClip BubblesClip;


    //Graveyard SFX
    [Header("Graveyard SFX")]
    [SerializeField]
    AudioClip GraveGateOpenClip;
    [SerializeField]
    AudioClip GraveGateCloseClip;


    //General SFX
    [Header("General SFX")]
    [SerializeField]
    AudioClip FireCacklingClip;
    [SerializeField]
    AudioClip LeverHitClip;
    [SerializeField]
    AudioClip SpikeActivateClip;
    [SerializeField]
    AudioClip ArrowsShootingClip;
    [SerializeField]
    AudioClip FlameTrapShootClip;
    [SerializeField]
    AudioClip HealingCircleClip;
    [SerializeField]
    AudioClip RoomCompleteClip;



    //Ghost SFX
    [Header("Ghost SFX")]
    [SerializeField]
    AudioClip GhostDeathClip;
    [SerializeField]
    AudioClip GhostHitClip;
    [SerializeField]
    AudioClip ColorGhostCorrectClip;
    [SerializeField]
    AudioClip ImmuneGhostTransformClip;

    //Boss SFX
    [Header("Boss SFX")]
    [SerializeField]
    AudioClip BossDefeatedClip;
    [SerializeField]
    AudioClip BossDazedClip;
    [SerializeField]
    AudioClip BossChargeClip;
    [SerializeField]
    AudioClip BossSpinClip;
    [SerializeField]
    AudioClip BossPossessClip;
    [SerializeField]
    AudioClip BossBounceClip;
    [SerializeField]
    AudioClip BossGhostSpawnClip;

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

    //master volume value
    static float volMaster = 1;
    //music volume value
    static float volMusic = 1;
    //sfx music volume
    static float volSFX = 1;
    //function that changes the music volume
    //function that changes the sfx volume

    //audio play format
    //SFXPlayer.PlayOneShot(clip, volume);
    //Put This In code To Make Noise
    //AudioManager.instance.Swing();

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

    //Volume Control
    public virtual void MasterVolume(float value) //NOTE: This may not be in the final menu depending how lazy I feel ¯\_(ツ)_/¯
    {
        volMaster = value;
    }

    public virtual void MusicVolume(float value)
    {
        volMusic = value;
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
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Sewer SFX Functions
    public virtual void Bubbling()
    {
        SFXPlayer.PlayOneShot(BubblesClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    //Graveyard SFX Function
    public virtual void GraveGateOpening()
    {
        SFXPlayer.PlayOneShot(GraveGateOpenClip, volSFX);
    }

    public virtual void GraveGateClosing()
    {
        SFXPlayer.PlayOneShot(GraveGateCloseClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //General SFX
    public virtual void FireCrack()
    {
        SFXPlayer.PlayOneShot(FireCacklingClip, volSFX);
    }

    public virtual void Lever()
    {
        SFXPlayer.PlayOneShot(LeverHitClip, volSFX);
    }

    public virtual void SpikeActivate()
    {
        SFXPlayer.PlayOneShot(SpikeActivateClip, volSFX);
    }

    public virtual void Arrows()
    {
        SFXPlayer.PlayOneShot(ArrowsShootingClip, volSFX);
    }

    public virtual void FlameTrap()
    {
        SFXPlayer.PlayOneShot(FlameTrapShootClip, volSFX);
    }

    public virtual void HealingCircle()
    {
        SFXPlayer.PlayOneShot(HealingCircleClip, volSFX);
    }

    public virtual void RoomComplete()
    {
        SFXPlayer.PlayOneShot(RoomCompleteClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Ghost SFX Functions
    public virtual void GhostDead()
    {
        SFXPlayer.PlayOneShot(GhostDeathClip, volSFX);
    }

    public virtual void GhostHit()
    {
        SFXPlayer.PlayOneShot(GhostHitClip, volSFX);
    }

    public virtual void GhostColorCorrect()
    {
        SFXPlayer.PlayOneShot(ColorGhostCorrectClip, volSFX);
    }

    public virtual void GhostImmuneTransform()
    {
        SFXPlayer.PlayOneShot(ImmuneGhostTransformClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Boss SFX Functions
    public virtual void BossDefeated()
    {
        SFXPlayer.PlayOneShot(BossDefeatedClip, volSFX);
    }

    public virtual void BossDazed()
    {
        SFXPlayer.PlayOneShot(BossDazedClip, volSFX);
    }

    public virtual void BossCharge()
    {
        SFXPlayer.PlayOneShot(BossChargeClip, volSFX);
    }

    public virtual void BossSpin()
    {
        SFXPlayer.PlayOneShot(BossSpinClip, volSFX);
    }

    public virtual void BossPossess()
    {
        SFXPlayer.PlayOneShot(BossPossessClip, volSFX);
    }

    public virtual void BossBounce()
    {
        SFXPlayer.PlayOneShot(BossBounceClip, volSFX);
    }

    public virtual void BossSpawnGhost()
    {
        SFXPlayer.PlayOneShot(BossGhostSpawnClip, volSFX);
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////


    //Music Functions Coming Soon


    /*   
    AudioClip BGMDungeon;
    AudioClip BGMSewer;
    AudioClip BGMGraveyard;
    AudioClip BGMCathedral;  
    */
}
