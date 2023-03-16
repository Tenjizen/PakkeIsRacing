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
            Debug.Log(CharacterManagerRef.InputManagement.Inputs.DeselectWeapon);
            if (CharacterManagerRef.InputManagement.Inputs.DeselectWeapon)
            {
                Debug.Log("deselect weapon");
                
                CharacterNavigationState characterNavigationState = new CharacterNavigationState(CharacterManagerRef.KayakController, CharacterManagerRef.InputManagement, CharacterManagerRef, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterNavigationState);

                CameraNavigationState cameraNavigationState = new CameraNavigationState(CameraManagerRef, MonoBehaviourRef);
                CameraManagerRef.SwitchState(cameraNavigationState);

            }
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