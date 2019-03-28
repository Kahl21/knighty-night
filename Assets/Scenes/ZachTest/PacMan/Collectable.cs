using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    PM_Manager _pmGameManager;

    [SerializeField]
    bool isSword = false;
	// Use this for initialization
	void Start ()
    {
        _pmGameManager = FindObjectOfType<PM_Manager>();
	}

    public void AddToScore()
    {
        _pmGameManager.AddToScore(this.gameObject);
        Destroy(this.gameObject);
    }

    public bool IsSword { get { return isSword; } }
}
