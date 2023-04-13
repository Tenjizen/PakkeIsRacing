using System;
using Character;
using UnityEngine;

namespace GPEs.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        [field:SerializeField] public Checkpoint CurrentCheckpoint { get; private set; }

        private Transform _baseTransform;

        private void Start()
        {
            _baseTransform = CharacterManager.Instance.transform;
        }

        public Transform GetRespawnPoint()
        {
            if (CurrentCheckpoint == null)
            {
                return _baseTransform;
            }

            return CurrentCheckpoint.GetTargetRespawnTransform();
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            CurrentCheckpoint = checkpoint;
        }
    }
}
