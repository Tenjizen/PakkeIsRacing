using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace MultiplayerLocal
{
    [RequireComponent(typeof(Camera))]
    public class MultipleTargetCamera : MonoBehaviour
    {
        [field: SerializeField] public List<Transform> Targets { get; set; } = new List<Transform>();
        [SerializeField] private float _maxPossibleDistance;
        [SerializeField] private float _minDistanceForZoom;
        [SerializeField] private float _smoothTime;
        [SerializeField] private float _minY;
        [SerializeField] private float _maxY;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (Targets.Count <= 0)
            {
                return;
            }

            Move();
            Zoom();
        }

        private void Zoom()
        {
            float greatestDistance = GetGreatestDistance();

            if (greatestDistance < _minDistanceForZoom)
            {
                greatestDistance = 0f;
            }

            float newY = Mathf.Lerp(_minY, _maxY, greatestDistance / _maxPossibleDistance);
            transform.position = new Vector3(transform.position.x,
                Mathf.Lerp(transform.position.y, newY, Time.deltaTime), transform.position.z);
            // float newZoom = Mathf.Lerp(_maxZoom, _minZoom, GetGreatestDistance() / _zoomLimiter);
            // _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, newZoom, Time.deltaTime);
        }

        private void Move()
        {
            Vector3 centerPoint = GetCenterPoint();
            centerPoint.y = transform.position.y;
            transform.position = Vector3.SmoothDamp(transform.position, centerPoint, ref _velocity, _smoothTime);
        }

        private float GetGreatestDistance()
        {
            Bounds bounds = CreateBounds();
            return bounds.size.x > bounds.size.z ? bounds.size.x : bounds.size.z;
        }

        private Vector3 GetCenterPoint()
        {
            if (Targets.Count <= 1)
            {
                return Targets[0].position;
            }

            Bounds bounds = CreateBounds();

            return bounds.center;
        }

        private Bounds CreateBounds()
        {
            Bounds bounds = new Bounds(Targets[0].position, Vector3.zero);

            foreach (Transform target in Targets)
            {
                bounds.Encapsulate(target.position);
            }

            return bounds;
        }
    }
}