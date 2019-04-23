using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredBlock : MonoBehaviour {

    [SerializeField]
    protected Color _myColor;

    protected DungeonMechanic _spawner;
    protected Light _mySpotlight;

    protected virtual void Awake()
    {
        GetComponent<MeshRenderer>().material.color = _myColor;
        _mySpotlight = transform.GetChild(0).GetComponent<Light>();
        _mySpotlight.color = _myColor;
    }

    public virtual void CorrectMatch()
    {
        AudioManager.instance.GhostColorCorrect();
        gameObject.SetActive(false);
        _spawner.CheckForEnd();
    }

    public DungeonMechanic SetSpawner { set { _spawner = value; } }
    public Color GetColor { get { return _myColor; } }
}
