using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour {

    protected NavMeshAgent _myAgent;
    protected RaycastHit hit;

    [Header("Base Glhost Variables")]
    [SerializeField]
    protected float _glhostDamage = .5f;
    [SerializeField]
    protected float _damageRange;
    [SerializeField]
    protected float _deathTimer;
    protected float _currTime;
    protected float _startTime;
    protected bool _dead = false;
    protected bool _actualDead = false;
    protected bool _hit = false;
    protected bool _canMove = true;
    protected Vector3 _deathDirection;
    [SerializeField]
    protected float _collisionCheckDist;
    protected float _knockBack;
    protected bool _init;
    
    [SerializeField]
    protected float _spookDistance;
    [SerializeField]
    protected float _spookyOffset;
    protected GameObject _myBody;
    protected SkinnedMeshRenderer _myRenderer;
    protected Material _mySpookiness;
    protected Color _spookColor;
    protected Color _myColor;

    [Header("Colored Glhost Extra Variables")]
    [SerializeField]
    protected float _cheatingDistance;
    [SerializeField]
    protected float _cheatingSensitivity;
    protected GameObject _myPillar;
    protected Animator _myAnimations;

    protected Menuing _menuRef;
    protected PlayerController _target;
    protected DungeonMechanic _mySpawner;
    [SerializeField]
    protected Mechanic _myMechanic;

    // Use this for initialization
    public virtual void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _mySpawner = _spawner;
        _myMechanic = _incomingMech;
        _target = PlayerController.Instance;
        _menuRef = Menuing.Instance;

        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _mySpookiness = _myRenderer.materials[1];
        if (_myMechanic == Mechanic.COLOR)
        {
            _mySpookiness.color = _myColor;
        }
        _spookColor = _mySpookiness.color;
        _spookColor.a = 0;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
    }

    // Update is called once per frame
    protected virtual void Update ()
    {

    }

    //moves the enemy toward the enemy
    protected virtual void Move()
    {
        if (Vector3.Distance(transform.position, _target.transform.position) <= _damageRange)
        {
            _myAgent.SetDestination(transform.position);
        }
        else
        {
            _myAgent.SetDestination(_target.transform.position);
        }

        if (Vector3.Distance(transform.position, _target.transform.position) < _spookDistance)
        {
            ChangeSpookiness();
        }
    }

    //checks to see if the enemy has hit the player
    //deals damage to the player
    protected virtual void CheckForHit()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _damageRange))
        {
            GameObject thingHit = hit.collider.gameObject;
            if (thingHit.GetComponent<PlayerController>())
            {
                thingHit.GetComponent<PlayerController>().TakeDamage(_glhostDamage);
            }
        }
    }

    //changes the transparency of the enemy material
    //changes depending on how close th enemy is to the player
    protected virtual void ChangeSpookiness()
    {
        _spookColor.a = (_spookDistance - Vector3.Distance(transform.position, _target.transform.position)) / _spookyOffset;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;
    }

    //called when the enemy gets hit
    //deals damage to the enemy
    public virtual void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        if (!_hit)
        {
            _hit = true;
            _myAgent.enabled = false;
            _knockBack = _knockBackForce;
            _deathDirection = _flyDir;
            _deathDirection.y = 0;

            _startTime = Time.time;
            _dead = true;
        }
    }

    //called while the ghost is dying;
    protected virtual void Die()
    {
        _currTime = (Time.time - _startTime) / _deathTimer;

        if(_currTime >= 1)
        {
            _mySpawner.RemoveMe(this);
            _mySpawner.CheckForEnd();
            Destroy(gameObject);
        }
    }

    //Called when a room is completed
    public virtual void Stop()
    {
        _canMove = false;
        if (!_dead && _myAgent.enabled)
        {
            _myAgent.SetDestination(transform.position);
        }
        _mySpawner.RemoveMe(this);

        Destroy(gameObject);
    }

    //various Getters and Setters

    public virtual Color SetColor { set { _myColor = value; } }
    public virtual GameObject SetPillar { set { _myPillar = value; } }
    public virtual bool AmHit { get { return _hit; } }
    public virtual float GetDamage { get { return _glhostDamage; } }
}