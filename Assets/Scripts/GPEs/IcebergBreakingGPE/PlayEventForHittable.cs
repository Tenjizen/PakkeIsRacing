using Character;
using Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayEventForHittable : MonoBehaviour, IHittable
{
    public Transform Transform { get; set; }

    [field: SerializeField] public UnityEvent OnHit { get; set; }

    private void Awake()
    {
        Transform = this.transform;
    }

    public virtual void Hit(GameObject owner, int damage)
    {
        OnHit.Invoke();
    }
}
