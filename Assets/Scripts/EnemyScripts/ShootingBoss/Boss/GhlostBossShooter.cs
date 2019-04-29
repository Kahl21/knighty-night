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
    Animator _ghostAnimations;

    [Header("Varied Pulse Attack variables")]
    [SerializeField]
    int _variedHowManyPulses;
    int _realVariedHowManyPulses;
    int _variedCurrentPulse = 0;
    [SerializeField]
    float _variedPulseDelay;
    float _realVariedPulseDelay;
    [SerializeField]
    float _variedGhlostTravelSpeed;
    float _realVariedGhlostTravelSpeed;
    [SerializeField]
    int _variedAmountPerRing;
    int _realVariedAmountPerRing;

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

    float _totalAttackPercentages;

    [Header("Special Pulse Percentages")]
    [SerializeField]
    float _specialPulseAttackPercentage;
    float _realSpecialPulseAttackPercentage;
    [SerializeField]
    float _regularPulseAttackPercentage;
    float _realRegularPulseAttackPercentage;
    float _totalPulsePercentage;


    [Header("Special Ghlost Variables")]
    [SerializeField]
    bool _specialGhlosts;
    [SerializeField]
    float _coloredSpawnPercentage;
    float _realColoredSpawnPercentage;
    [SerializeField]
    float _invincibleSpawnPercentage;
    float _realInvincibleSpawnPercentage;
    [SerializeField]
    float _normalSpawnPercentage;
    float _realNormalSpawnPercentage;
    float _totalSpawnPercentage;

    [Header("Colored Ghlost Variables")]
    [SerializeField]
    List<Color> _colorsForMinions;

    [Header("Invincible Ghlost Variables")]
    [SerializeField]
    protected Color _invincibleColor;
    [SerializeField]
    GameObject _immuneParticles;

    [Header("Hard Pulse Attack variables")]
    [SerializeField]
    int _hardVariedHowManyPulses;
    [SerializeField]
    float _hardVariedPulseDelay;
    [SerializeField]
    float _hardVariedPulseGhlostTravelSpeed;
    [SerializeField]
    int _hardVariedAmountPerRing;

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

    [Header("Hard Special Attack Percentages")]
    [SerializeField]
    float _hardSpecialPulseAttackPercentage;
    [SerializeField]
    float _hardRegularPulseAttackPercentage;


    [Header("Hard Special Ghlost Variables")]
    [SerializeField]
    float _hardColoredSpawnPercentage;
    [SerializeField]
    float _hardInvincibleSpawnPercentage;
    [SerializeField]
    float _hardNormalSpawnPercentage;

    List<GameObject> _ghlostsInScene = new List<GameObject>();

    ATTACKSTATE _attackState = ATTACKSTATE.WAITFORATK;
    float startTime;

    public void Init()
    {
        if (!GameManager.Instance.HardModeOn)
        {
            //Add hard Variebales

            _realLineAttackPercentage = _lineAttackPercentage;
            _realLineCastTime = _lineCastTime;
            _realLineGhlostTravelSpeed = _lineGhlostTravelSpeed;
            _realLineRotateSpeed = _lineRotateSpeed;
            _realLineSpawnRate = _lineSpawnRate;
            _realDegreeOfCastCone = _degreeOfCastCone;

            _realPulseAttackPercentage = _pulseAttackPercentage;
            _realVariedPulseDelay = _variedPulseDelay;
            _realVariedGhlostTravelSpeed = _variedGhlostTravelSpeed;
            _realVariedAmountPerRing = _variedAmountPerRing;
            _realVariedHowManyPulses = _variedHowManyPulses;

            _realSpiralAttackPercentage = _spiralAttackPercentage;
            _realSpiralCastTime = _spiralCastTime;
            _realSpiralGhlostTravelSpeed = _spiralGhlostTravelSpeed;
            _realSpiralRotateSpeed = _spiralRotateSpeed;
            _realSpiralSpawnRate = _spiralSpawnRate;

            _realSpecialPulseAttackPercentage = _specialPulseAttackPercentage;
            _realRegularPulseAttackPercentage = _regularPulseAttackPercentage;

            _realColoredSpawnPercentage = _coloredSpawnPercentage;
            _realInvincibleSpawnPercentage = _invincibleSpawnPercentage;
            _realNormalSpawnPercentage = _normalSpawnPercentage;
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
            _realVariedPulseDelay = _hardVariedPulseDelay;
            _realVariedGhlostTravelSpeed = _hardVariedPulseGhlostTravelSpeed;
            _realVariedAmountPerRing = _hardVariedAmountPerRing;
            _realVariedHowManyPulses = _hardVariedHowManyPulses;

            _realSpiralAttackPercentage = _hardSpiralAttackPercentage;
            _realSpiralCastTime = _hardSpiralCastTime;
            _realSpiralGhlostTravelSpeed = _hardSpiralGhlostTravelSpeed;
            _realSpiralRotateSpeed = _hardSpiralRotateSpeed;
            _realSpiralSpawnRate = _hardSpiralSpawnRate;

            _realSpecialPulseAttackPercentage = _hardSpecialPulseAttackPercentage;
            _realRegularPulseAttackPercentage = _hardRegularPulseAttackPercentage;

            _realColoredSpawnPercentage = _hardColoredSpawnPercentage;
            _realInvincibleSpawnPercentage = _hardInvincibleSpawnPercentage;
            _realNormalSpawnPercentage = _hardNormalSpawnPercentage;

            Debug.Log("Hard Variables Set");
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
        _totalAttackPercentages = _realPulseAttackPercentage + _realSpiralAttackPercentage + _realLineAttackPercentage;
        float _nextAttack = Random.Range(0, _totalAttackPercentages);
        Debug.Log("Next attack: " + _nextAttack);
        if (_nextAttack > 0 && _nextAttack <= _realPulseAttackPercentage)
        {
            Debug.Log("Pulse Attack");
            _totalPulsePercentage = _realSpecialPulseAttackPercentage + _realRegularPulseAttackPercentage;
            _nextAttack = Random.Range(0, _totalPulsePercentage);
            if (_nextAttack > 0 && _nextAttack <= _realRegularPulseAttackPercentage)
            {
                _ghostAnimations.Play("ShootStart", 0);
                _attackState = ATTACKSTATE.PULSEHARDATK;
                _variedCurrentPulse = 0;
                _variedCurrentPulse = 0;
            }
            else
            {
                _ghostAnimations.Play("ShootStart", 0);
                gameObject.GetComponent<ShootingBoss>().SetSpecialPulseAttack = true;
                _attackState = ATTACKSTATE.PULSEHARDATK;
                _variedCurrentPulse = 0;
                _variedCurrentPulse = 0;
            }
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
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _realSpiralGhlostTravelSpeed;
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
            gameObject.transform.LookAt(GetComponent<ShootingBoss>().GetPlayerRef.gameObject.transform);
            transform.rotation = Quaternion.AngleAxis(angle + transform.eulerAngles.y, Vector3.up);

            //Spawns a new glhost based on the spawnrate
            if (timeTaken >= _realLineSpawnRate * GhlostsCast)
            {
                //Debug.Log(timeTaken + " VS. " + _lineSpawnRate * GhlostsCast);
                GhlostsCast+= 1;
                ghlostObj = SpawnGhlost();
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _realLineGhlostTravelSpeed;
            }
        }
        else
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("Stop Casting");
        }
    }

    private void PulseHardAttack()
    {
        float timeTaken = Time.time - startTime;
        if (timeTaken > _realVariedPulseDelay * _variedCurrentPulse)
        {
            int spacingAngle = 360 / _realVariedAmountPerRing;
            GameObject ghlostObj;
            for (int i = 0; i < _realVariedAmountPerRing; i++)
            {
                transform.eulerAngles += new Vector3(0, spacingAngle + (spacingAngle / 2 * (_variedCurrentPulse % 2)), 0);
                //print(transform.rotation.ToString());
                ghlostObj = SpawnGhlost();
                //ghlostObj.GetComponent<BaseEnemy>().Init(gameObject.GetComponent<ShootingMiniBoss>().SetMyRoom, Mechanic.BOSS);
                ghlostObj.GetComponent<DumbBossGlhost>().GetSpeed = _realVariedGhlostTravelSpeed;
            }
            _variedCurrentPulse++;
        }
        if (_variedCurrentPulse == _realVariedHowManyPulses)
        {
            _attackState = ATTACKSTATE.WAITFORATK;
            Debug.Log("stop pulsing");
        }
    }


    private GameObject SpawnGhlost()
    {
        if (_specialGhlosts)
        {
            _totalSpawnPercentage = _realInvincibleSpawnPercentage + _realNormalSpawnPercentage + _realColoredSpawnPercentage;
            float _ghlostToSpawn = Random.Range(0, _totalSpawnPercentage);
            if (_ghlostToSpawn > 0 && _ghlostToSpawn <= _realNormalSpawnPercentage)
            {
                GameObject newNormalGhlost = InstantiateGhlost(_normalGhlostPrefab, Mechanic.NONE);
                _ghlostsInScene.Add(newNormalGhlost);
                return newNormalGhlost;
            }
            else if (_ghlostToSpawn > _realNormalSpawnPercentage && _ghlostToSpawn <= (_realNormalSpawnPercentage + _realInvincibleSpawnPercentage))
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
            //_ghlostsInScene.Remove(ghlostRef);
            Destroy(ghlostRef);
        }
        _ghlostsInScene = new List<GameObject>();
        GhlostsCast = 0;
    }

    public List<GameObject> GetGhlostsInScene { get { return _ghlostsInScene; } }
    public List<Color> GetGhlostColors { get { return _colorsForMinions; } }
    public Animator GetGhostAnimator { get { return _ghostAnimations; } set { _ghostAnimations = value; } }
}
