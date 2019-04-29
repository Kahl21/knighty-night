using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCutsceneGhost : MonoBehaviour
{
    Animator _myAnims;

    private void Awake()
    {
        _myAnims = GetComponent<Animator>();
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
        gameObject.SetActive(false);
    }
    
    public void MyReset()
    {
        gameObject.SetActive(true);
        _myAnims.Play("Idle", 0);
    }
}
