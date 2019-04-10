using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    [SerializeField]
    Vector3 _startPositionRelativeToPlayer;
    PlayerController _player;
    Vector3 _playerPos;
    Vector3 _followStartPos;
    Vector3 _offset;

    float _introDuration;
    float _currIntroTime;
    float _startIntroTime;
    Vector3 cam0, cam1;
    Vector3 rot0, rot1;

    bool _amFollowingPlayer = false;

    private void Awake()
    {
        _player = PlayerController.Instance;
        _player.GetCamera = this;
        _playerPos = _player.transform.position;
        _followStartPos = _player.transform.position + _startPositionRelativeToPlayer;
        transform.position = _followStartPos;
        _offset = transform.position - _playerPos;
        _amFollowingPlayer = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if(_amFollowingPlayer)
        {
            _playerPos = _player.transform.position;
            transform.position = _playerPos + _offset;
        }
    }

    public void BossIntroActive(Vector3 startPos, Vector3 posToMoveTo, Vector3 StartEuler, Vector3 EulerToTurnTo, float duration)
    {
        cam0 = startPos;
        cam1 = posToMoveTo;

        rot0 = StartEuler;
        rot1 = EulerToTurnTo;

        _introDuration = duration;
        _startIntroTime = Time.time;
    }

    public bool MoveCamera()
    {
        _currIntroTime = (Time.time - _startIntroTime) / _introDuration;


        if(_currIntroTime >= 1)
        {
            _currIntroTime = 1;
            return true;
        }

        Vector3 cam01;

        cam01 = (1 - _currIntroTime) * cam0 + _currIntroTime * cam1;

        transform.localEulerAngles = Vector3.Slerp(rot0, rot1, _currIntroTime);
        transform.position = cam01;

        return false;
    }

    public Vector3 GetOffset { get { return _offset; } }
    public bool AmFollowingPlayer { get { return _amFollowingPlayer; } set { _amFollowingPlayer = value; } }
}
