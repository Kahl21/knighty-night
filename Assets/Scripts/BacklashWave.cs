using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacklashWave : MonoBehaviour {

    RaycastHit _hit;
    RaycastHit _ghostHit;

    Vector3 _movement;

    [Header("Base Variables")]
    [SerializeField]
    float _waveDamage;
    [SerializeField]
    float _waveDamageDivideOffsetForSpecialBosses;
    [SerializeField]
    float _waveSpeed;
    [SerializeField]
    float _waveKnockBack;
    [SerializeField]
    float _scaleMultiply;
    [SerializeField]
    float _waveLifetime;
    float _currTime;
    float _startTime;
    [SerializeField]
    float _waveUpScaleInc;
    float _currWaveUpScale;

    [Header("Detection Variables")]
    [SerializeField]
    float _waveDamageDist;
    float _calcAngle;
    [SerializeField]
    float _detectCurveAngle;
    [SerializeField]
    int _numOfCasts;
    [SerializeField]
    bool _debug = false;

    bool _increasing = true;

    Vector3 _stopSize;

    GameObject _caster;
    List<GameObject> _effectsObjs;

    //Init
    private void Awake()
    {
        _caster = transform.GetChild(0).gameObject;
        _effectsObjs = new List<GameObject>();
        _effectsObjs.Add(transform.GetChild(1).gameObject);
        _effectsObjs.Add(transform.GetChild(2).gameObject);
        _currWaveUpScale = 1f + _waveUpScaleInc;
        _stopSize = transform.localScale * _scaleMultiply;
        _calcAngle = 0;
        _startTime = Time.time;
    }

    private void Update()
    {
        Move();
        Expand();
    }

    //once spawned in
    //moves in the direction the player is facing
    //after an amount of time it dies
    private void Move()
    {
        _currTime = (Time.time - _startTime) / _waveLifetime;

        for (int i = 0; i <= _numOfCasts; i++)
        {
            float Xpos = Mathf.Cos(_calcAngle * Mathf.Deg2Rad) * _waveDamageDist;
            float Zpos = Mathf.Sin(_calcAngle * Mathf.Deg2Rad) * _waveDamageDist;

            Vector3 RayDir = (transform.forward * Zpos) + (transform.right * Xpos);

            if(_debug)
            {
                Debug.DrawLine(_caster.transform.position, _caster.transform.position + (RayDir *(_waveDamageDist + _currWaveUpScale)), Color.black);
            }

            _calcAngle += _detectCurveAngle / _numOfCasts;

            if (Physics.Raycast(_caster.transform.position, RayDir, out _hit, _waveDamageDist + _currWaveUpScale))
            {
                Debug.Log("hit something");
                GameObject thingHit = _hit.collider.gameObject;

                if (thingHit.GetComponent<BaseEnemy>())
                {
                    thingHit.GetComponent<BaseEnemy>().GotHit(RayDir, _waveKnockBack);
                }
                else if (thingHit.GetComponent<BossEnemy>())
                {
                    if(thingHit.GetComponent<ColorBossGlhost>() || thingHit.GetComponent<MiniBossColor>())
                    {
                        thingHit.GetComponent<BossEnemy>().GotHit(_waveDamage/_waveDamageDivideOffsetForSpecialBosses);
                    }
                    else
                    {
                        thingHit.GetComponent<BossEnemy>().GotHit(_waveDamage);
                    }
                }
            }
        }

        if(_currTime >= 1)
        {
            Destroy(gameObject);
        }

        _calcAngle = 0;
        _movement = transform.forward * _waveSpeed * Time.deltaTime;

        transform.position += _movement;
    }

    //expands the wave to have wider detection as it grows
    private void Expand()
    {
        if(_increasing)
        {
            if (transform.localScale.x <= _stopSize.x)
            {
                for (int i = 0; i < _effectsObjs.Count; i++)
                {
                    _effectsObjs[i].transform.localScale *= 1 + _waveUpScaleInc;
                }
                transform.localScale += Vector3.one * _waveUpScaleInc;
                _currWaveUpScale += _waveUpScaleInc;
            }
            else
            {
                _increasing = false;
            }
        }
        
    }

}
