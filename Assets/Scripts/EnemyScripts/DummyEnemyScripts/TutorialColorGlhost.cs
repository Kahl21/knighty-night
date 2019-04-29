using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialColorGlhost : BaseEnemy  {

    Vector3 _spawnPoint;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _mySpawner = _spawner;
        _spawnPoint = transform.position;
        _myAnimations = GetComponent<Animator>();

        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _mySpookiness = _myRenderer.materials[1];

        _mySpookiness.color = _myColor;
        _myRenderer.materials[1] = _mySpookiness;

        _speaker = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_dead)
        {
            Die();
        }
    }

    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        if (!_hit)
        {
            Debug.Log("Dazed_Start");
            _hit = true;
            _myAgent.enabled = false;
            _knockBack = _knockBackForce;
            _deathDirection = _flyDir;
            _deathDirection.y = 0;

            _dead = true;
        }
    }

    protected override void Die()
    {
        if(_actualDead)
        {
            _myAnimations.Play("Dazed_Loop");
        }

        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if (hit.collider.GetComponent<ColoredBlock>())
            {
                ColoredBlock other = hit.collider.GetComponent<ColoredBlock>();
                if (_myColor == other.GetColor)
                {
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
            else
            {
                _deathDirection = Vector3.zero;
                _hit = false;
                _dead = false;
                transform.position = _spawnPoint;
            }
        }

        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }

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
    
}
