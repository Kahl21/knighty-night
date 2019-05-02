using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretBossCutsceneBosses : MonoBehaviour {
    
    [SerializeField]
    bool _amColorBoss;

    [SerializeField]
    float _initialMoveDistance;
    [SerializeField]
    float _moveDuration;

    [SerializeField]
    float _turnInvisibleDuration;

    [SerializeField]
    float _secondMoveDuration;
    float _startTime;
    float _currTime;

    bool _animating = false;
    bool _moving = true;
    bool _disappearing = true;
    bool _secondMove = true;
    bool _amDone = false;

    Vector3 c0, c1;

    GameObject _myBody;
    SkinnedMeshRenderer _myRenderer;
    Material _mySpookiness;
    Color _spookColor;
    Color _startingColor;

    Vector3 _startPos;
    Quaternion _startRot;
    GameObject _parentOBJ;

    Animator _myAnimations;

    BossEnemy _myBoss;
    PlayerController _player;

    private void Awake()
    {
        _parentOBJ = transform.parent.gameObject;
        _myBoss = _parentOBJ.GetComponentInParent<BossEnemy>();
        _myAnimations = GetComponent<Animator>();

        _myBody = transform.GetChild(1).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _mySpookiness = _myRenderer.materials[1];
        _startingColor = _mySpookiness.color;
        _spookColor = _mySpookiness.color;
        if (_amColorBoss)
        {
            _myAnimations.Play("Movement", 0);
        }
        else
        {
            _myAnimations.Play("Idle", 0);
        }
    }

    public void Init()
    {
        _parentOBJ = transform.parent.gameObject;
        _myBoss = _parentOBJ.GetComponentInParent<BossEnemy>();
        transform.parent = null;
        _startPos = transform.position;
        _startRot = transform.rotation;
        _player = PlayerController.Instance;

        transform.LookAt(_player.transform.position);

        c0 = transform.position - (transform.forward * _initialMoveDistance);
        c1 = transform.position;
        _startTime = Time.time;
        _animating = true;
    }

    private void Update()
    {
        if(_animating)
        {
            Animate();
        }
    }

    private void Animate()
    {
        if(_moving)
        {
            _currTime = (Time.time - _startTime) / _moveDuration;

            if(_currTime >= 1f)
            {
                _currTime = 1;
                
                _moving = false;
            }

            Vector3 c01;

            c01 = (c0 * (1 - _currTime)) + (c1 * _currTime);

            transform.LookAt(_player.transform.position);
            transform.position = c01;
        }
        else if(_disappearing)
        {
            _currTime = (Time.time - _startTime) / _turnInvisibleDuration;

            if (_currTime >= 1f)
            {
                _currTime = 1;

                _disappearing = false;
            }

            _spookColor.a = _currTime - 1;
            _mySpookiness.color = _spookColor;
            _mySpookiness = _myRenderer.materials[1];
        }
        else if(_secondMove)
        {
            _currTime = (Time.time - _startTime) / _secondMoveDuration;

            if (_currTime >= 1f)
            {
                _currTime = 1;

                _secondMove = false;
                _amDone = true;
            }

            Vector3 c01;

            c01 = (c0 * (1 - _currTime)) + (c1 * _currTime);

            transform.LookAt(_player.transform.position);
            transform.position = c01;
        }
    }

    public void ResetMe()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;

        _animating = false;
        _moving = true;
        _disappearing = true;
        _secondMove = true;
        _amDone = false;
    }

    public bool AmDone { get { return _amDone; } }
}
