using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CathedralGlhost : BasicGlhost {

    [Header("Cathedral Glhost Variables")]
    [SerializeField]
    GameObject _projectilePF;
    [SerializeField]
    float _projectileSpawnOffset;
    [SerializeField]
    float _projectileSpawnWaitTime;
    float _currProjSpawn;
    float _startProjSpawn;
    Vector3 _hoverPos;
    List<CathedralProjectile> _myProjectiles;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _myAnimations.Play("Idle", 0);
        _myProjectiles = new List<CathedralProjectile>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_menuRef.GameIsPaused)
        {
            if (!_dead)
            {
                if (_canMove)
                {
                    Move();
                }
                else
                {
                    Hover();
                    FireProjectile();
                }
            }
            else
            {
                Die();
            }
        }
    }

    //moves the enemy toward the enemy
    protected override void Move()
    {

        if (Vector3.Distance(transform.position, _target.transform.position) <= _damageRange)
        {
            _myAnimations.Play("Idle", 0);
            _myAgent.SetDestination(transform.position);
            transform.LookAt(_target.transform.position);
            _myAgent.enabled = false;
            _startProjSpawn = Time.time;
            _hoverPos = transform.position - _target.transform.position;
            _hoverPos.y = 0;
            _canMove = false;
        }
        else
        {
            _myAnimations.Play("Movement", 0);
            _myAgent.SetDestination(_target.transform.position);
        }

        if (Vector3.Distance(transform.position, _target.transform.position) < _spookDistance)
        {
            ChangeSpookiness();
        }
    }

    protected virtual void Hover()
    {
        
        transform.position = _target.transform.position + _hoverPos;
    }

    protected virtual void FireProjectile()
    {
        //_myAnimations.Play("Shoot", 0);
        _currProjSpawn = (Time.time - _startTime) / _projectileSpawnWaitTime;

        if(_currProjSpawn >=1)
        {
            _currProjSpawn = 0;
            _startTime = Time.time;
            Vector3 _spawnPos = transform.position + (transform.forward * _projectileSpawnOffset) + Vector3.up;
            GameObject _newProj = Instantiate<GameObject>(_projectilePF, _spawnPos, transform.rotation, null);
            _newProj.GetComponent<CathedralProjectile>().Init(gameObject);
            _myProjectiles.Add(_newProj.GetComponent<CathedralProjectile>());
        }
    }

    protected override void Die()
    {
        if(_myProjectiles.Count > 0)
        {
            for (int i = 0; i < _myProjectiles.Count; i=0)
            {
                _myProjectiles[i].Stop();
            }
        }

        base.Die();
        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
            {
                _myCollider.enabled = false;
                _mySpawner.RemoveMe(this);
                _mySpawner.CheckForEnd();

                Dead();
            }
        }
        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }

    public virtual void RemoveProj(CathedralProjectile _projToRemove)
    {
        _myProjectiles.Remove(_projToRemove);
    }

    public override void Stop()
    {
        if (_myProjectiles.Count > 0)
        {
            for (int i = 0; i < _myProjectiles.Count; i++)
            {
                _myProjectiles[i].Stop();
            }
        }
        base.Stop();
    }
}
