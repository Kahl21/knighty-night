using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniColorMinion : BasicGlhost {

    protected bool _goingToBoss;

    [Header("Color Boss Minion Variables")]
    [SerializeField]
    protected float _spinningSpeed;
    [SerializeField]
    protected float _spawnSpeed;
    [SerializeField]
    protected float _maxSpawnDistance;
    [SerializeField]
    protected float _timeToGetBackToBoss;
    protected float _startBackToBossTime;
    protected float _currBackToBossTime;
    protected bool _chillin;

    protected Vector3 c0, c1;
    protected Vector3 _spawnDir;
    protected MiniBossColor _myBoss;
    protected CapsuleCollider _myCapCollider;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _myBoss = _mySpawner.GetBigBoy.GetComponent<MiniBossColor>();

        _mySpookiness.color = _myColor;
        _mySpookiness = _myRenderer.materials[1];

        _myCapCollider = GetComponent<CapsuleCollider>();
        _mySpawner.AddEnemy(this);
        _myAnimations.Play("Nothing", 0);
        _goingToBoss = false;
        _chillin = false;
        _myAgent.enabled = false;
        _deathDirection = _spawnDir;
        _deathDirection.y = 0;
        _init = true;
    }

    protected override void Update()
    {
        if (_init)
        {
            if (!_menuRef.GameIsPaused)
            {
                if (!_dead)
                {
                    if (_chillin)
                    {
                        SpinAround();
                        CheckForHit();
                    }
                    else
                    {
                        if (!_goingToBoss)
                        {
                            SpinAround();
                            CheckForHit();
                            InitialSpawnMove();
                        }
                        else
                        {
                            CheckForHit();
                            MoveToBoss();
                        }
                    }
                }
                else
                {
                    Die();
                }
            }
        }
    }

    void SpinAround()
    {
        Debug.Log("spinning");
        transform.Rotate(Vector3.up, _spinningSpeed * Time.deltaTime);
    }

    void InitialSpawnMove()
    {
        if (Vector3.Distance(transform.position, _myBoss.transform.position) <= _maxSpawnDistance)
        {
            Debug.Log("moving");
            transform.position += _spawnDir * _spawnSpeed * Time.deltaTime;

            if (Physics.Raycast(transform.position + Vector3.up, _spawnDir, out hit, _collisionCheckDist))
            {
                if (!hit.collider.GetComponent<BossEnemy>() && !hit.collider.GetComponent<BaseEnemy>())
                {
                    _hit = false;
                }
            }
        }
        else
        {
            _hit = false;
        }
    }

    public void StartBackToBoss()
    {
        c0 = transform.position;
        c1 = _mySpawner.GetBigBoy.transform.position;
        _myCapCollider.enabled = false;

        _startBackToBossTime = Time.time;
        _dead = false;
        _hit = true;
        _goingToBoss = true;
    }

    private void MoveToBoss()
    {
        _currBackToBossTime = (Time.time - _startBackToBossTime) / _timeToGetBackToBoss;

        if (_currBackToBossTime >= 1)
        {
            _currBackToBossTime = 1;

            _mySpawner.RemoveMe(this);
            Destroy(gameObject);
        }

        Vector3 c01;

        c01 = (1 - _currBackToBossTime) * c0 + _currBackToBossTime * c1;

        transform.position = c01;
    }

    protected override void Die()
    {
        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if (hit.collider.GetComponent<PlayerController>())
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(_glhostDamage);
            }
            else if (hit.collider.GetComponent<MiniBossColor>())
            {
                if (_myBoss.CheckForColor(_glhostDamage, _myColor))
                {
                    _mySpawner.RemoveMe(this);
                    Destroy(gameObject);
                }
                else
                {
                    _dead = false;
                    _hit = false;
                }
            }
            else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<BossEnemy>())
            {
                _mySpawner.RemoveMe(this);
                Destroy(gameObject);
            }
        }
        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }

    public Vector3 GetStartDirection { get { return _spawnDir; } set { _spawnDir = value; } }
}

