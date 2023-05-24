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

        public State EnumState;
        [Header("Path"), SerializeField] private BezierSpline _splinePath;
        [SerializeField] Transform _playerTransform;
        private float _currentSplinePosition;

        public float MovingValue = 0.005f;
        public float SpeedLerpToMovingValue = 0.1f;
        private Transform splinePos;

        public List<GameObject> PosSednaDialog = new List<GameObject>();
        [SerializeField] private float RemoveAtYPosValue;
        private Rigidbody _rbKayak;

        public float MinimumVelocity = 1f;
        private GameObject _target;
        private bool _startMoving = false;

        private int _index = 0;
        public Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward };
        public bool EndDialog = false;


        public Rigidbody _rb;
        public float LerpMovingPos = 3;


        private void Start()
        {
            if (_rbKayak == null)
            {
                _rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody;
            }

            SetTarget();

            switch (EnumState)
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
                    Vector3 lookPos = _playerTransform.position - transform.position;
                    lookPos.y = 0;

                    Quaternion rota = Quaternion.LookRotation(lookPos);
                    transform.rotation = rota;

                    var pos = transform.position;
                    if (_target == null)
                    {
                        SetTarget();
                    }
                    pos = _target.transform.position;

                    pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(pos) - RemoveAtYPosValue - 10;
                    transform.position = pos;
                    break;

                default:
                    break;
            }
        }
        bool _goodHeight = false;

        //[SerializeField] private Transform _kayakTransform;
        //private Rigidbody _rigidbody;
        //private Vector3 _velocity;


        //private void Update()
        //{
        //    //if (EndDialog == true)
        //    //{
        //    //    Quaternion rota = transform.rotation;
        //    //    rota = Quaternion.Euler(-180, 0, 0);
        //    //    transform.rotation = Quaternion.Slerp(transform.rotation, rota, 0.02f * Time.deltaTime * 100);

        //    //    if (transform.eulerAngles.x <= 45 && transform.eulerAngles.x > 0
        //    //        || transform.eulerAngles.x >= -45 && transform.eulerAngles.x < 0)
        //    //    {
        //    //        var pos = transform.position;
        //    //        pos.y = -15;
        //    //        pos = Vector3.Lerp(transform.position, pos, Time.deltaTime * 1);
        //    //        transform.position = pos;
        //    //    }

        //    //    if (transform.position.y <= -10)
        //    //    {
        //    //        EndDialog = false;
        //    //        this.gameObject.SetActive(false);
        //    //    }
        //    //}


        //    //if (EndDialog == false)
        //    //{
        //        //switch (EnumState)
        //        //{
        //        //    case State.Moving:
        //        //        if (_currentSplinePosition < 1)
        //        //        {
        //        //            ManageMovementInSpline();
        //        //        }
        //        //        else
        //        //        {
        //        //            Moving();
        //        //        }
        //        //        break;

        //        //    case State.Dialog:
        //                //if (transform.position.y < (CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - RemoveAtYPosValue) - 0.05f && _goodHeight == false)
        //                //{
        //                //    Vector3 currentPosition = _target.transform.position;

        //                //    float newY = Mathf.Lerp(transform.position.y, CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - RemoveAtYPosValue, 0.01f * Time.deltaTime * 100);

        //                //    transform.position = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, newY, currentPosition.z), Time.fixedDeltaTime * 100);
        //                //    //var targetPosition = Vector3.Lerp(transform.position, new Vector3(currentPosition.x, newY, currentPosition.z), Time.fixedDeltaTime * LerpMovingPos);
        //                //    //var targetPosition = new Vector3(currentPosition.x, newY, currentPosition.z) * Time.fixedDeltaTime * LerpMovingPos;
        //                //    //var targetPosition = new Vector3(currentPosition.x, newY, currentPosition.z) * Time.fixedDeltaTime * LerpMovingPos;
        //                //    //_rb.MovePosition(targetPosition);

        //                //    if (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) > MinimumVelocity)
        //                //    {
        //                //        var targetRota = _playerTransform.eulerAngles;
        //                //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z), 0.02f * Time.deltaTime * 100);
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    _goodHeight = true;
        //                    //Moving();
        //                //}
        //        //        break;
        //        //    default:
        //        //        break;
        //        //}
        //    //}

        //}
        //private void HandlePosition()
        //{
        //    Vector3 current = new Vector3(transform.position.x, 0, transform.position.z);
        //    Vector3 target = new Vector3(_target.transform.position.x, 0, _target.transform.position.z);
        //    Vector3 positionToRefVelocity = Vector3.SmoothDamp(current, target, ref _velocity, 1);
        //    //apply velocity
        //    Vector3 velocity = new Vector3(_velocity.x, _rb.velocity.y, _velocity.z);
        //    _rb.velocity = velocity;
        //}

        public float distPoint = 3f;
        public Vector3 smooth;
        //public float speed;



        public float speed;
        public float NormalSpeed = 12;
        public float AccelSpeed = 13.5f;

        public float distTarget;


        private void Update()
        {
            distTarget = Vector3.Distance(_target.transform.position, transform.position);

            Vector3 direction = _target.transform.position - transform.position;
            //direction.z = 0;



            if (_rbKayak.velocity.magnitude > 1)
            {
                if (distTarget < 0.5f)
                {
                    speed = NormalSpeed;
                }
                else if (distTarget > 3)
                {
                    speed = AccelSpeed;
                }
                direction.Normalize();
                Vector3 movement = direction * speed * Time.deltaTime;
                transform.position += movement;
            }

        }

        private void Moving()
        {
            if (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) > MinimumVelocity)
            {


                Vector3 directionTarget = _target.transform.position - transform.position;
                directionTarget.Normalize();
                //Vector3 movement = new Vector3(directionTarget.x, 
                //    0, 
                //    directionTarget.z) * (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z)) * Time.deltaTime;

                //var pos = transform.position;
                //if (_target != null)
                //{
                //    //directionTarget = _target.transform.position - transform.position;
                //    pos = _target.transform.position;
                //}

                //directionTarget.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(directionTarget) - RemoveAtYPosValue;
                //directionTarget.Normalize();

                //pos.x += directionTarget.x /** (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z))*/;
                //pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(transform.position) - RemoveAtYPosValue;
                //pos.z += directionTarget.z /** (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z))*/;

                //transform.position = movement;

                float movement = speed * Time.deltaTime;
                transform.position += directionTarget * movement;
                //transform.Translate(movement * directionTarget);



                //transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime / 0.5f);


                //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * LerpMovingPos);
                //transform.position = Vector3.SmoothDamp(transform.position, pos, ref smooth, 1/ LerpMovingPos);

                //var targetPosition = pos * LerpMovingPos * Time.fixedDeltaTime ;


                //transform.position = pos;
                //_rb.MovePosition(targetPosition);



                //var targetRota = _playerTransform.eulerAngles;
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z), 0.02f * Time.deltaTime * 100);

                //SetTarget();
                //foreach (Vector3 direction in directions)
                //{
                //    Vector3 normalizedDirection = direction.normalized;

                //    RaycastHit hit;
                //    if (Physics.Raycast(transform.position, normalizedDirection, out hit, 3))
                //    {
                //        if (hit.collider.gameObject.GetComponent<KayakController>() != null)
                //        {
                //            return;
                //        }
                //        //if(direction == Vector3.forward)
                //        //if(direction == Vector3.right)
                //        //    _target = PosSednaDialog[]
                //        _target = PosSednaDialog[_index++ % PosSednaDialog.Count];
                //    }
                //}
            }
            //else
            //{
            //    _startMoving = false;

            //    Vector3 lookPos = _playerTransform.position - transform.position;
            //    lookPos.y = 0;

            //    Quaternion rota = Quaternion.LookRotation(lookPos);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, rota, 0.05f * Time.deltaTime * 100);
            //}
            //else if (dist > 0 ||
            //    Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) < MinimumVelocity)
            //{
            //    //dist = Vector3.Distance(transform.position, _target.transform.position);

            //}

        }
        float dist;
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

            foreach (var point in PosSednaDialog)
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
            splinePos.position = Vector3.Lerp(splinePos.position, pos, 0.01f * Time.deltaTime * 100);
            //splinePos.position = pos;

            Transform t = transform;



            Vector3 position = Vector3.Lerp(t.position, _splinePath.GetPoint(_currentSplinePosition), SpeedLerpToMovingValue * Time.deltaTime * 100);
            t.position = position;
            //rotation
            Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
            pointB.y = 0;
            Quaternion rota = Quaternion.LookRotation(pointB);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rota.x + 65, rota.y, rota.z), 0.05f * Time.deltaTime * 100);
            //Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z), 0.02f * Time.deltaTime * 100)

            //Vector3 rotation = t.transform.rotation.eulerAngles;
            //t.LookAt(pointB);
            //t.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f * Time.deltaTime * 100), rotation.z));


            //Vector3 lookPos = _playerTransform.position - transform.position;
            //lookPos.y = 0;

        }

        public void Enddialog()
        {
            EndDialog = true;
        }
    }
}
