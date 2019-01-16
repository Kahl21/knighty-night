﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour {

    protected enum BossAI
    {
        NONE,
        INTRO,
        FIGHTING,
        OUTRO
    }

    protected BossAI _myAI = BossAI.NONE;


    protected NavMeshAgent _enemyAgent;

    [Header("Boss UI Variables")]
    [SerializeField]
    protected string _bossName;
    [SerializeField]
    protected string _bossSubTitle;
    [SerializeField]
    protected float _healthLagDuration;
    [SerializeField]
    protected float _healthUpdateDuration;
    protected float _currHealthTime;
    protected float _startHealthTime;
    protected float h0, h1, h01;
    protected GameObject _bossBar;
    protected Image _actualBossHealthBar;
    protected Image _laggedBossHealthBar;
    protected Text _bossNameText;

    [Header("Generic Intro Variables")]
    [SerializeField]
    protected GameObject _playerIntroPos;
    [SerializeField]
    protected float _cameraIntroDuration;
    [SerializeField]
    protected float _deathDuration;
    protected bool _showingDeath = false;
    protected Vector3 cam0, cam1;
    protected Vector3 _camOffset;

    [Header("Generic Boss Variables")]
    [SerializeField]
    protected float _maxBossHealth;
    protected float _actualMaxHealth;
    protected float _currBossHealth;
    [SerializeField]
    protected float _bossDamage;
    [SerializeField]
    protected float _bossDefaultSpeed;
    protected float _actualBossSpeed;
    protected float _currSpeed;
    [SerializeField]
    protected float _bossCollisionDetectDistance;
    [SerializeField]
    protected float _InvincibleDuration;
    protected float _currInvincibleTime;
    protected float _startInvincibleTime;
    protected RaycastHit hit;
    protected Vector3 _startPos;
    protected Quaternion _startRot;
    protected MeshRenderer _myRenderer;
    protected Material _myMaterial;
    protected Color _myColor;

    [Header("Generic Hard Boss Values")]
    [SerializeField]
    protected float _maxHardBossHealth;
    [SerializeField]
    protected float _bossHardDefaultSpeed;

    protected bool _endingPlaying = false;
    protected bool _init = false;
    protected bool _laggingHealth = false;
    protected bool _updatingHealth = false;
    protected bool _dead = false;
    protected bool _amHit = false;
    protected bool _invincible = false;

    protected Menuing _menuRef;
    protected PlayerController _playerRef;
    protected GameManager _managerRef;
    protected CameraFollow _cameraRef;
    protected DungeonMechanic _myRoom;
    protected Animator _myAnimations;

    protected bool _hasInit = false;
    
    protected virtual void PlayIntro()
    {

    }

    public virtual void CheckForIntroEnd()
    {

    }

    public virtual void Init()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _myRenderer = GetComponent<MeshRenderer>();
        _myMaterial = _myRenderer.materials[1];
        _myColor = _myMaterial.color;
        _menuRef = Menuing.Instance;
        _playerRef = PlayerController.Instance;
        _managerRef = GameManager.Instance;
        _cameraRef = _playerRef.GetCamera;
        _camOffset = _cameraRef.GetOffset;

        _enemyAgent = GetComponent<NavMeshAgent>();
        _enemyAgent.enabled = false;

        if(!_managerRef.HardModeOn)
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
        if(_managerRef.HasSubtitles)
        {
            _bossNameText.text += ", " + _bossSubTitle;
        }
        _hasInit = true;
    }

    protected virtual void StartFight()
    {

    }

    protected virtual void Update()
    {
        if(_laggingHealth)
        {
            LagHealthBar();
        }
        else if(_updatingHealth)
        {
            UpdateHealthBar();
        }
    }

    protected virtual void LagHealthBar()
    {
        _currHealthTime = (Time.time - _startHealthTime) / _healthLagDuration;

        if(_currHealthTime >= 1)
        {
            _laggingHealth = false;
            _startHealthTime = Time.time;
            _updatingHealth = true;
        }

    }

    protected virtual void UpdateHealthBar()
    {
        _currHealthTime = (Time.time - _startHealthTime) / _healthUpdateDuration;

        h01 = (1 - _currHealthTime) * h0 + _currHealthTime * h1;

        if (_currHealthTime >= 1)
        {
            _currHealthTime = 1;

            _updatingHealth = false;
        }

        _laggedBossHealthBar.fillAmount = h01 / _actualMaxHealth;
    }

    public virtual void GotHit(float _damageTaken)
    {
        if (!_updatingHealth)
        {
            h0 = _currBossHealth;
            _currBossHealth -= _damageTaken;
            h1 = _currBossHealth;

            _actualBossHealthBar.fillAmount = h1 / _actualMaxHealth;
            _startHealthTime = Time.time;
            _laggingHealth = true;
        }
        else
        {
            _updatingHealth = false;

            h0 = h01;
            _currBossHealth -= _damageTaken;
            h1 = _currBossHealth;

            _actualBossHealthBar.fillAmount = h1 / _actualMaxHealth;
            _startHealthTime = Time.time;
            _laggingHealth = true;
        }
    }

    protected virtual void ResetAmHit()
    {
        _currInvincibleTime = (Time.time - _startInvincibleTime) / _InvincibleDuration;

        if (_currInvincibleTime >= 1)
        {
            //Debug.Log("not invincible");
            _currInvincibleTime = 1;

            _invincible = false;
            _amHit = false;
        }
    }

    protected virtual void Die()
    {

    }

    protected virtual void PlayEnd()
    {

    }

    public virtual void MyReset()
    {

    }

    public virtual bool AmHit { get { return _amHit; } }
    public virtual bool AmInvincible { get { return _invincible; } }
    public virtual Vector3 GetIntroPos { get { return _playerIntroPos.transform.position; } }
    public virtual DungeonMechanic SetMyRoom { set { _myRoom = value; } }
    public virtual float GetDamage { get { return _bossDamage; } }
}
