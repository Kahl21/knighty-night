using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardGlhost : BasicGlhost {

    [Header("Graveyard Ghlost Variables")]
    [SerializeField]
    protected Color _invincibleColor;
    protected Color _startingColor;
    [SerializeField]
    protected GameObject _particle;
    [SerializeField]
    protected float _invincibleChangeDuration;
    protected float _invCurrTime;
    protected float _invStartTime;
    protected bool _invincible = false;

    [Header("Chase Ghlost Variables")]
    [SerializeField]
    protected float _changedDamage;
    [SerializeField]
    protected float _changedSpeed;
    [SerializeField]
    protected bool _alwaysInvincible;
    [SerializeField]
    protected bool _doesNotChangeSpookieness;
    protected float _distanceToTransform;
    protected bool _canChange = false;
    protected GameObject _swarmChangePoint;
    

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);

        if(_doesNotChangeSpookieness)
        {
            _spookColor.a = 1;
            _mySpookiness.color = _spookColor;
            _myRenderer.materials[1] = _mySpookiness;
        }

        _startingColor = _spookColor;

        if (_alwaysInvincible)
        {
            _spookColor = _invincibleColor;
            _distanceToTransform = _mySpawner.GetChangeDistance;
            _particle.SetActive(true);
            _invincible = true;
            _canChange = true;
        }
        else
        {
            _particle.SetActive(false);
            _invStartTime = Time.time;
        }

        AudioManager.instance.ChaseStart();
    }
    
    protected override void Update()
    {
        if (!_menuRef.GameIsPaused)
        {
            if (!_dead)
            {
                if (_canMove)
                {
                    if(!_alwaysInvincible)
                    {
                        ChangeInvincible();
                    }

                    if(_canChange)
                    {
                        CheckForTransform();
                    }

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
    protected override void ChangeSpookiness()
    {
        if(!_doesNotChangeSpookieness)
        {
            base.ChangeSpookiness();
        }
    }

    protected void ChangeInvincible()
    {
        _invCurrTime = (Time.time - _invStartTime) / _invincibleChangeDuration;

        if(_invCurrTime >= 1f)
        {
            _invCurrTime = 1f;

            if(_invincible)
            {
                _invincible = false;
                _particle.SetActive(false);
                _startingColor.a = _spookColor.a;
                _spookColor = _startingColor;
                _mySpookiness.color = _spookColor;
                _myRenderer.materials[1] = _mySpookiness;
            }
            else
            {
                _invincible = true;
                _particle.SetActive(true);
            }
            _invStartTime = Time.time;
        }

        if(!_invincible)
        {
            AudioManager.instance.ChaseStop();
            Color _inBetweenColor;

            _inBetweenColor = _invCurrTime * _invincibleColor + (1 - _invCurrTime) * _startingColor;

            _inBetweenColor.a = _spookColor.a;
            _spookColor = _inBetweenColor;
            _mySpookiness.color = _spookColor;
            _myRenderer.materials[1] = _mySpookiness;
        }
    }

    public void ColorChange(Color incColor)
    {
        _invincibleColor = incColor;
        _spookColor = _invincibleColor;
        _mySpookiness.color = _spookColor;
        _myRenderer.materials[1] = _mySpookiness;
    }

    public override void GotHit(Vector3 _flyDir, float _knockBackForce)
    {
        if(!_invincible)
        {
            base.GotHit(_flyDir, _knockBackForce);
        }
    }

    private void CheckForTransform()
    {
        Vector3 changePoint = _swarmChangePoint.transform.position;
        changePoint.y = transform.position.y;

        if (Vector3.Distance(transform.position, changePoint) < _distanceToTransform)
        {
            _invincible = false;
            _particle.SetActive(false);
            _glhostDamage = _changedDamage;
            _myAgent.speed = _changedSpeed;
            _spookColor = _startingColor;
            _mySpookiness.color = _spookColor;
            _myRenderer.materials[1] = _mySpookiness;
            _canChange = false;
            _dead = false;
        }
    }

    public bool IsChanged { get { return _canChange; } set { _canChange = value; } }
    public bool AmInvincible { get { return _invincible; } }
    public GameObject SetEndPoint { set { _swarmChangePoint = value; } }
}
