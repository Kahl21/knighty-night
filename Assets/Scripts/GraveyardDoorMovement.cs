using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardDoorMovement : DoorMovement
{

    GameObject leftDoor;
    GameObject rightDoor;

    [SerializeField]
    float _doorMoveDuration;
    float _startTime;
    float _currTime;

    // Use this for initialization
    protected override void Awake()
    {
        leftDoor = this.transform.GetChild(0).gameObject;
        rightDoor = this.transform.GetChild(1).gameObject;
    }

    public override void Init()
    {
        _startTime = Time.time;
        falling = true;
    }

    public override void RoomDone()
    {
        _startTime = Time.time;
        rising = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (falling)
        {
            Fall();
        }
        else if (rising)
        {
            Rise();
        }
    }

    protected override void Fall()
    {
        _currTime = (Time.time - _startTime) / _doorMoveDuration;

        leftDoor.transform.Rotate(Vector3.up * _doorSpeed);
        rightDoor.transform.Rotate(Vector3.down * _doorSpeed);

        if(_currTime >= 1)
        {
            _currTime = 1;
            falling = false;
        }
    }

    protected override void Rise()
    {
        _currTime = (Time.time - _startTime) / _doorMoveDuration;

        leftDoor.transform.Rotate(Vector3.down * _doorSpeed);
        rightDoor.transform.Rotate(Vector3.up * _doorSpeed);

        if (_currTime >= 1)
        {
            _currTime = 1;
            rising = false;
        }
    }
}