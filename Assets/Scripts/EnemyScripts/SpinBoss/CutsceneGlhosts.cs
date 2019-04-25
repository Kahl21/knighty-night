using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneGlhosts : MonoBehaviour {

    bool _animating = false;
    [SerializeField]
    bool _AmLeft = false;
    [SerializeField]
    bool _AmMiddle = false;
    [SerializeField]
    bool _AmRight = false;
    PlayerController _player;
    GameManager _managerRef;
    Animator _myAnimations;
    string _introName;

    Vector3 _startPos;
    Quaternion _startRot;
    GameObject _parentOBJ;
    
    BossEnemy _myBoss;

    private void Awake()
    {
        _myAnimations = GetComponent<Animator>();
        _myAnimations.Play("Nothing", 0);
    }

    public void Init()
    {
        _parentOBJ = transform.parent.gameObject;
        _myBoss = _parentOBJ.GetComponentInParent<BossEnemy>();
        transform.parent = null;
        _startPos = transform.position;
        _startRot = transform.rotation;
        _player = PlayerController.Instance;
        BeginAnimation();
    }

    void BeginAnimation()
    {
        if(_AmLeft)
        {
            _myAnimations.Play("LeftIntro",0);
            _introName = "LeftIntro";
        }
        else if (_AmMiddle)
        {
            _myAnimations.Play("MiddleIntro", 0);
            _introName = "MiddleIntro";
        }
        else if (_AmRight)
        {
            _myAnimations.Play("RightIntro", 0);
            _introName = "RightIntro";
        }
        else
        {
            Debug.Log("animation bool not set");
        }

        _animating = true;
    }

    private void Update()
    {
        if (_animating)
        {
            DoTheAnimateThing();
        }
    }

    private void DoTheAnimateThing()
    {
        if (_myAnimations.GetCurrentAnimatorStateInfo(0).IsName(_introName))
        {
            ParentYouself();
            _animating = false;
        }
    }

    

    public void ParentYouself()
    {
        transform.parent = _parentOBJ.transform;
    }

    public void ResetCutscene()
    {
        transform.parent = _parentOBJ.transform;
        transform.position = _startPos;
        transform.rotation = _startRot;
        _animating = false;
        _myAnimations.Play("Nothing",0);
        
    }

    public bool AmDone { get { return _animating; } }
}
