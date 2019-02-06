using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingGrace : MonoBehaviour {

    [SerializeField]
    float _healingAmount;
    [SerializeField]
    float _fadeDuration;
    float _usedHealingAmount;
    float _currHealingAmount;

    float _currHealFade;
    float _startHealTime;
    float h0, h1;
    bool _fading;

    ParticleSystem _graceLight;
    [SerializeField]
    ParticleSystem.Particle[] _particles;

    Color _baseColor;
    Color _currentColor;

    //Check If Checkpoint
    [SerializeField]
    public bool isCheckpoint;

    private void Awake()
    {
        _graceLight = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_graceLight.particleCount];
        _baseColor = _particles[0].startColor;
        _currentColor = _baseColor;
        _usedHealingAmount = _healingAmount;
        _currHealingAmount = _healingAmount;
    }

    private void Update()
    {
        if(_fading)
        {
            FadeParticle();
        }
    }

    public void StartFade()
    {
        Debug.Log("fading");
        h0 = _usedHealingAmount;
        h1 = _currHealingAmount;

        _usedHealingAmount = _currHealingAmount;
        _startHealTime = Time.time;
        _fading = true;
    }

    private void FadeParticle()
    {
        _currHealFade = (Time.time - _startHealTime) / _fadeDuration;

        if(_currHealFade > 1)
        {
            _currHealFade = 1;

            _fading = false;
            if(_currHealingAmount <= 0)
            {
                gameObject.SetActive(false);
            }
        }

        float h01;

        h01 = (1 - _currHealFade) * h0 + _currHealFade * h1;

        _currentColor.a = h01;

        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].startColor = _currentColor;
        }

        //_graceLight.Play();
    }

    public void HealReset()
    {
        _usedHealingAmount = _healingAmount;
        _currHealingAmount = _healingAmount;
        _currentColor = _baseColor;
    }

    public float GetHealingAmount { get { return _currHealingAmount; } set { _currHealingAmount = value; } }
}
