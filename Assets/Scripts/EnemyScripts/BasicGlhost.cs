using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicGlhost : BaseEnemy { 

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
            }
            else
            {
                Die();
            }
        }       
	}

    //makes the ghost die in a certain way
    //depending on what kind of room the ghost is in
    protected override void Die()
    {
        //Debug.DrawLine(transform.position, transform.position + _deathDirection*_collisionCheckDist);

        base.Die();
        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
            {
                _mySpawner.RemoveMe(this);
                _mySpawner.CheckForEnd();
                Destroy(gameObject);
            }
        }
        transform.position += _deathDirection * _knockBack * Time.deltaTime;

        switch (_myMechanic)
        {
            case Mechanic.NONE:
                Debug.Log("No Mechanic");
                break;
            case Mechanic.SWARM:
                base.Die();
                if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
                {
                    if(!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
                    {
                        _mySpawner.RemoveMe(this);
                        _mySpawner.CheckForEnd();
                        Destroy(gameObject);
                    }
                }
                transform.position += _deathDirection * _knockBack * Time.deltaTime;
                break;
            case Mechanic.COLOR:
                Vector3 _newDeathDirection = _deathDirection;
                if(Vector3.Distance(transform.position, _myPillar.transform.position) <= _cheatingDistance)
                {
                    if(transform.position.z > _myPillar.transform.position.z)
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
                if (Physics.Raycast(transform.position + Vector3.up, _newDeathDirection,  out hit, _collisionCheckDist))
                {
                   
                    if (hit.collider.GetComponent<ColoredBlock>())
                    {
                        ColoredBlock other = hit.collider.GetComponent<ColoredBlock>();
                        if (_myColor == other.GetColor)
                        {
                            other.CorrectMatch();
                            _mySpawner.RemoveMe(this);
                            Destroy(gameObject);
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
                    }
                    
                }

                transform.position += _newDeathDirection * _knockBack * Time.deltaTime;
                break;
            default:
                break;
        }
    }
}
