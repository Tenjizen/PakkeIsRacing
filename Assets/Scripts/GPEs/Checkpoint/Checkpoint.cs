using Character;
using Sound;
using UI;
using UnityEngine;
using FMODUnity;

namespace GPEs.Checkpoint
{
    [RequireComponent(typeof(StudioEventEmitter))]
    public class Checkpoint : PlayerTriggerManager
    {
        [SerializeField] private string _zoneName;
        
        [Header("References"), SerializeField] private ZoneManager _zoneManager;
        [SerializeField] private Transform _targetRespawnTransform;
        [SerializeField] private ParticleSystem _activationParticles;
        [SerializeField] private AudioClip _activationClip;

        private StudioEventEmitter emitter;

        private bool _hasBeenUsed;

        private void Start()
        {
            OnPlayerEntered.AddListener(SetCheckPoint);
            OnPlayerEntered.AddListener(SetPlayerExperience);
            
        }
        private void OnDestroy()
        {
            OnPlayerEntered.RemoveListener(SetCheckPoint);
            OnPlayerEntered.RemoveListener(SetPlayerExperience);
        }

        public void SetCheckPoint()
        {
            if (_hasBeenUsed == false)
            {
                CharacterManager.Instance.CheckpointManagerProperty.SetCheckpoint(this);
                _hasBeenUsed = true;
            
                _activationParticles.Play();
                //CharacterManager.Instance.SoundManagerProperty.PlaySound(_activationClip);
                _zoneManager.ShowZone(_zoneName);

                AudioManager.instance.PlayOneShot(FMODEvents.instance.checkpointActivated, this.transform.position);
            }
        }

        public Transform GetTargetRespawnTransform()
        {
            return _targetRespawnTransform;
        }

        private void SetPlayerExperience()
        {
            float value = CharacterManager.Instance.ExperienceManagerProperty.Data.ExperienceGainedAtCheckpoint;
            CharacterManager.Instance.ExperienceManagerProperty.AddExperience(value);
        }

#if UNITY_EDITOR

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (_targetRespawnTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(_targetRespawnTransform.position,Vector3.one*0.5f);
            }
        }

#endif
    }
}
