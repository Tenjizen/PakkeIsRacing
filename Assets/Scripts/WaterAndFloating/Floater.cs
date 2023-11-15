﻿using System;
using Character;
using Kayak;
using UnityEngine;

namespace WaterAndFloating
{
    public class Floater : MonoBehaviour
    {
        [SerializeField] private Waves _waves;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _depthBeforeSubmerged = 1f;
        [SerializeField] private float _displacementAmount = 3f;
        [SerializeField] private float _displacementDownAmount = 3f;
        [SerializeField] private int _floaterCount = 1;
        [SerializeField] private float _waterDrag = 0.99f;
        [SerializeField] private float _waterAngularDrag = 0.5f;

        [Header("Physic Render Update"), SerializeField] private float _renderDistance;
        [ReadOnly, SerializeField] private bool _isSimulated;

        private Transform _playerTransform;

        private void Start()
        {
            if (GetComponentInParent<KayakController>() != null) _playerTransform = GetComponentInParent<KayakController>().Character.transform;
            _waves = GameManager.Instance.WavesRef;
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _playerTransform.position) > _renderDistance)
            {
                _isSimulated = false;
                return;
            }

            _isSimulated = true;
            ManageFloater();
        }

        private void ManageFloater()
        {
            Vector3 position = transform.position;
            _rigidbody.AddForceAtPosition(Physics.gravity / _floaterCount, position, ForceMode.Acceleration);

            float waveHeight = _waves.GetHeight(position);
            if (transform.position.y < waveHeight)
            {
                //get displacement multiplier (how far is the floater from water surface)
                float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / _depthBeforeSubmerged) * _displacementAmount;
                //force at position
                _rigidbody.AddForceAtPosition(
                     new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0),
                     position,
                     ForceMode.Acceleration);
                //velocity * waterDrag
                _rigidbody.AddForce(
                     -_rigidbody.velocity * (displacementMultiplier * _waterDrag * Time.fixedDeltaTime),
                     ForceMode.VelocityChange);
                //angularVelocity * waterAngularDrag
                _rigidbody.AddTorque(
                     -_rigidbody.angularVelocity * (displacementMultiplier * _waterAngularDrag * Time.fixedDeltaTime),
                     ForceMode.VelocityChange);
            }
            else
            {
                float displacementMultiplier = Mathf.Clamp(Mathf.Abs(waveHeight - transform.position.y) * _displacementDownAmount, 0, 5);
                _rigidbody.AddForce(
                     new Vector3(0, -displacementMultiplier * _waterDrag * Time.fixedDeltaTime * 5, 0),
                     ForceMode.VelocityChange);
            }
        }
    }
}