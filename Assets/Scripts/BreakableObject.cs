using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour {

    [SerializeField]
    GameObject _brokenObjectPrefab;
    
    public void BreakObject()
    {
        Instantiate(_brokenObjectPrefab, transform.position, transform.rotation, null);
        gameObject.SetActive(false);
    }
}
