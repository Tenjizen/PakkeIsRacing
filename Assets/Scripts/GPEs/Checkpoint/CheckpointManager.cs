using System;
using Character;
using UnityEngine;

namespace GPEs.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        [field: SerializeField] public Checkpoint CurrentCheckpoint { get; private set; }

        private Transform _baseTransform;
        public bool SednaIsMoving = false;
        float TimerSednaIsMoving;
        private void Start()
        {
            _baseTransform = CharacterManager.Instance.transform;
        }

        private void Update()
        {
            if (SednaIsMoving == true)
                PlayerRespawn();
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
        
        public void PlayerRespawn()
        {
            if (CurrentCheckpoint.NextCheckPoint == null)
            {
                return;
            }
            CharacterManager.Instance.SednaGameObject.SetActive(true);
            TimerSednaIsMoving += Time.deltaTime;
            if (TimerSednaIsMoving <= 3)
            {
                CharacterManager.Instance.SednaGameObject.transform.LookAt(CurrentCheckpoint.NextCheckPoint.transform);
                CharacterManager.Instance.SednaGameObject.transform.Translate(Vector3.forward * (5 * Time.deltaTime), Space.Self);
                //move sedna
            }
            else
            {
                SednaIsMoving = false;
                CharacterManager.Instance.SednaGameObject.SetActive(false);
            }

        }

    }
}
