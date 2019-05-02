using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MiniTrapBossGlhost : BossEnemy
{
    //Strategy Enum for the Spin Boss
    protected enum TRAPSTRATS
    {
        DOWNTIME,
        FINDTRAP,
        INSIDETRAP,
        FIREBEAM,
        XATTACK,
        QUADFIRE
    }

    [Header("Mini Trap Boss Intro Variables")]
    [SerializeField]
    Camera _additionalCam1;
    Vector3 _additionalPos1, _additionalRot1;
    bool _animating = false;
    bool _animatingSecond = false;

    [Header("Trap Boss Variables")]
    [SerializeField]
    float _offsetTrapJumpInDistance;
    bool _enteringTrap = false;
    [SerializeField]
    float _vertDetectOffset;
    [SerializeField]
    float _startAngle;
    [SerializeField]
    float _detectionAngle;
    float _calcAngle;
    [SerializeField]
    float _maxDistanceOut;
    [SerializeField]
    bool _debug;
    [SerializeField]
    float _detectDistance;
    bool trapComplete = false;
    int _numOfCasts = 4;
    RaycastHit hitObj = new RaycastHit();
    
    //traps in room
    int fcurrentTrap;
    GameObject currentTrap;

    [Header("Fire Trap Percentages")]
    [SerializeField]
    float _xAttackPercentage;
    float _realXAttackPercentage;
    [SerializeField]
    float _quadFirePercentage;
    float _realQuadFirePercentage;
    float _totalPercentageFireTrap;

    [Header("Quad Fire Variables")]
    [SerializeField]
    float _quadTrapDamage;
    float _realQuadTrapDamage;
    [SerializeField]
    float _quadShootDist;
    float _realQuadShootDist;
    [SerializeField]
    float _quadDetectDist;
    float _realQuadDetectDist;
    [SerializeField]
    float _quadFireStartDelay;
    float _realQuadFireStartDelay;
    float _quadStartDelay = 1;
    [SerializeField]
    float _quadBurnDuration;
    float _realQuadBurnDuration;
    bool _xAttack = false;

    [Header("Fire Trap Variables")]

    float _startAttackTime;
    float _currAttackTime;

    [Header("Trap Boss Hard Variables")]
    //[SerializeField]
    //float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
    float _hardXAttackPercentage;
    float _hardFireBeamPercentage;
    [SerializeField]
    float _hardQuadFirePercentage;
    //[SerializeField]
    //float _hardFireSpinPercentage;

    [Header("Hard Quad Fire Variables")]
    [SerializeField]
    float _hardQuadTrapDamage;
    [SerializeField]
    float _hardQuadShootDist;
    [SerializeField]
    float _hardQuadDetectDist;
    [SerializeField]
    float _hardQuadFireStartDelay;
    [SerializeField]
    float _hardQuadBurnDuration;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;


    [Header("Sound Variables")]
    [SerializeField]
    AudioClip bossPossess;
    [SerializeField]
    AudioClip bossDeath;
    [SerializeField]
    AudioClip fireAttack;




    bool _cameraInPosition;

    TRAPSTRATS _MyAttack = TRAPSTRATS.FINDTRAP;

    protected override void Awake()
    {
        base.Awake();
        _additionalCam1.transform.parent = null;
        _additionalPos1 = _additionalCam1.transform.position;
        _additionalRot1 = _additionalCam1.transform.localEulerAngles;
        _additionalCam1.gameObject.SetActive(false);
    }

    //intro cutscene function

    protected override void PlayIntro()
    {
        if (!_cameraInPosition)
        {
            if (_cameraRef.MoveCamera())
            {
                _cameraInPosition = true;
                //Debug.Log("Part1 Done");
            }

        }
        else if (!_animating)
        {
            if (_myAnimations.IsInTransition(0))
            {
                cam0 = _cameraRef.transform.position;
                cam1 = _additionalPos1;
                rot0 = _cameraRef.transform.localEulerAngles;
                rot1 = _additionalRot1;

                _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);
                _animating = true;

                //Debug.Log("Part2 Done");
            }
        }
        else if(!_animatingSecond)
        {
            if (_cameraRef.MoveCamera())
            {
                if (_myAnimations.IsInTransition(0))
                {
                    cam0 = _cameraRef.transform.position;
                    cam1 = _ogCamPos;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _ogCamRot;

                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

                    _animatingSecond = true;

                    //Debug.Log("Part2 Done");
                }
            }
        }
        else
        {
            if (_cameraRef.MoveCamera())
            {
                _startAttackTime = Time.time;
                _playerRef.AmInCutscene = false;
                StartFight();
            }
        }

    }

    //called for Init, after the cutscene
    public override void Init()
    {
        if (!_hasInit)
        {
            base.Init();

            if (!_managerRef.HardModeOn)
            {
                _realQuadFirePercentage = _quadFirePercentage;
                //_realFireSpinPercentage = _fireSpinPercentage;
                //_realTimeBetweenAttacks = _timeBetweenAttacks;
                _realQuadBurnDuration = _quadBurnDuration;
                _realQuadDetectDist = _quadDetectDist;
                _realQuadFireStartDelay = _quadFireStartDelay;
                _realQuadShootDist = _quadShootDist;
                _realQuadTrapDamage = _quadTrapDamage;
                //_realFollowDuration = _followDuration;
                _realXAttackPercentage = _xAttackPercentage;
            }
            else
            {
                _realQuadFirePercentage = _hardQuadFirePercentage;
                //_realFireSpinPercentage = _hardFireSpinPercentage;
                //_realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realQuadBurnDuration = _hardQuadBurnDuration;
                _realQuadDetectDist = _hardQuadDetectDist;
                _realQuadFireStartDelay = _hardQuadFireStartDelay;
                _realQuadShootDist = _hardQuadShootDist;
                _realQuadTrapDamage = _hardQuadTrapDamage;
                //_realFollowDuration = _hardFollowDuration;
                _realXAttackPercentage = _hardXAttackPercentage;
            }
        }

        _ogCamPos = _cameraRef.transform.position;
        _ogCamRot = _cameraRef.transform.localEulerAngles;
        cam0 = _cameraRef.transform.position;
        cam1 = _bossCameraPos;
        rot0 = _cameraRef.transform.localEulerAngles;
        rot1 = _bossCameraRot;
        _cameraRef.AmFollowingPlayer = false;

        _totalPercentageFireTrap = _realQuadFirePercentage + _realXAttackPercentage;

        _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

        _speaker = this.GetComponent<AudioSource>();


        _myAnimations.Play("MiniIntro1", 0);

        _startAttackTime = Time.time;
        _myAI = BossAI.INTRO;
    }

    //called when init and cutscene are done
    //starts fight
    protected override void StartFight()
    {
        if (!_init)
        {
            Debug.Log("start Fight");
            _bossBar.SetActive(true);
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _enemyAgent.enabled = true;
            _cameraRef.AmFollowingPlayer = true;
            _calcAngle = _startAngle;

            
        }
        _myAI = BossAI.FIGHTING;
        _init = true;
    }

    protected override void Update()
    {
        switch (_myAI)
        {
            case BossAI.NONE:
                break;
            case BossAI.INTRO:
                PlayIntro();
                break;
            case BossAI.FIGHTING:
                if (_init)
                {
                    if (_amHit)
                    {
                        ResetAmHit();
                    }

                    base.Update();
                    switch (_MyAttack)
                    {
                        case TRAPSTRATS.FINDTRAP:
                            findTrap();
                            break;
                        case TRAPSTRATS.INSIDETRAP:
                            possessTrap();
                            break;
                            
                        case TRAPSTRATS.XATTACK:
                            QuadFireBeams();
                            _xAttack = true;
                            break;
                        case TRAPSTRATS.QUADFIRE:
                            QuadFireBeams();
                            break;
                            
                        default:
                            //Debug.Log("No Attack Set");
                            break;
                    }
                }
                break;
            case BossAI.OUTRO:
                PlayEnd();
                break;
            default:
                break;
        }
    }

    //decides what the attack the boss will do next (Currently Out of Use but setup fordifferent attacks.
    private void WhatDoNext()
    {
            float _nextAttack = Random.Range(0, _totalPercentageFireTrap);
            Debug.Log("Next attack: " + _nextAttack);
            if (_nextAttack > 0 && _nextAttack <= _realQuadFirePercentage)
            {
                Debug.Log("Quad Fire");
                _MyAttack = TRAPSTRATS.QUADFIRE;
            }
            else
            {
                Debug.Log("XATTACK");
                _MyAttack = TRAPSTRATS.XATTACK;
            }
    }

    //Find Trap
    //Boss will find and go towards a trap
    //Than he will cast QuadFire, there is room to do a deciding attack for random attacks
    private void findTrap()
    {

        fcurrentTrap = Random.Range(0, _myRoom.GetCurrTrapList.Count);
        GameObject newTrap = _myRoom.GetCurrTrapList[fcurrentTrap].gameObject;
        //Debug.Log("1N = " + newTrap.name);


        if (newTrap == currentTrap)
        {
            return;
        }
        else
        {
            if (_myAnimations.IsInTransition(0))
            {
                currentTrap = newTrap;
                _enteringTrap = true;
                // Debug.Log("2N = " + newTrap.name);
                //Debug.Log("C = " + currentTrap.name);
                Vector3 _trapPos = currentTrap.transform.position;
                _trapPos.y = transform.position.y;
                transform.LookAt(_trapPos);
                _enemyAgent.SetDestination(currentTrap.transform.position - (transform.forward * _offsetTrapJumpInDistance));
            }
            else
            {
                return;
            }
        }


        //Debug.DrawRay(transform.position + Vector3.up, this.transform.forward);

        WhatDoNext();

    }

    //Attack
    //Boss possesses trap by disabling his own mesh and collider. (Room for animation)
    //When the trap finishes its attack he will be reinabled and find a new trap
    private void possessTrap()
    {
        _mySkinRenderer.enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        Vector3 possessPosition = currentTrap.transform.position;
        Debug.Log(currentTrap.transform.position);
        possessPosition.y = transform.position.y;
        transform.position = possessPosition;

        if (trapComplete)
        {
            _speaker.Stop();
            _xAttack = false;
            _mySkinRenderer.enabled = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            _myAnimations.Play("Rise", 0);
            _enemyAgent.SetDestination(transform.position);
            _MyAttack = TRAPSTRATS.FINDTRAP;
        }
    }


    //Detects when he gets to the targeted tower and starts the quad fire attack on the pillar.
    private void QuadFireBeams()
    {
        if (_enemyAgent.hasPath == false)
        {
            if (_enteringTrap)
            {
                _speaker.Stop();
                if (!_speaker.isPlaying)
                    _speaker.PlayOneShot(bossPossess, volSFX);

                _myAnimations.Play("Dive", 0);
                _enteringTrap = false;
            }
            else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f)
            {
                //Moves all the variables from the boss to each fire trap gameobject
                for (int z = 0; z < 4; z++)
                {
                    if(!_speaker.isPlaying)
                    _speaker.PlayOneShot(fireAttack, volSFX);
                    BossFireStatueTrap possessedTrap = currentTrap.GetComponent<Transform>().GetChild(z).GetComponent<BossFireStatueTrap>();

                    if (_xAttack)
                    {
                        possessedTrap.transform.eulerAngles += new Vector3(0, 45f, 0);
                        possessedTrap.GetXAttack = true;
                    }


                    possessedTrap.GetBossEntity = this.gameObject;
                    possessedTrap.GetFireDelay = _realQuadFireStartDelay;
                    possessedTrap.GetFireDistance = _realQuadShootDist;
                    possessedTrap.GetMaxDetectDistance = _realQuadDetectDist;
                    possessedTrap.GetStartDelay = _quadStartDelay;
                    possessedTrap.GetBurningDuration = _realQuadBurnDuration;
                    possessedTrap.GetFireDamage = _realQuadTrapDamage;
                    possessedTrap.StartingDelay();
                    trapComplete = false;
                }

                _MyAttack = TRAPSTRATS.INSIDETRAP;
            }
        }

    }

    //called when boss gets hit and takes damage
    public override void GotHit(float _damageTaken)
    {
        if (!_amHit && !_invincible)
        {
            //Debug.Log("got hit");
            base.GotHit(_damageTaken);

            _startInvincibleTime = Time.time;
            _amHit = true;

            if (_currBossHealth <= 0)
            {
                _bossBar.SetActive(false);

                Die();
            }
        }
    }

    //called once the boss is defeated
    protected override void Die()
    {

        _speaker.Stop();
        if (!_speaker.isPlaying)
            _speaker.PlayOneShot(bossDeath, volSFX);

        _myRoom.CheckForEnd();

        _enemyAgent.enabled = false;

        _playerRef.GoingToOutroCutscene();
        _enteringTrap = false;

        cam0 = _cameraRef.transform.position;
        cam1 = _bossCameraPos;
        rot0 = _cameraRef.transform.localEulerAngles;
        rot1 = _bossCameraRot;
        _cameraRef.AmFollowingPlayer = false;


        _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);


        _startAttackTime = Time.time;
        _cameraInPosition = false;
        _endingPlaying = true;

        _myAI = BossAI.OUTRO;
    }

    //plays ending cutscene
    protected override void PlayEnd()
    {
        if (!_cameraInPosition)
        {
            if (_cameraRef.MoveCamera())
            {
                if (_myAnimations.IsInTransition(0))
                {
                    _cameraInPosition = true;
                }
            }

        }
        else if (!_showingDeath)
        {
            //Debug.Log("showing death");

            if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .98f)
            {
                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;
                rot0 = _cameraRef.transform.localEulerAngles;
                rot1 = _ogCamRot;

                _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

                _mySkinRenderer.enabled = false;

                _startAttackTime = Time.time;
                _showingDeath = true;
            }

            _myColor.a = 1 - _myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _myMaterial.color = _myColor;
            _mySkinRenderer.materials[1] = _myMaterial;
            _mySkinRenderer.materials[0] = _myMaterial;
        }
        else
        {
            //Debug.Log("putting camera back");

            if (_cameraRef.MoveCamera())
            {
                //Debug.Log("camera put back");
                _currAttackTime = 1;
                _playerRef.AmInCutscene = false;
                _playerRef.ResetSword();
                _playerRef.ResetBossCutSceneCheck();
                _cameraRef.AmFollowingPlayer = true;
                _endingPlaying = false;

                //Debug.Log("dead");
                gameObject.SetActive(false);
            }
        }
    }

    //Reset function
    public override void MyReset()
    {
        if (_init)
        {
            gameObject.SetActive(true);
            _mySkinRenderer.enabled = true;
            _myColor.a = 1;
            _myMaterial.color = _myColor;
            _mySkinRenderer.materials[1] = _myMaterial;

            _enemyAgent.enabled = false;
            //Debug.Log("Boss Reset");
            transform.position = _startPos;
            transform.rotation = _startRot;
            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);
            _cameraInPosition = false;
            _animating = false;
            _animatingSecond = false;

            _endingPlaying = false;
            _laggingHealth = false;
            _updatingHealth = false;
            _dead = false;
            _amHit = false;
            _invincible = false;

            _myAI = BossAI.NONE;
            _MyAttack = TRAPSTRATS.FINDTRAP;
            _init = false;
        }
    }

    public bool IsPossessing { get { return trapComplete; } set { trapComplete = value; } }
}
