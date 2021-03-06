﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MiniSpinBoss : BossEnemy
{

    protected enum SPINSTRATS
    {
        DOWNTIME,
        FOLLOW,
        PINBALL,
        STUNNED
    }

    [Header("Spin Boss Intro Variables")]
    [SerializeField]
    float _revUpDuration;
    [SerializeField]
    float _spinDuration;
    Vector3 _ogCamPos;
    bool _cameraInPosition = false;
    bool _spinAroundStart = false;
    bool _spinAroundScreen = false;
    bool _spinAroundEnd = false;

    [Header("Spin Boss Variables")]
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
    bool _debug;

    [Header("Percentages of Attacks")]
    [SerializeField]
    float _followPercentage;
    float _realFollowPercentage;
    [SerializeField]
    float _pinballAttackPercentage;
    float _realPinballAttackPercentage;
    float _totalPercentage;

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Pinball Variables")]
    [SerializeField]
    float _spinningSpeed;
    float _realSpinningSpeed;
    float _currSpinningSpeed;
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

    [Header("Spin Boss Hard Variables")]
    [SerializeField]
    float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
    float _hardFollowPercentage;
    [SerializeField]
    float _hardPinballAttackPercentage;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;

    [Header("Hard Pinball Variables")]
    [SerializeField]
    float _hardSpinningSpeed;
    [SerializeField]
    float _hardSpeedWhilePinballing;
    [SerializeField]
    float _hardPinballTellDuration;
    [SerializeField]
    float _hardPinballAttackDuration;

    protected GameObject _myBody;
    protected SkinnedMeshRenderer _mySkinRenderer;

    SPINSTRATS _MyAttack = SPINSTRATS.FOLLOW;

    protected override void PlayIntro()
    {
        if(!_cameraInPosition)
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
        else if(!_spinAroundStart)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _revUpDuration;

            _currSpinningSpeed = _realSpinningSpeed * _currAttackTime;

            transform.Rotate(Vector3.up, _currSpinningSpeed);

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                _currSpinningSpeed = _realSpinningSpeed;
                _startAttackTime = Time.time;
                _spinAroundStart = true;
            }
        }
        else if(!_spinAroundScreen)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _spinDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;
                
                _startAttackTime = Time.time;
                _spinAroundScreen = true;
            }
            transform.Rotate(Vector3.up, _realSpinningSpeed);
        }
        else if(!_spinAroundEnd)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _revUpDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;

                _currSpinningSpeed = 0;
                _startAttackTime = Time.time;
                transform.LookAt(_playerRef.transform.position);
                _spinAroundEnd = true;
            }

            _currSpinningSpeed = _realSpinningSpeed * ( 1 -_currAttackTime);

            transform.Rotate(Vector3.up, _currSpinningSpeed);

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;

                _currSpinningSpeed = 0;
                _startAttackTime = Time.time;
                transform.LookAt(_playerRef.transform.position);
                _spinAroundEnd = true;
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
    }

    public override void Init()
    {
        if(!_hasInit)
        {
            _startPos = transform.position;
            _startRot = transform.rotation;
            _myBody = transform.GetChild(2).gameObject;
            _mySkinRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
            _myMaterial = _mySkinRenderer.materials[1];
            _myColor = _myMaterial.color;
            _menuRef = Menuing.Instance;
            _playerRef = PlayerController.Instance;
            _managerRef = GameManager.Instance;
            _cameraRef = _playerRef.GetCamera;
            _camOffset = _cameraRef.GetOffset;

            _enemyAgent = GetComponent<NavMeshAgent>();
            _enemyAgent.enabled = false;

            if (!_managerRef.HardModeOn)
            {
                _actualMaxHealth = _maxBossHealth;
                _actualBossSpeed = _bossDefaultSpeed;
                _realFollowPercentage = _followPercentage;
                _realPinballAttackPercentage = _pinballAttackPercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realStunnedDuration = _stunnedDuration;
                _realFollowDuration = _followDuration;
                _realSpinningSpeed = _spinningSpeed;
                _realSpeedWhilePinballing = _speedWhilePinballing;
                _realPinballTellDuration = _pinballTellDuration;
                _realPinballAttackDuration = _pinballAttackDuration;
            }
            else
            {
                _actualMaxHealth = _maxHardBossHealth;
                _actualBossSpeed = _bossHardDefaultSpeed;
                _realFollowPercentage = _hardFollowPercentage;
                _realPinballAttackPercentage = _hardPinballAttackPercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realStunnedDuration = _hardStunnedDuration;
                _realFollowDuration = _hardFollowDuration;
                _realSpinningSpeed = _hardSpinningSpeed;
                _realSpeedWhilePinballing = _hardSpeedWhilePinballing;
                _realPinballTellDuration = _hardPinballTellDuration;
                _realPinballAttackDuration = _hardPinballAttackDuration;
            }

            _currBossHealth = _actualMaxHealth;
            _currSpeed = _actualBossSpeed;

            _bossBar = _menuRef.GetBossBar;
            _laggedBossHealthBar = _bossBar.transform.GetChild(1).gameObject.GetComponent<Image>();
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar = _bossBar.transform.GetChild(2).gameObject.GetComponent<Image>();
            _actualBossHealthBar.fillAmount = 1;
            _bossNameText = _bossBar.transform.GetChild(3).gameObject.GetComponent<Text>();
            _bossNameText.text = _bossName;
            if (_managerRef.HasSubtitles)
            {
                _bossNameText.text += ", " + _bossSubTitle;
            }
            _hasInit = true;
        }
        
        _ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _totalPercentage = _realFollowPercentage + _realPinballAttackPercentage;

        _startAttackTime = Time.time;
        _myAI = BossAI.INTRO;
    }

    protected override void StartFight()
    {
        _bossBar.SetActive(true);
        _laggedBossHealthBar.fillAmount = 1;
        _actualBossHealthBar.fillAmount = 1;

        _enemyAgent.enabled = true;
        _cameraRef.AmFollowingPlayer = true;
        _calcAngle = _startAngle;

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
                        case SPINSTRATS.DOWNTIME:
                            WhatDoNext();
                            break;
                        case SPINSTRATS.FOLLOW:
                            FollowPlayer();
                            break;
                        case SPINSTRATS.PINBALL:
                            PlaySomePinball();
                            break;
                        case SPINSTRATS.STUNNED:
                            Stunned();
                            break;
                        default:
                           // Debug.Log("No Attack Set");
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

    private void WhatDoNext()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realTimeBetweenAttacks;
        //Debug.Log("thinking");

        if (_currAttackTime >= 1)
        {
            float _nextAttack = Random.Range(0, _totalPercentage);

            if (_nextAttack > 0 && _nextAttack <= _realFollowPercentage)
            {
                //Debug.Log("follow Chosen");
                _startAttackTime = Time.time;
                _MyAttack = SPINSTRATS.FOLLOW;
            }
            else
            {
                //Debug.Log("pinball Chosen");
                _tellPinballing = true;
                _enemyAgent.enabled = false;
                _startAttackTime = Time.time;
                _MyAttack = SPINSTRATS.PINBALL;
            }
        }
    }
    
    private void FollowPlayer()
    {
        _enemyAgent.SetDestination(_playerRef.transform.position);

        _currAttackTime = (Time.time - _startAttackTime) / _realFollowDuration;

        //Debug.Log("following");

        if (_currAttackTime >= 1)
        {
            _enemyAgent.SetDestination(transform.position);

            _MyAttack = SPINSTRATS.DOWNTIME;
            _startAttackTime = Time.time;
        }

        for (int i = 0; i <= _numOfCasts; i++)
        {
            float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;
            float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;

            Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);

            if (_debug)
            {
                Debug.DrawLine(transform.position + (Vector3.up * _vertDetectOffset), transform.position + (Vector3.up * _vertDetectOffset) + (RayDir * _bossCollisionDetectDistance), Color.red);
            }

            _calcAngle += _detectionAngle / _numOfCasts;

            if(Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
            {
                if(hit.collider.GetComponent<PlayerController>())
                {
                    hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                }
            }
        }

        _calcAngle = _startAngle;
    }

    
    private void PlaySomePinball()
    {
        if(_tellPinballing)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _realPinballTellDuration;
            
            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;
                _tellPinballing = false;
                _moveDir = (_playerRef.transform.position - transform.position).normalized;
                _moveDir.y = 0;
                _startAttackTime = Time.time;
            } 

            _currSpinningSpeed = _realSpinningSpeed * _currAttackTime;

            transform.Rotate(Vector3.up, _currSpinningSpeed);
        }
        else
        {
            _currAttackTime = (Time.time - _startAttackTime) / _realPinballAttackDuration;

            //Debug.Log("pinballing");

            for (int i = 0; i <= _pinballNumOfCasts; i++)
            {
                float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;
                float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;

                Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);

                if (_debug)
                {
                    Debug.DrawLine(transform.position + (Vector3.up * _vertDetectOffset), transform.position + (Vector3.up * _vertDetectOffset) + (RayDir * _bossCollisionDetectDistance), Color.red);
                }

                _calcAngle += _pinballDetectionAngle / _pinballNumOfCasts;

                if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
                {
                    if (hit.collider.GetComponent<PlayerController>())
                    {
                        hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                    }
                }
            }

            if (_debug)
            {
                Debug.DrawLine(transform.position + (Vector3.up * _vertDetectOffset), transform.position + (Vector3.up * _vertDetectOffset) + (_moveDir * _bossCollisionDetectDistance), Color.blue);
            }

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), _moveDir,out hit, _bossCollisionDetectDistance +1f))
            {
                Vector3 checkDir = _moveDir;
                GameObject thingHit = hit.collider.gameObject;

                //Debug.Log("reflected");
                if(!thingHit.GetComponent<BaseEnemy>() && !thingHit.GetComponent<BossEnemy>() && !thingHit.GetComponent<PlayerController>())
                {
                    _moveDir = Vector3.Reflect(checkDir, hit.normal);
                }
            }

            _calcAngle = 0;

            transform.Rotate(Vector3.up, _realSpinningSpeed);
            transform.position += _moveDir * _realSpeedWhilePinballing * Time.deltaTime;

            if (_currAttackTime >= 1)
            {
                _invincible = false;
                _enemyAgent.enabled = true;
                _MyAttack = SPINSTRATS.STUNNED;
                _startAttackTime = Time.time;
                return;
            }
        }

        
    }

    private void Stunned()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realStunnedDuration;

        //Debug.Log("stunned");

        if (_currAttackTime >= 1)
        {
            _MyAttack = SPINSTRATS.DOWNTIME;
            _startAttackTime = Time.time;
        }

    }

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
        else if(!_showingDeath)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _deathDuration;
            //Debug.Log("showing death");


            if (_currAttackTime > 1)
            {
                _currAttackTime = 1;

                cam0 = _cameraRef.transform.position;
                cam1 = _ogCamPos;

                _mySkinRenderer.enabled = false;

                _startAttackTime = Time.time;
                _showingDeath = true;
            }
            
            _myColor.a = 1 - _currAttackTime;
            _myMaterial.color = _myColor;
            _mySkinRenderer.materials[1] = _myMaterial;
            _mySkinRenderer.materials[0] = _myMaterial;
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
                gameObject.SetActive(false);
            }
        }
    }


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
            _spinAroundStart = false;
            _spinAroundScreen = false;
            _spinAroundEnd = false;

            _endingPlaying = false;
            _laggingHealth = false;
            _updatingHealth = false;
            _dead = false;
            _amHit = false;
            _invincible = false;

            _myAI = BossAI.NONE;
            _MyAttack = SPINSTRATS.FOLLOW;
            _init = false;
        }
    }
}
