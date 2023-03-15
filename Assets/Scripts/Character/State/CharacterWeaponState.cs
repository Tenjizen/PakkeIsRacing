using UnityEngine;

namespace Character.State
{
    public class CharacterWeaponState : CharacterStateBase
    {
        #region Constructor

        public CharacterWeaponState(CharacterManager characterManagerRef, MonoBehaviour monoBehaviour, CameraManager cameraManagerRef) : base(characterManagerRef, monoBehaviour, cameraManagerRef)
        {
        }

        #endregion

        #region Base methods

        public override void EnterState(CharacterManager character)
        {
            Debug.Log("entered weapon state");
        }

        public override void UpdateState(CharacterManager character)
        {
        }

        public override void FixedUpdate(CharacterManager character)
        {
        }

        public override void SwitchState(CharacterManager character)
        {
        }

        #endregion
        
    }
}