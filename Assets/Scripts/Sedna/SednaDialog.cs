using Character;
using GPEs;
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
        private Quaternion _targetRotation;

        public float MovingValue = 0.005f;
        public float SpeedLerpToMovingValue = 0.1f;
        private Transform splinePos;

        public GameObject PosSednaDialog;
        [SerializeField] private float RemoveAtYPosValue;
        private Rigidbody _rbKayak;
        private void Start()
        {
            if(_rbKayak == null)
            {
                _rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody;
            }
            if (EnumState == State.Moving)
            {
                _currentSplinePosition = 0;

                if (_splinePath == null)
                {
                    return;
                }
                splinePos = _splinePath.transform;
                Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
                transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
            }
        }
        bool canMove = false;
        private void Update()
        {
            if (EndDialog == true)
            {
                Quaternion rota = transform.rotation;
                rota = Quaternion.Euler(-180, 0, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rota, 0.02f * Time.deltaTime * 100);
                if (transform.eulerAngles.x >= 60 && transform.eulerAngles.x > 0 || transform.eulerAngles.x <= -60 && transform.eulerAngles.x < 0) canMove = true;

                if (transform.eulerAngles.x <= 35 && transform.eulerAngles.x > 0 && canMove == true 
                    || transform.eulerAngles.x >= -35 && transform.eulerAngles.x < 0 &&canMove == true)
                {
                    var pos = transform.position;
                    pos.y = -15;
                    pos = Vector3.Lerp(transform.position, pos, 0.01f * Time.deltaTime * 100);
                    transform.position = pos;
                }

                if (transform.position.y <= -10)
                {
                    EndDialog = false;
                    canMove = false;
                    this.gameObject.SetActive(false);
                }
            }
        }
        public float MinimumVelocity = 1f;
        private void FixedUpdate()
        {
            if (EndDialog == false)
            {
                switch (EnumState)
                {
                    case State.Moving:
                        ManageMovement();
                        break;
                    case State.Dialog:
                       
                        if (Mathf.Abs(_rbKayak.velocity.x) + Mathf.Abs(_rbKayak.velocity.z) > MinimumVelocity ||
                        (Mathf.Abs(CharacterManager.Instance.CurrentStateBaseProperty.RotationStaticForceY) > MinimumVelocity))
                        {
                            var pos = PosSednaDialog.transform.position;
                            pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(pos) - RemoveAtYPosValue;
                            transform.position = Vector3.Lerp(transform.position, pos, 0.015f * Time.deltaTime * 100);

                            var targetRota = _playerTransform.eulerAngles;
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRota.x + 65, targetRota.y, targetRota.z), 0.02f * Time.deltaTime * 100);
                        }
                        else
                        {
                            var pos = PosSednaDialog.transform.position;
                            pos.y = CharacterManager.Instance.SednaManagerProperty.Waves.GetHeight(pos) - RemoveAtYPosValue;
                            transform.position = Vector3.Lerp(transform.position, pos, 0.05f * Time.deltaTime * 100);


                            var lookPos = _playerTransform.position - transform.position;
                            //lookPos.y = transform.position.y;
                            lookPos.y = 0;

                            var rota = Quaternion.LookRotation(lookPos);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rota, 0.05f * Time.deltaTime * 100);
                        }

                        break;
                    default:
                        break;
                }
            }
        }
        private void ManageMovement()
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

            if (_currentSplinePosition >= 1)
            {
                //_parent.SetActive(false);

                Vector3 rotation = t.transform.rotation.eulerAngles;
                t.LookAt(_playerTransform);
                t.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f * Time.deltaTime * 100), rotation.z));
            }
            else
            {
                //rotation
                Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
                Vector3 rotation = t.transform.rotation.eulerAngles;
                t.LookAt(pointB);
                t.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f * Time.deltaTime * 100), rotation.z));
            }
        }

        public bool EndDialog = false;
        public void Enddialog()
        {
            EndDialog = true;
        }
    }
}
