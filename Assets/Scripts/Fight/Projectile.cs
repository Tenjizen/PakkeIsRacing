using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using WaterFlowGPE;

namespace Fight
{
    public abstract class Projectile : MonoBehaviour
    {
        public GameObject Owner;
        public WeaponData Data;
        [SerializeField] private SednaWeaponToPlayerController _sednaWeaponToPlayerPrefab;

        public UnityEvent OnProjectileDie = new UnityEvent();

        private float _lifetime;

        private void Start()
        {
            _lifetime = Data.Lifetime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Data.ForbiddenColliders.Contains(other.gameObject) ||
                Data.ForbiddenColliders.Contains(other.gameObject.transform.parent.gameObject) ||
                other.gameObject.GetComponent<WaterFlowBlock>() != null)
            {
                return;
            }
            
            IHittable hittable = other.gameObject.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.Hit(this, Owner);
                HitHittable(other);
                return;
            }
            HitNonHittable(other);
        }

        protected virtual void Update()
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0)
            {
                EndLifetime();
            }
        }

        protected virtual void HitHittable(Collider collider)
        {
            Die();
        }

        protected virtual void HitNonHittable(Collider collider)
        {
            Die();
        }

        protected virtual void EndLifetime()
        {
            Die();
        }
        public virtual void Launch(Vector3 direction) { }

        protected virtual void Die()
        {
            OnProjectileDie.Invoke();
            
            SednaWeaponToPlayerController sedna = Instantiate(_sednaWeaponToPlayerPrefab,transform.position,Quaternion.identity);
            Transform player = Owner.transform;
            sedna.PlayerTransform = player;
            sedna.transform.DOMove(player.position, Data.Cooldown);

            Data.ForbiddenColliders.Clear();
        }
    }
}