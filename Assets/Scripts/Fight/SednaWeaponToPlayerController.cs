using System;
using DG.Tweening;
using UnityEngine;

namespace Fight
{
    public class SednaWeaponToPlayerController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _dieParticle;
        public Transform PlayerTransform;

        private float _slerpValue = 0.02f;
        
        private void Update()
        {
            transform.position = Vector3.Slerp(transform.position, PlayerTransform.position, _slerpValue);
            _slerpValue += 0.0002f;

            if (Vector3.Distance(transform.position, PlayerTransform.position) <= 5f)
            {
                transform.DOScale(Vector3.zero, 1f).OnComplete(Die);
            }
        }

        private void Die()
        {
            _dieParticle.Play();
            _dieParticle.transform.parent = null;
            Destroy(gameObject);
        }
    }
}