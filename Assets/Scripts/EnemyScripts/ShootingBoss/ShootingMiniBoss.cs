using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ShootingMiniBoss : BossEnemy
{
    //Strategy Enum for the Spin Boss
    protected enum SHOOTERSTATES
    {
        STUNNED,
        ATTACKING,
        FOLLOWING
    }
    

    [Header("Shooting Boss Variables")]
    //[SerializeField]
    float _timeBetweenAttacks;
    float _realTimeBetweenAttacks;
    [SerializeField]
    float _stunnedTime;
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
    int _numOfCasts = 4;
    RaycastHit hitObj = new RaycastHit();

    [Header("Base Spawned Enemies")]
    [SerializeField]
    float _damageToBoss;
    [SerializeField]
    float _damageToPlayer;

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;
    float _realFollowDuration;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;
    [SerializeField]
    float _hardTimeBetweenAttacks;

    Vector3 _ogCamPos;
    bool _cameraInPosition;
    float _startTimer;
    float _currentTime;
    GhlostShooter _attachedShooter;
    [HideInInspector]
    float _startAttackTime;
    float _currAttackTime;

    

    SHOOTERSTATES _MyState = SHOOTERSTATES.FOLLOWING;

    //intro cutscene function
    
    protected override void PlayIntro()
    {
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
            _attachedShooter = gameObject.GetComponent<GhlostShooter>();

            if (!_managerRef.HardModeOn)
            {
                _realTimeBetweenAttacks = _timeBetweenAttacks;
                _realFollowDuration = _followDuration;
            }
            else
            {
                _realTimeBetweenAttacks = _hardTimeBetweenAttacks;
                _realFollowDuration = _hardFollowDuration;
            }
        }

        //_ogCamPos = _cameraRef.transform.position;
        cam0 = _cameraRef.transform.position;
        cam1 = transform.position + _camOffset;
        cam1.y = _cameraRef.transform.position.y;
        _cameraRef.AmFollowingPlayer = false;

        _startTimer = Time.time;
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
                    switch (_MyState)
                    {
                        case SHOOTERSTATES.FOLLOWING:
                            FollowPlayer();
                            break;
                        case SHOOTERSTATES.ATTACKING:
                            Attacking();
                            break;
                            
                        case SHOOTERSTATES.STUNNED:
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

    private void Attacking()
    {
        if (_attachedShooter.attackInProgress != true)
        {
            _MyState = SHOOTERSTATES.STUNNED;
            _startTimer = Time.time;
        }
    }

    private void Stunned()
    {
        float timeTaken = Time.time - _startTimer;
        if (timeTaken >= _stunnedTime)
        {
            _MyState = SHOOTERSTATES.FOLLOWING;
            _startTimer = Time.time;
        }
    }

    private void FollowPlayer()
    {
        _enemyAgent.SetDestination(_playerRef.transform.position);
        float timeTaken = Time.time - _startTimer;

        //Debug.Log("following");

        if (timeTaken > _realFollowDuration)
        {
            _enemyAgent.SetDestination(transform.position);

            _attachedShooter.newAttack = true;
            _attachedShooter.attackInProgress = true;
            _MyState = SHOOTERSTATES.ATTACKING;
        }

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

        _calcAngle = _startAngle;
    }

    //called by ghlosts once they get hit into me
    //checks to see if the ghlost has the same color
    public bool CheckForColor(float _damageTaken, Color _incColor)
    {
        if (_incColor == _myColor)
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

    public void HitByGhlost(GameObject objectHitting, float _damageHit)
    {
        if (objectHitting.GetComponent<DumbGlhost>())
        {
            base.GotHit(_damageHit);

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
        _attachedShooter.enabled = false;
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
            _attachedShooter.MyReset();
            gameObject.SetActive(true);
            _mySkinRenderer.enabled = true;
            _myColor.a = 1;
            _myMaterial.color = _myColor;
            _mySkinRenderer.materials[1] = _myMaterial;

            _enemyAgent.enabled = false;
            Debug.Log("Boss Reset");
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
            _MyState = SHOOTERSTATES.FOLLOWING;
            _init = false;
        }
    }

    public float GetDamageToBoss { get { return _damageToBoss; } }
    public float GetDamageToPlayer { get { return _damageToPlayer; } }
    public PlayerController GetPlayerRef { get { return _playerRef; } }
}
