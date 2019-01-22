using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : BaseTrap {

    enum SpikeState
    {
        NONE,
        TELL,
        ATTDELAY,
        ATTACK,
        RETDELAY,
        RETREAT,
        DONERETREAT,
        ROOMDONE
    }

    [Header("Spike Trap Variables")]
    [SerializeField]
    float _tellSpeed;
    [SerializeField]
    float _tellDistance;
    [SerializeField]
    float _attackDelayDuration;
    [SerializeField]
    float _attackSpeed;
    [SerializeField]
    float _attackDistance;
    [SerializeField]
    float _retreatDelayDuration;
    [SerializeField]
    float _retreatSpeed;

    float _currTime;
    float _startTIme;

    [Header("HitBox Variables")]
    [SerializeField]
    bool _debug;
    [SerializeField]
    float BoxRadius;

    GameObject _spikes;
    Vector3 _startPos;
    Vector3 _currBound;

    Vector3 _scanStartPos;
    Vector3 _topLeftCorner;
    Vector3 _bottomRightCorner;

    SpikeState myState = SpikeState.NONE;

    //Init function
    public override void Init()
    {
        base.Init();
        _spikes = transform.GetChild(0).gameObject;
        _startPos = _spikes.transform.localPosition;
        _scanStartPos = transform.position;
        
        _topLeftCorner = _scanStartPos + ((Vector3.forward + Vector3.left) * BoxRadius);
        _bottomRightCorner = _scanStartPos + ((Vector3.back + Vector3.right) * BoxRadius);
    }

    private void Update()
    {
        switch (myState)
        {
            case SpikeState.NONE:
                break;
            case SpikeState.TELL:
                DoTell();
                break;
            case SpikeState.ATTDELAY:
                StartAttackDelay();
                break;
            case SpikeState.ATTACK:
                Attack();
                break;
            case SpikeState.RETDELAY:
                StartRetreatDelay();
                break;
            case SpikeState.RETREAT:
                RetreatSpikes();
                break;
            case SpikeState.DONERETREAT:
                RoomDoneRetreat();
                break;
            case SpikeState.ROOMDONE:
                break;
            default:
                break;
        }

        if(_debug)
        {
            Debug.DrawLine(_topLeftCorner, _topLeftCorner + (Vector3.back * (BoxRadius * 2)));
            Debug.DrawLine(_topLeftCorner, _topLeftCorner + (Vector3.right * (BoxRadius * 2)));
            Debug.DrawLine(_bottomRightCorner, _bottomRightCorner + (Vector3.forward * (BoxRadius * 2)));
            Debug.DrawLine(_bottomRightCorner, _bottomRightCorner + (Vector3.left * (BoxRadius * 2)));
        }
    }

    //Starts spikes 
    public void StartTell()
    {
        if(myState == SpikeState.NONE)
        {
            _currBound = _spikes.transform.localPosition + (Vector3.up * _tellDistance);

            myState = SpikeState.TELL;
        }
    }

    //Does a tell for the spikes
    //spikes will come out of the holes but not deal damage
    private void DoTell()
    {
        if(_spikes.transform.localPosition.y <= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.up * _tellSpeed * Time.deltaTime;
        }
        else
        {
            _spikes.transform.localPosition = _currBound;

            _startTIme = Time.time;
            myState = SpikeState.ATTDELAY;
        }
    }

    //delay inbetween the tell and attack
    private void StartAttackDelay()
    {
        _currTime = (Time.time - _startTIme) / _attackDelayDuration;

        if(_currTime >= 1)
        {
            _currTime = 1;

            _currBound = _spikes.transform.localPosition + (Vector3.up * _attackDistance);
            myState = SpikeState.ATTACK;
        }
    }

    //spike shoot up from the ground
    //damages the player if they are on top of the plates
    private void Attack()
    {
        if (_spikes.transform.localPosition.y <= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.up * _attackSpeed * Time.deltaTime;
            Vector3 _playerPos = _playerRef.transform.position;
            if(_playerPos.z < _topLeftCorner.z && _playerPos.x > _topLeftCorner.x && _playerPos.z > _bottomRightCorner.z && _playerPos.x < _bottomRightCorner.x)
            {
                _playerRef.TakeDamage(_trapDamage);
            }
        }
        else
        {
            _spikes.transform.localPosition = _currBound;

            _startTIme = Time.time;
            myState = SpikeState.RETDELAY;
        }
    }

    //delay before the retreat
    private void StartRetreatDelay()
    {
        _currTime = (Time.time - _startTIme) / _attackDelayDuration;

        if (_currTime >= 1)
        {
            _currTime = 1;

            _currBound = _startPos;
            myState = SpikeState.RETREAT;
        }
    }

    //spikes will slowly retreat
    private void RetreatSpikes()
    {
        if (_spikes.transform.localPosition.y >= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.down * _retreatSpeed * Time.deltaTime;
        }
        else
        {
            _spikes.transform.localPosition = _startPos;

            myState = SpikeState.NONE;
        }
    }

    //if the room finishes while the spikes are doing anything
    //spikes will retreat
    private void RoomDoneRetreat()
    {
        if (_spikes.transform.localPosition.y >= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.down * _retreatSpeed * Time.deltaTime;
        }
        else
        {
            _spikes.transform.localPosition = _startPos;

            myState = SpikeState.ROOMDONE;
        }
    }

    //stops the trap if room is finished
    public override void DisableTrap()
    {
        if (myState != SpikeState.NONE)
        {
            _startTIme = Time.time;
            myState = SpikeState.DONERETREAT;
        }
        else
        {
            myState = SpikeState.ROOMDONE;
        }
    }

    //reset function
    public override void ResetTrap()
    {
        _spikes.transform.localPosition = _startPos;
        myState = SpikeState.NONE;
    }
}
