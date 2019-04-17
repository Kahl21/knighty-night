using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretLever : MonoBehaviour
{
    [Header("Secret Door")]
    [SerializeField]
    GameObject _door;

    //lever variables
    [Header("Lever Variables")]
    [SerializeField]
    float _startingRotation;
    [SerializeField]
    float _rotateSpeed;
    float _currRotate;
    float _startRotate;
    [SerializeField]
    float _rotateDuration;
    static bool _activated = false;
    bool _rotated = false;

    GameObject _lever;
    Vector3 _startPos;




    //door variables
    [Header("Door Variables")]
    [SerializeField]
    protected float _doorSpeed;
    [SerializeField]
    protected float _raycastDist;

    protected Vector3 _endDoor;

    protected bool staying = false;
    protected bool falling = false;

    protected MeshRenderer _myRenderer;
    protected Material _myMaterial;
    protected Color _myColor;
    [SerializeField]
    protected float _fadeInc;

    //Audio Variables
    
    GameObject _Audio;
    AudioSource _speaker;
    AudioManager _audioManager;
    float volSFX;
    [Header("Audio Variables")]
    [SerializeField]
    AudioClip leverHit;


    // Use this for initialization
    void Awake()
    {
        _lever = transform.GetChild(1).gameObject;
        _startPos.z = _startingRotation;
        _lever.transform.localEulerAngles = _startPos;


        _myRenderer = _door.GetComponent<MeshRenderer>();
        _myMaterial = _myRenderer.material;
        _myColor = _myMaterial.color;
        _myColor.a = 1;
        _myMaterial.color = _myColor;
        _myRenderer.material = _myMaterial;
        _endDoor = _door.transform.position;
        _myRenderer.enabled = true;

        staying = true;


        _Audio = GameObject.Find("AudioManager");
        _audioManager = _Audio.GetComponent<AudioManager>();
        volSFX = _audioManager.volSFX;
        _speaker = this.transform.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_activated && !_rotated)
        {
            activateSwitch();
            
        }
        if (falling)
            Fall();


        else if (staying)
            Stay();
    }

    private void activateSwitch()
    {
        if (!_speaker.isPlaying)
            _speaker.PlayOneShot(leverHit, volSFX);

        _currRotate = (Time.time - _startRotate) / _rotateDuration;
        falling = true;

        _lever.transform.Rotate(transform.forward, _rotateSpeed * Time.deltaTime);

        if (_currRotate >= 1)
        {
            _currRotate = 1;

            _rotated = true;


        }
    }

    public void StartRotation()
    {
        Debug.Log("this shit works");
        _activated = true;
        _startRotate = Time.time;
    }

    public void Stay()
    {
        if (_door.transform.position.y >= _endDoor.y)
        {
            staying = false;
            _myColor.a = 1;
            _myMaterial.color = _myColor;
            _myRenderer.material = _myMaterial;
        }

        _myColor.a += _fadeInc;
        _myMaterial.color = _myColor;
        _myRenderer.material = _myMaterial;
        _door.transform.position += Vector3.up * _doorSpeed * Time.deltaTime;
    }


    public void Fall()
    {
        //if (transform.position.y <= _endDoor.y)
        if (_door.transform.position.y <= _endDoor.y-_raycastDist)
        {
            falling = false;
            //_door.transform.position = _endDoor;
            _myColor.a = 0;
            _myMaterial.color = _myColor;
            _myRenderer.material = _myMaterial;
            _myRenderer.enabled = false;
        }
        else
        {
            _myColor.a -= _fadeInc;
            _myMaterial.color = _myColor;
            _myRenderer.material = _myMaterial;
            _door.transform.position += Vector3.down * _doorSpeed * Time.deltaTime;
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

}
