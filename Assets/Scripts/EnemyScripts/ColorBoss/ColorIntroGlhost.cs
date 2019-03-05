using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIntroGlhost : MonoBehaviour {

    bool _looking = false;
    bool _walking = false;
    bool _hopping = false;
    bool _amDone = false;
    bool _amEaten = false;

    Vector3 _startPos;
    Quaternion _startRot;
    GameObject _parentOBJ;

    float _currTime;
    float _startTime;
    [SerializeField]
    Color _myColor;
    [SerializeField]
    float _lookSpeed;
    [SerializeField]
    float _lookDuration;
    [SerializeField]
    float _hopDistance;
    [SerializeField]
    float _hopDuration;
    [SerializeField]
    float _runningSpeed;
    Vector3 c0, c1, c2;

    GameObject _myBody;
    SkinnedMeshRenderer _myRenderer;
    Material _myMaterial;
    CapsuleCollider _myCapCollider;
    Animator _myAnimations;

    BossEnemy _myBoss;

    public void Start()
    {
        _myAnimations = GetComponent<Animator>();
        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _myMaterial = _myRenderer.materials[1];
        _myMaterial.color = _myColor;
    }

    public void Init()
    {
        _parentOBJ = transform.parent.gameObject;
        _myBoss = _parentOBJ.GetComponentInParent<BossEnemy>();
        _myCapCollider = GetComponent<CapsuleCollider>();

        transform.parent = null;
        _startPos = transform.position;
        _startRot = transform.rotation;

        _startTime = Time.time;
        _looking = true;
    }

    private void Update()
    {
        if (_looking)
        {
            LookAtBoss();
        }
        else if(_hopping)
        {
            JumpSlightly();
        }
        else if (_walking)
        {
            RunAway();
        }
    }

    private void LookAtBoss()
    {
        _currTime = (Time.time - _startTime) / _lookDuration;

        transform.Rotate(Vector3.up, _lookSpeed * Time.deltaTime);

        if (_currTime >= 1)
        {
            c0 = transform.position;
            c2 = transform.position;
            c1 = transform.position + (Vector3.up * _hopDistance);
            transform.LookAt(_myBoss.transform.position);
            _startTime = Time.time;
            _amDone = true;
            _looking = false;
            _hopping = true;
        }
    }

    private void JumpSlightly()
    {
        _currTime = (Time.time - _startTime) / _hopDuration;

        if(_currTime >= 1)
        {
            _currTime = 1;

            transform.LookAt(transform.position - transform.forward);
            _myBoss.CheckForIntroEnd();

            _hopping = false;
            _walking = true;
            _myAnimations.Play("Movement", 0);
        }

        Vector3 c01, c12, c012;

        c01 = (1 - _currTime) * c0 + _currTime * c1;
        c12 = (1 - _currTime) * c1 + _currTime * c2;

        c012 = (1 - _currTime) * c01 + _currTime * c12;

        transform.position = c012;
    }

    private void RunAway()
    {
        transform.position += transform.forward * _runningSpeed * Time.deltaTime;
    }

    public Color GotEaten()
    {
        _amEaten = true;
        _walking = false;
        
        _myCapCollider.enabled = false;
        gameObject.SetActive(false);

        return _myColor;
    }

    public void ParentYouself()
    {
        transform.parent = _parentOBJ.transform;
    }

    public void ResetCutscene()
    {
        _myCapCollider.enabled = true;
        gameObject.SetActive(true);

        transform.parent = _parentOBJ.transform;
        transform.position = _startPos;
        transform.rotation = _startRot;
        _looking = false;
        _walking = false;
        _hopping = false;
        _amDone = false;
        _amEaten = false;
    }

    public bool LookingIsDone { get { return _amDone; } }
    public bool AmEaten { get { return _amEaten; } }
}
