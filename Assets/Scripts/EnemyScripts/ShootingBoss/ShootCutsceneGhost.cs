using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCutsceneGhost : MonoBehaviour
{
    Animator _myAnims;

    SkinnedMeshRenderer _myrenderer;

    private void Awake()
    {
        _myAnims = GetComponent<Animator>();
        _myrenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
    }

    public void StartBigBoss()
    {
        _myAnims.Play("BigBoss", 0);
    }

    public void StartSmallBoss()
    {
        _myAnims.Play("SmallBoss", 0);
    }

    public void HideThineSelf()
    {
        _myrenderer.enabled = false;
    }
    
    public void MyReset()
    {
        _myrenderer.enabled = true;

        _myAnims.Play("Idle", 0);
    }
}
