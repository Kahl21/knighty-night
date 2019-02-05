using System.Collections;
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
        QUADFIRE
    }

    
    [Header("Spin Boss Intro Variables")]
    [SerializeField]
    float _introFallAndStopDuration;
    [SerializeField]
    float _introFallSpeed;
    [SerializeField]
    float _downCheckDistance;
    [SerializeField]
    float _introTurnAroundSpeed;
    [SerializeField]
    float _introTurnAroundDuration;
    [SerializeField]
    GameObject _enemiesToCrush;
    List<CutsceneGlhosts> _GlhostsUnderMe;
    Vector3 _ogCamPos;
    bool _cameraInPosition = false;
    bool _fallFinished = false;
    bool _turnToPlayerFinished = false;
    bool _glhostsCrushed = false;
    

    [Header("Trap Boss Variables")]
    [SerializeField]
    float _timeBetweenAttacks;
    float _realTimeBetweenAttacks;
    [SerializeField]
    float _stunnedDuration;
    float _realStunnedDuration;
    [SerializeField]
    float _vertDetectOffset;
    [SerializeField]
    float _startAngle;
    [SerializeField]
    float _detectionAngle;
    [SerializeField]
    int _numOfCasts;
    float _calcAngle;
    [SerializeField]
    float _maxDistanceOut;
    [SerializeField]
    bool _debug;
    [SerializeField]
    float _detectDistance;
    public bool trapComplete = false;
    RaycastHit hit;

    [Header("Fire Trap Percentages")]
    [SerializeField]
    float _fireBeamPercentage;
    float _realFireBeamPercentage;
    [SerializeField]
    float _quadFirePercentage;
    float _realQuadFirePercentage;
    [SerializeField]
    float _fireSpinPercentage;
    float _realFireSpinPercentage;
    float _totalPercentageFireTrap;

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

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
    [SerializeField]
    float _pinballDetectionAngle;
    [SerializeField]
    int _pinballNumOfCasts;
    [SerializeField]
    float _speedWhilePinballing;
    float _realSpeedWhilePinballing;
    [SerializeField]
    float _pinballTellDuration;
    float _realPinballTellDuration;
    bool _tellPinballing;
    [SerializeField]
    float _pinballAttackDuration;
    float _realPinballAttackDuration;
    Vector3 _moveDir;

    float _startAttackTime;
    float _currAttackTime;

    [Header("Trap Boss Hard Variables")]
    [SerializeField]
    float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
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
        StartFight();
    }

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

    //called for Init, after the cutscene
    public override void Init()
    {
        if (!_hasInit)
        {
            base.Init();

            if (!_managerRef.HardModeOn)
            {
                _realFireBeamPercentage = _fireBeamPercentage;
                _realQuadFirePercentage = _quadFirePercentage;
                _realFireSpinPercentage = _fireSpinPercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realStunnedDuration = _stunnedDuration;
                _realFollowDuration = _followDuration;
                _realSpinningSpeed = _spinningSpeed;
                _realSpinAttackDuration = _spinAttackDuration;
                _realSpawnDelayDuration = _spawnDelayDuration;
                _realSpeedWhilePinballing = _speedWhilePinballing;
                _realPinballTellDuration = _pinballTellDuration;
                _realPinballAttackDuration = _pinballAttackDuration;
            }
            else
            {
                _realFireBeamPercentage = _hardFireBeamPercentage;
                _realQuadFirePercentage = _hardQuadFirePercentage;
                _realFireSpinPercentage = _hardFireSpinPercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realStunnedDuration = _hardStunnedDuration;
                _realFollowDuration = _hardFollowDuration;
                _realSpinningSpeed = _hardSpinningSpeed;
                _realSpinAttackDuration = _hardSpinAttackDuration;
                _realSpawnDelayDuration = _hardSpawnDelayDuration;
                _realSpeedWhilePinballing = _hardSpeedWhilePinballing;
                _realPinballTellDuration = _hardPinballTellDuration;
                _realPinballAttackDuration = _hardPinballAttackDuration;
            }

            _GlhostsUnderMe = new List<CutsceneGlhosts>();
            /*
            for (int i = 0; i < _enemiesToCrush.transform.childCount; i++)
            {
                _GlhostsUnderMe.Add(_enemiesToCrush.transform.GetChild(i).GetComponent<CutsceneGlhosts>());
            }
            */
        }

        _ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _totalPercentageFireTrap = _realFireBeamPercentage + _realQuadFirePercentage + _realFireSpinPercentage;

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
                            
                        case TRAPSTRATS.FIREBEAM:
                            ShootFireBeam();
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

    //decides what the attack the boss will do next
    private void WhatDoNext()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realTimeBetweenAttacks;
        //Debug.Log("thinking");

        if (_currAttackTime >= 1)
        {
            float _nextAttack = Random.Range(0, _totalPercentageFireTrap);

            if (_nextAttack > 0 && _nextAttack <= _realFireBeamPercentage)
            {
                //Debug.Log("Beam Chosen");
                _startAttackTime = Time.time;
                _MyAttack = TRAPSTRATS.FIREBEAM;
            }
            else
            {
                //Debug.Log("quad Fire Chosen");

                _enemyAgent.enabled = false;
                _startAttackTime = Time.time;
                _startSpawnTime = Time.time;
                _MyAttack = TRAPSTRATS.QUADFIRE;
            }
        }
    }

    //Find Trap
    //Boss will find and go towards a trap
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
        Debug.DrawRay(transform.position + Vector3.up, this.transform.forward);
        if (Physics.Raycast(transform.position + Vector3.up, this.transform.forward, out hit, _detectDistance))
        {
            
            GameObject hitObject = hit.collider.gameObject;
           
            if (hitObject == currentTrap)
            {
                BossFireStatueTrap possessedTrap = hitObject.GetComponent<Transform>().GetChild(0).GetComponent<BossFireStatueTrap>();
                possessedTrap.StartingDelay();
                possessedTrap.bossEntity = this.gameObject;
                possessedTrap._beginningDelay = _trapStartDelay;
                trapComplete = false;
                _MyAttack = TRAPSTRATS.INSIDETRAP;
            }
        }
        
    }

    //Attack
    //Boss will spin in place
    //Boss will spawn and shoot ghosts out in random directions
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

    private void ShootFireBeam()
    {

    }

    private void QuadFireBeams()
    {

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
            _enemiesToCrush.SetActive(true);
            for (int i = 0; i < _GlhostsUnderMe.Count; i++)
            {
                _GlhostsUnderMe[i].ResetCutscene();
            }
            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);
            _cameraInPosition = false;
            _fallFinished = false;
            _turnToPlayerFinished = false;
            _glhostsCrushed = false;

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
