using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CathedralProjectile : MonoBehaviour {

    RaycastHit hit;

    [Header("Base Projectile Variables")]
    [SerializeField]
    float _projectileDamage = .5f;
    [SerializeField]
    float _damageRange;
    [SerializeField]
    float _projectileSpeed;
    [SerializeField]
    float _deathTimer;
    float _currTime;
    float _startTime;
    bool _dead = false;
    bool _hit = false;
    bool _canMove = true;
     Vector3 _deathDirection;
    [SerializeField]
    bool _debugProjectile;
    bool _init;
    
    [Header("Cheating Variables")]
    [SerializeField]
    float _cheatingDistance;
    [SerializeField]
    float _cheatingSensitivity;
    GameObject _myPillar;

    Menuing _menuRef;

    // Use this for initialization
    public virtual void Init(GameObject _ghostThatShotMe)
    {
        _menuRef = Menuing.Instance;
        _myPillar = _ghostThatShotMe;
        _init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_init)
        {
            if (!_menuRef.GameIsPaused)
            {
                if (!_dead)
                {
                    Move();
                    CheckForHit();
                }
                else
                {
                    Die();
                }
            }
        }
    }

    //moves the enemy toward the enemy
    void Move()
    {
        transform.position += transform.forward * _projectileSpeed * Time.deltaTime;
    }

    //checks to see if the enemy has hit the player
    //deals damage to the player
    void CheckForHit()
    {
        if(_debugProjectile)
        {
            Debug.DrawRay(transform.position, transform.forward * _damageRange, Color.black);
        }

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _damageRange))
        {
            GameObject thingHit = hit.collider.gameObject;
            if (thingHit.GetComponent<PlayerController>())
            {
                thingHit.GetComponent<PlayerController>().TakeDamage(_projectileDamage);
            }
        }
    }

    //called when the enemy gets hit
    //deals damage to the enemy
    public void HitProjectile(Vector3 _flyDir)
    {
        if (!_hit)
        {
            _hit = true;
            _deathDirection = _flyDir;
            _deathDirection.y = 0;
            _startTime = Time.time;
            _dead = true;
        }
    }

    //called while the ghost is dying;
    void Die()
    {
        _currTime = (Time.time - _startTime) / _deathTimer;

        Vector3 _newDeathDirection = _deathDirection;
        try
        {
            if (Vector3.Distance(transform.position, _myPillar.transform.position) <= _cheatingDistance)
            {
                if (transform.position.z > _myPillar.transform.position.z)
                {
                    _newDeathDirection.z -= _cheatingSensitivity;
                }
                else if (transform.position.z < _myPillar.transform.position.z)
                {
                    _newDeathDirection.z += _cheatingSensitivity;
                }

                if (transform.position.x > _myPillar.transform.position.x)
                {
                    _newDeathDirection.x -= _cheatingSensitivity;
                }
                else if (transform.position.x > _myPillar.transform.position.x)
                {
                    _newDeathDirection.x += _cheatingSensitivity;
                }
            }
        }
        catch
        {
            Destroy(gameObject);
        }

        if (_debugProjectile)
        {
            Debug.DrawRay(transform.position, _newDeathDirection * _damageRange, Color.black);
        }

        if (Physics.Raycast(transform.position + Vector3.up, _newDeathDirection, out hit, _damageRange))
        {
            if (hit.collider.GetComponent<CathedralGlhost>())
            {
                _myPillar.GetComponent<CathedralGlhost>().RemoveProj(this);
                hit.collider.GetComponent<BaseEnemy>().GotHit(_newDeathDirection, 0f);
            }
            else if (!hit.collider.GetComponent<BaseEnemy>() || !hit.collider.GetComponent<PlayerController>())
            {
                _myPillar.GetComponent<CathedralGlhost>().RemoveProj(this);
                Destroy(gameObject);
            }
        }

        _deathDirection = _newDeathDirection;
        transform.position += _deathDirection * _projectileSpeed * Time.deltaTime;


        if (_currTime >= 1)
        {
            _myPillar.GetComponent<CathedralGlhost>().RemoveProj(this);
            Destroy(gameObject);
        }
    }

    public void Stop()
    {
        Destroy(gameObject);
    }

    //various Getters and Setters

    public virtual GameObject SetPillar { set { _myPillar = value; } }
    public virtual bool AmHit { get { return _hit; } }
    public virtual float GetDamage { get { return _projectileDamage; } }

}
