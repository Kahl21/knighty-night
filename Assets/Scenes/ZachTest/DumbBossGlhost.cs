using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DumbBossGlhost : BaseEnemy
{
    //Strategy Enum for the DumbBossGhlosts
    public enum DUMBSTATE
    {
        NONE,
        ABSORBING,
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
    GhlostBossShooter _shooterRef;

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

        _spookColor = _mySpookiness.color;
        _spookColor.a = 0;
        _myRenderer.materials[1] = _mySpookiness;

        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Moving", 0);
        _startTime = Time.time;
    }

    public void InitColor(Color incColor)
    {
        _mySpookiness.color = incColor;
        _mySpookiness = _myRenderer.materials[1];
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!_dead)
        {
            if (_canMove)
            {
                switch (_myMechanic)
                {
                    case Mechanic.NONE:
                        Move();
                        CheckForHit();
                        break;

                    case Mechanic.SWARM:
                        break;

                    case Mechanic.CHASE:
                        switch (_myDumbState)
                        {
                            case DUMBSTATE.NONE:
                                Move();
                                CheckForHit();
                                break;

                            case DUMBSTATE.ABSORBING:
                                Move();
                                CheckForHit();
                                break;

                            case DUMBSTATE.DIE:
                                Move();
                                CheckForHit();
                                break;
                        }
                        break;

                    case Mechanic.COLOR:
                        Move();
                        CheckForHit();
                        break;

                    case Mechanic.TRAP:
                        break;

                    case Mechanic.BOSS:
                        break;

                    case Mechanic.HEAL:
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


    protected override void Move()
    {
        
        _myAgent.enabled = false;
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    public void ShootMe(Quaternion Shootingrotation)
    {
        _canMove = true;
        transform.rotation = _shooterRef.transform.rotation;
        _myDumbState = DUMBSTATE.NONE;
    }

    public void setMove(Vector3 rotation)
    {
        if (_myAgent.enabled == true)
        {
            _myAgent.SetDestination(transform.position);
            _myAgent.enabled = false;
        }
        _canMove = true;
        transform.LookAt(_shooterRef.transform);
        _myDumbState = DUMBSTATE.NONE;
    }

    protected void ColorMove()
    {
        _currTime = Time.time - _startTime;
        if (_currTime <= _maxTravelTime)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            moveSpeed = 0;
        }
    }

    void MoveToBoss()
    {
        _myAgent.enabled = true;
        _myAgent.speed = moveSpeed;
        _myAgent.SetDestination(_shooterRef.gameObject.transform.position);
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
            else if(thingHit.GetComponent<ShootingBoss>())
            {
                if (_myMechanic == Mechanic.COLOR)
                {
                    if (thingHit.GetComponent<ShootingBoss>().CheckForColor(0, _myColor) == true)
                    {
                        _shooterRef.removeGhlostFromScene(gameObject);
                        Destroy(gameObject);
                    }
                    else
                    {
                        thingHit.GetComponent<ShootingBoss>().HitByGhlost(gameObject, 0f);
                    }
                }
                else
                {
                    thingHit.GetComponent<ShootingBoss>().HitByGhlost(gameObject, 0f);
                }

            }
            else if(!thingHit.GetComponent<PlayerController>() && !thingHit.GetComponent<DumbBossGlhost>() && !thingHit.GetComponent<ShootingBoss>())
            {
                if (_myMechanic == Mechanic.NONE || _myMechanic == Mechanic.COLOR || _myDumbState == DUMBSTATE.DIE)
                {
                    _shooterRef.removeGhlostFromScene(gameObject);
                    Destroy(gameObject);
                }
                else if(_myMechanic == Mechanic.CHASE)
                {
                    _canMove = false;
                    moveSpeed = 0;
                }
            }
        }
    }

    //changes how clear the ghost is depending on how close they are to the player
    protected override void ChangeSpookiness()
    {
        _spookColor.a = (_spookDistance - Vector3.Distance(transform.position, _target.transform.position)) / 7.5f;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;
    }

    //function to be called when the player hits them
    //sets the ghost to the "Dead" state
    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        Debug.Log("Glhost Got Hit");
        if (!_hit && (_myMechanic == Mechanic.COLOR || _myMechanic == Mechanic.NONE))
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

        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if (hit.collider.GetComponent<ShootingBoss>())
            {
                hit.collider.GetComponent<ShootingBoss>().HitByGhlost(this.gameObject, _DamageToBoss);
                _shooterRef.removeGhlostFromScene(gameObject);
                Destroy(gameObject);
            }
            else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
            {
                _shooterRef.removeGhlostFromScene(gameObject);
                Destroy(gameObject);
            }
        }
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
        _shooterRef.removeGhlostFromScene(gameObject);
        Destroy(gameObject);
    }

    public float GetSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float SetDamageToBoss { set { _DamageToBoss = value; } }
    public float SetDamageToPlayer { set { _DamageToPlayer = value; } }
    public float SetMaxTravelTime { set { _maxTravelTime = value; } }
    public GhlostBossShooter SetShooterRef { set { _shooterRef = value; } }
    public Mechanic GetMyMechanic { get { return _myMechanic; } set { _myMechanic = value; } }
    public DUMBSTATE setMyState { set { _myDumbState = value; } }
}
