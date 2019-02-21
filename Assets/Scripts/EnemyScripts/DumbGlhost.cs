using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DumbGlhost : BaseEnemy
{
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float _DamageToBoss;
    [SerializeField]
    float _DamageToPlayer;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        _myAgent.enabled = false;
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
    protected override void Update ()
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
            CheckForHit();
            Die();
        }       
	}


    //Movement just makes all of the ghosts slowly move towards the player using an enemyAgent
    protected override void Move()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
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
                thingHit.GetComponent<PlayerController>().TakeDamage(_DamageToPlayer);
            }
            else
            {
                Destroy(gameObject);
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
        Debug.Log("Glhost Got Hit");
        if(!_hit)
        {
            _hit = true;
            //_myAgent.enabled = false;
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
            if (hit.collider.GetComponent<ShootingMiniBoss>())
            {
                hit.collider.GetComponent<ShootingMiniBoss>().HitByGhlost(this.gameObject, _DamageToPlayer);
                Destroy(gameObject);
            }
            else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<PlayerController>())
            {
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
        if(!_dead)
        {
            _myAgent.SetDestination(transform.position);
        }
        _mySpawner.RemoveMe(this);
        Destroy(gameObject);
    }

    public float GetSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float SetDamageToBoss { set { _DamageToBoss = value; } }
    public float SetDamageToPlayer { set { _DamageToPlayer = value; } }
}
