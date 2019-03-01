using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicGlhost : BaseEnemy {

    
    private void Start()
    {
        _myCollider = GetComponent<Collider>();
        if(_canMove)
            _myAnimations.Play("Moving");
    }

    // Update is called once per frame
    protected override void Update ()
    {
        
        if(!_menuRef.GameIsPaused)
        {
            if (!_dead)
            {
                if (_canMove)
                {
                    Move();
                    CheckForHit();
                }
                else
                {
                    _myAnimations.Play("Nothing");
                }
            }
            else
            {
                Die();
            }
        }       
	}

    //Movement just makes all of the ghosts slowly move towards the player using an enemyAgent
    protected override void Move()
    {
        if(Vector3.Distance(transform.position, _target.transform.position) <= _damageRange)
        {
            //_myAnimations.Play("Moving");
            //this is where you match the animation speed with the ghost speed
            //_myAnimations.speed = 10;
            _myAgent.SetDestination(transform.position);
        }
        else
        {
            //animation breaker
            //_myAnimations.Play("Moving");
            _myAgent.SetDestination(_target.transform.position);
        }

        if (Vector3.Distance(transform.position, _target.transform.position) < _spookDistance)
        {
            ChangeSpookiness();
        }
    }

    //shoots a raycast in front of the enemy
    //if the raycast hits the player
    //the player takes damage
    protected override void CheckForHit()
    {
        if(Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, _damageRange))
        {
            GameObject thingHit = hit.collider.gameObject;
            if(thingHit.GetComponent<PlayerController>())
            {
                thingHit.GetComponent<PlayerController>().TakeDamage(_glhostDamage);
            }
        }
    }

    //changes how clear the ghost is depending on how close they are to the player
    protected override void ChangeSpookiness()
    {
        _spookColor.a = (_spookDistance - Vector3.Distance(transform.position, _target.transform.position))/7.5f;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;
    }

    //function to be called when the player hits them
    //sets the ghost to the "Dead" state
    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        if(!_hit)
        {
            _myAnimations.Play("Dazed");
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
        if (dead == false)
        {
            switch (_myMechanic)
            {
                case Mechanic.NONE:
                    Debug.Log("No Mechanic");
                    break;
                case Mechanic.SWARM:
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
                    break;
                case Mechanic.COLOR:

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
                    if (Physics.Raycast(transform.position + Vector3.up, _newDeathDirection, out hit, _collisionCheckDist))
                    {

                        if (hit.collider.GetComponent<ColoredBlock>())
                        {
                            ColoredBlock other = hit.collider.GetComponent<ColoredBlock>();
                            if (_myColor == other.GetColor)
                            {
                                _myCollider.enabled = false;
                                other.CorrectMatch();
                                _mySpawner.RemoveMe(this);

                                Dead();
                            }
                            else
                            {
                                _deathDirection = Vector3.zero;
                                _myAgent.enabled = true;
                                _hit = false;
                                _dead = false;
                            }
                        }
                        else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())

                        {
                            _newDeathDirection = Vector3.zero;
                            _myAgent.enabled = true;
                            _hit = false;
                            _dead = false;
                            _myAnimations.Play("Moving");
                        }

                    }

                    transform.position += _newDeathDirection * _knockBack * Time.deltaTime;
                    break;
                case Mechanic.BOSS:
                    break;
                default:
                    break;
            }
        }
    }

    //called when the room that the ghost is in is complete
    //removes the ghost from the game
    public override void Stop()
    {
        _canMove = false;
        if(!_dead && _myAgent.enabled)
        {
            _myAgent.SetDestination(transform.position);
        }
        _myCollider.enabled = false;
        _mySpawner.RemoveMe(this);

        Dead();
    }


    void Dead()
    {
        dead = true;
        _myAnimations.Play("Death");
        Destroy(gameObject, 1f);
    }
}
