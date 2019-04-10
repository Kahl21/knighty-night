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

    GameObject _Audio;
    AudioSource _speaker;
    AudioManager _audioManager;
    float volSFX;
    public AudioClip leverHit;

    //init function while in a room
    public void Init(DungeonMechanic Room)
    {
        _myRoom = Room;
        _lever = transform.GetChild(1).gameObject;
        //Debug.Log(_lever.name);
        _startPos.z = _startingRotation;
        _lever.transform.localEulerAngles = _startPos;

        _Audio = GameObject.Find("AudioManager");
        _audioManager = _Audio.GetComponent<AudioManager>();
        volSFX = _audioManager.volSFX;
        _speaker = this.transform.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_activated && !_rotated)
        {
            activateSwitch();
        }
    }

    //called when player hits the lever
    //checks to see if all levers are hit
    public void StartRotation()
    {
        _activated = true;

        _myRoom.CheckForEnd();

        _startRotate = Time.time;
    }

    //rotates the lever to show its activated
    private void activateSwitch()
    {
        if (!_speaker.isPlaying)
            _speaker.PlayOneShot(leverHit, volSFX);

        _currRotate = (Time.time - _startRotate) / _rotateDuration;

        _lever.transform.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);

        if(_currRotate >= 1)
        {
            _currRotate = 1;

            _rotated = true;
        }
    }

    //reset function
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
