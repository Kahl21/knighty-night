using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardFloor : MonoBehaviour {


    [SerializeField]
    float _hazardDamage;

    public float GetHazardDamage { get { return _hazardDamage; } }
}
