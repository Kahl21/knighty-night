using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenLight : MonoBehaviour {

    float _damage;
    [SerializeField]
    float _detectionDistance;
    [SerializeField]
    bool _debug;
    bool _init;

    float _lightningStrikeDelay;
    float _lightningLifeTime;

    float _currAttackDelay;
    float _startAttackDelay;
    float _currLifeTime;
    float _startLifeTime;

    ParticleSystem _myParticle;

    PlayerController _playerRef;
    SecretBoss _myBoss;

	// Use this for initialization
	public void Init (float damage, float attackDelay, float lifeTime, SecretBoss myBossRef)
    {
        _myParticle = GetComponent<ParticleSystem>();

        _myBoss = myBossRef;
        _myBoss.AddAttack(this);

        _damage = damage;
        _lightningStrikeDelay = attackDelay;
        _lightningLifeTime = lifeTime;
        _playerRef = PlayerController.Instance;
        _startAttackDelay = Time.time;
        _startLifeTime = Time.time;
        _init = true;
	}

    private void OnDrawGizmos()
    {
        if(_debug)
        {
            Gizmos.DrawWireSphere(transform.position, _detectionDistance);
        }
    }

    // Update is called once per frame
    void Update () {
        if(_init)
        {
            Delay();
        }
	}

    void Delay()
    {

        _currAttackDelay = (Time.time - _startAttackDelay) / _lightningStrikeDelay;
        _currLifeTime = (Time.time - _startLifeTime) / _lightningLifeTime;
        if(_currAttackDelay >= 1f)
        {
            if(!_myParticle.isPlaying)
            {
                _myParticle.Play();
            }

            if(Vector3.Distance(transform.position, _playerRef.transform.position) <= _detectionDistance)
            {
                _playerRef.TakeDamage(_damage);
            }

            if(_currLifeTime >= 1f)
            {
                RemoveMe();
            }
        }
    }

    public void RemoveMe()
    {
        _init = false;
        _myBoss.RemoveAttack(this);
        Destroy(gameObject);
    }
}
