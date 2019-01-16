using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLever : MonoBehaviour {

    [SerializeField]
    float _startingRotation;
    [SerializeField]
    float _rotateSpeed;
    float _currRotate;
    float _startRotate;
    [SerializeField]
    float _rotateDuration;
    bool _activated = false;
    bool _rotated = false;

    GameObject _lever;
    Vector3 _startPos;

    DungeonMechanic _myRoom;

    public void Init(DungeonMechanic Room)
    {
        _myRoom = Room;
        _lever = transform.GetChild(1).gameObject;
        //Debug.Log(_lever.name);
        _startPos.z = _startingRotation;
        _lever.transform.localEulerAngles = _startPos;
    }

    private void Update()
    {
        if(_activated && !_rotated)
        {
            activateSwitch();
        }
    }

    public void StartRotation()
    {
        _activated = true;

        _myRoom.CheckForEnd();

        _startRotate = Time.time;
    }

    private void activateSwitch()
    {
        _currRotate = (Time.time - _startRotate) / _rotateDuration;

        _lever.transform.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);

        if(_currRotate >= 1)
        {
            _currRotate = 1;

            _rotated = true;
        }
    }

    public void ResetLever()
    {
        _activated = false;
        _rotated = false;
        _lever = transform.GetChild(1).gameObject;
        //Debug.Log(_lever.name);
        _lever.transform.localEulerAngles = _startPos;
    }

    public bool CheckIfActivated { get { return _activated; } }
    public DungeonMechanic SetMyRoom { set { _myRoom = value; } }
}
