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
    bool _firing;

    [SerializeField]
    AudioClip shootClip;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _myAnimations.Play("Idle", 0);
        _myProjectiles = new List<CathedralProjectile>();

        _speaker = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_menuRef.GameIsPaused)
        {
            if (!_dead)
            {
                Move();
                
                if(_firing)
                {
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
            if(!_idling)
            {
                _myAnimations.Play("Idle", 0);
                _idling = true;
                _attacking = false;
            }

            if(!_firing)
            {
                _myAgent.SetDestination(transform.position);
                _startProjSpawn = Time.time;
                _hoverPos = transform.position - _target.transform.position;
                _hoverPos.y = transform.position.y;
                _firing = true;
            }
            else
            {
                Hover();
            }
        }
        else
        {
            if(!_attacking)
            {
                _myAnimations.Play("Movement", 0);
                _attacking = true;
                _idling = false;
                _firing = false;
            }
            _myAgent.SetDestination(_target.transform.position);
        }

        if (Vector3.Distance(transform.position, _target.transform.position) < _spookDistance)
        {
            ChangeSpookiness();
           
        }
    }

    protected virtual void Hover()
    {
        //transform.position = _target.transform.position + _hoverPos;
        if (Vector3.Distance(transform.position, _target.transform.position) >= _damageRange)
        {
            _idling = false;
            _firing = false;
            _attacking = false;
        }
    }

    protected virtual void FireProjectile()
    {
        //_myAnimations.Play("Shoot", 0);
        _currProjSpawn = (Time.time - _startTime) / _projectileSpawnWaitTime;

        

        if(_currProjSpawn >=1)
        {
            if(!_speaker.isPlaying)
            _speaker.PlayOneShot(shootClip, volSFX);
            _myAnimations.Play("Shoot", 0);
            _currProjSpawn = 0;
            _startTime = Time.time;
            Vector3 _spawnPos = transform.position + (transform.forward * _projectileSpawnOffset);
            GameObject _newProj = Instantiate<GameObject>(_projectilePF, _spawnPos, transform.rotation, null);
            _newProj.GetComponent<CathedralProjectile>().Init(gameObject);
            _myProjectiles.Add(_newProj.GetComponent<CathedralProjectile>());
        }
        Vector3 lookPos = _target.transform.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
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
