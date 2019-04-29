using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhlostShooter : MonoBehaviour
{
    protected enum ATTACKSTATE
    {
        WAITFORATK,
        FINDATK,
        LINEATK,
        SPIRALATK,
        PULSEATK,
        PULSEHARDATK,
        FINISH
    }

    [Header("Ghlost Prefab")]
    [SerializeField]
    GameObject _ghlostPrefab;
    //[HideInInspector]
    public bool newAttack = false;
    [HideInInspector]
    public bool attackInProgress = false;
    [SerializeField]
    float _SpawnedDistAway;
    Animator _ghostAnimations;

    [Header("Pulse Attack variables")]
    [SerializeField]
    int _howManyPulses;
    int _realHowManyPulses;
    int _currentPulse = 0;
    [SerializeField]
    float _pulseDelay;
    float _realPulseDelay;
    [SerializeField]
    float _pulseGhlostTravelSpeed;
    float _realPulseGhlostTravelSpeed;
    [SerializeField]
    int _amountPerRing;
    int _realAmountPerRing;

    [Header("Line Attack Variables")]
    [SerializeField]
    float _lineCastTime;
    float _realLineCastTime;
    [SerializeField]
    int _degreeOfCastCone;
    int _realDegreeOfCastCone;
    [SerializeField]
    float _lineSpawnRate;
    float _realLineSpawnRate;
    [SerializeField]
    float _lineRotateSpeed;
    float _realLineRotateSpeed;
    [SerializeField]
    float _lineGhlostTravelSpeed;
    float _realLineGhlostTravelSpeed;
    int GhlostsCast = 0;
    Vector3 _startingAngle;

    [Header("Spiral Attack Variables")]
    [SerializeField]
    float _spiralCastTime;
    float _realSpiralCastTime;
    [SerializeField]
    float _spiralSpawnRate;
    float _realSpiralSpawnRate;
    [SerializeField]
    float _spiralRotateSpeed;
    float _realSpiralRotateSpeed;
    [SerializeField]
    float _spiralGhlostTravelSpeed;
    float _realSpiralGhlostTravelSpeed;

    [Header("Attack Percentages")]
    [SerializeField]
    float _pulseAttackPercentage;
    float _realPulseAttackPercentage;
    [SerializeField]
    float _lineAttackPercentage;
    float _realLineAttackPercentage;
    [SerializeField]
    float _spiralAttackPercentage;
    float _realSpiralAttackPercentage;

    [Header("Hard Pulse Attack variables")]
    [SerializeField]
    int _hardHowManyPulses;
    [SerializeField]
    float _hardPulseDelay;
    [SerializeField]
    float _hardPulseGhlostTravelSpeed;
    [SerializeField]
    int _hardAmountPerRing;

    [Header("Hard Line Attack Variables")]
    [SerializeField]
    float _hardLineCastTime;
    [SerializeField]
    int _hardDegreeOfCastCone;
    [SerializeField]
    float _hardLineSpawnRate;
    [SerializeField]
    float _hardLineRotateSpeed;
    [SerializeField]
    float _hardLineGhlostTravelSpeed;

    [Header("Hard Spiral Attack Variables")]
    [SerializeField]
    float _hardSpiralCastTime;
    [SerializeField]
    float _hardSpiralSpawnRate;
    [SerializeField]
    float _hardSpiralRotateSpeed;
    [SerializeField]
    float _hardSpiralGhlostTravelSpeed;

    [Header("Hard Attack Percentages")]
    [SerializeField]
    float _hardPulseAttackPercentage;
    [SerializeField]
    float _hardLineAttackPercentage;
    [SerializeField]
    float _hardSpiralAttackPercentage;

    float _totalAttackPercentages;

    [SerializeField]
    List<GameObject> _ghlostsInScene = new List<GameObject>();

    ATTACKSTATE _attackState = ATTACKSTATE.WAITFORATK;
    float startTime;
	
    public void Init()
    {
        if (!GameManager.Instance.HardModeOn)
        {
            _realLineAttackPercentage = _lineAttackPercentage;
            _realLineCastTime = _lineCastTime;
            _realLineGhlostTravelSpeed = _lineGhlostTravelSpeed;
            _realLineRotateSpeed = _lineRotateSpeed;
            _realLineSpawnRate = _lineSpawnRate;
            _realDegreeOfCastCone = _degreeOfCastCone;

            _realPulseAttackPercentage = _pulseAttackPercentage;
            _realPulseDelay = _pulseDelay;
            _realPulseGhlostTravelSpeed = _pulseGhlostTravelSpeed;
            _realAmountPerRing = _amountPerRing;
            _realHowManyPulses = _howManyPulses;

            _realSpiralAttackPercentage = _spiralAttackPercentage;
            _realSpiralCastTime = _spiralCastTime;
            _realSpiralGhlostTravelSpeed = _spiralGhlostTravelSpeed;
            _realSpiralRotateSpeed = _spiralRotateSpeed;
            _realSpiralSpawnRate = _spiralSpawnRate;
        }
        else
        {
            _realLineAttackPercentage = _hardLineAttackPercentage;
            _realLineCastTime = _hardLineCastTime;
            _realLineGhlostTravelSpeed = _hardLineGhlostTravelSpeed;
            _realLineRotateSpeed = _hardLineRotateSpeed;
            _realLineSpawnRate = _hardLineSpawnRate;
            _realDegreeOfCastCone = _hardDegreeOfCastCone;

            _realPulseAttackPercentage = _hardPulseAttackPercentage;
            _realPulseDelay = _hardPulseDelay;
            _realPulseGhlostTravelSpeed = _hardPulseGhlostTravelSpeed;
            _realAmountPerRing = _hardAmountPerRing;
            _realHowManyPulses = _hardHowManyPulses;

            _realSpiralAttackPercentage = _hardSpiralAttackPercentage;
            _realSpiralCastTime = _hardSpiralCastTime;
            _realSpiralGhlostTravelSpeed = _hardSpiralGhlostTravelSpeed;
            _realSpiralRotateSpeed = _hardSpiralRotateSpeed;
            _realSpiralSpawnRate = _hardSpiralSpawnRate;
        }
    }

	// Update is called once per frame
	protected void Update ()
    {
		switch(_attackState)
        {
            case ATTACKSTATE.WAITFORATK:
                WaitForAttack();
                break;
            case ATTACKSTATE.FINDATK:
                FindAttack();
                break;
            case ATTACKSTATE.LINEATK:
                LineAttack();
                break;
            case ATTACKSTATE.SPIRALATK:
                spiralAttack();
                break;
            case ATTACKSTATE.PULSEATK:
                PulseAttack();
                break;
            case ATTACKSTATE.PULSEHARDATK:
               
                break;
            case ATTACKSTATE.FINISH:
                break;
        }
	}

    private void WaitForAttack()
    {
        if (newAttack == true)
        {
            newAttack = false;
            attackInProgress = true;
            _attackState = ATTACKSTATE.FINDATK;
        }
        else
        {
            attackInProgress = false;
        }
    }

    private void FindAttack()
    {
        Debug.Log("New Attack");
        GhlostsCast = 0;
        _totalAttackPercentages = _realPulseAttackPercentage + _realSpiralAttackPercentage + _realLineAttackPercentage;
        float _nextAttack = Random.Range(0, _totalAttackPercentages);
        Debug.Log("Next attack: " + _nextAttack);
        if (_nextAttack > 0 && _nextAttack <= _realPulseAttackPercentage)
        {
            Debug.Log("Pulse Attack");

            _ghostAnimations.Play("ShootStart", 0);
            _attackState = ATTACKSTATE.PULSEATK;
            _currentPulse = 0;
        }
        else if (_nextAttack > _realPulseAttackPercentage && _nextAttack <= (_realPulseAttackPercentage + _realLineAttackPercentage))
        {
            Debug.Log("Line Attack");
            _ghostAnimations.Play("ShootStart", 0);
            _startingAngle = transform.eulerAngles;
            _attackState = ATTACKSTATE.LINEATK;
        }
        else
        {
            Debug.Log("Spiral Attack");
            _ghostAnimations.Play("ShootStart", 0);
            _attackState = ATTACKSTATE.SPIRALATK;
        }
        startTime = Time.time;
    }

    private void spiralAttack()
    {
        GameObject ghlostObj;
        float timeTaken = Time.time - startTime;

        if (timeTaken < _realSpiralCastTime)
        {
            transform.eulerAngles += new Vector3(0, 1 * _realSpiralRotateSpeed * Time.deltaTime, 0);

            if (timeTaken >= _realSpiralSpawnRate * GhlostsCast)
            {
                //Debug.Log(timeTaken + " VS. " + _spiralSpawnRate * GhlostsCast);
                GhlostsCast += 1;
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbGlhost>().GetSpeed = _realSpiralGhlostTravelSpeed;
            }
        }
        else
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("Stop Casting");
        }
    }

    private void LineAttack()
    {
        GameObject ghlostObj;
        float timeTaken = Time.time - startTime;
        
        if (timeTaken < _realLineCastTime)
        {
            //Rotates back and forth through the cone angle / 2 * -1 and 1
            float angle = Mathf.Sin(Time.time * _realLineRotateSpeed) * ((_realDegreeOfCastCone / 2));
            gameObject.transform.LookAt(GetComponent<ShootingMiniBoss>().GetPlayerRef.gameObject.transform);
            transform.rotation = Quaternion.AngleAxis(angle + transform.eulerAngles.y, Vector3.up);

            //Spawns a new glhost based on the spawnrate
            if (timeTaken >= _realLineSpawnRate * GhlostsCast)
            {
                //Debug.Log(timeTaken + " VS. " + _lineSpawnRate * GhlostsCast);
                GhlostsCast+= 1;
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbGlhost>().GetSpeed = _realLineGhlostTravelSpeed;
            }
        }
        else
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("Stop Casting");
        }
    }

    private void PulseAttack()
    {
        
        float timeTaken = Time.time - startTime;
        if (timeTaken > _realPulseDelay * _currentPulse)
        {
            int spacingAngle = 360 / _realAmountPerRing;
            GameObject ghlostObj;
            for (int i = 0; i < _realAmountPerRing; i++)
            {
                transform.eulerAngles += new Vector3(0, spacingAngle, 0);
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbGlhost>().GetSpeed = _realPulseGhlostTravelSpeed;
            }
            _currentPulse++;
        }
        if (_currentPulse == _realHowManyPulses)
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("stop pulsing");
        }
    }

    private GameObject SpawnGhlost()
    {
        GameObject newGhlost;
        newGhlost = Instantiate(_ghlostPrefab, transform.position + (transform.forward * _SpawnedDistAway), transform.rotation);
        newGhlost.GetComponent<DumbGlhost>().SetDamageToBoss = gameObject.GetComponent<ShootingMiniBoss>().GetDamageToBoss;
        newGhlost.GetComponent<DumbGlhost>().SetDamageToPlayer = gameObject.GetComponent<ShootingMiniBoss>().GetDamageToPlayer;
        newGhlost.transform.parent = null;
        _ghlostsInScene.Add(newGhlost);

        newGhlost.GetComponent<DumbGlhost>().SetShooterRef = gameObject.GetComponent<GhlostShooter>();
        return newGhlost;
    }

    public void removeGhlostFromScene(GameObject ghlostsToRemove)
    {
        for (int i = 0; i < _ghlostsInScene.Count; i++)
        {
            if (_ghlostsInScene[i] == ghlostsToRemove)
            {
                _ghlostsInScene.Remove(ghlostsToRemove);
                return;
            }
        }
        Debug.Log("Not In Scene");
    }

    public void MyReset()
    {
        _attackState = ATTACKSTATE.WAITFORATK;
        for (int index = 0; index < _ghlostsInScene.Count; index++)
        {
            GameObject ghlostRef;
            ghlostRef = _ghlostsInScene[index];
            Destroy(ghlostRef);
            
            
        }
        _ghlostsInScene.Clear();
    }

    public Animator GetBossAnimator { get { return _ghostAnimations; } set { _ghostAnimations = value; } }
}
