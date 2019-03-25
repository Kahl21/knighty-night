using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhlostBossShooter : MonoBehaviour
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
    GameObject _normalGhlostPrefab;
    [SerializeField]
    GameObject _InvincibleGhlostPrefab;
    //[SerializeField]
    GameObject _ColoredGhlostPrefab;
    //[HideInInspector]
    public bool newAttack = false;
    [HideInInspector]
    public bool attackInProgress = false;
    [SerializeField]
    float _SpawnedDistAway;

    //[Header("Pulse Attack variables")]
    //[SerializeField]
    int _howManyPulses;
    int _currentPulse = 0;
    //[SerializeField]
    float _pulseDelay;
    //[SerializeField]
    float _pulseGhlostTravelSpeed;
    //[SerializeField]
    int _amountPerRing;

    [Header("Varied Pulse Attack variables")]
    [SerializeField]
    int _variedHowManyPulses;
    int _variedCurrentPulse = 0;
    [SerializeField]
    float _variedPulseDelay;
    [SerializeField]
    float _variedGhlostTravelSpeed;
    [SerializeField]
    int _variedAmountPerRing;

    [Header("Line Attack Variables")]
    [SerializeField]
    float _lineCastTime;
    [SerializeField]
    int _degreeOfCastCone;
    [SerializeField]
    float _lineSpawnRate;
    [SerializeField]
    float _lineRotateSpeed;
    [SerializeField]
    float _lineGhlostTravelSpeed;
    int GhlostsCast = 0;
    Vector3 _startingAngle;

    [Header("Spiral Attack Variables")]
    [SerializeField]
    float _spiralCastTime;
    [SerializeField]
    float _spiralSpawnRate;
    [SerializeField]
    float _spiralRotateSpeed;
    [SerializeField]
    float _spiralGhlostTravelSpeed;

    [Header("Attack Percentages")]
    [SerializeField]
    float _pulseAttackPercentage;
    //[SerializeField]
    float _variedPulseAttackPercentage;
    [SerializeField]
    float _lineAttackPercentage;
    [SerializeField]
    float _spiralAttackPercentage;
    float _totalAttackPercentages;

    [Header("Special Pulse Percentages")]
    [SerializeField]
    float _specialPulsePercentage;
    [SerializeField]
    float _regularPulsePercentage;
    float _totalPulsePercentage;


    [Header("Special Ghlost Variables")]
    [SerializeField]
    bool _specialGhlosts;
    [SerializeField]
    float _coloredSpawnPercentage;
    [SerializeField]
    float _invincibleSpawnPercentage;
    [SerializeField]
    float _normalSpawnPercentage;
    float _totalSpawnPercentage;

    [Header("Colored Ghlost Variables")]
    [SerializeField]
    List<Color> _colorsForMinions;
    [SerializeField]
    float _maxColorTravelDistance;

    [Header("Invincible Ghlost Variables")]
    [SerializeField]
    protected Color _invincibleColor;
    [SerializeField]
    GameObject _immuneParticles;

    List<GameObject> _ghlostsInScene = new List<GameObject>();

    ATTACKSTATE _attackState = ATTACKSTATE.WAITFORATK;
    float startTime;
	
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
                PulseHardAttack();
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
        _totalAttackPercentages = _pulseAttackPercentage + _spiralAttackPercentage + _lineAttackPercentage;
        float _nextAttack = Random.Range(0, _totalAttackPercentages);
        Debug.Log("Next attack: " + _nextAttack);
        if (_nextAttack > 0 && _nextAttack <= _pulseAttackPercentage)
        {
            Debug.Log("Pulse Attack");
            _totalPulsePercentage = _specialPulsePercentage + _regularPulsePercentage;
            _nextAttack = Random.Range(0, _totalPulsePercentage);
            if (_nextAttack > 0 && _nextAttack <= _regularPulsePercentage)
            {
                _attackState = ATTACKSTATE.PULSEHARDATK;
                _currentPulse = 0;
                _variedCurrentPulse = 0;
            }
            else
            {
                gameObject.GetComponent<ShootingBoss>().SetSpecialPulseAttack = true;
                _attackState = ATTACKSTATE.PULSEHARDATK;
                _currentPulse = 0;
                _variedCurrentPulse = 0;
            }
        }
        else if (_nextAttack > _pulseAttackPercentage && _nextAttack <= (_pulseAttackPercentage + _lineAttackPercentage))
        {
            Debug.Log("Line Attack");
            _startingAngle = transform.eulerAngles;
            _attackState = ATTACKSTATE.LINEATK;
        }
        else
        {
            Debug.Log("Spiral Attack");
            _attackState = ATTACKSTATE.SPIRALATK;
        }
        startTime = Time.time;
    }

    private void spiralAttack()
    {
        GameObject ghlostObj;
        float timeTaken = Time.time - startTime;

        if (timeTaken < _spiralCastTime)
        {
            transform.eulerAngles += new Vector3(0, 1 * _spiralRotateSpeed * Time.deltaTime, 0);

            if (timeTaken >= _spiralSpawnRate * GhlostsCast)
            {
                //Debug.Log(timeTaken + " VS. " + _spiralSpawnRate * GhlostsCast);
                GhlostsCast += 1;
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _spiralGhlostTravelSpeed;
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
        
        if (timeTaken < _lineCastTime)
        {
            //Rotates back and forth through the cone angle / 2 * -1 and 1
            float angle = Mathf.Sin(Time.time * _lineRotateSpeed) * ((_degreeOfCastCone / 2));
            gameObject.transform.LookAt(GetComponent<ShootingBoss>().GetPlayerRef.gameObject.transform);
            transform.rotation = Quaternion.AngleAxis(angle + transform.eulerAngles.y, Vector3.up);

            //Spawns a new glhost based on the spawnrate
            if (timeTaken >= _lineSpawnRate * GhlostsCast)
            {
                //Debug.Log(timeTaken + " VS. " + _lineSpawnRate * GhlostsCast);
                GhlostsCast+= 1;
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _lineGhlostTravelSpeed;
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
        if (timeTaken > _pulseDelay * _currentPulse)
        {
            int spacingAngle = 360 / _amountPerRing;
            GameObject ghlostObj;
            for (int i = 0; i < _amountPerRing; i++)
            {
                transform.eulerAngles += new Vector3(0, spacingAngle, 0);
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _pulseGhlostTravelSpeed;
            }
            _currentPulse++;
        }
        if (_currentPulse == _howManyPulses)
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("stop pulsing");
        }
    }

    private void PulseHardAttack()
    {
        float timeTaken = Time.time - startTime;
        if (timeTaken > _variedPulseDelay * _variedCurrentPulse)
        {
            int spacingAngle = 360 / _variedAmountPerRing;
            GameObject ghlostObj;
            for (int i = 0; i < _variedAmountPerRing; i++)
            {
                transform.eulerAngles += new Vector3(0, spacingAngle + (spacingAngle / 2 * (_variedCurrentPulse % 2)), 0);
                //print(transform.rotation.ToString());
                ghlostObj = SpawnGhlost();
                //ghlostObj.GetComponent<BaseEnemy>().Init(gameObject.GetComponent<ShootingMiniBoss>().SetMyRoom, Mechanic.BOSS);
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _variedGhlostTravelSpeed;
            }
            _variedCurrentPulse++;
        }
        if (_variedCurrentPulse == _variedHowManyPulses)
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("stop pulsing");
        }
    }


    private GameObject SpawnGhlost()
    {
        if (_specialGhlosts)
        {
            _totalSpawnPercentage = _invincibleSpawnPercentage + _normalSpawnPercentage + _coloredSpawnPercentage;
            float _ghlostToSpawn = Random.Range(0, _totalSpawnPercentage);
            if (_ghlostToSpawn > 0 && _ghlostToSpawn <= _normalSpawnPercentage)
            {
                GameObject newNormalGhlost = InstantiateGhlost(_normalGhlostPrefab, Mechanic.NONE);
                _ghlostsInScene.Add(newNormalGhlost);
                return newNormalGhlost;
            }
            else if (_ghlostToSpawn > _normalSpawnPercentage && _ghlostToSpawn <= (_normalSpawnPercentage + _invincibleSpawnPercentage))
            {
                GameObject newInvincibleGhlost = InstantiateGhlost(_InvincibleGhlostPrefab, Mechanic.CHASE);
                //GameObject newImmunePS = Instantiate(_immuneParticles);
                //newImmunePS.transform.position = newInvincibleGhlost.transform.position;
                //for (int i = 0; i < 4; i++)
                //{
                    //newImmunePS.transform.GetChild(i).position = newInvincibleGhlost.transform.position;
                //}
                //newImmunePS.transform.SetParent(newInvincibleGhlost.transform);
                _ghlostsInScene.Add(newInvincibleGhlost);
                if (gameObject.GetComponent<ShootingBoss>().SetSpecialPulseAttack != true)
                {
                    newInvincibleGhlost.GetComponent<DumbBossGlhost>().setMyState = DumbBossGlhost.DUMBSTATE.DIE;
                }
                return newInvincibleGhlost;
            }
            else
            {
                GameObject newColorGlhost = InstantiateGhlost(_normalGhlostPrefab, Mechanic.COLOR);
                newColorGlhost.GetComponent<DumbBossGlhost>().SetMaxTravelTime = _maxColorTravelDistance;
                _ghlostsInScene.Add(newColorGlhost);
                return newColorGlhost;
            }
        }
        else
        {
            GameObject newNormalGhlost = InstantiateGhlost(_normalGhlostPrefab, Mechanic.NONE);
            _ghlostsInScene.Add(newNormalGhlost);
            return newNormalGhlost;
        }
    }

    private GameObject InstantiateGhlost(GameObject ghlostPrefab, Mechanic myMechanic)
    {
        GameObject newGhlost;
        newGhlost = Instantiate(ghlostPrefab, transform.position + (transform.forward * _SpawnedDistAway), transform.rotation);
        newGhlost.GetComponent<DumbBossGlhost>().SetShooterRef = gameObject.GetComponent<GhlostBossShooter>();
        newGhlost.GetComponent<DumbBossGlhost>().SetDamageToBoss = gameObject.GetComponent<ShootingBoss>().GetDamageToBoss;
        newGhlost.GetComponent<DumbBossGlhost>().SetDamageToPlayer = gameObject.GetComponent<ShootingBoss>().GetDamageToPlayer;
        newGhlost.GetComponent<DumbBossGlhost>().Init(gameObject.GetComponent<ShootingBoss>().SetMyRoom, myMechanic);

        if (myMechanic == Mechanic.CHASE)
        {
            newGhlost.GetComponent<DumbBossGlhost>().InitColor(_invincibleColor);
        }
        else if (myMechanic == Mechanic.COLOR)
        {
            int _rando = Random.Range(0, _colorsForMinions.Count);
            newGhlost.GetComponent<DumbBossGlhost>().InitColor(_colorsForMinions[_rando]);
        }
        newGhlost.transform.parent = null;
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
            _ghlostsInScene.Remove(ghlostRef);
        }
    }

    public List<GameObject> GetGhlostsInScene { get { return _ghlostsInScene; } }
    public List<Color> GetGhlostColors { get { return _colorsForMinions; } }
}
