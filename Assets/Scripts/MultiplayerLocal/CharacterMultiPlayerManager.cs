using Character;
using Kayak;
using System;
using UnityEngine;

public class CharacterMultiPlayerManager : MonoBehaviour
{
    public CharacterManager CharacterManager;
    public InputManagement InputManager;
    public KayakController Kayak;
    public ColorPlayer ColorPlayer;

    [SerializeField] ParticleSystem _particleSystem;

    public bool InSharkZone = false;

    private int _points;
    public int Points => _points;


    private float _timerInTrigger = 0;
    public float TimerInTrigger => _timerInTrigger;


    public bool MaxPts = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Kayak.transform.position = Vector3.one * 100;


        if (InSharkZone == true && GameManager.Instance.EnnemyPossessed == true)
        {
            if (MaxPts == true) return;

            _timerInTrigger += Time.deltaTime;

            if (_timerInTrigger >= GameManager.Instance.TimerInTriggerShark)
            {
                Point();
                _timerInTrigger -= GameManager.Instance.TimerInTriggerShark;
            }
        }
        else
        {
            _timerInTrigger = 0;
        }
    }

    [SerializeField] int _addValueParticleRateOverTime = 10;
    int _addRateOverTimeParticle = 10;
    public void SetPosKayak(Vector3 target)
    {
        Kayak.transform.position = target;
    }
    private void Point()
    {
        if (_points < GameManager.Instance.MaxPointToUnlockButton)
        {
            _points += GameManager.Instance.PointsWin;

            var emission = _particleSystem.emission;
            _addRateOverTimeParticle += _addValueParticleRateOverTime;
            emission.rateOverTime = _addRateOverTimeParticle;
        }
        else if (MaxPts == false)
        {
            MaxPts = true;
            var colorParticle = _particleSystem.main;
            colorParticle.startColor = Color.yellow;
        }
    }

    public void RemovePoint(int pts)
    {
        _points -= pts;

        var emission = _particleSystem.emission;
        _addRateOverTimeParticle -= _addValueParticleRateOverTime * pts;
        emission.rateOverTime = _addRateOverTimeParticle;

        if (MaxPts == true)
        {
            MaxPts = false;
            var colorParticle = _particleSystem.main;
            colorParticle.startColor = Color.white;
        }
    }
}