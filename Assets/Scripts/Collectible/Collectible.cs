using System;
using Character;
using Collectible.Data;
using DG.Tweening;
using Fight;
using Json;
using UnityEngine;
using UnityEngine.Events;

namespace Collectible
{
    public class Collectible : MonoBehaviour, IHittable
    {
        public UnityEvent OnCollected = new UnityEvent();
        public CollectibleData Data;

        [SerializeField, Header("Parameters")] private float _levitationHeight;
        [SerializeField] private float _levitationSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Ease _easeType;

        public Transform Transform
        {
            get { return transform; }
            set { }
        }

        [field:SerializeField] public UnityEvent OnHit { get; set; }

        private void Start()
        {
            StartLevitateUp();
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, _rotationSpeed);
        }

        public void Hit(Projectile projectile, GameObject owner)
        {
            OnHit.Invoke();
            SetCollected();
        }

        private void SetCollected()
        {
            gameObject.SetActive(false);
            OnCollected.Invoke();
            CharacterManager.Instance.ExperienceManagerProperty.AddExperience(CharacterManager.Instance.ExperienceManagerProperty.Data.ExperienceGainedAtColletible);
            
            CollectibleJsonFileManager instance = JsonFilesManagerSingleton.Instance.CollectibleJsonFileManagerProperty;
            if (instance != null)
            {
                instance.SetCollectibleCollected(this);
            }
        }

        public void SetCollectedAtStart()
        {
            gameObject.SetActive(false);
        }

        #region Levitation

        private void StartLevitateUp()
        {
            Vector3 position = transform.position;
            transform.DOMoveY(position.y + _levitationHeight, _levitationSpeed).SetEase(_easeType).OnComplete(StartLevitateDown);
        }

        private void StartLevitateDown()
        {
            Vector3 position = transform.position;
            transform.DOMoveY(position.y - _levitationHeight, _levitationSpeed).SetEase(_easeType).OnComplete(StartLevitateUp);
        }

        #endregion
    }
}
