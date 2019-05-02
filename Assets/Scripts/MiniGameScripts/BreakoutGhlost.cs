using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutGhlost : BaseEnemy {

    [SerializeField]
    float _spinningSpeed;
    [SerializeField]
    float _defaultMoveSpeed;
    float _moveSpeed;
    Vector3 _moveDir;

    Vector3 _startPos;

    BreakoutManager _myManager;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _moveSpeed = _defaultMoveSpeed;
        _startPos = transform.position;
        _init = true;
        Debug.Log("called");
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if(_init)
        {
            if(!_menuRef.GameIsPaused)
            {
                if (!_dead)
                {
                    SpinAround();
                }
                else
                {
                    CheckForHit();
                    Die();
                }
            }
        }
		
	}

    private void SpinAround()
    {
        transform.Rotate(Vector3.up, _spinningSpeed);
    }

    protected override void CheckForHit()
    {
        if (Physics.Raycast(transform.position + Vector3.up, _moveDir, out hit, _collisionCheckDist))
        {
            Vector3 checkDir = _moveDir;
            GameObject thingHit = hit.collider.gameObject;

            //Debug.Log("reflected");
            if(thingHit.GetComponent<BreakoutBlock>() && !thingHit.GetComponent<BreakoutBlock>().SetDeathPillar)
            {
                _moveDir = Vector3.Reflect(checkDir, hit.normal);
                thingHit.GetComponent<BreakoutBlock>().CorrectMatch();
                _hit = false;
            }
            else if (thingHit.GetComponent<BreakoutBlock>() && thingHit.GetComponent<BreakoutBlock>().SetDeathPillar)
            {
                thingHit.GetComponent<BreakoutBlock>().DamagePlayer();
                _moveDir = new Vector3(0, 0, 0);
                transform.position = _startPos;
                _hit = false;
            }
            else if (!thingHit.GetComponent<BaseEnemy>())
            {
                _moveDir = Vector3.Reflect(checkDir, hit.normal);
                _hit = false;
            }
            

        }
    }

    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        if(!_hit)
        {
            _flyDir.y = transform.position.y;
            _moveDir = _flyDir;
            _dead = true;
            _hit = true;
        }
    }

    protected override void Die()
    {
        _moveDir.y = transform.position.y;
        transform.Rotate(Vector3.up, _spinningSpeed);
        transform.position += _moveDir * _moveSpeed * Time.deltaTime;
    }

    public BreakoutManager GetMyManager { get { return _myManager; } set { _myManager = value; } }
}
