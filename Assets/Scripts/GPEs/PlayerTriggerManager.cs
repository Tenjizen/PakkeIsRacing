using System;
using Character;
using Kayak;
using Tools.HideIf;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GPEs
{
    public class PlayerTriggerManager : MonoBehaviour
    {
        [Serializable]
        private enum TriggerType
        {
            BoxTrigger = 0,
            SphereTrigger = 1
        }

        [Header("Trigger"), SerializeField] private TriggerType _triggerType;
        [SerializeField] private bool _showTriggerGizmos = true;
        [SerializeField, HideIf("_triggerType", TriggerType.SphereTrigger)] private Vector3 _triggerBoxSize = Vector3.one;
        [SerializeField, HideIf("_triggerType", TriggerType.BoxTrigger)] private float _triggerSphereSizeRadius = 1;
        [SerializeField] private Vector3 _triggerOffsetPosition = Vector3.zero;
        [SerializeField] private LayerMask _playerLayerMask;
        
        [Header("Event")] public UnityEvent OnPlayerEntered = new UnityEvent();
        public UnityEvent OnPlayerStay = new UnityEvent();
        public UnityEvent OnPlayerExited = new UnityEvent();

        public KayakController PropKayakController { get; private set; }
        
        private RaycastHit[] _hits = new RaycastHit[20];

        public virtual void Update()
        {
            CheckForPlayerInTrigger();
        }

        private void CheckForPlayerInTrigger()
        {
            bool isKayakInTrigger = false;
            Array.Clear(_hits,0,_hits.Length);

            switch (_triggerType)
            {
                case TriggerType.BoxTrigger:
                {
                    int numHits = Physics.BoxCastNonAlloc(transform.position + _triggerOffsetPosition, new Vector3(_triggerBoxSize.x / 2, _triggerBoxSize.y / 2, _triggerBoxSize.z / 2), Vector3.forward, _hits, transform.rotation, 0f);
                    break;
                }
                case TriggerType.SphereTrigger:
                {
                    int numHits = Physics.SphereCastNonAlloc(transform.position + _triggerOffsetPosition,_triggerSphereSizeRadius, Vector3.forward,_hits, 0f);
                    break;
                }
            }

            KayakController kayakManager = CharacterManager.Instance.KayakControllerProperty;
            for (int i = 0; i < _hits.Length; i++)
            {
                if (_hits[i].collider == null || _hits[i].collider.gameObject != kayakManager.gameObject)
                {
                    continue;
                }

                if (PropKayakController == null)
                {
                    PropKayakController = kayakManager;
                    OnPlayerEntered.Invoke();
                }
                
                OnPlayerStay.Invoke();
                isKayakInTrigger = true;
            }
            
            if (PropKayakController == null || isKayakInTrigger)
            {
                return;
            }
            
            PropKayakController = null;
            OnPlayerExited.Invoke();
        }

#if UNITY_EDITOR

        public virtual void OnDrawGizmos()
        {
            if (_showTriggerGizmos)
            {
                Gizmos.color = Color.green;
                if (_triggerType == TriggerType.BoxTrigger)
                {
                    Gizmos.DrawWireCube(transform.position + _triggerOffsetPosition, _triggerBoxSize);
                }
                if (_triggerType == TriggerType.SphereTrigger)
                {
                    Gizmos.DrawWireSphere(transform.position + _triggerOffsetPosition, _triggerSphereSizeRadius);
                }
            }
        }

#endif
    }
}
