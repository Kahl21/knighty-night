using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMinion : BasicGlhost {

    protected bool _justSpawned;
    [SerializeField]
    protected float _initalSpawnSpeed;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _mySpawner.AddEnemy(this);
        _myAnimations.Play("Nothing", 0);
        _justSpawned = true;
        _hit = true;
        _myAgent.enabled = false;
        _knockBack = _initalSpawnSpeed;
        _deathDirection = transform.forward;
        _deathDirection.y = 0;
        _dead = true;
    }

    protected override void Die()
    {
        if (Physics.Raycast(transform.position + Vector3.up, _deathDirection, out hit, _collisionCheckDist))
        {
            if(hit.collider.GetComponent<PlayerController>())
            {
                hit.collider.GetComponent<PlayerController>().TakeDamage(_glhostDamage);
            }
            else if(hit.collider.GetComponent<BossEnemy>() && !_justSpawned)
            {
                hit.collider.GetComponent<BossEnemy>().GotHit(_glhostDamage);
                _mySpawner.RemoveMe(this);
                Destroy(gameObject);
            }
            else if (!hit.collider.GetComponent<BaseEnemy>() && !hit.collider.GetComponent<BossEnemy>())
            {
                if(_justSpawned)
                {
                    _justSpawned = false;
                    _hit = false;
                    _myAgent.enabled = true;
                    _dead = false;
                    _myAnimations.Play("Moving", 0);
                }
                else
                {
                    _mySpawner.RemoveMe(this);
                    Destroy(gameObject);
                }
            }
        }
        transform.position += _deathDirection * _knockBack * Time.deltaTime;
    }
}
