using System;
using DG.Tweening;
using Fight.Data;
using GPEs.WaterFlowGPE;
using UnityEngine;
using UnityEngine.Events;
using WaterFlowGPE;

namespace Fight
{
    public abstract class Projectile : MonoBehaviour
    {
        [field:SerializeField] public GameObject Owner { get; private set; }
        [field:SerializeField] public WeaponData Data { get; private set; }
        
        [SerializeField] private SednaWeaponToPlayerController _sednaWeaponToPlayerPrefab;

        public UnityEvent OnProjectileDie = new UnityEvent();

        private float _lifetime;
        protected IHittable AutoAimHittable;
        protected bool HasTouched;

        private void Start()
        {
            _lifetime = Data.Lifetime;
        }

        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HasTouched)
            {
                return;
            }
            
            if (Data.ForbiddenColliders.Contains(other.gameObject) || 
                (other.gameObject.transform.parent != null && Data.ForbiddenColliders.Contains(other.gameObject.transform.parent.gameObject)) ||
                other.gameObject.GetComponent<WaterFlowBlock>() != null)
            {
                return;
            }

            HasTouched = true;           
            Die();
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

        protected virtual void HitHittable(Collider colliderHittable)
        {
            
        }

        protected virtual void HitNonHittable(Collider colliderNonHittable)
        {
           
        }

        protected virtual void EndLifetime()
        {
            Die();
        }
        
        public virtual void Launch(Vector3 direction, float power) { }

        protected virtual void Die()
        {
            OnProjectileDie.Invoke();
            
            SednaWeaponToPlayerController sedna = Instantiate(_sednaWeaponToPlayerPrefab,transform.position,Quaternion.identity);
            Transform player = Owner.transform;
            sedna.SetSednaPlayerTransform(player);
            sedna.transform.DOMove(player.position, Data.Cooldown);

            Data.ForbiddenColliders.Clear();
        }

        public void SetHittableAutoAim(IHittable hittable)
        {
            AutoAimHittable = hittable;
        }
    }
}