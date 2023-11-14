using System;
using Character;
using Fight;
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
        [field:SerializeField] public UnityEvent OnNotUsableWeaponHitTaken { get; set; }
        [field:SerializeField] public Sprite EnemySprite { get; private set; }
        [field:SerializeField, ReadOnly] public float CurrentLife { get; set; }
        [field:SerializeField, ReadOnly] public float MaxLife { get; set; }
        [field:SerializeField, ReadOnly] public bool IsPossessed { get; set; }
        [field:SerializeField] public GameObject PossessedVisualGameObject { get; set; }

        protected void HandlePlayerDistanceToSetUI(Transform player, float distance)
        {
            Debug.Log("comm");

            //if (Vector3.Distance(transform.position, player.position) > distance || CurrentLife <= 0)
            //{
            //    CharacterManager.Instance.EnemyUIManager.DisableEnemyUI();
            //}
            //else
            //{
            //    CharacterManager.Instance.EnemyUIManager.ActiveEnemyUI(EnemySprite);
            //    CharacterManager.Instance.EnemyUIManager.SetScreenPositionFromEnemyPosition(transform.position);
            //}
        }

        public virtual void Hit(GameObject owner, int damage)
        {
            OnHit.Invoke();
            CurrentLife -= damage;
            SetEnemyLifeUIGauge();
            
            if (CurrentLife <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            OnDie.Invoke();

            Debug.Log("comm");
            //CharacterManager.Instance.EnemyUIManager.DisableEnemyUI();
            IsPossessed = false;

            if (PossessedVisualGameObject != null)
            {
                PossessedVisualGameObject.SetActive(false);
            }
        }

        public virtual void SetUpStartEnemyUI()
        {
            Debug.Log("comm");
            //UIEnemyManager enemyUI = CharacterManager.Instance.EnemyUIManager;
            //enemyUI.ActiveEnemyUI(EnemySprite);
            //enemyUI.SetGauge(CurrentLife, MaxLife);
        }

        protected virtual void SetEnemyLifeUIGauge()
        {
            Debug.Log("comm");

            //CharacterManager.Instance.EnemyUIManager.SetGauge(CurrentLife, MaxLife);
        }

        protected virtual void SetPlayerExperience(float value)
        {
            Debug.Log("comm");
            //CharacterManager.Instance.ExperienceManagerProperty.AddExperience(value);
        }
    }
}