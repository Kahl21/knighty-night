﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStatueTrap : BaseTrap {

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
    ParticleSystem _myFire;

    FireState _mystate = FireState.NONE;
    protected override void Start()
    {
        _myFire = transform.GetChild(0).transform.GetComponent<ParticleSystem>();
        _myFire.Stop();
        base.Start();
    }

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

    void StartFire()
    {
        _currDelay = (Time.time - _startDelay) / _fireDelay;

        if(_currDelay >= 1)
        {
            _currDelay = 1;

            _myFire.Play();
            _startDelay = Time.time;
            _mystate = FireState.FLAMEON;
        }
    }

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

    void BurnBabyBurn()
    {
        _currDelay = (Time.time - _startDelay) / _burningDuration;
        
        LookForPlayer();

        if (_currDelay >= 1)
        {
            _currDelay = 1;

            _myFire.Stop();

            _startDelay = Time.time;
            _mystate = FireState.FLAMEOFF;
        }

    }

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

    public override void DisableTrap()
    {
        _mystate = FireState.ROOMDONE;
    }

    public override void ResetTrap()
    {
        _mystate = FireState.NONE;
    }
}
