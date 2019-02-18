using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (FindObjectOfType<PlayerController>())
                {
                    _instance = FindObjectOfType<PlayerController>();
                    return _instance;
                }
                else
                {
                    Debug.Log("there's nothing here");
                    return null;
                }
            }
        }
    }

    enum Interactions
    {
        NONE,
        WALKING,
        ATTACKING,
        WAVESPECIAL,

        //ETC
    }

    //menu Vars
    bool _inMenu = true;
    bool _inCutscene = false;
    bool _bossCutsceneInit = false;
    float _currCutsceneTime;
    float _startCutsceneTime;
    Vector3 _posToGoInCutscene;
    Vector3 cs0, cs1;
    [SerializeField]
    Menuing _menuRef;
    List<GameObject> _gameMenus;
    
    [Header("Player UI")]
    GameObject _healthBar;
    bool _movingHealth = false;
    float _startHealthTime;
    float _currLerpHealth;
    [SerializeField]
    float _healthBarLerpDuration;
    float h0, h1;
    GameObject _specialBar;
    bool _movingSpecial = false;
    float _startSpecialTime;
    float _currSpecial;
    [SerializeField]
    float _specialBarLerpDuration;
    float s0, s1;
    [SerializeField]
    GameObject _winImage;
    [SerializeField]
    GameObject _loseImage;

    [Header("Player Variables")]
    [SerializeField]
    float _playerHealth;
    float _maxHealthValue;
    [SerializeField]
    float _playerDamage;
    [SerializeField]
    float _playerDamageDivideOffsetForSpecialBosses;
    [SerializeField]
    float _playerSpeed;
    [SerializeField]
    float _speedWhileSwinging;
    float _currPlayerSpeed;
    [SerializeField]
    float _playerCutsceneMovementDuration;
    [SerializeField]
    float _collisionDetectDist;
    Vector3 _playerStartPos;
    Quaternion _playerStartRot;
    Vector3 _playerMenuPos;
    Quaternion _playerMenuRot;
    Vector3 _move;
    Vector3 _look;
    Vector3 _currLook;
    [SerializeField]
    float _iFrameTime;
    GameObject _myRenderer;
    Animator _myAnimations;
    RaycastHit hit;

    bool _doingSomething = false;
    bool _isIntroPlaying = false;
    bool _healing = false;
    bool _invincible = false;
    bool _blinkin = false;

    [Header("Sword Variables")]
    [SerializeField]
    float _swordSwingKnockback;
    [SerializeField]
    float _swordSlashDist;
    [SerializeField]
    float _swordCurveAngle;
    float _calcAngle;
    [SerializeField]
    float _swordSwingDuration;
    float _currentSwingTime;
    float _SwingStartTime;
    Vector3 c0, c1;
    RaycastHit _swordHit;
    float _swing;
    [SerializeField]
    float _numOfSwordCasts;
    [SerializeField]
    bool _debugSword;

    [Header("Back Slash Wave Varibales")]
    [SerializeField]
    GameObject BWPrefab;
    float _currSpecialAmount = 0;
    [SerializeField]
    float _MaxSpecialAmount;
    [SerializeField]
    float _specialInc;

    [SerializeField]
    GameManager _managerRef;
    CameraFollow _cameraRef;
    BossEnemy _bossImFighting;
    
    Interactions _whatImDoing = Interactions.NONE;
    MenuOrient _menuOrient = MenuOrient.VERT;


    //Johns Checkpoint Variables
    bool reachCheckpoint;
    Vector3 checkpointPos;

    // Use this for initialization
    void Awake()
    {
        if (Instance == this)
        {
            _menuRef = Menuing.Instance;
            _managerRef = GameManager.Instance;
            _managerRef.SetGameReset += ResetPlayer;

            _playerMenuPos = transform.position;
            _playerMenuRot = transform.rotation;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Debug.Log("player copy Destroyed");
            Destroy(gameObject);
        }

        _playerStartPos = _playerMenuPos;
        _playerStartRot = _playerMenuRot;
        
        _myRenderer = transform.GetChild(0).gameObject;
        _myAnimations = GetComponent<Animator>();

        reachCheckpoint = false;
    }

    //Secondary Initialization
    private void Start()
    {
        _gameMenus = new List<GameObject>();
        _gameMenus = _menuRef.GetMenus;

        _maxHealthValue = _playerHealth;
        _currSpecialAmount = 0;

        _healthBar = _gameMenus[2].transform.GetChild(0).gameObject;
        _specialBar = _gameMenus[2].transform.GetChild(1).gameObject;
        _specialBar.GetComponent<Image>().fillAmount = 0;
    }

    //Called to reset the Player Stats
    public void ResetPlayer()
    {
        _playerHealth = _maxHealthValue;
        _healthBar.GetComponent<Image>().fillAmount = _playerHealth / _maxHealthValue;

        _currSpecialAmount = 0;
        _specialBar.GetComponent<Image>().fillAmount = _currSpecialAmount / _MaxSpecialAmount;

        

        _currPlayerSpeed = _playerSpeed;
        StopAllCoroutines();
        _doingSomething = false;
        _invincible = false;
        _blinkin = false;
        _inMenu = false;
        _inCutscene = false;
        _movingHealth = false;
        _movingSpecial = false;
        _bossCutsceneInit = false;
        _myRenderer.SetActive(true);

        if (reachCheckpoint == true)                        //if the player has reached a checkpoint
            transform.position = checkpointPos;             //spawn player at checkpoint
        else
            transform.position = _playerStartPos;           //else spawn player at players start position
        _whatImDoing = Interactions.NONE;
        _myAnimations.Play("StandingIdle", 0);
    }

    // Update is called once per frame
    void Update () {

        if(_inMenu)                                         //if the player is in the menu
        {
            if(!_menuRef.AreCreditsRolling)                 //if the credits arent rolling in the menuing script
            {
                MoveMenu();                                 //move menu function
                MenuInput();                                //menu input 
            }
            else
            {
                if (Input.GetButtonDown("MenuSelect"))      //if the player presses down menu select
                {
                    _menuRef.StopCredits();                 //stop credits function in the menuing script
                }
            }

        }
        else if(_inCutscene)                                //if the player is in a cutscene
        {
            _startHealthTime = Time.time;                   //player heathtime equals current time
            _startSpecialTime = Time.time;                  //players special time is current time
            if(_isIntroPlaying)                             //if the intro is playing
            {
                MoveIntoPosition();                         //move player into position function
            }
        }
        else
        {
            CheckForPause();                                //check for pause function
            if (_movingHealth)                              //if moving health is true
            {
                LerpHealthBar();                            //lerp health bar function
            }

            if(_movingSpecial)                              //if moving special
            {
                LerpSpecialBar();                           //lerp special bar function
            }
            CheckForMovement();                             //check for movement function

            if (!_doingSomething)                           //if not doing something
            {
                LookAround();                               //look around function
                CheckForActionInput();                      //check for action input
            }
            else
            {
                WhatAmIEvenDoing();                         //else what are you doing with your life. theres so much meaning to this universe and instead you are reading this extrordinarily long comment which really doesnt have a point much like, i assume, this function that probably just checks to see what else the player can do/ why it isnt doing what its supposed to
            }
        }
	}

    //Called every frame to see if the input to bring up the pause menu is pressed
    private void CheckForPause()                            //check for pause function
    {
        if(Input.GetButtonDown("Pause"))                    //if the player presses the pause button
        {
            _menuRef.Pause();                               //start pause function from the menuing script
        }
    }

    //Checks to see what orientation the menu is in (Vert/Horiz)
    //looks for input on control stick to see if the player moved in correct direction
    private void MoveMenu()                                 //move menu script
    {
        if(_menuOrient == MenuOrient.VERT)                  //if menu orient is equal to menu orient vert
        {
            float _updown = Input.GetAxis("VertMove");      //updown is equal to vertical movement
            if(_updown > 0.5f)                              //if there is some vert movement greater than 0.5
            {
                _menuRef.MenuUpOrDown(true);                //MenuUpOrDown from the Menuing Script is true
            }
            else if(_updown < -0.5f)                        //else if its less than -0.05f
            {
                _menuRef.MenuUpOrDown(false);               //MenuUpOrDown from the Menuing Script is false
            }
        }
        else                                                //else 
        {
            float _leftright = Input.GetAxis("HorizMove");  //_leftright equals horizontal movement
            if(_leftright > 0.5f)                           //if leftright is greater than or equal to 0.5f
            {
                _menuRef.MenuUpOrDown(true);                //the MenuUpOrDown from menuing script is true
            }
            else if (_leftright < -0.5f)                    //else if leftright is left than 0.5f
            {
                _menuRef.MenuUpOrDown(false);               //MenuUpOrDown from the Menuing Script is false
            }
        }
    }

    //checks to see if action button is pressed
    //OnClicks the button that is selected
    private void MenuInput()
    {
        if(Input.GetButtonDown("MenuSelect"))           //if the player presses the menu select button
        {
            _menuRef.SelectButton();                    //start the selectButton Function from the Menuing Script
        }
    }

    //increments Health bar if the player is damaged or healing
    private void IncHealthMeter(float _amount, bool PositiveIncrementQuestionMark)
    {
        h0 = _playerHealth;                                         //h0 equals playerHealth

        if (PositiveIncrementQuestionMark)                          //if the positiveIncrementQuestionMArk is true
        {
            _playerHealth += _amount;                               //playerhealth += the amount given
            if (_playerHealth > _maxHealthValue)                    //if player health ever ends up above maximum health
            {
                _playerHealth = _maxHealthValue;                    //player health is equal to the max possible health
            }
        }
        else
        {
            _playerHealth -= _amount;                               //otherwise, take the amount given away from the current player health
            if (_playerHealth < 0)
            {
                _playerHealth = 0;                                  //if the players health ends up below zero, make the player health zero
            }
        }

        h1 = _playerHealth;                                         //h1 equals player health
        _startHealthTime = Time.time;                               //start health time is equal to current time
        _movingHealth = true;                                       //moving health is true

    }

    //lerps the onscreen health bar to give a smooth transistion of hearts
    private void LerpHealthBar()                    
    {
        _currLerpHealth = (Time.time - _startHealthTime) / _healthBarLerpDuration;          //current lerp health is equal to current time minus start health time then divided by lerp health duration

        float h01;                                                                          //float h01

        h01 = (1 - _currLerpHealth) * h0 + _currLerpHealth * h1;                            //h01 equals 1 minus current lerp health times 0 plus current lerp health times h1

        if (_currLerpHealth >= 1)                                                           //if current lerp health is greater than one
        {
            _currLerpHealth = 1;                                                            //current lurp health is equal to one

            _movingHealth = false;                                                          //moving health equals false 
            _healing = false;                                                               //healing is false
        }

        _healthBar.GetComponent<Image>().fillAmount = h01 / _maxHealthValue;                //the healthbar image is filled h01 divided by max health value
    }

    //increments Special bar if the player is hits and enemy or uses special
    private void IncSpecialMeter(float _amount, bool PositiveIncrementQuestionMark)
    {
        s0 = _currSpecialAmount;                                                            //s0 is the current special amount

        if (PositiveIncrementQuestionMark)                                                  //if its a positive increment
        {
            _currSpecialAmount += _amount;                                                  //add the amount given to the current special amount
            if (_currSpecialAmount > _MaxSpecialAmount)                                     //if the current special amount is greater than the max
            {
                _currSpecialAmount = _MaxSpecialAmount;                                       //make the current the max
            }
        }
        else                                                                                //if its a negative increment
        {
            _currSpecialAmount -= _amount;                                                  //take away the amount given to the current special amount
            if (_currSpecialAmount < 0)                                                     //if the current is less than 0
            {
                _currSpecialAmount = 0;                                                     //make it zero
            }
        }

        s1 = _currSpecialAmount;                                                            //s1 is the current special amount
        _startSpecialTime = Time.time;                                                      //strat special time is equal to current time
        _movingSpecial = true;                                                              //moving special is true
    }

    //lerps the onscreen special bar to give a smooth transistion of buildup
    private void LerpSpecialBar()
    {
        _currSpecial = (Time.time - _startSpecialTime) / _specialBarLerpDuration;           //current special is equal to the current time minus the start special time divided by the special bar lerp duration
    
        float s01;                                                                          

        s01 = (1 - _currSpecial) * s0 + _currSpecial * s1;                                  //s01 is equal to 1 minus current special times s0 plus current special times s1

        if (_currSpecial >= 1)                                                              //if current special is greate rthan or equal to 1
        {
            _currSpecial = 1;                                                               //make the current special 1

            _movingSpecial = false;                                                         //moving special is false
        }

        _specialBar.GetComponent<Image>().fillAmount = s01 / _MaxSpecialAmount;             //special bar image gets filled s01 divided by the maximum special amount
    }

    //call when the player is about to go into a boss fight
    public void GoingToIntroCutscene(BossEnemy thingToInit)
    {
        
        _bossImFighting = thingToInit;                                                      //the boss im fighting from the boss enemy script equals thing to initialize
        cs0 = transform.position;                                                           //cs0 (Vector3) is equal to the transform position
        cs1 = _bossImFighting.GetIntroPos;                                                  //cs1 is equal to the bosses intro position
        cs1.x = transform.position.x;                                                       //cs1's x positon is equal to the players current x
        cs1.y = transform.position.y;                                                       //cs1's y position is equal to the players current y
        _startCutsceneTime = Time.time;                                                     //start cutscene is equal to the current time
        _myAnimations.Play("StandingIdle", 0);                                              //play animation standing idle 
        _isIntroPlaying = true;                                                             //the intro is playing
        _inCutscene = true;                                                                 //the player is in a cutscene
    }

    //call when player has defeated a boss
    public void GoingToOutroCutscene()
    {
        _isIntroPlaying = false;                                                            //the intro is not playing
        _inCutscene = true;                                                                 //the player is in a cutscene
    }

    //Once in a cutscene
    private void MoveIntoPosition()                                                         //move into position
    {
        _currCutsceneTime = (Time.time - _startCutsceneTime) / _playerCutsceneMovementDuration;         //current cutscene time is equal to the current time minus the start cutscene time divided by the player cutscene movement duration

        if(_currCutsceneTime >= 1)                                                          //if the current cutscene time is greater than 1
        {
            _currCutsceneTime = 1;                                                          //then the current cutscene time is 1
            if(!_bossCutsceneInit)
            {
                _bossImFighting.Init();                                                     //initialize the boss im bout to fight
                _bossCutsceneInit = true;                                                   //boss cutscene starts
            }
           
        }

        Vector3 cs01;

        cs01 = (1 - _currCutsceneTime) * cs0 + _currCutsceneTime * cs1;                         

        transform.position = cs01;                                                              //the players current position is equal to cs01
        transform.LookAt(transform.position + Vector3.forward);                                 //make the player look at its current position plus forward
    }

    //checks for input from the stick and moves the player in that direction
    //casts raycast for collision detection in same direction
    //casts raycast to look below for various things as well
    private void CheckForMovement()
    {
        _move = new Vector3(Input.GetAxis("HorizMove"), 0, -Input.GetAxis("VertMove"));         //move is euqal to the new vecto3 which only allows forward, backward, left, and right
        

        if (_move.magnitude <= 0)                                                               //if the moves magnitude is less than or equal to zero
        {
            if(!_doingSomething)                                                                //if not doing anything
            {
                _myAnimations.Play("StandingIdle", 0);                                          //play the idle animation
            }
            _move = Vector3.zero;                                                               //there is no movement
        }
        else                                                                                    //if the move magnitude is greater than zero
        {
            if (!_doingSomething)                                                               //if  not doing something
            {
                _myAnimations.Play("Moving", 0);                                                //play the moving animation
            }
        }

        if (Physics.Raycast(transform.position + Vector3.up, _move, out hit, _collisionDetectDist))     //create a raycast going forward
        {
            GameObject thingHit = hit.collider.gameObject;                                              //thingHit is whatever the raycast hit
            if (thingHit.GetComponent<WinScript>())                                                     //if the thing hit is a win script
            {
                _move = Vector3.zero;                                                                   //there is no movement
                if (thingHit.GetComponent<WinScript>().IsLastLevel())                                   //if the win script is attached to the last level
                {
                    EndLevel(true);                                                                     //end level is equal to true
                }
                else
                {
                    _menuRef.NextLevel();                                                               //otherwise go to the next level
                }
            }
            else if (thingHit.GetComponent<DungeonMechanic>())                                          //else if the player hit a dungeon mechanic
            {
                thingHit.GetComponent<DungeonMechanic>().Init();                                        //initialize the mechanic
            }
            else if (thingHit.GetComponent<BaseEnemy>())                                                //else if the player hit a base enemy
            {
                TakeDamage(thingHit.GetComponent<BaseEnemy>().GetDamage);                               //have the player take damage
            }
            else if (thingHit.GetComponent<BossEnemy>())                                                //else if the player hit a boss
            {
                TakeDamage(thingHit.GetComponent<BossEnemy>().GetDamage);                               //have the player take damage
            }
<<<<<<< HEAD
            else if(thingHit.GetComponent<CathedralProjectile>())
            {
                TakeDamage(thingHit.GetComponent<CathedralProjectile>().GetDamage);
            }
            else if(!thingHit.GetComponent<HealingGrace>() || !thingHit.GetComponent<SpikeTrap>())
=======
            else if(!thingHit.GetComponent<HealingGrace>() || !thingHit.GetComponent<SpikeTrap>())      //else if the player did not hit any of the above and it isnt a spike trap or healing spot
>>>>>>> JohnsInCaseIFuckUpBranch
            {
                _move = Vector3.zero;                                                                   //you should probably stop cause i got not clue what you hit homeboy
            }
        }

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, _collisionDetectDist))      //creates a raycast infront of the player
        {
            GameObject thingHit = hit.collider.gameObject;                                                      //thing his is whatever the player hit

            if (thingHit.GetComponent<HealingGrace>())                                                          //if the player hit a healing spot
            {
                if (thingHit.GetComponent<HealingGrace>().isCheckpoint)                                          //if this healing grace is a checkpoint
                {
                    reachCheckpoint = true;                                                                     //the player has reached a checkpoint
                    checkpointPos.x = thingHit.GetComponent<HealingGrace>().transform.position.x;               //the checkpoint's x position is whatever the healing graces is
                    checkpointPos.z = thingHit.GetComponent<HealingGrace>().transform.position.z;               //the checkpoint's y position is whatever the healing graces is
                }

                if (!_healing && _playerHealth < _maxHealthValue)                                               //if the player is not healing and current health is less than max health
                {
                    Debug.Log("am hurt");
                    if (thingHit.GetComponent<HealingGrace>().GetHealingAmount > 0)                             //if healing graces healing amount is greater than 0
                    {
                        _healing = true;                                                                        //healing is true
                        float _amountHeal = _playerHealth + thingHit.GetComponent<HealingGrace>().GetHealingAmount;     //amountHeal is the playerhealth plus whatever the healing amount is
                        if (_amountHeal > _maxHealthValue)                                                      //if the healing amount is greater than the maximum health value
                        {
                            float leftoverHealth = _amountHeal - _maxHealthValue;                               //leftover health is equal to amount heal minus the max health value
                            _amountHeal -= leftoverHealth;                                                      //amount heal is subrtacts by the leftoverhealth

                            thingHit.GetComponent<HealingGrace>().GetHealingAmount = leftoverHealth;            //the healing spots healing amount is now equal to the leftover health
                            thingHit.GetComponent<HealingGrace>().StartFade();                                  //start the healing grace's fade
                        }
                        else                                                                                    //if the amount heal is less than or equal to max possible health
                        { 
                            thingHit.GetComponent<HealingGrace>().GetHealingAmount = 0;                         //healing graces healing amount is zero
                            thingHit.GetComponent<HealingGrace>().StartFade();                                  //start healing grace's fade
                        }
                        _amountHeal -= _playerHealth;                                                           //the amount heal is minus whatever the player health is


                        IncHealthMeter(_amountHeal, true);                                                      //increment the health bar whatever amount heal is

                    }
                }


            }
            else if(thingHit.GetComponent<SpikeTrap>())                                                         //if the playe hit a spike trap
            {
                thingHit.GetComponent<SpikeTrap>().StartTell();                                                 
            }
            else if(thingHit.GetComponent<HazardFloor>())
            {
                TakeDamage(thingHit.GetComponent<HazardFloor>().GetHazardDamage);
            }
        }

        if (_doingSomething)
        {
            _currPlayerSpeed = _speedWhileSwinging;
        }
        else
        {
            _currPlayerSpeed = _playerSpeed;
        }

        transform.position += _move * _currPlayerSpeed * Time.deltaTime;
    }


    //checks player input of left stick to rotate player
    private void LookAround()
    {   
        _look = new Vector3(Input.GetAxis("HorizLook"), 0, -Input.GetAxis("VertLook"));

        if (_look.magnitude > 0.5f)
        {
            _currLook = _look;
        }

        transform.rotation = Quaternion.LookRotation(_currLook, Vector3.up);
    }

    //checks to see if the player has swung the sword
    //checks to see if the player has used their special
    private void CheckForActionInput()
    {
        _swing = Input.GetAxis("SwordSwing");
        if (_swing < 0)
        {
            ImaStartSwinging();
        }
        else if (_swing > 0)
        {
            SpecialUsed();
        }
    }

    //called right before the player attacks
    private void ImaStartSwinging()
    {
        //Debug.Log("Start the swing");
        _doingSomething = true;
        _whatImDoing = Interactions.ATTACKING;
        _calcAngle = 0;
        _myAnimations.Play("SwordSwing", 0);
        _SwingStartTime = Time.time;
        AudioManager.instance.Swing();
    }

    //resets the player attack
    public void ResetSword()
    {
        _doingSomething = false;
        _whatImDoing = Interactions.NONE;
    }

    //called right before the player uses their special
    //checks to see if the player has full special meter before using
    private void SpecialUsed()
    {
        if (_currSpecialAmount >= _MaxSpecialAmount)
        {
            _myAnimations.Play("SwordSwing", 0);
            IncSpecialMeter(_MaxSpecialAmount, false);
            _doingSomething = true;
            Instantiate<GameObject>(BWPrefab, transform.position + transform.forward + Vector3.up, transform.rotation);
            _SwingStartTime = Time.time;
            _whatImDoing = Interactions.WAVESPECIAL;
            AudioManager.instance.FireAttack();
        }
    }

    //checks to see what type of attack the player is doing
    private void WhatAmIEvenDoing()
    {
        switch (_whatImDoing)
        {
            case Interactions.NONE:
                break;
            case Interactions.ATTACKING:
                _currentSwingTime = (Time.time - _SwingStartTime) / _swordSwingDuration;

                if (_currentSwingTime >= 1)
                {
                    _currentSwingTime = 1;
                    ResetSword();
                }
                else
                {
                    float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _swordSlashDist;
                    float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _swordSlashDist;

                    Vector3 SwingDir = (transform.forward * Zpos) + (transform.right * Xpos);

                    _calcAngle += _swordCurveAngle / _numOfSwordCasts;

                    if (_debugSword)
                    {
                        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + SwingDir);
                    }

                    if (Physics.Raycast(transform.position + Vector3.up, SwingDir, out _swordHit, _swordSlashDist))
                    {
                        GameObject thingHit = _swordHit.collider.gameObject;

                        if (thingHit.GetComponent<BaseEnemy>())
                        {
                            if (!thingHit.GetComponent<BaseEnemy>().AmHit)
                            {
                                thingHit.GetComponent<BaseEnemy>().GotHit(transform.forward, _swordSwingKnockback);
                                IncSpecialMeter(_specialInc, true);
                            }
                        }
                        else if (thingHit.GetComponent<BossEnemy>())
                        {
                            if (!thingHit.GetComponent<BossEnemy>().AmHit && !thingHit.GetComponent<BossEnemy>().AmInvincible)
                            {
                                if(thingHit.GetComponent<ColorBossGlhost>())
                                {
                                    thingHit.GetComponent<BossEnemy>().GotHit(_playerDamage/_playerDamageDivideOffsetForSpecialBosses);

                                    IncSpecialMeter(_specialInc, true);
                                }
                                else
                                {
                                    thingHit.GetComponent<BossEnemy>().GotHit(_playerDamage);

                                    IncSpecialMeter(_specialInc, true);
                                }
                               
                            }
                        }
                        else if(thingHit.GetComponent<TrapLever>())
                        {
                            thingHit.GetComponent<TrapLever>().StartRotation();
                        }
                        else if(thingHit.GetComponent<CathedralProjectile>())
                        {
                            thingHit.GetComponent<CathedralProjectile>().HitProjectile(transform.forward);
                        }
                    }
                }
                break;
            case Interactions.WAVESPECIAL:
                _currentSwingTime = (Time.time - _SwingStartTime) / _swordSwingDuration;

                if(_currentSwingTime > 1)
                {
                    _currentSwingTime = 1;
                    ResetSword();
                }
                break;
            default:
                break;
        }
    }

    //called by objects that damage the player 
    //damages the player
    public void TakeDamage(float _damageTaken)
    {
        if (!_invincible)
        {
            _invincible = true;
            IncHealthMeter(_damageTaken, false);

            if (_playerHealth > 0)
            {
                StartCoroutine(IFrames());
            }
            else
            {
                _myAnimations.Play("Death");
                EndLevel(false);
            }
        }        
    }

    //started when the player gets hit
    //makes them invulnerable for a short time
    IEnumerator IFrames()
    {
        _blinkin = true;
        StartCoroutine(Blinkin());

        yield return new WaitForSeconds(_iFrameTime);

        _blinkin = false;
        _myRenderer.SetActive(true);

        _invincible = false;
        StopCoroutine(IFrames());
    }

    //called to show that the player has Iframes
    IEnumerator Blinkin()
    {
        while(_blinkin)
        {
            yield return new WaitForSeconds(.15f);
            if (_myRenderer.activeInHierarchy)
            {
                _myRenderer.SetActive(false);
            }
            else
            {
                _myRenderer.SetActive(true);
            }
        }

        _myRenderer.SetActive(true);
        StopCoroutine(Blinkin());
    }

    //checks to see if the player has reached the end of a level
    private void EndLevel(bool didtheywin)
    {
        _menuRef.SetMenu(WhichUIMenu.WIN);
        _winImage = _menuRef.GetMenus[(int)WhichUIMenu.WIN].transform.GetChild(1).gameObject;
        _loseImage = _menuRef.GetMenus[(int)WhichUIMenu.WIN].transform.GetChild(2).gameObject;
        if (didtheywin)
        {
            _winImage.SetActive(true);
            _loseImage.SetActive(false);
            _menuRef.StartCredits();
        }
        else
        {
            _winImage.SetActive(false);
            _loseImage.SetActive(true);
        }
        _inMenu = true;
    }

    //various getters and setters

    public Menuing SetMenuRef { get { return _menuRef; } set { _menuRef = value; } }
    public bool InMenu { get { return _inMenu; } set { _inMenu = value; } }
    public List<GameObject> SetMenus { get { return _gameMenus; } set { _gameMenus = value; } }
    public MenuOrient SetOrientation { get { return _menuOrient; } set { _menuOrient = value; } }

    public CameraFollow GetCamera { get { return _cameraRef; } set { _cameraRef = value; } }

    public Animator GetPlayerAnimator { get { return _myAnimations; } }
    public Vector3 SetStartPos { get { return _playerStartPos; } set { _playerStartPos = value; } }
    public Quaternion SetStartRot { get { return _playerStartRot; } set { _playerStartRot = value; } }
    public Vector3 GetMenuPos { get { return _playerMenuPos; } set { _playerMenuPos = value; } }
    public Quaternion GetMenuRot { get { return _playerMenuRot; } set { _playerMenuRot = value; } }

    public bool AmInCutscene { get { return _inCutscene; } set { _inCutscene = value; } }
    public void ResetBossCutSceneCheck() { _bossCutsceneInit = false; }
    public BossEnemy BossImFighting { get { return _bossImFighting; } set { _bossImFighting = value; } }

    public GameObject SetWinImage { set { _winImage = value; } }
    public GameObject SetLoseImage { set { _loseImage = value; } }
}
