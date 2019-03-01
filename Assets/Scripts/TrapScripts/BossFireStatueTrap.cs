using System.Collections;
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
        FLAMEBURNOUT,
        ROOMDONE
    }

    [Header("Fire Statue Variables")]
    
    [SerializeField]
    float _beginningDelay;
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
    float _fireDamage;

    [Header("Detection Variables")]
    [SerializeField]
    bool _debugDamageArea;
    [SerializeField]
    float _spaceBetweenRays;
    
    float _maxDetectDistance;
    float _currDetectDistance;
    RaycastHit hit;
    ParticleSystem _myFire;
    GameObject bossEntity;
    bool _XAttack = false;

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
            case FireState.FLAMEBURNOUT:
                Burnout();
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
        var main = _myFire.main;
        main.startLifetime = _fireDistance;

        _trapDamage = _fireDamage;

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

            if(bossEntity.GetComponent<TrapBossGlhost>())
            {
                bossEntity.GetComponent<TrapBossGlhost>().IsNotPossessing = true;
            }
            else if(bossEntity.GetComponent<MiniTrapBossGlhost>())
            {
                bossEntity.GetComponent<MiniTrapBossGlhost>().IsPossessing = true;
            }

            _mystate = FireState.FLAMEBURNOUT;
        }
    }

    void Burnout()
    {
        ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[_myFire.main.maxParticles];
        int liveParticles = _myFire.GetParticles(particleArray);
        //print(liveParticles + " Particles Alive");
        if (liveParticles <= 0)
        {
            if (_XAttack)
            {
                transform.eulerAngles += new Vector3(0, 45f, 0);
                _XAttack = false;
            }
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
                _RayPos = transform.position + Vector3.down + (transform.right * _spaceBetweenRays * i);
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
                _RayPos = transform.position + Vector3.down + (transform.right * _spaceBetweenRays * i) + (transform.forward * _maxDetectDistance);
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
        _myFire.Stop();
        _mystate = FireState.ROOMDONE;
    }

    //reset function
    public override void ResetTrap()
    {
        _mystate = FireState.NONE;
    }

    public bool GetXAttack { get { return _XAttack; } set { _XAttack = value; } }
    public GameObject GetBossEntity { get { return bossEntity; } set { bossEntity = value; } }
    public float GetFireDelay { get { return _fireDelay; } set { _fireDelay = value; } }
    public float GetFireDistance { get { return _fireDistance; } set { _fireDistance = value; } }
    public float GetMaxDetectDistance { get { return _maxDetectDistance; } set { _maxDetectDistance = value; } }
    public float GetStartDelay { get { return _startDelay; } set { _startDelay = value; } }
    public float GetBurningDuration { get { return _burningDuration; } set { _burningDuration = value; } }
    public float GetFireDamage { get { return _fireDamage; } set { _fireDamage = value; } }

}
