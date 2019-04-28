using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBossGlhost : BossEnemy {

    //Strategies for the Color Boss
    protected enum ColorStrats
    {
        DOWNTIME,
        FOLLOW,
        BOUNCE,
        CHARGE,
        STUNNED,
        EATGLHOSTS
    }

    [Header("Color Boss Intro Variables")]
    [SerializeField]
    Camera _additionalCam1;
    [SerializeField]
    Camera _additionalCam2, _additionalCam3;
    Vector3 _additionalPos1, _additionalRot1, _additionalPos2, _additionalRot2, _additionalPos3, _additionalRot3;
    [SerializeField]
    GameObject _colorIntroGlhosts;
    List<ColorIntroGlhost> _introGlhostList;
    bool _cameraInPosition = false;
    bool _animating = false;
    bool _animatingSecond = false;
    bool _animatingThird = false;
    bool _animatingFourth = false;
    bool _breakfastEaten = false;
    bool _jumpingFinished = false;
    bool _lookDone = false;
    bool _startedRunning = false;

    [Header("Color Boss Variables")]
    [SerializeField]
    Color _basicColor;
    [SerializeField]
    GameObject _colorBossMinion;
    [SerializeField]
    List<Color> _colorsForMinions;
    List<Color> _realColorsForMinions;
    List<Color> _ColorsLeft;
    List<Color> _currColors;
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

    [Header("Percentage Variables")]
    float _startAttackTime;
    float _currAttackTime;
    [SerializeField]
    float _followPercentage;
    float _realFollowPercentage;
    [SerializeField]
    float _bouncePercentage;
    float _realBouncePercentage;
    [SerializeField]
    float _chargePercentage;
    float _realChargePercentage;
    float _totalPercentage;

    [Header("Follow Player Variables")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Bounce Attack Variables")]
    [SerializeField]
    int _numberOfBounces;
    int _realNumOfBounces;
    int _currBounces;
    [SerializeField]
    float _bouncingAirtime;
    float _realBouncingAirtime;
    [SerializeField]
    float _bounceHeight;
    [SerializeField]
    float _bounceSpawnAngle;
    [SerializeField]
    float _bounceSpawnAngleOffset;
    Vector3 c0, c1, c2;
    bool _jumping;

    [Header("Charge Attack Variables")]
    [SerializeField]
    float _chargeUpDuration;
    float _realChargeUpDuration;
    [SerializeField]
    float _chargeUpBackwardsSpeed;
    float _realChargeUpBackwardsSpeed;
    [SerializeField]
    float _chargeMoveSpeed;
    float _realChargeSpeed;
    [SerializeField]
    float _chargeDetectionAngle;
    [SerializeField]
    int _chargeNumOfCasts;
    [SerializeField]
    float _chargeSpawnAngle;
    [SerializeField]
    float _chargeSpawnAngleOffset;
    Vector3 _moveDir;
    bool _tellCharging;

    [Header("Color Boss Hard Variables")]
    [SerializeField]
    List<Color> _hardColorsForMinions;
    [SerializeField]
    float _hardTimeBetweenAttacks;
    [SerializeField]
    float _hardStunnedDuration;

    [Header("Hard Percentages")]
    [SerializeField]
    float _hardFollowPercentage;
    [SerializeField]
    float _hardBouncePercentage;
    [SerializeField]
    float _hardChargePercentage;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;

    [Header("Hard Bounce Variables")]
    [SerializeField]
    int _hardNumofBounces;
    [SerializeField]
    float _hardBounceAirtime;

    [Header("Hard Charge Variables")]
    [SerializeField]
    float _hardChargeMoveSpeed;
    [SerializeField]
    float _hardChargeUpDuration;
    [SerializeField]
    float _hardChargeUpBackwardsSpeed;


    [Header("Sound Options")]
    public AudioClip bossDeath;
    public AudioClip bossDazed;
    public AudioClip bossBounce;
    public AudioClip bossCharge;
    public AudioClip ghostSpawn;


    ColorStrats _myAttack = ColorStrats.FOLLOW;

    protected override void Awake()
    {
        base.Awake();
        _additionalCam1.transform.parent = null;
        _additionalPos1 = _additionalCam1.transform.position;
        _additionalRot1 = _additionalCam1.transform.localEulerAngles;
        _additionalCam1.gameObject.SetActive(false);

        _additionalCam2.transform.parent = null;
        _additionalPos2 = _additionalCam2.transform.position;
        _additionalRot2 = _additionalCam2.transform.localEulerAngles;
        _additionalCam2.gameObject.SetActive(false);

        _additionalCam3.transform.parent = null;
        _additionalPos3 = _additionalCam3.transform.position;
        _additionalRot3 = _additionalCam3.transform.localEulerAngles;
        _additionalCam3.gameObject.SetActive(false);
    }

    //intro cutscene function
    protected override void PlayIntro()
    {
        if (!_cameraInPosition)
        {
            if (_cameraRef.MoveCamera())
            {
                _currAttackTime = (Time.time - _startAttackTime) / _cameraIntroDuration * 2;

                if (_currAttackTime >= 1)
                {
                    cam0 = _cameraRef.transform.position;
                    cam1 = _additionalPos1;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _additionalRot1;

                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration / 2);

                    _cameraInPosition = true;
                }
            }
            else
            {
                _startAttackTime = Time.time;
            }
        }
        else if (!_animating)
        {
            if (_cameraRef.MoveCamera())
            {
                if (!_lookDone)
                {
                    for (int i = 0; i < _introGlhostList.Count; i++)
                    {
                        _introGlhostList[i].LookAtBoss(i + 1);
                    }
                    _lookDone = true;
                }
                _currAttackTime = (Time.time - _startAttackTime) / (_cameraIntroDuration * 3);

                if (_currAttackTime >= 1)
                {
                    cam0 = _cameraRef.transform.position;
                    cam1 = _additionalPos2;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _additionalRot2;

                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

                    _animating = true;
                }
            }
            else
            {
                _startAttackTime = Time.time;
            }
        }
        else if (!_animatingSecond)
        {
            if (_cameraRef.MoveCamera())
            {
                if (!_startedRunning)
                {
                    _myAnimations.Play("BigIntro", 0);

                    for (int i = _introGlhostList.Count - 1; i >= 0; i--)
                    {
                        _introGlhostList[i].StartRun();
                    }

                    _mySkinRenderer.enabled = true;
                    _startedRunning = true;

                }

                if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .98f)
                {
                    _myMaterial.color = _introGlhostList[4].GotEaten();
                    _mySkinRenderer.materials[1] = _myMaterial;

                    cam0 = _cameraRef.transform.position;
                    cam1 = _additionalPos3;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _additionalRot3;

                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);
                    _animatingSecond = true;
                }
                else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .88f)
                {
                    _myMaterial.color = _introGlhostList[3].GotEaten();
                    _mySkinRenderer.materials[1] = _myMaterial;
                }
                else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .78f)
                {
                    _myMaterial.color = _introGlhostList[2].GotEaten();
                    _mySkinRenderer.materials[1] = _myMaterial;
                }
                else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .68f)
                {
                    _myMaterial.color = _introGlhostList[1].GotEaten();
                    _mySkinRenderer.materials[1] = _myMaterial;
                }
                else if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .58f)
                {
                    _myMaterial.color = _introGlhostList[0].GotEaten();
                    _mySkinRenderer.materials[1] = _myMaterial;
                }
            }
        }
        else if (!_animatingThird)
        {
            if(_cameraRef.MoveCamera())
            {
                if (_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f)
                {
                    cam0 = _cameraRef.transform.position;
                    cam1 = _ogCamPos;
                    rot0 = _cameraRef.transform.localEulerAngles;
                    rot1 = _ogCamRot;


                    _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);
                    _animatingThird = true;
                }
                else if(_myAnimations.GetCurrentAnimatorStateInfo(0).normalizedTime > .6f)
                {
                    _myMaterial.color = _basicColor;
                    _mySkinRenderer.materials[1] = _myMaterial;
                }
            }
        }
        else
        {
            if (_cameraRef.MoveCamera())
            {
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
            _myMaterial.color = _basicColor;
            _mySkinRenderer.materials[1] = _myMaterial;

            _realColorsForMinions = new List<Color>();
            _ColorsLeft = new List<Color>();

            if (!_managerRef.HardModeOn)
            {
                _realColorsForMinions = _colorsForMinions;
                _realFollowPercentage = _followPercentage;
                _realBouncePercentage = _bouncePercentage;
                _realChargePercentage = _chargePercentage;
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realStunnedDuration = _stunnedDuration;
                _realFollowDuration = _followDuration;
                _realNumOfBounces = _numberOfBounces;
                _realBouncingAirtime = _bouncingAirtime;
                _realChargeSpeed = _chargeMoveSpeed;
                _realChargeUpDuration = _chargeUpDuration;
                _realChargeUpBackwardsSpeed = _chargeUpBackwardsSpeed;
            }
            else
            {
                _realColorsForMinions = _hardColorsForMinions;
                _realFollowPercentage = _hardFollowPercentage;
                _realBouncePercentage = _hardBouncePercentage;
                _realChargePercentage = _hardChargePercentage;
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realStunnedDuration = _hardStunnedDuration;
                _realFollowDuration = _hardFollowDuration;
                _realNumOfBounces = _hardNumofBounces;
                _realBouncingAirtime = _hardBounceAirtime;
                _realChargeSpeed = _hardChargeMoveSpeed;
                _realChargeUpDuration = _hardChargeUpDuration;
                _realChargeUpBackwardsSpeed = _hardChargeUpBackwardsSpeed;
            }

            _ColorsLeft = _realColorsForMinions;

            _introGlhostList = new List<ColorIntroGlhost>();

            for (int i = 0; i < _colorIntroGlhosts.transform.childCount; i++)
            {
                _introGlhostList.Add(_colorIntroGlhosts.transform.GetChild(i).GetComponent<ColorIntroGlhost>());
            }
        }

        _mySkinRenderer.enabled = false;

        _ogCamPos = _cameraRef.transform.position;
        _ogCamRot = _cameraRef.transform.localEulerAngles;
        cam0 = _cameraRef.transform.position;
        cam1 = _bossCameraPos;
        rot0 = _cameraRef.transform.localEulerAngles;
        rot1 = _bossCameraRot;
        _cameraRef.AmFollowingPlayer = false;

        _speaker = this.transform.GetComponent<AudioSource>();

        _totalPercentage = _realFollowPercentage + _realBouncePercentage + _realChargePercentage;

        _cameraRef.BossIntroActive(cam0, cam1, rot0, rot1, _cameraIntroDuration);

        _startAttackTime = Time.time;
        _myAI = BossAI.INTRO;
    }

    //called when init and cutscene are done
    //starts fight
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
                    switch (_myAttack)
                    {
                        case ColorStrats.DOWNTIME:
                            WhatDoNext();
                            break;
                        case ColorStrats.FOLLOW:
                            FollowPlayer();
                            break;
                        case ColorStrats.BOUNCE:
                            BounceAttack();
                            break;
                        case ColorStrats.CHARGE:
                            ChargePlayer();
                            break;
                        case ColorStrats.STUNNED:
                            Stunned();
                            break;
                        case ColorStrats.EATGLHOSTS:
                            WaitingForFood();
                            break;
                        default:
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
                _myAttack = ColorStrats.FOLLOW;
                _myAnimations.Play("Movement", 0);
            }
            else if (_nextAttack > _realFollowPercentage && _nextAttack <= (_realFollowPercentage + _realBouncePercentage))
            {
                //Debug.Log("Bounce Chosen");

                c0 = transform.position;
                c2 = _playerRef.transform.position;
                c1 = ((c0 + c2) / 2) + (Vector3.up * _bounceHeight);

                _enemyAgent.enabled = false;
                _jumping = false;
                _jumpingFinished = false;
                _myAnimations.Play("StartJump", 0);
                _myAttack = ColorStrats.BOUNCE;
            }
            else
            {
                //Debug.Log("Charge Chosen");
                _tellCharging = true;
                _enemyAgent.enabled = false;
                _startAttackTime = Time.time;
                _myAnimations.Play("RearUp", 0);
                _myAttack = ColorStrats.CHARGE;
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

            _myAttack = ColorStrats.DOWNTIME;
            _startAttackTime = Time.time;
        }

        for (int i = 0; i <= _numOfCasts; i++)
        {
            float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;
            float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;

            Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);

            if (_debug)
            {
                Debug.DrawRay(transform.position + (Vector3.up * _vertDetectOffset),RayDir * _bossCollisionDetectDistance, Color.red);
            }

            _calcAngle += _detectionAngle / _numOfCasts;

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
            {
                if (hit.collider.GetComponent<PlayerController>())
                {
                    hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                }

                if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), Vector3.down, out hit, _bossCollisionDetectDistance))
                {
                    if (hit.collider.GetComponent<PlayerController>())
                    {
                        hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                    }
                }
            }
        }

        _calcAngle = _startAngle;
    }

    //Attack
    //Boss will jump into the air and slam down onto the players position
    //will do it any number of times
    private void BounceAttack()
    {
        if (!_jumping)
        {
            if (_myAnimations.IsInTransition(0))
            {
                _jumping = true;
                _jumpingFinished = false;
                _startAttackTime = Time.time;
            }
        }
        else
        {
            _currAttackTime = (Time.time - _startAttackTime) / _realBouncingAirtime;
            //Debug.Log("spinning");

            if (_currAttackTime >= 1)
            {
                //Debug.Log("spinning Done");
                if (_currBounces >= _realNumOfBounces)
                {
                    _currAttackTime = 1;
                    
                    if (!_jumpingFinished)
                    {
                        _myAnimations.Play("EndJump", 0);
                        _jumpingFinished = true;
                    }
                    else if (_myAnimations.IsInTransition(0))
                    {
                        _jumpingFinished = false;
                        int _randomColor = Random.Range(0, _ColorsLeft.Count);
                        _myColor = _ColorsLeft[_randomColor];
                        _myMaterial.color = _myColor;
                        _mySkinRenderer.materials[1] = _myMaterial;

                        SpawnGlhosts(_bounceSpawnAngle, _bounceSpawnAngleOffset);
                        _enemyAgent.enabled = true;
                        _currBounces = 0;
                        _myAnimations.Play("Spawn", 0);
                        _myAttack = ColorStrats.STUNNED;
                        _startAttackTime = Time.time;
                    }
                }
                else
                {
                    _currAttackTime = 1;

                    if (!_speaker.isPlaying)
                    {
                        _speaker.PlayOneShot(bossBounce, volSFX);
                    }

                    _calcAngle = _startAngle;

                    if(!_jumpingFinished)
                    {
                        _myAnimations.Play("EndJump", 0);
                        _jumpingFinished = true;
                    }
                    else if (_myAnimations.GetCurrentAnimatorStateInfo(0).IsName("StartJump"))
                    {
                        _currBounces++;
                        c0 = transform.position;
                        c2 = _playerRef.transform.position;
                        c1 = ((c0 + c2) / 2) + (Vector3.up * _bounceHeight);
                        _jumpingFinished = false;
                        _startAttackTime = Time.time;
                    }
                }
            }
            else
            {
                for (int i = 0; i <= _numOfCasts; i++)
                {
                    float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;
                    float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;

                    Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);

                    if (_debug)
                    {
                        Debug.DrawRay(transform.position + (Vector3.up * _vertDetectOffset), RayDir * _bossCollisionDetectDistance, Color.red);
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

                Vector3 c01, c12, c012;

                c01 = (1 - _currAttackTime) * c0 + _currAttackTime * c1;
                c12 = (1 - _currAttackTime) * c1 + _currAttackTime * c2;

                c012 = (1 - _currAttackTime) * c01 + _currAttackTime * c12;

                transform.position = c012;
            }
        }
    }

    //Attack
    //Boss will rev up by moving backwards
    //boss will then charge at the player and stop once it hits something
    private void ChargePlayer()
    {
        if (_tellCharging)
        {
            _speaker.PlayOneShot(bossCharge, volSFX);

            _currAttackTime = (Time.time - _startAttackTime) / _realChargeUpDuration;

            if (_currAttackTime >= 1)
            {
                _currAttackTime = 1;
                _tellCharging = false;
                _moveDir = (_playerRef.transform.position - transform.position).normalized;
                _moveDir.y = 0;
                
                _calcAngle = _startAngle;
                _startAttackTime = Time.time;
            }
            transform.LookAt(_playerRef.transform.position);

            if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), -transform.forward, out hit, _bossCollisionDetectDistance*2))
            {
                if(!hit.collider.GetComponent<BossWall>() || !hit.collider.GetComponent<DoorMovement>())
                {
                    transform.position -= transform.forward * _realChargeUpBackwardsSpeed * Time.deltaTime;
                }
            }

        }
        else
        {
            //Debug.Log("charging");

            for (int i = 0; i <= _chargeNumOfCasts; i++)
            {
                float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;
                float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _bossCollisionDetectDistance;

                Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);
                
                if (_debug)
                {
                    Debug.DrawRay(transform.position + (Vector3.up * _vertDetectOffset), RayDir * _bossCollisionDetectDistance, Color.red);
                }

                _calcAngle += _detectionAngle / _chargeNumOfCasts;

                if (Physics.Raycast(transform.position + (Vector3.up * _vertDetectOffset), RayDir, out hit, _bossCollisionDetectDistance))
                {
                    if (hit.collider.GetComponent<PlayerController>())
                    {
                        hit.collider.GetComponent<PlayerController>().TakeDamage(_bossDamage);
                    }
                    else if(hit.collider.GetComponent<BossWall>() || hit.collider.GetComponent<DoorMovement>())
                    {
                        int _randomColor = Random.Range(0, _ColorsLeft.Count);
                        _myColor = _ColorsLeft[_randomColor];
                        _myMaterial.color = _myColor;
                        _mySkinRenderer.materials[1] = _myMaterial;

                        SpawnGlhosts(_chargeSpawnAngle, _chargeSpawnAngleOffset);

                        _enemyAgent.enabled = true;
                        _myAnimations.Play("Spawn", 0);
                        _myAttack = ColorStrats.STUNNED;
                        _startAttackTime = Time.time;
                        return;
                    }
                }
               
            }

            if (_debug)
            {
                Debug.DrawRay(transform.position + (Vector3.up * _vertDetectOffset), _moveDir * _bossCollisionDetectDistance, Color.blue);
            }

            _calcAngle = _startAngle;

            transform.position += _moveDir * _realChargeSpeed * Time.deltaTime;
        }
    }

    //spawn the glhosts that can damage him
    private void SpawnGlhosts(float _spawnAngle, float _angleOffset)
    {
        _calcAngle = _spawnAngle;
        _currColors = new List<Color>();

        _speaker.PlayOneShot(ghostSpawn, volSFX);


        for (int i = 0; i < _ColorsLeft.Count; i=0)
        {
            float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad);
            float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad);

            Vector3 SpawnDir = (transform.forward * Zpos) + (transform.right * Xpos);

            _calcAngle += _angleOffset / _ColorsLeft.Count + 1;

            int _rando = Random.Range(0, _ColorsLeft.Count);

            GameObject _newGlhost = Instantiate<GameObject>(_colorBossMinion, transform.position, transform.rotation, null);
            _newGlhost.GetComponent<ColorMinion>().SetColor = _ColorsLeft[_rando];
            _newGlhost.GetComponent<ColorMinion>().GetStartDirection = SpawnDir;
            _currColors.Add(_ColorsLeft[_rando]);
            _ColorsLeft.Remove(_ColorsLeft[_rando]);
            _newGlhost.GetComponent<ColorMinion>().Init(_myRoom, Mechanic.BOSS);
        }

        _ColorsLeft = _currColors;
    }

    //once the boss is done with their attack
    //will do nothing for an amount of time
    private void Stunned()
    {
        _currAttackTime = (Time.time - _startAttackTime) / _realStunnedDuration;

        //Debug.Log("stunned");

        if (_currAttackTime >= 1)
        {
            if (!_speaker.isPlaying)
                _speaker.PlayOneShot(bossDazed, volSFX);

            _myAnimations.Play("SuckUp", 0);
            _myColor = _basicColor;
            _myMaterial.color = _myColor;
            _mySkinRenderer.materials[1] = _myMaterial;

            for (int i = 0; i < _myRoom.GetCurrEnemyList.Count; i++)
            {
                _myRoom.GetCurrEnemyList[i].GetComponent<ColorMinion>().StartBackToBoss();
            }
            _myAttack = ColorStrats.EATGLHOSTS;
        }
    }

    //called once the boss is done being stunned and will suck up any remaining glhosts
    private void WaitingForFood()
    {
        if(_myRoom.GetCurrEnemyList.Count == 0)
        {
            _myAnimations.Play("Movement", 0);
            _myAttack = ColorStrats.DOWNTIME;
            _startAttackTime = Time.time;
        }
    }

    //called by ghlosts once they get hit into me
    //checks to see if the ghlost has the same color
    public bool CheckForColor(float _damageTaken, Color _incColor)
    {
        if(_incColor == _myColor)
        {
            GotHit(_damageTaken);
            return true;
        }
        else
        {
            return false;
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
            _basicColor.a = 1;
            _myMaterial.color = _basicColor;
            _mySkinRenderer.materials[1] = _myMaterial;

            _enemyAgent.enabled = false;
            //Debug.Log("Boss Reset");
            transform.position = _startPos;
            transform.rotation = _startRot;
            _colorIntroGlhosts.SetActive(true);
            for (int i = 0; i < _introGlhostList.Count; i++)
            {
                _introGlhostList[i].ResetCutscene();
            }

            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);
            _cameraInPosition = false;
            _jumpingFinished = false;

            _endingPlaying = false;
            _laggingHealth = false;
            _updatingHealth = false;
            _dead = false;
            _amHit = false;
            _invincible = false;

            _myAI = BossAI.NONE;
            _myAttack = ColorStrats.FOLLOW;
            _init = false;
        }
    }
    
    public Color GetColor { get { return _myColor; } }
}

