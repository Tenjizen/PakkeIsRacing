using UnityEngine;
using WaterFlowGPE.Bezier;
using SharkWithPath.Data;
using System;
using System.Collections.Generic;

public class SharkWithPathController : MonoBehaviour
{
    [Header("Data"), SerializeField] private SharkWithPathData _data;

    private int _indexTarget;
    public bool StartRunning = false;

    public GameObject Shpere;
    public ParticleSystem ParticleSystemPurify;
    [SerializeField] Transform _targetRoot;
    private List<Transform> _targets = new List<Transform>();

    [Header("Debug"), SerializeField] private bool _slow;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (StartRunning == false) return;
        ManageMovement();
    }
    private void FixedUpdate()
    {
        if (StartRunning == false) return;
        DistanceManager();
    }

    private void DistanceManager()
    {
        float bestDist = float.MaxValue;
        foreach (var item in GameManager.Instance.MultiTargetRef.Targets)
        {
            if (item != GameManager.Instance.MultiTargetRef.Targets[0])
            {
                float distTarget = Vector3.Distance(item.transform.position, this.transform.position);
                if (distTarget < bestDist)
                {
                    bestDist = distTarget;

                }
            }
        }
        if (bestDist > _data.MaxDistBetweenSharkAndClosestPlayer)
        {
            _slow = true;
        }
        else
        {
            _slow = false;
        }
    }

    public void Initialize()
    {

        _indexTarget = 1;


        for (int i = 0; i < _targetRoot.childCount; i++)
        {
            _targets.Add(_targetRoot.GetChild(i));
        }

        if (_targets.Count <= 0) return;

        Vector3 position = _targets[0].position;
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }


    private void ManageMovement()
    {
        Transform t = transform;

        var target = _targets[_indexTarget].position;
        var distTarget = Vector3.Distance(t.position, target);

        if (distTarget <= 0.5f) { _indexTarget = (_indexTarget + 1) % _targets.Count; }
        var speed = _data.MovingValue;

        if (_slow == true)
            speed += _data.SlowMovingValue * Time.deltaTime;

        var position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
        t.position = position;


        //rotation
        var dir = _targets[_indexTarget].position - t.position;

        var rota = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(dir), _data.RotationSpeedValue * Time.deltaTime);
        rota.x = 0;
        rota.z = 0;
        t.rotation = rota;

    }

   



}
