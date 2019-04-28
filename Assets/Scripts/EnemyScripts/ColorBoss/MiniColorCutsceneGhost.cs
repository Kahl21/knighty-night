using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniColorCutsceneGhost : MonoBehaviour {

    [SerializeField]
    Color _myColor;
    GameObject _myBody;
    SkinnedMeshRenderer _myRenderer;
    Material _myMaterial;

    private void Awake()
    {
        _myBody = transform.GetChild(2).gameObject;
        _myRenderer = _myBody.GetComponent<SkinnedMeshRenderer>();
        _myMaterial = _myRenderer.materials[1];
        _myMaterial.color = _myColor;
    }

    public Color GetColor { get { return _myColor; } }
}
