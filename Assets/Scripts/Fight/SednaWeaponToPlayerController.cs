using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Fight
{
    public class SednaWeaponToPlayerController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _dieParticle;
        [field:SerializeField] public Transform PlayerTransform { get; private set; }

        private void Update()
        {
            if (Vector3.Distance(transform.position, PlayerTransform.position) < 5f)
            {
                transform.DOScale(Vector3.zero, 0.7f).OnComplete(Die);
            }
        }

        private void Die()
        {
            _dieParticle.Play();
            _dieParticle.transform.parent = null;
            Destroy(gameObject);
        }

        public void SetSednaPlayerTransform(Transform transformToSet)
        {
            PlayerTransform = transformToSet;
        }
    }
}