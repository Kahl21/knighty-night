using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardDoorMovement : DoorMovement
{

    GameObject leftDoor;
    GameObject rightDoor;

    [SerializeField]
    float _rotationAmount;
    Vector3 _leftRot;
    Vector3 _rightRot;
    Vector3 _leftGoTo;

    // Use this for initialization
    protected override void Awake()
    {
        leftDoor = this.transform.GetChild(0).gameObject;
        leftDoor.transform.parent = null;
        _leftRot = leftDoor.transform.localEulerAngles;
        _leftGoTo = _leftRot;
        _leftGoTo.y += _rotationAmount;
        rightDoor = this.transform.GetChild(0).gameObject;
        rightDoor.transform.parent = null;
        _rightRot = rightDoor.transform.localEulerAngles;
        _rightRot.y -= _rotationAmount;
    }

    public override void Init()
    {
        falling = true;
    }

    public override void RoomDone()
    {
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
        if(leftDoor.transform.localEulerAngles.y <= _leftGoTo.y)
        {
            leftDoor.transform.Rotate(Vector3.up * _doorSpeed);
            rightDoor.transform.Rotate(Vector3.down * _doorSpeed);
        }
        else
        {
            falling = false;
        }
        
    }

    protected override void Rise()
    {

        if (leftDoor.transform.localEulerAngles.y >= _leftRot.y)
        {
            leftDoor.transform.Rotate(Vector3.down * _doorSpeed);
            rightDoor.transform.Rotate(Vector3.up * _doorSpeed);
        }
        else
        {
            rising = false;
        }
    } 
}