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

        protected Rigidbody RigidbodyProjectile;
        protected bool HasTouched;
        protected Transform Target;
        protected float CurrentTime;
        protected float TimeToReachTarget;
        protected float TimeToReachApexFromBase, TimeToReachTargetFromApex;
        protected float ApexHeight;
        protected Vector3 PositionToReach;
        
        private float _lifetime;

        private void Start()
        {
            _lifetime = Data.Lifetime;
        }

        protected virtual void FixedUpdate()
        {
            if (Target == null)
            {
                return;
            }

            CurrentTime += Time.deltaTime;
            Vector3 position = transform.position;
            
            //height
            float percentage = CurrentTime <= TimeToReachApexFromBase
                ? CurrentTime / TimeToReachApexFromBase
                : (CurrentTime - TimeToReachApexFromBase) / TimeToReachTargetFromApex;
            if (percentage >= 1)
            {
                PositionToReach = Target.position; 
                goto UpdateEnd;
            }
            
            percentage = Mathf.Clamp01(percentage);
            percentage = CurrentTime <= TimeToReachApexFromBase
                ? Data.ArcMovementParameters.BaseToApexCurve.Evaluate(percentage)
                : Data.ArcMovementParameters.ApexToTargetCurve.Evaluate(percentage);

            float height = Target.position.y + percentage * ApexHeight;
            PositionToReach = new Vector3(position.x, height, position.z);
            
            UpdateEnd:
            //velocity
            Vector3 direction = (Target.position - position).normalized;
            Vector3 desiredVelocity = direction * Data.ArcMovementParameters.ArcMovementSpeed;
            RigidbodyProjectile.velocity = desiredVelocity;
            
            transform.position = Vector3.Lerp(position, PositionToReach,0.05f);
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

        public virtual void Launch(Vector3 direction, float power)
        {
            SetRigidbody();
        }

        public virtual void Launch(Transform hittable)
        {
            SetRigidbody();
                
            Target = hittable;
            CurrentTime = 0;

            float distance = Vector3.Distance(transform.position, Target.position);
            ApexHeight = distance * Data.ArcMovementParameters.BaseApexHeightForDistance1;
            TimeToReachTarget = distance / Data.ArcMovementParameters.ArcMovementSpeed;
            TimeToReachApexFromBase = TimeToReachTarget * Data.ArcMovementParameters.PercentOfFlyTimeToReachApex;
            TimeToReachTargetFromApex = TimeToReachTarget - TimeToReachApexFromBase;
        }

        private void SetRigidbody()
        {
            RigidbodyProjectile = GetComponent<Rigidbody>();
            RigidbodyProjectile.isKinematic = false;
        }

        protected virtual void Die()
        {
            OnProjectileDie.Invoke();
            
            SednaWeaponToPlayerController sedna = Instantiate(_sednaWeaponToPlayerPrefab,transform.position,Quaternion.identity);
            Transform player = Owner.transform;
            sedna.SetSednaPlayerTransform(player);
            sedna.transform.DOMove(player.position, Data.Cooldown);

            Data.ForbiddenColliders.Clear();
        }
    }
}