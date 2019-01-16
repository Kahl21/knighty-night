using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredBlock : MonoBehaviour {

    [SerializeField]
    Color _myColor;

    DungeonMechanic _spawner;
    Light _mySpotlight;

    private void Awake()
    {
        GetComponent<MeshRenderer>().material.color = _myColor;
        _mySpotlight = transform.GetChild(0).GetComponent<Light>();
        _mySpotlight.color = _myColor;
    }

    public void CorrectMatch()
    {
        gameObject.SetActive(false);
        _spawner.CheckForEnd();
    }

    public DungeonMechanic SetSpawner { set { _spawner = value; } }
    public Color GetColor { get { return _myColor; } }
}
