using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour {

    [SerializeField]
    float _doorSpeed;
    [SerializeField]
    float _raycastDist;

    Vector3 _startPos;

    bool falling = false;
    bool rising = false;

    MeshRenderer _myRenderer;
    Material _myMaterial;
    Color _myColor;
    [SerializeField]
    float _fadeInc;

	// Use this for initialization
	void Awake () {
        _myRenderer = GetComponent<MeshRenderer>();
        _myMaterial = _myRenderer.material;
        _myColor = _myMaterial.color;
        _myColor.a = 0;
        _myMaterial.color = _myColor;
        _myRenderer.material = _myMaterial;
        _startPos = transform.position;
        _myRenderer.enabled = false;
	}

    public void Init()
    {
        _myRenderer.enabled = true;
        falling = true;
    }

    public void RoomDone()
    {
        rising = true;
    }

    // Update is called once per frame
    void Update () {
        if(falling)
        {
            Fall();
        }
        else if(rising)
        {
            Rise();
        }
	}

    void Fall()
    {
        if(Physics.Raycast(transform.position, Vector3.down, _raycastDist))
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

    void Rise()
    {
        if(transform.position.y >= _startPos.y)
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
