using System;
using Character;
using UnityEngine;

public class TestSednaFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField, Range(0,0.2f)] private float _movementLerp;

    private Rigidbody _kayak;

    private void Start()
    {
        _kayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody;
    }

    private void FixedUpdate()
    {
        Vector3 direction = _target.position - transform.position;
        direction.Normalize();

        if (_kayak.velocity.magnitude <= 1)
        {
            return;
        }
        
        transform.position = Vector3.Lerp(transform.position, _target.position, _movementLerp);
        
        transform.rotation = Quaternion.Euler(new Vector3(0,_kayak.transform.rotation.eulerAngles.y,0));
    }
}
