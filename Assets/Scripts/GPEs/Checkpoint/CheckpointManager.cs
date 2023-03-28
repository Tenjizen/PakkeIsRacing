using Character;
using UnityEngine;
using Tools.SingletonClassBase;

namespace GPEs.Checkpoint
{
    public class CheckpointManager
    {
        [field:SerializeField] public Checkpoint CurrentCheckpoint { get; private set; }

        public Transform GetRespawnPoint()
        {
            if (CurrentCheckpoint == null)
            {
                Debug.LogError("No checkPoint Set");
                return CharacterManager.Instance.transform;
            }

            return CurrentCheckpoint.GetTargetRespawnTransform();
        }

        public void SetCheckpoint(Checkpoint checkpoint)
        {
            CurrentCheckpoint = checkpoint;
        }
    }
}
