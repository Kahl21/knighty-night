﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

public class ShootingBoss : BossEnemy
{
    //Strategy Enum for the Shooting Boss
    protected enum SHOOTERSTATES
    {
        STUNNED,
        ATTACKING,
        ABSORB,
        CHECKFORABSORB,
        SHOOT,
        FOLLOWING
    }
    

    [Header("Shooting Boss Variables")]
    [SerializeField]
    float _stunnedTime;                                             //Time Boss is stunned in between attacks
    [SerializeField]
    float _vertDetectOffset;                                        //Offset for raycasts above the ground
    [SerializeField]
    float _startAngle;                                              //Starting Angle for following
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
    float _damageToBoss;                                            //Damage to boss from reflected ghlosts
    [SerializeField]
    float _damageToPlayer;                                          //Damage to player from reflected ghlosts

    [Header("Follow Player Varibales")]
    [SerializeField]
    float _followDuration;                                          //Duration the boss follows player
    float _realFollowDuration;

    [Header("Hard Follow Player Varibales")]
    [SerializeField]
    float _hardFollowDuration;
    [SerializeField]
    float _hardTimeBetweenAttacks;

    [Header("Color Attack Variables")]
    [SerializeField]
    float _colorDamageToBoss;                                       //Damage to boss from same colored ghlosts

    [Header("Special Attack Variables")]
    [SerializeField]
    float _absorbAttackPercentage;                                  //How often the absorb happens
    [SerializeField]
    float _normalAtttackPercentage;                                 //The rest of the percentage

    [Header("Absorb Attack Variables")]
    [SerializeField]
    float _absorbDelay;                                             //Delay the boss waits before absorbing the ghlosts
    [SerializeField]
    float _absorbSpeed;                                             //How fast ghlosts are absorbed
    [SerializeField]
    float _rotateSpeed;                                             //How fas the boss rotates while absorbing
    [SerializeField]
    float _shootingFollowOffset;                                    //How much the boss lags behind the player movement while shooting
    [SerializeField]
    float _shootRate;                                               //How often the boss shoots them out


    float _rotationAngle = 0;                                       //Current angle of rotation

    int _ghlostsShot = 0;                                           //Count of shot ghlosts
    bool _invinciblesAddad = false;                                 //Check for determining if all invincible ghlosts have been counted
    [SerializeField]
    List<GameObject> _absorbingGhlosts;                             //List of ghlosts to absorb
    [SerializeField]
    List<GameObject> _absorbedGhlosts;                              //List of absorbed ghlosts
    bool _specialPulseAttack = false;                               //Check for determing if the absorb attack should go throguh

    Vector3 _ogCamPos;
    bool _cameraInPosition;
    float _startTimer;
    float _currentTime;
    GhlostBossShooter _attachedShooter;                             //Quick reference for the attached shooter
    [HideInInspector]
    public bool _attackInProgress = false;                          //Way for the shooter to tell the boss that an attack is in progress

    //Old variables for animations and such.
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
            _attachedShooter = gameObject.GetComponent<GhlostBossShooter>();

            if (!_managerRef.HardModeOn)
            {
                //_realTimeBetweenAttacks = _timeBetweenAttacks;
                _realFollowDuration = _followDuration;
            }
            else
            {
                //_realTimeBetweenAttacks = _hardTimeBetweenAttacks;
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
            _absorbingGhlosts = new List<GameObject>();
            _absorbedGhlosts = new List<GameObject>();
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

                        case SHOOTERSTATES.ABSORB:
                            AbsorbAtk();
                            break;

                        case SHOOTERSTATES.CHECKFORABSORB:
                            CheckForCanAbsorb();
                            break;

                        case SHOOTERSTATES.SHOOT:
                            ShootAtPlayer();
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

    //Called while the shooter attached to him is doing an attack.
    //While there is an attacking going, nothing happens.
    private void Attacking()
    {
        if (_attachedShooter.attackInProgress != true)
        {
            if (_specialPulseAttack == true)                                //Checks to see if he should absorb
            {
                _MyState = SHOOTERSTATES.CHECKFORABSORB;                    //Starts the absorb attack
                _specialPulseAttack = false;
                _startTimer = Time.time;
            }
            else
            {
                _MyState = SHOOTERSTATES.STUNNED;                           //No secondary attack so he is stunned
                _startTimer = Time.time;
            }
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

    //Check to see if it time to absorb the ghlosts
    //Will do it after a set amount of time
    private void CheckForCanAbsorb()                            
    {
        float timeTaken = Time.time - _startTimer;
        if (timeTaken > _absorbDelay)                                       //After the absorb delay start absorbing
        {
            _absorbingGhlosts = new List<GameObject>();                     //Reset lists of ghlosts
            _absorbedGhlosts = new List<GameObject>();
            _invinciblesAddad = false;
            _startTimer = Time.time;
            _MyState = SHOOTERSTATES.ABSORB;                                //Start Absorbing - AbsorbAtk();
        }
        
    }

    /*Absorb Attack
    * Boss spins while the ghlosts are sucked up to him.
    * As they touch him, they are deactivated and added to a list and compared to a list of total
    * invincible ghlosts in the scene.
    * When they are equal, it starts the shooting phase
    */
    private void AbsorbAtk()
    {
        _rotationAngle += 1 * _rotateSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y+ _rotationAngle, 0);
        if (_invinciblesAddad == false)
        {
            for (int index = 0; index < _attachedShooter.GetGhlostsInScene.Count; index++)      //Counts the invincible ghlosts in the scene
            {
                if (_attachedShooter.GetGhlostsInScene[index].GetComponent<DumbBossGlhost>().GetMyMechanic == Mechanic.CHASE)
                {
                    DumbBossGlhost ghlostRef = _attachedShooter.GetGhlostsInScene[index].GetComponent<DumbBossGlhost>();
                    ghlostRef.GetSpeed = _absorbSpeed;
                    ghlostRef.setMove(transform.eulerAngles);                           //Rotates ghlosts towards boss and moves them towards him
                    _absorbingGhlosts.Add(_attachedShooter.GetGhlostsInScene[index]);   //Adds each ghlosts to a list
                }
            }
            _invinciblesAddad = true;
        }
        else if(_absorbingGhlosts.Count != _absorbedGhlosts.Count)          //While not all of the ghlosts are in the scene
        {                                                                   
            _rotationAngle += 1 + _rotateSpeed * Time.deltaTime;            //The boss spins at a designated speed
            transform.eulerAngles = new Vector3(0, _rotationAngle, 0);
        }
        else if (_absorbingGhlosts.Count == _absorbedGhlosts.Count)         //After all ghlosts are absorbed
        {
            _ghlostsShot = 0;                                               //Resets how many ghlosts have been shot
            _MyState = SHOOTERSTATES.SHOOT;                                 //Starts shooting - ShootAtPlayer();
            _startTimer = Time.time;                                        //Resets timer
        }
    }


    /*Shoots ghlosts back at player
     * The boss shoots all the previously absorbed ghlosts out at the player
     * They come out at wierd angles because they are not instatiated but take from their previous position
     * When he shoots them all, the boss is stunned
     */
    private void ShootAtPlayer()
    {
        if (_ghlostsShot < _absorbedGhlosts.Count)                          //Checks to see if all of the absorbed ghlosts have been shot out
        {
            float timeTaken = Time.time - _startTimer;
            gameObject.transform.LookAt(_playerRef.transform);              //Boss looks at player

            //Spawns a new glhost based on the spawnrate
            if (timeTaken >= _shootRate * _ghlostsShot)                     //Shoots at a rate specified
            {
                DumbBossGlhost ghlostRef = _absorbedGhlosts[_ghlostsShot].GetComponent<DumbBossGlhost>();
                ghlostRef.transform.rotation = transform.rotation;          //Shot ghlost's rotation is set to the bosses
                ghlostRef.gameObject.SetActive(true);                       //Enable the shot ghlost
                ghlostRef.ShootMe(_playerRef.transform.rotation);           //Shoots the ghlost out
                ghlostRef.setMyState = DumbBossGlhost.DUMBSTATE.DIE;        //makes it so the invincible ghlost can die against a wall
                _ghlostsShot++;                                             //Adds to the tally of ghlosts shot
            }
            Debug.Log("Shooting Ghlosts");
        }
        else
        {
            _startTimer = Time.time;
            _MyState = SHOOTERSTATES.STUNNED;                               //Stuns the boss - Stunned();
        }
        
    }

    /*Changes the color of the boss
     * The boss changes color after every attack and can be hit for more damage by ghlosts of the same color
     * Random color is chosen by the colors that the LDer's choose for the colored ghlosts
     */
    private void changeColor()                                            
    {
        Debug.Log("Change Color");
        List<Color> possibleColors = _attachedShooter.GetGhlostColors;  //Grabs the list of potential colored ghlosts from the shooter
        int _rando = Random.Range(0, possibleColors.Count);             
        _myColor = possibleColors[_rando];                              //Sets his color for color detection
        _myMaterial.color = _myColor;                                   //Sets his material color
        _mySkinRenderer.materials[1] = _myMaterial;
    }

    /*Follow the player
     * Same as all other bosses
     */
    private void FollowPlayer()
    {
        _enemyAgent.SetDestination(_playerRef.transform.position);
        float timeTaken = Time.time - _startTimer;

        //Debug.Log("following");

        if (timeTaken > _realFollowDuration)
        {
            _enemyAgent.SetDestination(transform.position);
            changeColor();
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

    /*Checks for color
    * called by ghlosts once they get hit into me
    * checks to see if the ghlost has the same color
    */
    public bool CheckForColor(float _damageTaken, Color _incColor)
    {
        if (_incColor == _myColor)
        {
            GotHit(_colorDamageToBoss);
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

    /*
     * Checks to see when the boss has been hit by a ghlost
     * If the ghlost hits him and the boss is absorbing, he adds them to a list and deactivates them
     * If any ghlost hits him he takes damage from them and checks to see if he died
     */
    public void HitByGhlost(GameObject objectHitting, float _damageHit)
    {
        if (objectHitting.GetComponent<DumbBossGlhost>() && (_MyState == SHOOTERSTATES.ABSORB))
        {
            _absorbedGhlosts.Add(objectHitting);
            objectHitting.SetActive(false);
        }
        else if (objectHitting.GetComponent<DumbBossGlhost>())
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

            _currBossHealth = _actualMaxHealth;
            _laggedBossHealthBar.fillAmount = 1;
            _actualBossHealthBar.fillAmount = 1;

            _bossBar.SetActive(false);
            _cameraInPosition = false;

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
    public bool SetSpecialPulseAttack { get { return _specialPulseAttack; } set { _specialPulseAttack = value; } }
}