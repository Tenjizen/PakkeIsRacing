using System.Collections.Generic;
using Character;
using Kayak;
using UnityEngine;
using WaterFlowGPE;
using Random = System.Random;

namespace GPEs.WaterFlowGPE
{
    public class WaterFlowBlock : MonoBehaviour
    {
        //TODO to scriptable object
        [Header("Parameters"), SerializeField]
        private float _speed;

        [SerializeField, Range(0, 1), Tooltip("Max balance value added to kayak while in water flow")]
        private float _balanceValue;

        [SerializeField, Tooltip("The range in-between which the balance value will be multiplied randomly")]
        private Vector2 _balanceValueRandomMultiplierRange;

        [SerializeField, Range(0, 1), Tooltip("Multiplier applied to the speed when the boat isn't facing the direction")]
        private float _speedNotFacingMultiplier = 0.5f;

        [SerializeField, Range(0, 0.1f), Tooltip("The lerp applied to the boat rotation to match the flow direction when the boat is already facing the flow direction")]
        private float _rotationLerpWhenInDirection = 0.05f;

        [SerializeField, Range(0, 0.05f), Tooltip("The lerp applied to the boat rotation to match the flow direction when the boat is not facing the flow direction")]
        private float _rotationLerpWhenNotInDirection = 0.005f;

        [SerializeField, Range(0, 0.05f), Tooltip("The lerp applied to the boat rotation to match the flow direction when the player is trying to move away")]
        private float _rotationLerpWhenMoving = 0.005f;

        [Header("Particles"), SerializeField]
        private List<ParticleSystem> _particlesList;
        [SerializeField] private Transform _particlesTransformsRoot;
        private List<Transform> _particlesTransformsList = new List<Transform>();
        private List<ParticleSystem> _particlesTempList = new List<ParticleSystem>(); //private list for take the particules you instantiate

        [SerializeField, Tooltip("One of the particles will play at a random time between those two values")]
        private Vector2 _randomPlayOfParticleTime;


        [Header("Infos")]
        [ReadOnly] public Vector3 Direction;
        [ReadOnly] public bool IsActive = true;
        [ReadOnly, SerializeField] private float _playParticleTime;
        [ReadOnly, SerializeField] private float _playSecondParticleTime;

        private void Start()
        {
            _playParticleTime = UnityEngine.Random.Range(_randomPlayOfParticleTime.x, _randomPlayOfParticleTime.y);
            _playSecondParticleTime = _playParticleTime / 2;

            for (int i = 0; i < _particlesTransformsRoot.childCount; i++)
            {
                _particlesTransformsList.Add(_particlesTransformsRoot.GetChild(i).transform);
            }

            for (int i = 0; i < _particlesTransformsList.Count; i++)
            {
                _particlesTempList.Add(Instantiate(_particlesList[i % _particlesList.Count], _particlesTransformsList[i]));
            }
        }

        private void Update()
        {
            ManageParticles();
        }

        private void OnTriggerStay(Collider other)
        {
            Debug.Log(other + " ontriggerstay");
            var character = other.gameObject.transform.GetComponentInParent<CharacterMultiPlayerManager>();
            if (character != null)
                CheckForKayak(character, other);
        }

        /// <summary>
        /// This method checks if a collider contains a KayakController component, and if so, applies rotation and
        /// velocity to the kayak based on its facing direction and movement. It also sets the closest block to the
        /// player if a WaterFlowManager is present.
        /// </summary>
        /// <param name="collider"> The collider to check </param>
        private void CheckForKayak(CharacterMultiPlayerManager character, Collider collider)
        {
            KayakController kayakController = character.CharacterManager.KayakControllerProperty;

            if (kayakController.gameObject != collider.gameObject ||
                character.CharacterManager.CurrentStateBaseProperty.CanBeMoved == false)
            {
                return;
            }

            if (IsActive == false)
            {
                return;
            }


            //In water flow
            character.CharacterManager.InWaterFlow = true;

            //get rotation
            Quaternion currentRotation = kayakController.transform.rotation;
            Vector3 currentRotationEuler = currentRotation.eulerAngles;
            //get target rotation
            float targetYAngle = Quaternion.LookRotation(Direction).eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(currentRotationEuler.x, targetYAngle, currentRotationEuler.z);

            //check if the boat is facing the flow direction or not
            const float ANGLE_TO_FACE_FLOW = 20f;
            float angleDifference = Mathf.Abs(Mathf.Abs(currentRotationEuler.y) - Mathf.Abs(targetYAngle));
            bool isFacingFlow = angleDifference <= ANGLE_TO_FACE_FLOW;

            //apply rotation
            InputManagement inputManagement = character.CharacterManager.InputManagementProperty;

            bool isMoving = inputManagement.Inputs.PaddleLeft || inputManagement.Inputs.PaddleRight ||
                            Mathf.Abs(inputManagement.Inputs.RotateLeft) > 0.1f ||
                            Mathf.Abs(inputManagement.Inputs.RotateRight) > 0.1f;
            kayakController.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation,
                isMoving ? _rotationLerpWhenMoving :
                isFacingFlow ? _rotationLerpWhenInDirection : _rotationLerpWhenNotInDirection);

            //apply velocity by multiplying current by speed
            Vector3 velocity = kayakController.Rigidbody.velocity;
            float speed = _speed * (isFacingFlow ? _speed : _speed * _speedNotFacingMultiplier);

            kayakController.Rigidbody.velocity = new Vector3(
                velocity.x + speed * Mathf.Sign(velocity.x),
                velocity.y,
                velocity.z + speed * Mathf.Sign(velocity.z));
        }

        private void ManageParticles()
        {
            _playParticleTime -= Time.deltaTime;
            _playSecondParticleTime -= Time.deltaTime;
            if (_playParticleTime <= 0)
            {
                int rand = new Random().Next(4, _particlesTransformsList.Count);
                for (int i = 0; i < rand; i++)
                {
                    _particlesTempList[i % _particlesTempList.Count].Emit(new ParticleSystem.EmitParams(), 1);

                }
                _playParticleTime = UnityEngine.Random.Range(_randomPlayOfParticleTime.x, _randomPlayOfParticleTime.y);
            }
            if (_playSecondParticleTime <= 0)
            {
                int rand = new Random().Next(4, _particlesTransformsList.Count);
                for (int i = 0; i < rand; i++)
                {
                    _particlesTempList[i % _particlesTempList.Count].Emit(new ParticleSystem.EmitParams(), 1);

                }
                _playSecondParticleTime = UnityEngine.Random.Range(_randomPlayOfParticleTime.x, _randomPlayOfParticleTime.y);
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Direction * 2);
            Gizmos.DrawSphere(transform.position + Direction * 2, 0.1f);
        }

#endif
    }
}