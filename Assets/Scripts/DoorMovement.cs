using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour {

    [SerializeField]
    protected float _doorSpeed;
    [SerializeField]
    protected float _raycastDist;

    protected Vector3 _startPos;

    protected bool falling = false;
    protected bool rising = false;

    protected MeshRenderer _myRenderer;
    protected Material _myMaterial;
    protected Color _myColor;
    [SerializeField]
    protected float _fadeInc;

    protected GameObject _Audio;
    protected AudioManager _audioManager;
    protected float volSFX;
    protected AudioSource _speaker;

    // Use this for initialization
    protected  virtual void Awake ()
    {
        _myRenderer = GetComponent<MeshRenderer>();
        _myMaterial = _myRenderer.material;
        _myColor = _myMaterial.color;
        _myColor.a = 0;
        _myMaterial.color = _myColor;
        _myRenderer.material = _myMaterial;
        _startPos = transform.position;
        _myRenderer.enabled = false;
	}

    public virtual void Init()
    {
        _myRenderer.enabled = true;
        falling = true;

        _Audio = GameObject.Find("AudioManager");
        _audioManager = _Audio.GetComponent<AudioManager>();
        volSFX = _audioManager.volSFX;
    }

    public virtual void RoomDone()
    {
        falling = false;
        rising = true;
    }

    // Update is called once per frame
    protected virtual void Update () {
        if(falling)
        {
            Fall();
        }
        else if(rising)
        {
            Rise();
        }
    }

    protected virtual void Fall()
    {
        if (Physics.Raycast(transform.position, Vector3.down, _raycastDist))
        {
            falling = false;
            _myColor.a = 1;
            _myMaterial.color = _myColor;
            _myRenderer.material = _myMaterial;
        }

        _myColor.a += _fadeInc;
        _myMaterial.color = _myColor;
        _myRenderer.material = _myMaterial;
        transform.position += Vector3.down * _doorSpeed * Time.deltaTime;
    }

    protected virtual void Rise()
    {
        if (transform.position.y >= _startPos.y)
        {
            rising = false;
            transform.position = _startPos;
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
            transform.position += Vector3.up * _doorSpeed * Time.deltaTime;
        }
    }
}
