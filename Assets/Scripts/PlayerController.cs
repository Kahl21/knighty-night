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
        
        transform.position = _playerStartPos;
        _whatImDoing = Interactions.NONE;
        _myAnimations.Play("StandingIdle", 0);
    }

    // Update is called once per frame
    void Update () {

        if(_inMenu)
        {
            if(!_menuRef.AreCreditsRolling)
            {
                MoveMenu();
                MenuInput();
            }
            else
            {
                if (Input.GetButtonDown("MenuSelect"))
                {
                    _menuRef.StopCredits();
                }
            }

        }
        else if(_inCutscene)
        {
            _startHealthTime = Time.time;
            _startSpecialTime = Time.time;
            if(_isIntroPlaying)
            {
                MoveIntoPosition();
            }
        }
        else
        {
            CheckForPause();
            if (_movingHealth)
            {
                LerpHealthBar();
            }

            if(_movingSpecial)
            {
                LerpSpecialBar();
            }
            CheckForMovement();

            if (!_doingSomething)
            {
                LookAround();
                CheckForActionInput();
            }
            else
            {
                WhatAmIEvenDoing();
            }
        }
	}

    //Called every frame to see if the input to bring up the pause menu is pressed
    private void CheckForPause()
    {
        if(Input.GetButtonDown("Pause"))
        {
            _menuRef.Pause();
        }
    }

    //Checks to see what orientation the menu is in (Vert/Horiz)
    //looks for input on control stick to see if the player moved in correct direction
    private void MoveMenu()
    {
        if(_menuOrient == MenuOrient.VERT)
        {
            float _updown = Input.GetAxis("VertMove"); 
            if(_updown > 0.5f)
            {
                _menuRef.MenuUpOrDown(true);
            }
            else if(_updown < -0.5f)
            {
                _menuRef.MenuUpOrDown(false);
            }
        }
        else
        {
            float _leftright = Input.GetAxis("HorizMove");
            if(_leftright > 0.5f)
            {
                _menuRef.MenuUpOrDown(true);
            }
            else if (_leftright < -0.5f)
            {
                _menuRef.MenuUpOrDown(false);
            }
        }
    }

    //checks to see if action button is pressed
    //OnClicks the button that is selected
    private void MenuInput()
    {
        if(Input.GetButtonDown("MenuSelect"))
        {
            _menuRef.SelectButton();
        }
    }

    //increments Health bar if the player is damaged or healing
    private void IncHealthMeter(float _amount, bool PositiveIncrementQuestionMark)
    {
        h0 = _playerHealth;

        if (PositiveIncrementQuestionMark)
        {
            _playerHealth += _amount;
            if (_playerHealth > _maxHealthValue)
            {
                _playerHealth = _maxHealthValue;
            }
        }
        else
        {
            _playerHealth -= _amount;
            if (_playerHealth < 0)
            {
                _playerHealth = 0;
            }
        }

        h1 = _playerHealth;
        _startHealthTime = Time.time;
        _movingHealth = true;

    }

    //lerps the onscreen health bar to give a smooth transistion of hearts
    private void LerpHealthBar()
    {
        _currLerpHealth = (Time.time - _startHealthTime) / _healthBarLerpDuration;

        float h01;

        h01 = (1 - _currLerpHealth) * h0 + _currLerpHealth * h1;

        if (_currLerpHealth >= 1)
        {
            _currLerpHealth = 1;

            _movingHealth = false;
            _healing = false;
        }

        _healthBar.GetComponent<Image>().fillAmount = h01 / _maxHealthValue;
    }

    //increments Special bar if the player is hits and enemy or uses special
    private void IncSpecialMeter(float _amount, bool PositiveIncrementQuestionMark)
    {
        s0 = _currSpecialAmount;

        if (PositiveIncrementQuestionMark)
        {
            _currSpecialAmount += _amount;
            if (_currSpecialAmount > _MaxSpecialAmount)
            {
                _currSpecialAmount = _MaxSpecialAmount;
            }
        }
        else
        {
            _currSpecialAmount -= _amount;
            if (_currSpecialAmount < 0)
            {
                _currSpecialAmount = 0;
            }
        }

        s1 = _currSpecialAmount;
        _startSpecialTime = Time.time;
        _movingSpecial = true;
    }

    //lerps the onscreen special bar to give a smooth transistion of buildup
    private void LerpSpecialBar()
    {
        _currSpecial = (Time.time - _startSpecialTime) / _specialBarLerpDuration;
    
        float s01;

        s01 = (1 - _currSpecial) * s0 + _currSpecial * s1;

        if (_currSpecial >= 1)
        {
            _currSpecial = 1;

            _movingSpecial = false;
        }

        _specialBar.GetComponent<Image>().fillAmount = s01 / _MaxSpecialAmount;
    }

    //call when the player is about to go into a boss fight
    public void GoingToIntroCutscene(BossEnemy thingToInit)
    {
        
        _bossImFighting = thingToInit;
        cs0 = transform.position;
        cs1 = _bossImFighting.GetIntroPos;
        cs1.x = transform.position.x;
        cs1.y = transform.position.y;
        _startCutsceneTime = Time.time;
        _myAnimations.Play("StandingIdle", 0);
        _isIntroPlaying = true;
        _inCutscene = true;
    }

    //call when player has defeated a boss
    public void GoingToOutroCutscene()
    {
        _isIntroPlaying = false;
        _inCutscene = true;
    }

    //Once in a cutscene
    private void MoveIntoPosition()
    {
        _currCutsceneTime = (Time.time - _startCutsceneTime) / _playerCutsceneMovementDuration;

        if(_currCutsceneTime >= 1)
        {
            _currCutsceneTime = 1;
            if(!_bossCutsceneInit)
            {
                _bossImFighting.Init();
                _bossCutsceneInit = true;
            }
           
        }

        Vector3 cs01;

        cs01 = (1 - _currCutsceneTime) * cs0 + _currCutsceneTime * cs1;

        transform.position = cs01;
        transform.LookAt(transform.position + Vector3.forward);
    }

    //checks for input from the stick and moves the player in that direction
    //casts raycast for collision detection in same direction
    //casts raycast to look below for various things as well
    private void CheckForMovement()
    {
        _move = new Vector3(Input.GetAxis("HorizMove"), 0, -Input.GetAxis("VertMove"));
        

        if (_move.magnitude <= 0) 
        {
            if(!_doingSomething)
            {
                _myAnimations.Play("StandingIdle", 0);
            }
            _move = Vector3.zero;
        }
        else
        {
            if (!_doingSomething)
            {
                _myAnimations.Play("Moving", 0);
            }
        }

        if (Physics.Raycast(transform.position + Vector3.up, _move, out hit, _collisionDetectDist))
        {
            GameObject thingHit = hit.collider.gameObject;
            if (thingHit.GetComponent<WinScript>())
            {
                _move = Vector3.zero;
                if (thingHit.GetComponent<WinScript>().IsLastLevel())
                {
                    EndLevel(true);
                }
                else
                {
                    _menuRef.NextLevel();
                }
            }
            else if (thingHit.GetComponent<DungeonMechanic>())
            {
                thingHit.GetComponent<DungeonMechanic>().Init();
            }
            else if (thingHit.GetComponent<BaseEnemy>())
            {
                TakeDamage(thingHit.GetComponent<BaseEnemy>().GetDamage);
            }
            else if (thingHit.GetComponent<BossEnemy>())
            {
                TakeDamage(thingHit.GetComponent<BossEnemy>().GetDamage);
            }
            else if(!thingHit.GetComponent<HealingGrace>() || !thingHit.GetComponent<SpikeTrap>())
            {
                _move = Vector3.zero;
            }

        }

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, _collisionDetectDist))
        {
            GameObject thingHit = hit.collider.gameObject;

            if (thingHit.GetComponent<HealingGrace>())
            {
                if (!_healing && _playerHealth < _maxHealthValue)
                {
                    Debug.Log("am hurt");
                    if (thingHit.GetComponent<HealingGrace>().GetHealingAmount > 0)
                    {
                        _healing = true;
                        float _amountHeal = _playerHealth + thingHit.GetComponent<HealingGrace>().GetHealingAmount;
                        if (_amountHeal > _maxHealthValue)
                        {
                            float leftoverHealth = _amountHeal - _maxHealthValue;
                            _amountHeal -= leftoverHealth;

                            thingHit.GetComponent<HealingGrace>().GetHealingAmount = leftoverHealth;
                            thingHit.GetComponent<HealingGrace>().StartFade();
                        }
                        else
                        {
                            thingHit.GetComponent<HealingGrace>().GetHealingAmount = 0;
                            thingHit.GetComponent<HealingGrace>().StartFade();
                        }
                        _amountHeal -= _playerHealth;


                        IncHealthMeter(_amountHeal, true);

                    }
                }
            }
            else if(thingHit.GetComponent<SpikeTrap>())
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
