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


    public bool isGate;
    GameObject leftDoor;
    GameObject rightDoor;

    // Use this for initialization
    void Awake () {
        

        if(isGate)
        {
            leftDoor = this.transform.GetChild(0).gameObject;
            rightDoor = this.transform.GetChild(1).gameObject;

        }

        else
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
	}

    public void Init()
    {
        if(!isGate)
        {
            _myRenderer.enabled = true;
            
        }

        falling = true;

    }

    public void RoomDone()
    {
        falling = false;
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
        if(isGate)
        {
            if(leftDoor.transform.eulerAngles.y < 180)
            {
                leftDoor.transform.Rotate(Vector3.up * _doorSpeed);
                rightDoor.transform.Rotate(Vector3.down * _doorSpeed);
            }

            /*if (leftDoor.transform.eulerAngles.y >= 0)
            {
                
            }

            else
            {
                rightDoor.transform.rotation = leftDoor.transform.rotation;
            }*/

        }

        else
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
        
    }

    void Rise()
    {
        if (isGate)
        {
            if (rightDoor.transform.eulerAngles.y != 90)
            {
                leftDoor.transform.Rotate(Vector3.down * _doorSpeed);
            }

            if (rightDoor.transform.eulerAngles.y != 90)
            {
                rightDoor.transform.Rotate(Vector3.up * _doorSpeed);
            }
        }

        else
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
}
