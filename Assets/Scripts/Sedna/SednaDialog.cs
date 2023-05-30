using Character;
using GPEs;
using Kayak;
using System.Collections.Generic;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Sedna
{
    public class SednaDialog : MonoBehaviour
    {
        public enum State { Moving = 0, Dialog = 1 };
        [SerializeField] private State _enumState;

        [SerializeField] private List<GameObject> _posSednaDialog = new List<GameObject>();
        [SerializeField] private float _removeAtYPosValue;
        [SerializeField] private float _minimumPlayerVelocity = 1f;

        [SerializeField] float _lerpMovingPos = 15;
        [SerializeField] float _lerpMovingRota = 5;

        private Rigidbody _rbKayak;
        private GameObject _target;
        private int _index = 0;
        private bool _goodHeight = false;
        private Vector3[] directionsRaycast = { Vector3.right, Vector3.left, Vector3.forward };


        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] private Transform _playerTransform;
        private float _currentSplinePosition;

        [Header("Spline")]
        public float MovingValue = 0.005f;
        public float SpeedLerpToMovingValue = 0.1f;
        private Transform splinePos;

        [Header("Debug"), SerializeField, ReadOnly] private bool _startMoving = false;
        [SerializeField, ReadOnly] private bool _endDialog = false;

        private void Awake()
        {
            if (_rbKayak == null)
            {
                _rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody;
            }

            //SetTarget();
            _target = _posSednaDialog[0];
        }

        private void Start()
        {
            switch (_enumState)
            {
                case State.Moving:
                    _currentSplinePosition = 0;

                    if (_splinePath == null)
                    {
                        return;
                    }
                    splinePos = _splinePath.transform;
                    Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
                    transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
                    break;

                case State.Dialog:
                    if (_target != null)
                    {
                        var pos = _target.transform.position;
                        pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(pos) - _removeAtYPosValue - 10;
                        transform.position = pos;


                        Vector3 lookPos = _playerTransform.position - transform.position;
                        lookPos.y = 0;

                        Quaternion rota = Quaternion.LookRotation(lookPos);
                        transform.rotation = rota;
                    }
                    break;
            }
        }


        private void FixedUpdate()
        {
            if (_endDialog == true)
            {
                Quaternion rota = transform.rotation;
                rota = Quaternion.Euler(-180, 0, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rota, Time.deltaTime * _lerpMovingRota);

                if (transform.eulerAngles.x <= 45 && transform.eulerAngles.x > 0
                    || transform.eulerAngles.x >= -45 && transform.eulerAngles.x < 0)
                {
                    var pos = transform.position;
                    pos.y = -15;
                    pos = Vector3.Lerp(transform.position, pos, Time.deltaTime * 1);
                    transform.position = pos;
                }

                if (transform.position.y <= -10)
                {
                    _endDialog = false;
                    this.gameObject.SetActive(false);
                }
            }

            if (_endDialog == false)
            {
                switch (_enumState)
                {
                    case State.Moving:
                        if (_currentSplinePosition < 0.95f)
                        {
                            ManageMovementInSpline();
                        }
                        else
                        {
                            Moving();
                        }
                        break;

                    case State.Dialog:
                        if (transform.position.y < (CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue) - 0.05f && _goodHeight == false)
                        {
                            Vector3 currentPosition = _target.transform.position;

                            float newY = Mathf.Lerp(transform.position.y, CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue, Time.deltaTime * _lerpMovingPos);

                            transform.position = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, newY, currentPosition.z), Time.deltaTime * _lerpMovingPos);

                            if (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) > _minimumPlayerVelocity)
                            {
                                var targetRota = _playerTransform.eulerAngles;
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z),Time.deltaTime * _lerpMovingRota);
                            }
                        }
                        else
                        {
                            _goodHeight = true;
                            Moving();
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private void Moving()
        {
            if (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) > _minimumPlayerVelocity && _target != null)
            {
                SetTarget();

                var pos = _target.transform.position;
                pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue;
                transform.position = Vector3.Lerp(transform.position, pos, _lerpMovingPos * Time.deltaTime);

                //Vector3 currentPosition = _target.transform.position;
                //float newY = Mathf.Lerp(transform.position.y, CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue, 0.01f * Time.deltaTime * 100);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, newY, currentPosition.z), Time.deltaTime * _lerpMovingPos);


                var targetRota = _playerTransform.eulerAngles;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z), Time.deltaTime * _lerpMovingRota);

                RaycastDirection();
            }
            else
            {
                _startMoving = false;

                var pos = transform.position;
                pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue;
                transform.position = Vector3.Lerp(transform.position, pos, _lerpMovingPos);

                Vector3 lookPos = _playerTransform.position - transform.position;
                lookPos.y = 0;

                Quaternion rota = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rota, _lerpMovingRota * Time.deltaTime);
            }
        }

        private void RaycastDirection()
        {
            foreach (Vector3 direction in directionsRaycast)
            {
                Vector3 normalizedDirection = direction.normalized;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, normalizedDirection, out hit, 3))
                {
                    if (hit.collider.gameObject.GetComponent<KayakController>() != null)
                    {
                        return;
                    }

                    _target = _posSednaDialog[_index++ % _posSednaDialog.Count];
                }
            }
        }

        private void SetTarget()
        {
            if (_target == null || _startMoving == false)
            {
                _target = GetClosestPoint(transform.position);
                _startMoving = true;
            }
        }

        public GameObject GetClosestPoint(Vector3 position)
        {
            float bestDistance = float.MaxValue;
            GameObject bestPoint = null;

            foreach (var point in _posSednaDialog)
            {
                Vector3 direction = point.transform.position - position;

                float distance = direction.sqrMagnitude;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = point;
                }
            }

            return bestPoint;
        }

        private void ManageMovementInSpline()
        {
            _currentSplinePosition += MovingValue * Time.deltaTime;
            _currentSplinePosition = Mathf.Clamp01(_currentSplinePosition);

            var pos = splinePos.position;
            pos.x = _playerTransform.position.x;
            pos.z = _playerTransform.position.z;
            splinePos.position = Vector3.Lerp(splinePos.position, pos, 0.02f * Time.deltaTime * 100);

            //splinePos.position = pos;
            //var rotationSpline = splinePos.rotation;
            //rotationSpline.y = _playerTransform.rotation.y;
            //splinePos.rotation = rotationSpline;


            Transform t = transform;

            Vector3 position = Vector3.Lerp(t.position, _splinePath.GetPoint(_currentSplinePosition), SpeedLerpToMovingValue * Time.deltaTime * 100);
            t.position = position;

            //rotation
            Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
            t.LookAt(pointB);
            Vector3 rotation = t.transform.rotation.eulerAngles;
            t.rotation = Quaternion.Euler(rotation.x + 65, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f), rotation.z);
        }


        public void EndDialog()
        {
            _endDialog = true;
        }
    }
}