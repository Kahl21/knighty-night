using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SecretBoss : BossEnemy {

    protected enum SECRETSTRATS
    {
        DOWNTIME,
        FOLLOW,
        FLAME,
        DASHFLAME,
        CHOSENLIGHT,
        STUNNED
    }

    //[Header("Secret Boss Intro Variables")]
    bool _cameraInPosition = false;


    [Header("Secret Boss Variables")]
    [Tooltip("Flame Wave Prefab")]
    [SerializeField]
    GameObject _BWPrefab;
    [Tooltip("Chosen Light Prefab")]
    [SerializeField]
    GameObject _ChosenLightPrefab;
    float _XMin, _XMax, _ZMin, _ZMax;
    [Tooltip("Time to wait before the next ability is used (must be >0)")]
    [SerializeField]
    float _timeBetweenAttacks;
    float _realTimeBetweenAttacks;
    [Tooltip("How long the enemy will not do anything after an ability (must be >0)")]
    [SerializeField]
    float _stunnedDuration;
    float _realStunnedDuration;
    [Tooltip("How high up from the models origin to start detection")]
    [SerializeField]
    float _vertDetectOffset;
    [Tooltip("Angle to start the first raycast at")]
    [SerializeField]
    float _startAngle;
    [Tooltip("How many angles from the first raycast to cast the last raycast")]
    [SerializeField]
    float _detectionAngle;
    [Tooltip("how many raycasts total during detection")]
    [SerializeField]
    int _numOfCasts;
    float _calcAngle;
    [SerializeField]
    bool _debug;
    float _startAttackTime;
    float _currAttackTime;
    SECRETSTRATS _lastAttack = SECRETSTRATS.FOLLOW;
    SECRETSTRATS _myAttack = SECRETSTRATS.FOLLOW;

    [Header("Sword Variables")]
    [SerializeField]
    float _swordSlashDist;
    [SerializeField]
    float _swordCurveAngle;
    float _swordCalcAngle;
    [SerializeField]
    float _swordSwingDuration;
    float _currentSwingTime;
    float _SwingStartTime;
    RaycastHit _swordHit;
    float _swing;
    [SerializeField]
    float _numOfSwordCasts;
    [SerializeField]
    bool _debugSword;


    [Header("Percentages of Attacks")]
    [SerializeField]
    float _followPercentage;
    float _realFollowPercentage;
    [SerializeField]
    float _normalFlameAttackPercentage;
    float _realNormalFlameAttackPercentage;
    [SerializeField]
    float _dashFlameAttackPercentage;
    float _realDashFlameAttackPercentage;
    [SerializeField]
    float _chosenLightAttackPercentage;
    float _realChosenLightAttackPercentage;
    float _totalPercentage;

    [Header("Follow Player Varibales")]
    [Tooltip("(must be >0)")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Normal Flame Attack Variables")]
    [Tooltip("How many flames to spawn before the ability ends")]
    [SerializeField]
    int _howManyFlames;
    int _realHowManyFlames;
    int _currFlamesSpawned = 0;
    [Tooltip("Time in between flame spawns (must be >0)")]
    [SerializeField]
    float _flameSpawnOffset;
    float _realFlameSpawnOffset;
    [Tooltip("How much damage one flame does")]
    [SerializeField]
    float _flameDamage;
    float _realFlameDamage;
    [Tooltip("How fast one flame is")]
    [SerializeField]
    float _flameSpeed;
    float _realFlameSpeed;
    [Tooltip("How big one flame can get")]
    [SerializeField]
    float _flameMaxScale = 1;
    float _realFlameMaxScale;
    [Tooltip("How fast the flame reaches its max size")]
    [SerializeField]
    float _flameScaleSpeed;
    float _realFlameScaleSpeed;
    [Tooltip("How long the flame lives for")]
    [SerializeField]
    float _flameLifetime;
    float _realFlameLifetime;

    [Header("Dash Flame Attack Variables")]
    [SerializeField]
    ParticleSystem _dashParticle;
    [Tooltip("How many times the boss will dash before the ability ends")]
    [SerializeField]
    int _howManyDashes;
    int _realHowManyDashes;
    float _currDashTime;
    [Tooltip("How long to wait inbetween dashes (must be >0)")]
    [SerializeField]
    float _timeBetweenDashes;
    float _realTimeBetweenDashes;
    [Tooltip("how steep the angle of a dash will be")]
    [SerializeField]
    float _dashAngle;
    float _whichDirection;
    Vector3 _moveDir, c0, c1;
    [Tooltip("how long it will take for one dash to finish (must be >0)")]
    [SerializeField]
    float _dashDuration;
    float _realDashDuration;
    [Tooltip("How far the enemy will dash during one dash")]
    [SerializeField]
    float _dashDistance;
    float _realDashDistance;
    [Tooltip("How much damage the dash flame will do")]
    [SerializeField]
    float _dashFlameDamage;
    float _realDashFlameDamage;
    [Tooltip("How fast the dash flame is")]
    [SerializeField]
    float _dashFlameSpeed;
    float _realDashFlameSpeed;
    [Tooltip("The max size of a spawned dash flame")]
    [SerializeField]
    float _dashFlameMaxScale = 1;
    float _realDashFlameMaxScale;
    [Tooltip("How fast the dash flame size will increase")]
    [SerializeField]
    float _dashFlameScaleSpeed;
    float _realDashFlameScaleSpeed;
    [Tooltip("How long a dash flame will last in this ability")]
    [SerializeField]
    float _dashFlameLifetime;
    float _realDashFlameLifetime;

    [Header("Chosen Light Attack Variables")]
    [Tooltip("How many lightnings to spawn until the ability stops")]
    [SerializeField]
    float _numberOfSmitingLights;
    float _realNumberOfSmitingLights;
    [Tooltip("How many lightnings until one targets the player  EX) 'every X lightning will spawn above the player'")]
    [SerializeField]
    float _cheatTargetPlayerOffset;
    float _realCheatTargetPlayerOffset;
    [Tooltip("How much damage the lightning does")]
    [SerializeField]
    float _lightDamage;
    float _realLightDamage;
    [Tooltip("Time in between lightning spawns (must be >0)")]
    [SerializeField]
    float _lightSpawnOffset;
    float _realLightSpawnOffset;
    [Tooltip("How high up in the air the lightning starts at")]
    [SerializeField]
    float _lightVertSpawnOffset;
    [Tooltip("How fast the lightning falls")]
    [SerializeField]
    float _lightFallSpeed;
    float _realLightFallSpeed;

    [Header("Secret Boss Hard Variables")]
    [SerializeField]
    float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
    float _hardFollowPercentage;
    [SerializeField]
    float _hardNormalFlameAttackPercentage;
    [SerializeField]
    float _hardDashFlameAttackPercentage;
    [SerializeField]
    float _hardChosenLightAttackPercentage;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;

    [Header("Hard Normal Flame Attack Variables")]
    [SerializeField]
    int _hardHowManyFlames;
    [SerializeField]
    float _hardFlameSpawnOffset;
    [SerializeField]
    float _hardFlameDamage;
    [SerializeField]
    float _hardFlameSpeed;
    [SerializeField]
    float _hardFlameMaxScale = 1;
    [SerializeField]
    float _hardFlameScaleSpeed;
    [SerializeField]
    float _hardFlameLifetime;

    [Header("Hard Dash Flame Attack Variables")]
    [SerializeField]
    int _hardHowManyDashes;
    [SerializeField]
    float _hardTimeBetweenDashes;
    [SerializeField]
    float _hardDashDuration;
    [SerializeField]
    float _hardDashDistance;
    [SerializeField]
    float _hardDashFlameDamage;
    [SerializeField]
    float _hardDashFlameSpeed;
    [SerializeField]
    float _hardDashFlameMaxScale = 1;
    [SerializeField]
    float _hardDashFlameScaleSpeed;
    [SerializeField]
    float _hardDashFlameLifetime;

    [Header("Hard Chosen Light Attack Variables")]
    [SerializeField]
    float _hardNumberOfSmitingLights;
    [SerializeField]
    float _hardCheatTargetPlayerOffset;
    [SerializeField]
    float _hardLightDamage;
    [SerializeField]
    float _hardLightSpawnOffset;
    [SerializeField]
    float _hardLightFallSpeed;

    [Header("Sound Variables")]
    [SerializeField]
    AudioClip swingClip;
    [SerializeField]
    AudioClip fireClip;
    [SerializeField]
    AudioClip shootClip;
    [SerializeField]
    AudioClip lightningClip;
    [SerializeField]
    AudioClip hitClip;
    [SerializeField]
    AudioClip deathClip;


    protected override void Awake()
    {
        base.Awake();
        _dashParticle.Stop();
    }

    //function that is called once the player enters the room
    //plays the intro cutscene
    protected override void PlayIntro()
    {
        _playerRef.AmInCutscene = false;
        _cameraRef.AmFollowingPlayer = true;
        StartFight();
    }

    //called for Init, after the cutscene
    public override void Init()
    {
        if (!_hasInit)
        {
            _startPos = transform.position;
            _startRot = transform.rotation;
            _menuRef = Menuing.Instance;
            _playerRef = PlayerController.Instance;
            _managerRef = GameManager.Instance;
            _myAnimations = GetComponent<Animator>();
            _cameraRef = _playerRef.GetCamera;
            _camOffset = _cameraRef.GetOffset;

            _enemyAgent = GetComponent<NavMeshAgent>();
            _enemyAgent.enabled = false;

            if (!_managerRef.HardModeOn)
            {
                _actualMaxHealth = _maxBossHealth;
                _actualBossSpeed = _bossDefaultSpeed;
            }
            else
            {
                _actualMaxHealth = _maxHardBossHealth;
                _actualBossSpeed = _bossHardDefaultSpeed;
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

            _Audio = GameObject.Find("AudioManager");
            _audioManager = _Audio.GetComponent<AudioManager>();
            volSFX = _audioManager.volSFX;


            _XMin = (_myRoom.transform.position.x - (_myRoom.transform.localScale.x * 5f));     //set left wall of spawn
            _ZMin = (_myRoom.transform.position.z - (_myRoom.transform.localScale.z * 5f));     //set back wall of spawn
            _XMax = (_myRoom.transform.position.x + (_myRoom.transform.localScale.x * 5f));     //set right wall of spawn
            _ZMax = (_myRoom.transform.position.z + (_myRoom.transform.localScale.z * 5f));     //set back wall of spawn

            if (!_managerRef.HardModeOn)
            {
                _realFollowPercentage = _followPercentage;
                _realNormalFlameAttackPercentage = _normalFlameAttackPercentage;
                _realDashFlameAttackPercentage = _dashFlameAttackPercentage;
                _realChosenLightAttackPercentage = _chosenLightAttackPercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realStunnedDuration = _stunnedDuration;
                _realFollowDuration = _followDuration;
                _realHowManyFlames = _howManyFlames;
                _realFlameDamage = _flameDamage;
                _realFlameSpawnOffset = _flameSpawnOffset;
                _realFlameSpeed = _flameSpeed;
                _realFlameMaxScale = _flameMaxScale;
                _realFlameScaleSpeed = _flameScaleSpeed;
                _realFlameLifetime = _flameLifetime;
                _realHowManyDashes = _howManyDashes;
                _realTimeBetweenDashes = _timeBetweenDashes;
                _realDashDistance = _dashDistance;
                _realDashDuration = _dashDuration;
                _realDashFlameDamage = _dashFlameDamage;
                _realDashFlameSpeed = _dashFlameSpeed;
                _realDashFlameMaxScale = _dashFlameMaxScale;
                _realDashFlameScaleSpeed = _dashFlameScaleSpeed;
                _realDashFlameLifetime = _dashFlameLifetime;
                _realNumberOfSmitingLights = _numberOfSmitingLights;
                _realCheatTargetPlayerOffset = _cheatTargetPlayerOffset;
                _realLightDamage = _lightDamage;
                _realLightSpawnOffset = _lightSpawnOffset;
                _realLightFallSpeed = _lightFallSpeed;
            }
            else
            {
                _realFollowPercentage = _hardFollowPercentage;
                _realNormalFlameAttackPercentage = _hardNormalFlameAttackPercentage;
                _realDashFlameAttackPercentage = _hardDashFlameAttackPercentage;
                _realChosenLightAttackPercentage = _hardChosenLightAttackPercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realStunnedDuration = _hardStunnedDuration;
                _realFollowDuration = _hardFollowDuration;
                _realHowManyFlames = _hardHowManyFlames;
                _realFlameDamage = _hardFlameDamage;
                _realFlameSpawnOffset = _hardFlameSpawnOffset;
                _realFlameSpeed = _hardFlameSpeed;
                _realFlameMaxScale = _hardFlameMaxScale;
                _realFlameScaleSpeed = _hardFlameScaleSpeed;
                _realFlameLifetime = _hardFlameLifetime;
                _realHowManyDashes = _hardHowManyDashes;
                _realTimeBetweenDashes = _hardTimeBetweenDashes;
                _realDashDistance = _hardDashDistance;
                _realDashDuration = _hardDashDuration;
                _realDashFlameDamage = _hardDashFlameDamage;
                _realDashFlameSpeed = _hardDashFlameSpeed;
                _realDashFlameMaxScale = _hardDashFlameMaxScale;
                _realDashFlameScaleSpeed = _hardDashFlameScaleSpeed;
                _realDashFlameLifetime = _hardDashFlameLifetime;
                _realNumberOfSmitingLights = _hardNumberOfSmitingLights;
                _realCheatTargetPlayerOffset = _hardCheatTargetPlayerOffset;
                _realLightDamage = _hardLightDamage;
                _realLightSpawnOffset = _hardLightSpawnOffset;
                _realLightFallSpeed = _hardLightFallSpeed;
            }

            _hasInit = true;
        }

        _ogCamPos = _cameraRef.transform.position;
        _ogCamRot = _cameraRef.transform.localEulerAngles;
        cam0 = _cameraRef.transform.position;
        cam1 = _bossCameraPos;
        rot0 = _cameraRef.transform.localEulerAngles;
        rot1 = _bossCameraRot;
        _cameraRef.AmFollowingPlayer = false;

        _speaker = this.transform.GetComponent<AudioSource>();

        _totalPercentage = _realFollowPercentage + _realNormalFlameAttackPercentage + _realDashFlameAttackPercentage + _realChosenLightAttackPercentage;

        _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);
        
        _myAI = BossAI.INTRO;
    }

    //called when init and cutscene are done
    //starts fight
    protected override void StartFight()
    {
        if (!_init)
        {
            _bossBar.SetActive(true);
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _enemyAgent.enabled = true;
            _cameraRef.AmFollowingPlayer = true;
            _calcAngle = _startAngle;
            _startAttackTime = Time.time;

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
                    switch (_myAttack)
                    {
                        case SECRETSTRATS.DOWNTIME:
                            WhatDoNext();
                            break;
                        case SECRETSTRATS.FOLLOW:
                            FollowPlayer();
                            break;
                        case SECRETSTRATS.FLAME:
                            UseFlameAttack();
                            break;
                        case SECRETSTRATS.DASHFLAME:
                            DashWithFlames();
                            break;
                        case SECRETSTRATS.CHOSENLIGHT:
                            ALLKNOWINGDESTRUCTION();
                            break;
                        case SECRETSTRATS.STUNNED:
                            Stunned();
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
            float _nextAttack = Random.Range(0, _totalPercentage);

            if (_nextAttack > 0 && _nextAttack <= _realFollowPercentage)
            {
                if(_lastAttack != SECRETSTRATS.FOLLOW)
                {
                    //Debug.Log("follow Chosen");
                    _myAnimations.Play("Moving", 0);
                    _swordCalcAngle = 0;
                    _startAttackTime = Time.time;
                    _SwingStartTime = Time.time;
                    _myAttack = SECRETSTRATS.FOLLOW;
                    _lastAttack = SECRETSTRATS.FOLLOW;
                }
                else
                {
                    return;
                }
            }
            else if(_nextAttack > _realFollowPercentage && _nextAttack <= (_realFollowPercentage + _realNormalFlameAttackPercentage))
            {
                if (_lastAttack != SECRETSTRATS.FLAME)
                {
                    _enemyAgent.enabled = false;
                    _myAnimations.Play("SwordSwing", 0);
                    _currFlamesSpawned = 0;
                    _startAttackTime = Time.time;
                    _myAttack = SECRETSTRATS.FLAME;
                    _lastAttack = SECRETSTRATS.FLAME;
                }
                else
                {
                    return;
                }
            }
            else if (_nextAttack > (_realFollowPercentage + _realNormalFlameAttackPercentage) && _nextAttack <= (_realFollowPercentage + _realNormalFlameAttackPercentage + _realDashFlameAttackPercentage))
            {
                if (_lastAttack != SECRETSTRATS.DASHFLAME)
                {
                    //Debug.Log("spin Chosen");
                    _currFlamesSpawned = 0;

                    _enemyAgent.enabled = false;

                    if (transform.position.x < _playerRef.transform.position.x)
                    {
                        _moveDir = (transform.forward + (transform.right * (_dashAngle / 100)));
                    }
                    else
                    {
                        _moveDir = (transform.forward - (transform.right * (_dashAngle / 100)));
                    }

                    c0 = transform.position;
                    c1 = transform.position + (_moveDir * _dashDistance);

                    _dashParticle.Play();
                    _startAttackTime = Time.time;
                    _myAttack = SECRETSTRATS.DASHFLAME;
                    _lastAttack = SECRETSTRATS.DASHFLAME;
                }
                else
                {
                    return;
                }

            }
            else
            {
                if (_lastAttack != SECRETSTRATS.CHOSENLIGHT)
                {
                    _currFlamesSpawned = 0;
                    //Debug.Log("pinball Chosen");
                    _enemyAgent.enabled = false;
                    _startAttackTime = Time.time;
                    _invincible = true;

                    _myAnimations.Play("Invincible", 0);
                    _myAttack = SECRETSTRATS.CHOSENLIGHT;
                    _lastAttack = SECRETSTRATS.CHOSENLIGHT;
                }
                else
                {
                    return;
                }

            }
        }
    }

    //Attack
    //Enemy just follows the player for a certain amount of time
    private void FollowPlayer()
    {
        _enemyAgent.SetDestination(_playerRef.transform.position);

        _currAttackTime = (Time.time - _startAttackTime) / _realFollowDuration;

        //Debug.Log("following");

        if (_currAttackTime >= 1)
        {
            _enemyAgent.SetDestination(transform.position);

            _myAttack = SECRETSTRATS.DOWNTIME;
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

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
            {
                if (hit.collider.GetComponent<PlayerController>())
                {
                    hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                }
            }
        }
        Swinging();
        _calcAngle = _startAngle;
    }

    private void Swinging()
    {
        _currentSwingTime = (Time.time - _SwingStartTime) / _swordSwingDuration;

        if (_currentSwingTime >= 1)
        {
            _currentSwingTime = 1;
            _swordCalcAngle = 0;
            _SwingStartTime = Time.time;
            _speaker.PlayOneShot(swingClip, volSFX);
            _myAnimations.Play("SwordSwing", 0);
        }
        else
        {
            float Xpos = Mathf.Cos(_swordCalcAngle * Mathf.Deg2Rad) * _swordSlashDist;
            float Zpos = Mathf.Sin(_swordCalcAngle * Mathf.Deg2Rad) * _swordSlashDist;

            Vector3 SwingDir = (transform.forward * Zpos) + (transform.right * Xpos);

            _swordCalcAngle += _swordCurveAngle / _numOfSwordCasts;

            if (_debug)
            {
                Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + SwingDir, Color.blue);
            }

            if (Physics.Raycast(transform.position + Vector3.up, SwingDir, out _swordHit, _swordSlashDist))
            {
                GameObject thingHit = _swordHit.collider.gameObject;

                if (thingHit.GetComponent<PlayerController>())
                {
                    thingHit.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                }
            }
        }
    }

    private void UseFlameAttack()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realFlameSpawnOffset;

        if(_currAttackTime >= 1)
        {
            _currAttackTime = 1;

            GameObject _newWave = Instantiate<GameObject>(_BWPrefab, transform.position + Vector3.up, transform.rotation);
            _newWave.GetComponent<BacklashWave>().Init(_realFlameDamage, _realFlameSpeed, _realFlameMaxScale, _realFlameScaleSpeed, _realFlameLifetime);
            _speaker.PlayOneShot(fireClip, volSFX);
            _myAnimations.Play("SwordSwing", 0);
            _currFlamesSpawned++;
            if(_currFlamesSpawned>=_realHowManyFlames)
            {
                _currFlamesSpawned = 0;
                _myAttack = SECRETSTRATS.STUNNED;
                _startAttackTime = Time.time;
                return;
            }

            _startAttackTime = Time.time;
        }

        Vector3 _playerPos = _playerRef.transform.position;
        _playerPos.y = transform.position.y;
        transform.LookAt(_playerPos);
    }

    private void DashWithFlames()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _timeBetweenDashes;
        _currDashTime = (Time.time - _startAttackTime) / _dashDuration;


        if (_debug)
        {
            Debug.DrawLine(transform.position, c1, Color.green);
            Debug.DrawRay(transform.position, transform.position + _moveDir * _dashDistance);
        }

        if (Physics.Raycast(transform.position + (transform.forward), _moveDir, out hit, _bossCollisionDetectDistance))
        {
            if(hit.collider.GetComponent<BossWall>() || hit.collider.GetComponent<DoorMovement>())
            {
                c1 = transform.position;
                //_currAttackTime = 1;
                _currDashTime = 1;
            }
        }

        if (_currDashTime >= 1 && _currAttackTime >= 1)
        {
            _currAttackTime = 1;
            _currDashTime = 1;

            GameObject _newWave = Instantiate<GameObject>(_BWPrefab, transform.position + Vector3.up, transform.rotation);
            _newWave.GetComponent<BacklashWave>().Init(_realDashFlameDamage, _realDashFlameSpeed, _realDashFlameMaxScale, _realDashFlameScaleSpeed, _realDashFlameLifetime);
            _speaker.PlayOneShot(fireClip, volSFX);

            _myAnimations.Play("SwordSwing", 0);
            _currFlamesSpawned++;
            if (_currFlamesSpawned >= _realHowManyDashes)
            {
                _currFlamesSpawned = 0;
                _dashParticle.Stop();
                _myAttack = SECRETSTRATS.STUNNED;
                _startAttackTime = Time.time;
                return;
            }
            
            if (transform.position.x < _playerRef.transform.position.x)
            {
                _moveDir = (transform.forward + (transform.right * (_dashAngle / 100)));
            }
            else
            {
                _moveDir = (transform.forward - (transform.right * (_dashAngle / 100)));
            }

            c0 = transform.position;
            c1 = transform.position + (_moveDir * _dashDistance);

            _startAttackTime = Time.time;
        }
        else if(_currDashTime < 1)
        {
            Debug.Log("dashing");
            Vector3 _playerPos = _playerRef.transform.position;
            _playerPos.y = transform.position.y;
            transform.LookAt(_playerPos);

            Vector3 c01;

            c01 = (1 - _currDashTime) * c0 + _currDashTime * c1;
            
            transform.position = c01;
        }
    }

    private void ALLKNOWINGDESTRUCTION()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realLightSpawnOffset;

        if (_currAttackTime >= 1)
        {
            _currAttackTime = 1;
            Vector3 lightPos = Vector3.zero;
            if(_currFlamesSpawned%_realCheatTargetPlayerOffset==0)
            {
                lightPos.x = _playerRef.transform.position.x;
                lightPos.z = _playerRef.transform.position.z;
            }
            else
            {
                lightPos.x = Random.Range(_XMin, _XMax);
                lightPos.z = Random.Range(_ZMin, _ZMax);
            }

            lightPos.y = _lightVertSpawnOffset;

            GameObject _newLight = Instantiate<GameObject>(_ChosenLightPrefab, lightPos, transform.rotation);
            _newLight.GetComponent<ChosenLight>().Init(_realLightDamage, _realLightFallSpeed);
            _speaker.PlayOneShot(lightningClip, volSFX);
            _currFlamesSpawned++;
            if (_currFlamesSpawned >= _realNumberOfSmitingLights)
            {
                _currFlamesSpawned = 0;
                _myAnimations.Play("StandingIdle");
                _invincible = false;
                _myAttack = SECRETSTRATS.STUNNED;
                return;
            }

            _startAttackTime = Time.time;
        }
    }

    //once the boss is done with their attack
    //will do nothing for an amount of time
    private void Stunned()
    {
        //dazedParticle.SetActive(true);
        _currAttackTime = (Time.time - _startAttackTime) / _realStunnedDuration;

        


        //Debug.Log("stunned");

        if (_currAttackTime >= 1)
        {
            //dazedParticle.SetActive(false);
            _myAttack = SECRETSTRATS.DOWNTIME;

            _enemyAgent.enabled = true;
            _startAttackTime = Time.time;
        }

    }

    //function that is called when damage is taken
    //boss takes damage
    //boss enters invulnerable state
    public override void GotHit(float _damageTaken)
    {
        if (!_amHit && !_invincible)
        {
            if (_speaker.isPlaying)
                _speaker.PlayOneShot(hitClip, volSFX);
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

    //resets the invulerable state that the boss is in
    //after certain amount of time defined by "InvincibleDuration"
    protected override void ResetAmHit()
    {
        _currInvincibleTime = (Time.time - _startInvincibleTime) / _InvincibleDuration;

        if (_currInvincibleTime >= 1)
        {
            //Debug.Log("not invincible");
            _currInvincibleTime = 1;

            _amHit = false;
        }
    }

    //Function that tells that boss that it is dead and should start to play the ending cutscene
    protected override void Die()
    {
        _myRoom.CheckForEnd();

        _enemyAgent.enabled = false;

        _dashParticle.Stop();

        _playerRef.GoingToOutroCutscene();

        _ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _speaker.Stop();
        _speaker.PlayOneShot(deathClip, volSFX);

        _startAttackTime = Time.time;
        //_cameraInPosition = false;
        _endingPlaying = true;

        _myAI = BossAI.OUTRO;
    }

    //called whe the boss dies and the ending cutscene starts to play
    protected override void PlayEnd()
    {
        if (!_cameraInPosition)
        {
            _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;

                _startAttackTime = Time.time;
                _myAnimations.Play("Death", 0);
                _cameraInPosition = true;
            }

            Vector3 cam01;

            cam01 = (1 - _currAttackTime) * cam0 + _currAttackTime * cam1;

            _cameraRef.transform.position = cam01;
        }
        else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime>.9f)
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

    //Reset function
    public override void MyReset()
    {
        if (_init)
        {
            gameObject.SetActive(true);

            _enemyAgent.enabled = false;
            //Debug.Log("Boss Reset");
            transform.position = _startPos;
            transform.rotation = _startRot;
            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);

            _endingPlaying = false;
            _laggingHealth = false;
            _updatingHealth = false;
            _dead = false;
            _amHit = false;
            _invincible = false;

            _myAI = BossAI.NONE;
            _myAttack = SECRETSTRATS.FOLLOW;
            _init = false;
        }
    }

    //Various Getters and Setters

    public override bool AmHit { get { return _amHit; } }
    public override bool AmInvincible { get { return _invincible; } }
    public override Vector3 GetIntroPos { get { return _playerIntroPos.transform.position; } }
    public override DungeonMechanic SetMyRoom { get { return _myRoom; } set { _myRoom = value; } }
    public override float GetDamage { get { return _bossDamage; } }

}
