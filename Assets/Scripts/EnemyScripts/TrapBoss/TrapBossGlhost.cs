﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TrapBossGlhost : BossEnemy
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
    

    [Header("Trap Boss Variables")]
    [SerializeField]
    float _timeBetweenAttacks;
    float _realTimeBetweenAttacks;
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
    public bool trapComplete = false;
    RaycastHit hitObj = new RaycastHit();

    [Header("Fire Trap Percentages")]
    [SerializeField]
    float _xAttackPercentage;
    float _realXAttackPercentage;
    [SerializeField]
    float _quadFirePercentage;
    float _realQuadFirePercentage;
    //[SerializeField]
    float _fireSpinPercentage;
    float _realFireSpinPercentage;
    float _totalPercentageFireTrap;

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Quad Fire Variables")]
    [SerializeField]
    float _quadTrapDamage;
    [SerializeField]
    float _quadShootDist;
    [SerializeField]
    float _quadDetectDist;
    [SerializeField]
    float _quadFireStartDelay;
    [SerializeField]
    float _quadBurnDuration;
    [SerializeField]
    bool _quadShoot;

    [Header("Spawn Trap Variables")]
    [SerializeField]
    GameObject _enemyToSpawn;
    [SerializeField]
    float _spinningSpeed;
    float _realSpinningSpeed;
    float _currSpinningSpeed;
    [SerializeField]
    float _spinAttackDuration;
    float _realSpinAttackDuration;
    [SerializeField]
    float _spawnDelayDuration;
    float _realSpawnDelayDuration;
    float _currSpawnTime;
    float _startSpawnTime;

    [Header("Traps in Room")]
    [SerializeField]
    List<GameObject> _listOfTraps;
    int fcurrentTrap;
    GameObject currentTrap;

    [Header("Fire Trap Variables")]
    [SerializeField]
    float _trapStartDelay;

    float _startAttackTime;
    float _currAttackTime;

    [Header("Trap Boss Hard Variables")]
    [SerializeField]
    float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
    float _hardXAttackPercentage;
    float _hardFireBeamPercentage;
    [SerializeField]
    float _hardQuadFirePercentage;
    [SerializeField]
    float _hardFireSpinPercentage;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;

    [Header("Hard Spawn Spin Variables")]
    [SerializeField]
    float _hardSpinningSpeed;
    [SerializeField]
    float _hardSpinAttackDuration;
    [SerializeField]
    float _hardSpawnDelayDuration;

    [Header("Hard Pinball Variables")]
    [SerializeField]
    float _hardSpeedWhilePinballing;
    [SerializeField]
    float _hardPinballTellDuration;
    [SerializeField]
    float _hardPinballAttackDuration;

    Vector3 _ogCamPos;
    bool _cameraInPosition;

    TRAPSTRATS _MyAttack = TRAPSTRATS.FINDTRAP;

    //intro cutscene function
    
    protected override void PlayIntro()
    {
        /*
        if (!_cameraInPosition)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;
                if (!_glhostsCrushed)
                {
                    for (int i = 0; i < _GlhostsUnderMe.Count; i++)
                    {
                        _GlhostsUnderMe[i].Init();
                    }
                    _glhostsCrushed = true;
                }
            }

            Vector3 cam01;

            cam01 = (1 - _currAttackTime) * cam0 + _currAttackTime * cam1;

            _cameraRef.transform.position = cam01;
        }
        else if (!_fallFinished)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _introFallAndStopDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                if (_enemiesToCrush.activeInHierarchy)
                {
                    for (int i = 0; i < _GlhostsUnderMe.Count; i++)
                    {
                        _GlhostsUnderMe[i].ParentYouself();
                    }
                    _enemiesToCrush.SetActive(false);
                }

                _startAttackTime = Time.time;
                _fallFinished = true;
            }

            Vector3 fall = Vector3.down * Time.deltaTime * _introFallSpeed;
            if (Physics.Raycast(transform.position, fall, _downCheckDistance))
            {

                fall = Vector3.zero;
            }

            transform.position += fall;
        }
        else if (!_turnToPlayerFinished)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _introTurnAroundDuration;

            transform.Rotate(Vector3.up, _introTurnAroundSpeed);

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;

                _startAttackTime = Time.time;
                transform.LookAt(_playerRef.transform.position);
                _turnToPlayerFinished = true;
            }


        }
        else
        {
            _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration;

            Vector3 cam01;

            cam01 = (1 - _currAttackTime) * cam0 + _currAttackTime * cam1;

            _cameraRef.transform.position = cam01;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;
                _startAttackTime = Time.time;
                _playerRef.AmInCutscene = false;
                StartFight();
            }
        }
        */
        _playerRef.AmInCutscene = false;
        _cameraInPosition = true;
        StartFight();
        
    }

    /*
    //called when any other objects for the cutscene are done with their intros
    public override void CheckForIntroEnd()
    {
        for (int i = 0; i < _GlhostsUnderMe.Count; i++)
        {
            if (!_GlhostsUnderMe[i].AmDone)
            {
                //Debug.Log("returned");
                return;
            }
        }

        //Debug.Log("falling started");
        _startAttackTime = Time.time;
        _cameraInPosition = true;
    }
    */

    //called for Init, after the cutscene
    public override void Init()
    {
        if (!_hasInit)
        {
            base.Init();

            if (!_managerRef.HardModeOn)
            {
                _realQuadFirePercentage = _quadFirePercentage;
                _realFireSpinPercentage = _fireSpinPercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realFollowDuration = _followDuration;
                _realSpinningSpeed = _spinningSpeed;
                _realSpinAttackDuration = _spinAttackDuration;
                _realSpawnDelayDuration = _spawnDelayDuration;
                _realXAttackPercentage = _xAttackPercentage;
            }
            else
            {
                _realQuadFirePercentage = _hardQuadFirePercentage;
                _realFireSpinPercentage = _hardFireSpinPercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realFollowDuration = _hardFollowDuration;
                _realSpinningSpeed = _hardSpinningSpeed;
                _realSpinAttackDuration = _hardSpinAttackDuration;
                _realSpawnDelayDuration = _hardSpawnDelayDuration;
                _realXAttackPercentage = _hardXAttackPercentage;
            }
        }

        //_ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _totalPercentageFireTrap = _realQuadFirePercentage + _realXAttackPercentage;

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
                            XAttackFireBeams();
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
        
        if (_enemyAgent.hasPath == false)
        {
            Debug.Log("findTrap");
            fcurrentTrap = Random.Range(0, _listOfTraps.Count);
            GameObject newTrap = _listOfTraps[fcurrentTrap];
            if (newTrap == currentTrap)
            {
                return;
            }
            else
            {
                currentTrap = newTrap;
                _enemyAgent.SetDestination(currentTrap.transform.position);
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
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        if (trapComplete)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            _MyAttack = TRAPSTRATS.FINDTRAP;
        }
    }

    private void XAttackFireBeams()
    {
        if (Physics.Raycast(transform.position + Vector3.up, this.transform.forward, out hitObj, _detectDistance))
        {

            GameObject hitObject = hitObj.collider.gameObject;

            if (hitObject == currentTrap)
            {
                if (_quadShoot)
                {
                    //Moves all the variables from the boss to each fire trap gameobject
                    for (int z = 0; z < 4; z++)
                    {
                        BossFireStatueTrap possessedTrap = hitObject.GetComponent<Transform>().GetChild(z).GetComponent<BossFireStatueTrap>();

                        possessedTrap.transform.eulerAngles += new Vector3(0, 45f, 0);

                        possessedTrap.bossEntity = this.gameObject;
                        possessedTrap._startDelay = _trapStartDelay;
                        possessedTrap._fireDistance = _quadShootDist;
                        possessedTrap._maxDetectDistance = _quadDetectDist;
                        //possessedTrap._beginningDelay = _quadFireStartDelay;
                        possessedTrap._burningDuration = _quadBurnDuration;
                        possessedTrap._fireDamage = _quadTrapDamage;
                        possessedTrap.StartingDelay();

                        trapComplete = false;
                    }

                    _MyAttack = TRAPSTRATS.INSIDETRAP;
                }
            }
        }
    }

    //Detects when he gets to the targeted tower and starts the quad fire attack on the pillar.
    private void QuadFireBeams()
    {
        if (Physics.Raycast(transform.position + Vector3.up, this.transform.forward, out hitObj, _detectDistance))
        {

            GameObject hitObject = hitObj.collider.gameObject;

            if (hitObject == currentTrap)
            {
                if (_quadShoot)
                {
                    //Moves all the variables from the boss to each fire trap gameobject
                    for (int z = 0; z < 4; z++)
                    {
                        BossFireStatueTrap possessedTrap = hitObject.GetComponent<Transform>().GetChild(z).GetComponent<BossFireStatueTrap>();

                        possessedTrap.bossEntity = this.gameObject;
                        //possessedTrap._beginningDelay = _trapStartDelay;
                        possessedTrap._fireDistance = _quadShootDist;
                        possessedTrap._maxDetectDistance = _quadDetectDist;
                        possessedTrap._startDelay = _quadFireStartDelay;
                        possessedTrap._burningDuration = _quadBurnDuration;
                        possessedTrap._fireDamage = _quadTrapDamage;
                        possessedTrap.StartingDelay();
                        trapComplete = false;
                    }

                    _MyAttack = TRAPSTRATS.INSIDETRAP;
                }
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
        _myRoom.CheckForEnd();

        _enemyAgent.enabled = false;

        _playerRef.GoingToOutroCutscene();

        _ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;


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
            _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                _startAttackTime = Time.time;
                _cameraInPosition = true;
            }

            Vector3 cam01;

            cam01 = (1 - _currAttackTime) * cam0 + _currAttackTime * cam1;

            _cameraRef.transform.position = cam01;
        }
        else if (!_showingDeath)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _deathDuration;
            //Debug.Log("showing death");


            if (_currAttackTime > 1)
            {
                _currAttackTime = 1;

                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;

                _myRenderer.enabled = false;

                _startAttackTime = Time.time;
                _showingDeath = true;
            }

            _myColor.a = 1 - _currAttackTime;
            _myMaterial.color = _myColor;
            _myRenderer.materials[1] = _myMaterial;
            _myRenderer.materials[0] = _myMaterial;
        }
        else
        {
            _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration;

            Vector3 cam01;

            cam01 = (1 - _currAttackTime) * cam0 + _currAttackTime * cam1;

            _cameraRef.transform.position = cam01;
            //Debug.Log("putting camera back");

            if (_currAttackTime >= 1)
            {
                //Debug.Log("camera put back");
                _currAttackTime = 1;
                _playerRef.AmInCutscene = false;
                _playerRef.ResetSword();
                _playerRef.ResetBossCutSceneCheck();
                _cameraRef.AmFollowingPlayer = true;
                _endingPlaying = false;

                //Debug.Log("dead");
                _myRoom.EndAll();
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
            _myRenderer.enabled = true;
            _myColor.a = 1;
            _myMaterial.color = _myColor;
            _myRenderer.materials[1] = _myMaterial;

            _enemyAgent.enabled = false;
            //Debug.Log("Boss Reset");
            transform.position = _startPos;
            transform.rotation = _startRot;
            /*
            _enemiesToCrush.SetActive(true);
            for (int i = 0; i < _GlhostsUnderMe.Count; i++)
            {
                _GlhostsUnderMe[i].ResetCutscene();
            }
            */
            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);
            _cameraInPosition = false;
            //_fallFinished = false;
            //_turnToPlayerFinished = false;
            //_glhostsCrushed = false;

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
}
