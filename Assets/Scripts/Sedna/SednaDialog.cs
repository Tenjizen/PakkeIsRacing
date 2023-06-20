using Character;
using GPEs;
using Kayak;
using System.Collections.Generic;
using UI.Dialog;
using UnityEngine;
using WaterFlowGPE.Bezier;

namespace Sedna
{
    public class SednaDialog : MonoBehaviour
    {
        private enum State
        {
            Moving = 0, 
            Dialog = 1
        };
        
        [SerializeField] private State _enumState;
        [SerializeField] private DialogCreator dialog;

        [SerializeField] private List<GameObject> _positionSednaDialog = new List<GameObject>();
        [SerializeField] private float _removeAtYPosValue;
        [SerializeField] private float _minimumPlayerVelocity = 1f;

        [SerializeField] float _lerpMovingPos = 15;
        [SerializeField] float _lerpMovingRota = 5;

        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] private Transform _playerTransform;
        private float _currentSplinePosition;

        [Header("Spline")]
        public float MovingValue = 0.005f;
        public float SpeedLerpToMovingValue = 0.1f;

        [Header("Debug"), SerializeField, ReadOnly] private bool _startMoving = false;
        [SerializeField, ReadOnly] private bool _endDialog = false;
        
        private bool _goodHeight = false;
        private int _index = 0;
        private Transform _spline;
        private Rigidbody _rigidbodyKayak;
        private GameObject _target;
        private Vector3[] _directionsRaycast = { Vector3.right, Vector3.left, Vector3.forward };

        private void Awake()
        {
            if (_rigidbodyKayak == null)
            {
                _rigidbodyKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody;
            }

            if (_positionSednaDialog.Count > 0)
            {
                _target = _positionSednaDialog[0];
            }
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
                    _spline = _splinePath.transform;
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
            Transform t = transform;
            
            if (_endDialog)
            {
                Quaternion rotation = t.rotation;
                rotation = Quaternion.Euler(-180, 0, 0);
                t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * _lerpMovingRota);

                if (t.eulerAngles.x <= 45 && t.eulerAngles.x > 0
                    || t.eulerAngles.x >= -45 && t.eulerAngles.x < 0)
                {
                    var pos = t.position;
                    pos.y = -15;
                    pos = Vector3.Lerp(t.position, pos, Time.deltaTime * 1);
                    t.position = pos;
                }

                if (t.position.y <= -10)
                {
                    _endDialog = false;
                    gameObject.SetActive(false);
                }
            }

            if (_endDialog)
            {
                return;
            }
            
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

                        float newY = Mathf.Lerp(transform.position.y, CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(t.position) - _removeAtYPosValue, Time.deltaTime * _lerpMovingPos);

                        t.position = Vector3.Lerp(t.position, new Vector3(currentPosition.x, newY, currentPosition.z), Time.deltaTime * _lerpMovingPos);

                        if (Mathf.Abs(_rigidbodyKayak.velocity.x) + Mathf.Abs(_rigidbodyKayak.velocity.z) > _minimumPlayerVelocity)
                        {
                            var targetRota = _playerTransform.eulerAngles;
                            t.rotation = Quaternion.Slerp(t.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z),Time.deltaTime * _lerpMovingRota);
                        }
                    }
                    else
                    {
                        _goodHeight = true;
                        Moving();
                    }
                    break;
            }
        }


        private void Moving()
        {
            if (Mathf.Abs(_rigidbodyKayak.velocity.x) + Mathf.Abs(_rigidbodyKayak.velocity.z) > _minimumPlayerVelocity && _target != null)
            {
                SetTarget();

                var pos = _target.transform.position;
                pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - _removeAtYPosValue;
                transform.position = Vector3.Lerp(transform.position, pos, _lerpMovingPos * Time.deltaTime);

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
            foreach (Vector3 direction in _directionsRaycast)
            {
                Vector3 normalizedDirection = direction.normalized;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, normalizedDirection, out hit, 3))
                {
                    if (hit.collider.gameObject.GetComponent<KayakController>() != null)
                    {
                        return;
                    }

                    _target = _positionSednaDialog[_index++ % _positionSednaDialog.Count];
                }
            }
        }

        private void SetTarget()
        {
            if (_target != null && _startMoving)
            {
                return;
            }
            
            _target = GetClosestPoint(transform.position);
            _startMoving = true;
        }

        private GameObject GetClosestPoint(Vector3 position)
        {
            float bestDistance = float.MaxValue;
            GameObject bestPoint = null;

            foreach (GameObject point in _positionSednaDialog)
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

            if (_spline == null)
            {
                return;
            }
            
            Vector3 p = _spline.position;
            p.x = _playerTransform.position.x;
            p.z = _playerTransform.position.z;
            _spline.position = Vector3.Lerp(_spline.position, p, 0.02f * Time.deltaTime * 100);

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
        public void StartDialog(GameObject gameObject)
        {
            if(dialog.HasEnded == true && dialog.CanBeReplayed == false)
            {
                return;
            }

            gameObject.SetActive(true);

        }

    }
}