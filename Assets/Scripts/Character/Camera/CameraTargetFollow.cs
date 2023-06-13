using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Camera
{
    public class CameraTargetFollow : MonoBehaviour
    {
        [SerializeField] private Transform _kayakTransform;

        private Vector3 _localPositionBase;

        private void Start()
        {
            _localPositionBase = transform.localPosition;
        }

        private void Update()
        {
            //position
            transform.position = _kayakTransform.position + _localPositionBase;
        }
    }
}
