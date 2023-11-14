using Character;
using UnityEngine;

namespace Enemies.Shark.State
{
    public class SharkFreeRoamState : SharkBaseState
    {
        private enum FreeRoamState
        {
            GoToRotate = 0, 
            RotateToMoveTarget = 1
        };
   
        private FreeRoamState _state;
        private float _distSharkPointTarget;
        private float _randTimer;
        private float _timer;

        public override void EnterState(SharkManager sharkManager)
        {
            _state = FreeRoamState.RotateToMoveTarget;

            Debug.Log("comm");
            //CharacterManager.Instance.EnemyUIManager.DisableEnemyUI();

            sharkManager.SharkCollider.enabled = false;

            Transform shark = sharkManager.transform;
            Vector3 rotation = shark.localEulerAngles;
            rotation.x = 0;
            rotation.y = 0;
            rotation.z = 0;
            shark.localEulerAngles = rotation;
        }

        public override void UpdateState(SharkManager sharkManager)
        {
            if(sharkManager.transform.position.y > sharkManager.Data.ElevationOffset || sharkManager.transform.position.y < sharkManager.Data.ElevationOffset)
            {
                var pos = sharkManager.Forward.transform.position;
                pos.y = Mathf.Lerp(pos.y ,sharkManager.Data.ElevationOffset, 0.01f);
                sharkManager.transform.position = pos;
            }
            
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);

            switch (_state)
            {
                case FreeRoamState.RotateToMoveTarget:
                    if (_distSharkPointTarget <= 5)
                    {
                        _randTimer = Random.Range(7, 10);
                        _state = FreeRoamState.GoToRotate;
                    }
                    else
                    {
                        MoveToTarget(sharkManager);
                    }
                    break;
                case FreeRoamState.GoToRotate:
                    RotateFreeRoam(sharkManager);
                    break;
            }
        }

        public override void FixedUpdate(SharkManager sharkManager)
        {

        }

        public override void SwitchState(SharkManager sharkManager)
        {

        }


        private void RotateFreeRoam(SharkManager sharkManager)
        {
            Transform forwardTransform = sharkManager.Forward.transform;
            Transform pointTargetTransform = sharkManager.PointTarget.transform;
            Vector2 targetPos = new Vector2(pointTargetTransform.position.x, pointTargetTransform.position.z);
            Vector2 sharkForward = new Vector2(forwardTransform.forward.x, forwardTransform.forward.z);
            Vector2 sharkPos = new Vector2(forwardTransform.position.x, forwardTransform.position.z);

            float angle = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
            if (angle < 0)
            {
                angle += 360;
            }
            
            Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;

            if (_distSharkPointTarget > sharkManager.Data.MinDistanceBetweenPointFreeRoam)
            {
                switch (angle)
                {
                    case > 90 and < 180:
                        rotation.y -= Time.deltaTime * sharkManager.Data.SpeedFreeRoamRotationStatic;
                        break;
                    case > 270 and < 360:
                        rotation.y -= Time.deltaTime * sharkManager.Data.SpeedFreeRoamRotationStatic;
                        break;
                    default:
                        rotation.y += Time.deltaTime * sharkManager.Data.SpeedFreeRoamRotationStatic;
                        break;
                }
            }

            sharkManager.Forward.transform.localEulerAngles = rotation;
            Vector3 translation = Vector3.forward * (sharkManager.Data.SpeedFreeRoamRotationAroundPoint * Time.deltaTime);
            sharkManager.Forward.transform.Translate(translation, Space.Self);

            _timer += Time.deltaTime;
            if (_timer < _randTimer)
            {
                return;
            }
            _state = FreeRoamState.RotateToMoveTarget;
            _timer = 0;
        }

        private void MoveToTarget(SharkManager sharkManager)
        {
            Vector2 targetPosition = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
            Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
            Vector2 sharkPosition = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

            float angle = Vector2.SignedAngle(targetPosition - sharkPosition, forward);
            if (angle < 0)
            { 
                angle += 360;
            }

            //rotate to angle 0 to target
            Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
            if (angle >= 0 && angle <= 180)
            {
                rotation.y += Time.deltaTime * sharkManager.Data.SpeedFreeRoamRotationStatic;
            }
            else if (angle < 360 && angle > 180)
            {
                rotation.y -= Time.deltaTime * sharkManager.Data.SpeedFreeRoamRotationStatic;
            }

            //apply rotation
            sharkManager.Forward.transform.localEulerAngles = rotation;
            
            Vector3 position = sharkManager.Forward.transform.position;
            position.y = sharkManager.Data.ElevationOffset;
            sharkManager.Forward.transform.position = position;

            sharkManager.SwitchSpeed(sharkManager.Data.SpeedToMoveToTarget);

            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);
        }
    }
}
