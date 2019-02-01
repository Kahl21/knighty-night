﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFireStatueTrap : BaseTrap {

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
    float _beginningDelay;
    [SerializeField]
    float _fireDelay;
    [SerializeField]
    float _fireIncDuration;
    [SerializeField]
    float _burningDuration;
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
    ParticleSystem[] _myFire;

    FireState _mystate = FireState.NONE;

    //Start Function
    protected override void Start()
    {
        _myFire = new ParticleSystem[4];
        for (int fireInstance = 0; fireInstance < 4; fireInstance++)
        {
            _myFire[fireInstance] = transform.GetChild(fireInstance).transform.GetComponent<ParticleSystem>();
        }
        
        
        for (int i = 0; i < _myFire.Length; i++)
        {
            _myFire[i].Stop();
            Debug.Log("Stop system: " + _myFire[i]);
        }
        base.Start();
    }

    //Initilizes trap
    public override void Init()
    {
        base.Init();
        _currDetectDistance = 0;
        _startDelay = Time.time;
        _mystate = FireState.BEGINBURNING;
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
    void StartingDelay()
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

            
            for (int j = 0; j < _myFire.Length; j++)
            {
                _myFire[j].Play();
            }
            _startDelay = Time.time;
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

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            for (int f = 0; f < _myFire.Length; f++)
            {
                _myFire[f].Stop();
            }
            
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
            _mystate = FireState.FLAMEDELAY;
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
