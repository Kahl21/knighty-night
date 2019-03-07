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
    protected bool _alwaysInvincible;
    [SerializeField]
    protected float _distanceToTransform;
    protected bool _canChange = true;
    protected GameObject _swarmChangePoint;

    public override void Init(DungeonMechanic _spawner, Mechanic _incomingMech)
    {
        base.Init(_spawner, _incomingMech);
        _startingColor = _spookColor;
        if(_alwaysInvincible)
        {
            _spookColor = _invincibleColor;
            _particle.SetActive(true);
            _invincible = true;
        }
        else
        {
            _particle.SetActive(false);
            _invStartTime = Time.time;
        }
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

                    if(!_canChange)
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
        if(Vector3.Distance(transform.position, _swarmChangePoint.transform.position) < _distanceToTransform)
        {
            _invincible = false;
            _particle.SetActive(false);
            _spookColor = _startingColor;
            _myMechanic = Mechanic.SWARM;
            _canChange = false;
        }
    }

    public bool IsChanged { get { return _canChange; } set { _canChange = value; } }
}
