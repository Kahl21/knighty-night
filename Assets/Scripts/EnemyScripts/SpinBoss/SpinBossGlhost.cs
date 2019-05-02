using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SpinBossGlhost : BossEnemy
{
    //Strategy Enum for the Spin Boss
    protected enum SPINSTRATS
    {
        DOWNTIME,
        FOLLOW,
        SPIN,
        PINBALL,
        STUNNED
    }

    [Header("Spin Boss Intro Variables")]
    [SerializeField]
    Camera _additionalCam1;
    Vector3 _additionalPos1, _additionalRot1;
    [SerializeField]
    GameObject _enemiesToCrush;
    List<CutsceneGlhosts> _GlhostsUnderMe;
    bool _cameraInPosition = false;
    bool _animating = false;
    bool _animatingSecond = false;

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
    float _spinAttackPercentage;
    float _realSpinAttackPercentage;
    [SerializeField]
    float _pinballAttackPercentage;
    float _realPinballAttackPercentage;
    float _totalPercentage;

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Spawn Spin Variables")]
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
    bool _tellSpawning;

    [Header("Pinball Variables")]
    [SerializeField]
    float _pinballDetectionAngle;
    [SerializeField]
    int _pinballNumOfCasts;
    [SerializeField]
    float _speedWhilePinballing;
    float _realSpeedWhilePinballing;
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
    float _hardSpinAttackPercentage;
    [SerializeField]
    float _hardPinballAttackPercentage;

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
    float _hardPinballAttackDuration;

    [Header("Sound Variables")]
    public AudioClip bossSpin;
    public AudioClip bossDazed;
    public AudioClip bossDeath;

    GameObject dazedParticle;

    SPINSTRATS _MyAttack = SPINSTRATS.FOLLOW;
    

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

                _myAnimations.Play("BigIntro", 0);

                for (int i = 0; i < _GlhostsUnderMe.Count; i++)
                {
                    _GlhostsUnderMe[i].Init();
                }

                _mySkinRenderer.enabled = true;
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
            }
        }
        else if(!_animatingSecond)
        {
            if (_cameraRef.MoveCamera())
            {
                if(_myAnimations.IsInTransition(0))
                {
                    cam0 = _cameraRef.transform.position;
                    cam1 = _ogCamPos;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _ogCamRot;

                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);
                    _enemiesToCrush.SetActive(false);
                    _animatingSecond = true;
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
                _realFollowPercentage = _followPercentage;
                _realSpinAttackPercentage = _spinAttackPercentage;
                _realPinballAttackPercentage = _pinballAttackPercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realStunnedDuration = _stunnedDuration;
                _realFollowDuration = _followDuration;
                _realSpinningSpeed = _spinningSpeed;
                _realSpinAttackDuration = _spinAttackDuration;
                _realSpawnDelayDuration = _spawnDelayDuration;
                _realSpeedWhilePinballing = _speedWhilePinballing;
                _realPinballAttackDuration = _pinballAttackDuration;
            }
            else
            {
                _realFollowPercentage = _hardFollowPercentage;
                _realSpinAttackPercentage = _hardSpinAttackPercentage;
                _realPinballAttackPercentage = _hardPinballAttackPercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realStunnedDuration = _hardStunnedDuration;
                _realFollowDuration = _hardFollowDuration;
                _realSpinningSpeed = _hardSpinningSpeed;
                _realSpinAttackDuration = _hardSpinAttackDuration;
                _realSpawnDelayDuration = _hardSpawnDelayDuration;
                _realSpeedWhilePinballing = _hardSpeedWhilePinballing;
                _realPinballAttackDuration = _hardPinballAttackDuration;
            }

            _GlhostsUnderMe = new List<CutsceneGlhosts>();

            for (int i = 0; i < _enemiesToCrush.transform.childCount; i++)
            {
                _GlhostsUnderMe.Add(_enemiesToCrush.transform.GetChild(i).GetComponent<CutsceneGlhosts>());
            }
        }

        _ogCamPos = _cameraRef.transform.position;
        _ogCamRot = _cameraRef.transform.localEulerAngles;
        cam0 = _cameraRef.transform.position;
        cam1 = _bossCameraPos;
        rot0 = _cameraRef.transform.localEulerAngles;
        rot1 = _bossCameraRot;
        _cameraRef.AmFollowingPlayer = false;
        
        _mySkinRenderer.enabled = false;

        _speaker = this.transform.GetComponent<AudioSource>();

        dazedParticle = this.transform.GetChild(5).gameObject;
        dazedParticle.SetActive(false);


        _totalPercentage = _realFollowPercentage + _realSpinAttackPercentage + _realPinballAttackPercentage;

        _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

        _startAttackTime = Time.time;
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
                        case SPINSTRATS.DOWNTIME:
                            WhatDoNext();
                            break;
                        case SPINSTRATS.FOLLOW:
                            FollowPlayer();
                            break;
                        case SPINSTRATS.SPIN:
                            SpinInPlace();
                            break;
                        case SPINSTRATS.PINBALL:
                            PlaySomePinball();
                            break;
                        case SPINSTRATS.STUNNED:
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
                //Debug.Log("follow Chosen");
                _startAttackTime = Time.time;
                _myAnimations.Play("Movement", 0);
                _MyAttack = SPINSTRATS.FOLLOW;
            }
            else if (_nextAttack > _realFollowPercentage && _nextAttack <= (_realFollowPercentage + _realSpinAttackPercentage))
            {
                //Debug.Log("spin Chosen");

                _tellSpawning = true;
                _enemyAgent.enabled = false;
                _startAttackTime = Time.time;
                _startSpawnTime = Time.time;
                _myAnimations.Play("Struggle", 0);
                _MyAttack = SPINSTRATS.SPIN;
            }
            else
            {
                //Debug.Log("pinball Chosen");
                _tellPinballing = true;
                _enemyAgent.enabled = false;
                _startAttackTime = Time.time;
                _myAnimations.Play("SpinTell", 0);
                _MyAttack = SPINSTRATS.PINBALL;
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

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
            {
                if (hit.collider.GetComponent<PlayerController>())
                {
                    hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                }
            }
        }

        _calcAngle = _startAngle;
    }

    //Attack
    //Boss will spin in place
    //Boss will spawn and shoot ghosts out in random directions
    private void SpinInPlace()
    {
        if (_tellSpawning)
        {
            if (_myAnimations.IsInTransition(0))
            {
                _currAttackTime = 1;
                _tellSpawning = false;
                _moveDir = (_playerRef.transform.position - transform.position).normalized;
                _moveDir.y = 0;
                _startAttackTime = Time.time;
                _startSpawnTime = Time.time;
            }
        }
        else
        {
            _currAttackTime = (Time.time - _startAttackTime) / _realSpinAttackDuration;
            _currSpawnTime = (Time.time - _startSpawnTime) / _realSpawnDelayDuration;

            
            // _myAnimations.Play("Spin");

            for (int i = 0; i <= _numOfCasts; i++)
            {
                if (!_speaker.isPlaying)
                    _speaker.PlayOneShot(bossSpin, volSFX);

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

            _calcAngle = _startAngle;

            transform.Rotate(Vector3.up, _realSpinningSpeed);
            //Debug.Log("spinning");

            if (_currAttackTime >= 1)
            {
                //Debug.Log("spinning Done");
                _invincible = false;
                _enemyAgent.enabled = true;
                _MyAttack = SPINSTRATS.STUNNED;
                _myAnimations.Play("Dazed", 0);
                _startAttackTime = Time.time;
            }

            if (_currSpawnTime >= 1)
            {
                _currSpawnTime = 1;
                Vector3 spawnPos = transform.position;
                spawnPos += Vector3.up * _vertDetectOffset;
                GameObject _newEnemy = Instantiate<GameObject>(_enemyToSpawn, spawnPos, transform.rotation);
                _newEnemy.GetComponent<BaseEnemy>().Init(_myRoom, Mechanic.BOSS);
                _startSpawnTime = Time.time;
            }
        }
    }

    //Attack
    //Boss will spin
    //Boss will shoot out towards the player and richochet off of walls for a duration
    private void PlaySomePinball()
    {
        if (_tellPinballing)
        {
            Vector3 playerPos = _playerRef.transform.position;
            playerPos.y = transform.position.y;
            transform.LookAt(playerPos);

            if (_myAnimations.IsInTransition(0))
            {
                _currAttackTime = 1;
                _tellPinballing = false;
                _moveDir = (_playerRef.transform.position - transform.position).normalized;
                _moveDir.y = 0;
                _startAttackTime = Time.time;
            }
        }
        else
        {
            if (!_speaker.isPlaying)
            {
                _speaker.PlayOneShot(bossSpin, volSFX);
            }

            _currAttackTime = (Time.time - _startAttackTime) / _realPinballAttackDuration;

            //Debug.Log("pinballing");

            for (int i = 0; i <= _pinballNumOfCasts; i++)
            {
                if (!_speaker.isPlaying)
                    _speaker.PlayOneShot(bossSpin, volSFX);

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

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), _moveDir, out hit, _bossCollisionDetectDistance + 1f))
            {
                Vector3 checkDir = _moveDir;
                GameObject thingHit = hit.collider.gameObject;

                //Debug.Log("reflected");
                if (thingHit.GetComponent<BossWall>()|| thingHit.GetComponent<DoorMovement>())
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
                _myAnimations.Play("Dazed", 0);
                _MyAttack = SPINSTRATS.STUNNED;
                _startAttackTime = Time.time;
                return;
            }
        }


    }

    //once the boss is done with their attack
    //will do nothing for an amount of time
    private void Stunned()
    {
        dazedParticle.SetActive(true);
        _currAttackTime = (Time.time - _startAttackTime) / _realStunnedDuration;

        _speaker.Stop();
        if (!_speaker.isPlaying)
            _speaker.PlayOneShot(bossDazed, volSFX);
        //Debug.Log("stunned");

        if (_currAttackTime >= 1)
        {
            dazedParticle.SetActive(false);
            _MyAttack = SPINSTRATS.DOWNTIME;
            _startAttackTime = Time.time;
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
        _myRoom.CheckForEnd();

        _enemyAgent.enabled = false;

        _playerRef.GoingToOutroCutscene();

        _ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _speaker.PlayOneShot(bossDeath, volSFX);

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
            _animating = false;
            _animatingSecond = false;

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
