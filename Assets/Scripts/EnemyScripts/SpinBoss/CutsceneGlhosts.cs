using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneGlhosts : MonoBehaviour {

    bool _looking = false;
    bool _walking = false;
    bool _amDone = false;
    PlayerController _player;
    GameManager _managerRef;

    Vector3 _startPos;
    Quaternion _startRot;
    GameObject _parentOBJ;

    float _currTime;
    float _startTime;
    [SerializeField]
    float _lookSpeed;
    [SerializeField]
    float _lookDuration;
    [SerializeField]
    float _walkDuration;
    [SerializeField]
    float _walkDistance;
    Vector3 c0, c1;
    BossEnemy _myBoss;

    public void Init()
    {
        _parentOBJ = transform.parent.gameObject;
        _myBoss = _parentOBJ.GetComponentInParent<BossEnemy>();
        transform.parent = null;
        _startPos = transform.position;
        _startRot = transform.rotation;
        _player = PlayerController.Instance;

        _startTime = Time.time;
        _looking = true;
    }

    private void Update()
    {
        if (_looking)
        {
            LookAtPlayer();
        }
        else if (_walking)
        {
            WalkAtPlayer();
        }
    }

    private void LookAtPlayer()
    {

        _currTime = (Time.time - _startTime) / _lookDuration;

        transform.Rotate(Vector3.up, _lookSpeed * Time.deltaTime);

        if (_currTime >= 1)
        {
            transform.LookAt(transform.position - Vector3.forward);

            c0 = transform.position;
            c1 = transform.position + (transform.forward * _walkDistance);
            _startTime = Time.time;
            _looking = false;
            _walking = true;
        }

        
    }

    private void WalkAtPlayer()
    {
        _currTime = (Time.time - _startTime) / _walkDuration;

        Vector3 c01;

        c01 = (1 - _currTime) * c0 + _currTime * c1;

        if (_currTime >= 1)
        {
            _currTime = 1;

            _amDone = true;

            _myBoss.CheckForIntroEnd();

           _walking = false;
        }

        transform.position = c01;
    }

    public void ParentYouself()
    {
        transform.parent = _parentOBJ.transform;
    }

    public void ResetCutscene()
    {
        transform.parent = _parentOBJ.transform;
        transform.position = _startPos;
        transform.rotation = _startRot;
        _looking = false;
        _walking = false;
        _amDone = false;
    }

    public bool AmDone { get { return _amDone; } }
}
