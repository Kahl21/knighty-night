using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpikeTrap : SpikeTrap {

    bool _possessed;

    float _possessedTime;
    float _currPossessTime;
    float _startPossessTime;

    float _realAttackDelayDuration;
    float _realAttackSpeed;
    float _realRetreatSpeed;
    float _moveSpeed;

    TrapBossGlhost _bossRef;

    public override void Init()
    {
        base.Init();
        _realAttackDelayDuration = _attackDelayDuration;
        _realAttackSpeed = _attackSpeed;
        _realRetreatSpeed = _retreatSpeed;
    }

    protected override void Update()
    {
        base.Update();

        if(_possessed)
        {
            StalkPlayer();
        }
    }

    public void BecomePossessed(TrapBossGlhost myboss, float possessDuration, float attackSpeed, float attackDelay, float retreatSpeed, float moveSpeed)
    {
        _bossRef = myboss;
        _possessedTime = possessDuration;
        _realAttackDelayDuration = attackDelay;
        _realAttackSpeed = attackSpeed;
        _realRetreatSpeed = retreatSpeed;
        _moveSpeed = moveSpeed;

        _possessed = true;
        base.StartTell();
        _startPossessTime = Time.time;
    }

    //Does a tell for the spikes
    //spikes will come out of the holes but not deal damage
    protected override void DoTell()
    {
        if (_spikes.transform.localPosition.y <= _currBound.y)
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
    protected override void StartAttackDelay()
    {
        _currTime = (Time.time - _startTIme) / _realAttackDelayDuration;

        if (_currTime >= 1)
        {
            _currTime = 1;

            _currBound = _spikes.transform.localPosition + (Vector3.up * _attackDistance);
            myState = SpikeState.ATTACK;
        }
    }

    //spike shoot up from the ground
    //damages the player if they are on top of the plates
    protected override void Attack()
    {
        if (_spikes.transform.localPosition.y <= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.up * _realAttackSpeed * Time.deltaTime;
            Vector3 _playerPos = _playerRef.transform.position;
            if (_playerPos.z < _topLeftCorner.z && _playerPos.x > _topLeftCorner.x && _playerPos.z > _bottomRightCorner.z && _playerPos.x < _bottomRightCorner.x)
            {
                _playerRef.TakeDamage(_trapDamage);
                _possessed = false;
                _realAttackDelayDuration = _attackDelayDuration;
                _realAttackSpeed = _attackSpeed;
                _realRetreatSpeed = _retreatSpeed;
                _bossRef.IsNotPossessing = true;
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
    protected override void StartRetreatDelay()
    {
        _currTime = (Time.time - _startTIme) / _realAttackDelayDuration;

        if (_currTime >= 1)
        {
            _currTime = 1;

            _currBound = _startPos;
            myState = SpikeState.RETREAT;
        }
    }

    //spikes will slowly retreat
    protected override void RetreatSpikes()
    {
        if (_spikes.transform.localPosition.y >= _currBound.y)
        {
            _spikes.transform.localPosition += Vector3.down * _realRetreatSpeed * Time.deltaTime;
        }
        else
        {
            _spikes.transform.localPosition = _startPos;

            myState = SpikeState.NONE;

            if (_possessed)
            {
                base.StartTell();
            }
        }
    }

    private void StalkPlayer()
    {
        _currPossessTime = (Time.time - _startPossessTime) / _possessedTime;

        if (_currPossessTime > 1)
        {
            _currPossessTime = 1;
            _possessed = false;
            _realAttackDelayDuration = _attackDelayDuration;
            _realAttackSpeed = _attackSpeed;
            _realRetreatSpeed = _retreatSpeed;
            _bossRef.IsNotPossessing = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, _playerRef.transform.position, _moveSpeed * Time.deltaTime);
    }
}
