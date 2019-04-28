using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIntroGlhost : MonoBehaviour {

    bool _running = false;

    Vector3 _startPos;
    Quaternion _startRot;
    GameObject _parentOBJ;
    [SerializeField]
    Color _myColor;
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
    }

    private void Update()
    {
        if(_running)
        {
            RunAway();
        }
    }

    public void LookAtBoss(int ghostNum)
    {
        _myAnimations.Play("Ghost" + ghostNum, 0);
        if(ghostNum ==2)
        {
            Vector3 lookpos = transform.position + Vector3.left;
            lookpos.y = transform.position.y;
            transform.LookAt(lookpos);
        }
    }

    public void StartRun()
    {
        _myAnimations.Play("Movement", 0);

        Vector3 lookpos = transform.position + Vector3.right;
        lookpos.y = transform.position.y;
        transform.LookAt(lookpos);
        _running = true;
    }

    private void RunAway()
    {
        transform.position += transform.forward * _runningSpeed * Time.deltaTime;
    }

    public Color GotEaten()
    {        
        gameObject.SetActive(false);

        return _myColor;
    }

    public void ParentYouself()
    {
        transform.parent = _parentOBJ.transform;
    }

    public void ResetCutscene()
    {
        gameObject.SetActive(true);

        transform.parent = _parentOBJ.transform;
        transform.position = _startPos;
        transform.rotation = _startRot;
        _running = false;
    }
}
