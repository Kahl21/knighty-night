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
    protected bool _hit = false;
    protected bool _canMove = true;
    protected Vector3 _deathDirection;
    [SerializeField]
    protected float _collisionCheckDist;
    protected float _knockBack;
    protected bool _init;
    
    [SerializeField]
    protected float _spookDistance;
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
    protected Mechanic _myMechanic;

    protected Collider _myCollider;
    protected bool dead;

    // Use this for initialization
    public virtual void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _mySpawner = _spawner;
        _myMechanic = _incomingMech;
        _target = PlayerController.Instance;
        _menuRef = Menuing.Instance;

        _myBody = transform.GetChild(3).gameObject;
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
        //_myAnimations.Play("Moving");
    }

    // Update is called once per frame
    protected virtual void Update ()
    {

	}

    //moves the enemy toward the enemy
    protected virtual void Move()
    {
       
    }

    //checks to see if the enemy has hit the player
    //deals damage to the player
    protected virtual void CheckForHit()
    {
        
    }

    //changes the transparency of the enemy material
    //changes depending on how close th enemy is to the player
    protected virtual void ChangeSpookiness()
    {
       
    }

    //called when the enemy gets hit
    //deals damage to the enemy
    public virtual void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        
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
       
    }

    //various Getters and Setters

    public virtual Color SetColor { set { _myColor = value; } }
    public virtual GameObject SetPillar { set { _myPillar = value; } }
    public virtual bool AmHit { get { return _hit; } }
    public virtual float GetDamage { get { return _glhostDamage; } }
}
