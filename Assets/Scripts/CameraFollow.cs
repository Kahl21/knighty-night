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

    public Vector3 GetOffset { get { return _offset; } }
    public bool AmFollowingPlayer { get { return _amFollowingPlayer; } set { _amFollowingPlayer = value; } }
}
