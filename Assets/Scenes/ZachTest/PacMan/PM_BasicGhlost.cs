using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PM_BasicGhlost : BaseEnemy
{
    protected enum BASICSTATES
    {
        NONE,
        CHASING,
        INVINCIBLE,
        MOVING,
        FINDTARGET,
        DIE
    }

    [Header("Basic PM Ghlost Variables")]
    [SerializeField]
    float _checkForOpeningDistance;
    [SerializeField]
    float _checkDirectionDelay;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    Vector3 moveDirection;

    int random;
    GameObject _staticTarget;
    PM_Manager _managerInstance;

    BASICSTATES _myState = BASICSTATES.MOVING;

    // Update is called once per frame
    protected override void Update()
    {
        //if (!_menuRef.GameIsPaused)
        //{
        if (!_dead)
        {
            if (_canMove)
            {
                switch (_myState)
                {
                    case BASICSTATES.NONE:
                        break;

                    case BASICSTATES.MOVING:
                        Move();
                        CheckForHit();
                        break;

                    case BASICSTATES.INVINCIBLE:
                        Move();
                        CheckForHit();
                        break;

                    case BASICSTATES.CHASING:
                        FollowPlayer();
                        break;

                    case BASICSTATES.FINDTARGET:
                        findNewTarget();
                        break;

                    case BASICSTATES.DIE:
                        break;
                }
                //Move();
                //CheckForHit();
            }
        }
        else
        {
            Die();
        }
        //}
    }

    // Use this for initialization
    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {

        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;


    }

    public void BasicInit(PM_Manager managerInstance)
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _myAgent.enabled = true;
        //_myAgent.radius = 0.001f;
        _target = PlayerController.Instance;
        _menuRef = Menuing.Instance;

        _myBody = transform.GetChild(2).gameObject;

        _managerInstance = managerInstance;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
        _startTime = Time.time;

        random = Random.Range(0, _managerInstance.GetTargetPoints.Count - 1);
        //GetComponent<NavMeshAgent>().radius = 0.00001f;
        _staticTarget = _managerInstance.GetTargetPoints[random];
        _myAgent.SetDestination(_staticTarget.transform.position);
        
        _myState = BASICSTATES.INVINCIBLE;
    }

    protected override void Move()
    {
        if (_myState == BASICSTATES.CHASING)
        {
            _myAgent.SetDestination(_target.transform.position);
            
        }
        else
        {

        }

        if (_myAgent.hasPath == false)
        {
            Debug.Log("No Path");
            _myState = BASICSTATES.FINDTARGET;
        }
        //BasicMovement();
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void findNewTarget()
    {
        random = Random.Range(0, _managerInstance.GetTargetPoints.Count - 1);

        if (_managerInstance.GetTargetPoints[random] != _staticTarget)
        {
            _staticTarget = _managerInstance.GetTargetPoints[random];
        }

        _myAgent.SetDestination(_staticTarget.transform.localPosition);
        _myState = BASICSTATES.INVINCIBLE;
    }

    //This is used for the ghlosts secondary movement.
    //The ghlosts will follow the player until he is a certain distance away.
    //Than he will switch back to regular random movement.
    void FollowPlayer()
    {

    }

    //checks to see if the enemy has hit the player
    //deals damage to the player
    protected override void CheckForHit()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _damageRange))
        {
            GameObject thingHit = hit.collider.gameObject;
            if (thingHit.GetComponent<PlayerController>())
            {
                thingHit.GetComponent<PlayerController>().TakeDamage(_glhostDamage);
            }
            else if (thingHit == _staticTarget)
            {
                //_myState = BASICSTATES.FINDTARGET;
            }
        }
    }

    //function to be called when the player hits them
    //sets the ghost to the "Dead" state
    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        
        if (!_hit && (_myState == BASICSTATES.FINDTARGET || _myState == BASICSTATES.MOVING || _myState == BASICSTATES.NONE))
        {
            Debug.Log("Glhost Got Hit");
            _hit = true;
            _myAgent.enabled = false;
            _knockBack = _knockBackForce;
            _deathDirection = _flyDir;
            _deathDirection.y = 0;

            _startTime = Time.time;
            _dead = true;
        }
    }

    //makes the ghost die in a certain way
    protected override void Die()
    {
        //Debug.DrawLine(transform.position, transform.position + _deathDirection*_collisionCheckDist);

        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, .4f))
        {
            if (_myState == BASICSTATES.INVINCIBLE)
            {

            }
            else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
            {
                //_shooterRef.removeGhlostFromScene(gameObject);
                Destroy(gameObject);
            }
        }
        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }

    public bool SetDeath { set { _dead = value; } }
}
