using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PM_ColoredGhlost : BaseEnemy
{
    //Strategy Enum for the DumbBossGhlosts
    public enum DUMBSTATE
    {
        NONE,
        TRAVELING,
        DIE
    }

    [Header("Dumb Ghlost Variables")]
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float _DamageToBoss;
    [SerializeField]
    float _DamageToPlayer;
    [SerializeField]
    float _maxTravelTime;
    [SerializeField]
    PM_Manager _pmManagerRef;

    List<GameObject> _travelPoints;
    GameObject _currentTarget;

    DUMBSTATE _myDumbState = DUMBSTATE.NONE;


    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        _canMove = true;
        _myAgent = gameObject.GetComponent<NavMeshAgent>();
        _myAgent.enabled = false;
        _mySpawner = _spawner;
        _myMechanic = _incomingMech;
        _menuRef = Menuing.Instance;

        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _mySpookiness = _myRenderer.materials[1];

        //_spookColor = _mySpookiness.color;
        //_spookColor.a = 0;
        _myRenderer.materials[1] = _mySpookiness;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
        _startTime = Time.time;
    }

    public void InitColor(Color Color, PM_Manager _managerRef, float travelRadius, GameObject myPillar)
    {
        _travelPoints = new List<GameObject>();
        _pmManagerRef = _managerRef;
        _myPillar = myPillar;

        _canMove = true;
        _myAgent = gameObject.GetComponent<NavMeshAgent>();
        _myAgent.enabled = false;

        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _mySpookiness = _myRenderer.materials[1];

        _myColor = Color;
        _mySpookiness.color = Color;

        _spookColor = _mySpookiness.color;
        _spookColor.a = 1;
        _mySpookiness.color = _spookColor;

        _myRenderer.materials[1] = _mySpookiness;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
        _startTime = Time.time;

        

        _myAgent.enabled = true;

        _travelPoints = _managerRef.GetTargetPoints;

        List<GameObject> newPointList = new List<GameObject>();


        //Pulls the list of travel points and checks to see if each one is in range to travel to.
        //If it is that point is added to a new list
        for (int index = 0; index < _travelPoints.Count; index++)
        {
            //Debug.Log("Point " + index + " is " + Vector3.Distance(_travelPoints[index].transform.position, transform.position) + " away");

            GameObject travelPointInQuesstion = _travelPoints[index];

            if (Vector3.Distance(travelPointInQuesstion.transform.position, transform.position) <= travelRadius)
            {
                newPointList.Add(travelPointInQuesstion);
            }
            
        }

        _travelPoints = newPointList;

        /* Debug
        for (int count = 0; count < _travelPoints.Count; count++)
        {
            Debug.Log("Point " + count + " is " +
                Vector3.Distance(_travelPoints[count].transform.position, transform.position) + " away");
        }
        */

        findNewTarget();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_dead)
        {
            if (_canMove)
            {
                switch (_myDumbState)
                {
                    case DUMBSTATE.NONE:

                        break;

                    case DUMBSTATE.TRAVELING:
                        Move();
                        CheckForHit();
                        break;

                    case DUMBSTATE.DIE:

                        break;
                }
            }
        }
        else
        {
            CheckForHit();
            Die();
        }
    }

    void findNewTarget()
    {
        int random;
        random = Random.Range(0, _travelPoints.Count);

        if (_travelPoints[random] != _currentTarget)
        {
            _currentTarget = _travelPoints[random];
        }

        _myAgent.SetDestination(_currentTarget.transform.localPosition);
        _myDumbState = DUMBSTATE.TRAVELING;
    }

    protected override void Move()
    {
        if (_myAgent.hasPath == false)
        {
            Debug.Log("No Path");
            findNewTarget();
        }
        //BasicMovement();
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    //shoots a raycast in front of the enemy
    //if the raycast hits the player
    //the player takes damage
    protected override void CheckForHit()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _damageRange))
        {
            GameObject thingHit = hit.collider.gameObject;
            if (thingHit.GetComponent<PlayerController>())
            {
                thingHit.GetComponent<PlayerController>().TakeDamage(_DamageToPlayer);
            }
        }
    }

    /*
    //changes how clear the ghost is depending on how close they are to the player
    protected override void ChangeSpookiness()
    {
        _spookColor.a = (_spookDistance - Vector3.Distance(transform.position, _target.transform.position)) / 7.5f;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;
    }
    */

    //function to be called when the player hits them
    //sets the ghost to the "Dead" state
    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        Debug.Log("Glhost Got Hit");
        if (!_hit && (_myDumbState == DUMBSTATE.TRAVELING))
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

    //makes the ghost die in a certain way
    //depending on what kind of room the ghost is in
    protected override void Die()
    {
        //Debug.DrawLine(transform.position, transform.position + _deathDirection*_collisionCheckDist);
        Vector3 _newDeathDirection = _deathDirection;
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

        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, .4f))
        {
            if (hit.collider.GetComponent<ColoredBlock>())
            {
                if (hit.collider.GetComponent<ColoredBlock>().GetColor == _myColor)
                {
                    _pmManagerRef.DisablePillar(hit.collider.gameObject);
                    Destroy(this.gameObject);
                }

            }
            else
            {
                _newDeathDirection = Vector3.zero;
                _myAgent.enabled = true;
                findNewTarget();
                _hit = false;
                _dead = false;
            }
        }

        _deathDirection = _newDeathDirection;
        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }

    //called when the room that the ghost is in is complete
    //removes the ghost from the game
    public override void Stop()
    {
        _canMove = false;
        if (!_dead)
        {
            _myAgent.SetDestination(transform.position);
        }
        _mySpawner.RemoveMe(this);
        Destroy(gameObject);
    }

    public float GetSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float SetDamageToBoss { set { _DamageToBoss = value; } }
    public float SetDamageToPlayer { set { _DamageToPlayer = value; } }
    public float SetMaxTravelTime { set { _maxTravelTime = value; } }
    public PM_Manager SetPMManager { set { _pmManagerRef = value; } }
    public Mechanic GetMyMechanic { get { return _myMechanic; } set { _myMechanic = value; } }
    public DUMBSTATE setMyState { set { _myDumbState = value; } }
}
