﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFireStatueTrap : BaseTrap {

    private enum FireState
    {
        NONE,
        BEGINBURNING,
        FLAMEDELAY,
        FLAMEON,
        BURNING,
        FLAMEOFF,
        ROOMDONE
    }

    [Header("Flame Statue Variables")]
    [SerializeField]
    public float _beginningDelay;
    [SerializeField]
    float _fireDelay;
    [SerializeField]
    float _fireIncDuration;
    [SerializeField]
    float _burningDuration;
    [SerializeField]
    float _fireDistance;
    float _startDelay;
    float _currDelay;

    [Header("Detection Variables")]
    [SerializeField]
    bool _debugDamageArea;
    [SerializeField]
    float _spaceBetweenRays;
    [SerializeField]
    float _maxDetectDistance;
    float _currDetectDistance;
    RaycastHit hit;
    ParticleSystem _myFire;
    public GameObject bossEntity;

    FireState _mystate = FireState.NONE;

    //Start Function
    protected override void Start()
    {
        _myFire = transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        var main = _myFire.main;
        main.startLifetime = _fireDistance;
        _myFire.Stop();
    }

    //Initilizes trap
    public override void Init()
    {
        base.Init();
        _currDetectDistance = 0;
        _mystate = FireState.NONE;
    }

    private void Update()
    {
        switch (_mystate)
        {
            case FireState.NONE:
                break;
            case FireState.BEGINBURNING:
                StartingDelay();
                break;
            case FireState.FLAMEDELAY:
                StartFire();
                break;
            case FireState.FLAMEON:
                IncreaseFire();
                break;
            case FireState.BURNING:
                BurnBabyBurn();
                break;
            case FireState.FLAMEOFF:
                StopFire();
                break;
            case FireState.ROOMDONE:
                break;
            default:
                break;
        }
    }

    //starts delay to spit out fire
    public void StartingDelay()
    {
        _currDelay = (Time.time - _startDelay) / _beginningDelay;

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _startDelay = Time.time;
            _mystate = FireState.FLAMEDELAY;
        }
    }

    //plays fire animation after a delay
    void StartFire()
    {
        _currDelay = (Time.time - _startDelay) / _fireDelay;

        if(_currDelay >= 1)
        {
            _currDelay = 1;

            _myFire.Play();
            _startDelay = Time.time;
            Debug.Log("Shoot Fire");
            _mystate = FireState.FLAMEON;
        }
    }

    //shoots out three raycasts along with the fire animation
    //player will take damage if he walks through the fire
    void IncreaseFire()
    {
        _currDelay = (Time.time - _startDelay) / _fireIncDuration;

        _currDetectDistance = _maxDetectDistance * _currDelay;

        LookForPlayer();

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _currDetectDistance = _maxDetectDistance;

            _startDelay = Time.time;
            _mystate = FireState.BURNING;
        }
    }

    //Keeps the animation playing and damage raycasts up for a certain amount of time
    void BurnBabyBurn()
    {
        _currDelay = (Time.time - _startDelay) / _burningDuration;
        
        LookForPlayer();
        Debug.Log("burn Player");
        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _myFire.Stop();

            _startDelay = Time.time;
            _mystate = FireState.FLAMEOFF;
        }

    }

    //stops fire animation
    //receeds raycasts with fire animation
    void StopFire()
    {
        _currDelay = (Time.time - _startDelay) / _fireIncDuration;

        _currDetectDistance = _maxDetectDistance * (1 - _currDelay);

        LookForPlayer();

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _currDetectDistance = 0;
            
            _startDelay = Time.time;

            bossEntity.GetComponent<TrapBossGlhost>().trapComplete = true;
            _mystate = FireState.NONE;
        }
    }

    //detecction for fire
    void LookForPlayer()
    {
        for (int i = -1; i < 2; i++)
        {
            Vector3 _RayPos;

            if (_mystate != FireState.FLAMEOFF)
            {
                _RayPos = transform.position + Vector3.up + (transform.right * _spaceBetweenRays * i);
                if (_debugDamageArea)
                {
                    Debug.DrawRay(_RayPos, transform.forward * _currDetectDistance);
                }

                if(Physics.Raycast(_RayPos, transform.forward, out hit, _currDetectDistance))
                {
                    if(hit.collider.GetComponent<PlayerController>())
                    {
                        hit.collider.GetComponent<PlayerController>().TakeDamage(_trapDamage);
                    }
                }
            }
            else
            {
                _RayPos = transform.position + Vector3.up + (transform.right * _spaceBetweenRays * i) + (transform.forward * _maxDetectDistance);
                if (_debugDamageArea)
                {
                    Debug.DrawRay(_RayPos, -transform.forward * _currDetectDistance);
                }

                if (Physics.Raycast(_RayPos, -transform.forward, out hit, _currDetectDistance))
                {
                    if (hit.collider.GetComponent<PlayerController>())
                    {
                        hit.collider.GetComponent<PlayerController>().TakeDamage(_trapDamage);
                    }
                }
            }
        }
    }

    //stops trap once the room is finished
    public override void DisableTrap()
    {
        _mystate = FireState.ROOMDONE;
    }

    //reset function
    public override void ResetTrap()
    {
        _mystate = FireState.NONE;
    }
}
