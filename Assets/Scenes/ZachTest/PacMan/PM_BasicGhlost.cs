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
        _target = PlayerController.Instance;
        _menuRef = Menuing.Instance;

        _myBody = transform.GetChild(2).gameObject;

        _managerInstance = managerInstance;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
        _startTime = Time.time;

        random = Random.Range(0, _managerInstance.GetTargetPoints.Count - 1);
        _staticTarget = _managerInstance.GetTargetPoints[random];
        _myAgent.SetDestination(_staticTarget.transform.position);
        _myState = BASICSTATES.MOVING;
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

    //This is used for the ghlosts primary movement.
    //The ghlosts will go down a walkway until they get the opportunity to turn more than one direction
    //If this happens he will choose a random direction to travel down.
    void BasicMovement()
    {
        _currTime = Time.time - _startTime;
        if (_currTime >= _checkDirectionDelay)
        {
            List<Vector3> openDirections = CheckForOpenings();
            Vector3[] openSides = new Vector3[3];

            if (openDirections.Count == 1)
            {
                RaycastHit thingHit;
                if (Physics.Raycast(transform.position, openDirections[0], out thingHit))
                {
                    moveDirection = openDirections[0];
                    transform.LookAt(openDirections[0] * 2);
                }

            }
            else if (openDirections.Count > 1)
            {
                int direction;
                direction = Random.Range(0, openDirections.Count - 1);
                Debug.Log(openDirections[direction]);
                RaycastHit thingHit;
                //Debug.DrawLine(transform.position + Vector3.up, openDirections[direction]);
                if (Physics.Raycast(transform.position + Vector3.up, openDirections[direction], out thingHit))
                {
                    moveDirection = openDirections[direction];
                    transform.LookAt(openDirections[direction] *2);
                }
            }
            _startTime = Time.time;
        }
        
    }

    void findNewTarget()
    {
        random = Random.Range(0, _managerInstance.GetTargetPoints.Count - 1);

        if (_managerInstance.GetTargetPoints[random] != _staticTarget)
        {
            _staticTarget = _managerInstance.GetTargetPoints[random];
        }

        _myAgent.SetDestination(_staticTarget.transform.localPosition);
        _myState = BASICSTATES.MOVING;
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
            else if(thingHit == _staticTarget)
            {
                //_myState = BASICSTATES.FINDTARGET;
            }
        }
    }

    //Shoots out 3 raycasts: left, right, and center, that check for openings one each side
    //0 = Front
    //1 = Left
    //2 = Right
    List<Vector3> CheckForOpenings()
    {
        List<Vector3> openSides = new List<Vector3>();
        RaycastHit[] hits = new RaycastHit[3];
        
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * _checkForOpeningDistance);
        Debug.DrawRay(transform.position + Vector3.up, transform.right * -1 * _checkForOpeningDistance);
        Debug.DrawRay(transform.position + Vector3.up, transform.right * _checkForOpeningDistance);
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hits[0], _checkForOpeningDistance))
        {
            
        }
        else
        {
            openSides.Add(transform.forward);
        }
        if (Physics.Raycast(transform.position + Vector3.up, transform.right * -1, out hits[1], _checkForOpeningDistance))
        {

        }
        else
        {
            openSides.Add(transform.right * -1);
        }
        if (Physics.Raycast(transform.position + Vector3.up, transform.right, out hits[2], _checkForOpeningDistance))
        {
            
        }
        else
        {
            openSides.Add(transform.right);
        }
        Debug.Log(openSides.Count);
        return openSides;
    }

    //makes the ghost die in a certain way
    protected override void Die()
    {
        //Debug.DrawLine(transform.position, transform.position + _deathDirection*_collisionCheckDist);
    }

    public bool SetDeath { set { _dead = value; } }
}
