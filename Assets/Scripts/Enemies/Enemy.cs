﻿using Character;
using Fight;
using Fight.Data;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IHittable
    {
        public Transform Transform
        {
            get { return transform;}
            set {}
        }

        [field:SerializeField] public UnityEvent OnHit { get; set; }
        [field:SerializeField] public UnityEvent OnDie { get; set; }
        
        [field:SerializeField] public WeaponType WeaponThatCanHitEnemy { get; set; }
        [field:SerializeField] public Sprite EnemySprite { get; private set; }
        [field:SerializeField, ReadOnly] public float CurrentLife { get; set; }
        [field:SerializeField, ReadOnly] public float MaxLife { get; set; }
        [field:SerializeField, ReadOnly] public bool IsPossessed { get; set; }
        [field:SerializeField] public GameObject PossessedVisualGameObject { get; set; }

        public virtual void Hit(Projectile projectile, GameObject owner)
        {
            if (projectile.Data.Type != WeaponThatCanHitEnemy)
            {
                Debug.Log($"{projectile.Data.Type} can't hit {gameObject.name}");
                return;
            }
            
            OnHit.Invoke();
            CurrentLife -= 1;
            Debug.Log($"hit, life : {CurrentLife}");
            SetEnemyLifeUIGauge();
            if (CurrentLife <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            OnDie.Invoke();
            CharacterManager.Instance.EnemyUIManager.DisableEnemyUI();
            IsPossessed = false;

            if (PossessedVisualGameObject != null)
            {
                PossessedVisualGameObject.SetActive(false);
            }
        }

        public virtual void SetUpStartEnemyUI()
        {
            UIEnemyManager enemyUI = CharacterManager.Instance.EnemyUIManager;
            enemyUI.ActiveEnemyUI(EnemySprite);
            enemyUI.SetGauge(CurrentLife, MaxLife);
        }

        public virtual void SetEnemyLifeUIGauge()
        {
            CharacterManager.Instance.EnemyUIManager.SetGauge(CurrentLife, MaxLife);
        }
    }
}