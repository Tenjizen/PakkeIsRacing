using UnityEngine;
using Tools.SingletonClassBase;

namespace GPEs.Checkpoint
{
    public class CheckpointManager : Singleton<CheckpointManager>
    {

        public Checkpoint CurrentCheckpoint;

        public Transform GetRespawnPoint()
        {
            if (CurrentCheckpoint == null)
            {
                return transform;
            }

            return CurrentCheckpoint.GetTargetRespawnTransform();
        }
    }
}
