using System;
using Character;
using Character.Camera.State;
using Character.State;
using UI.WeaponWheel;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponWheelButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _button;
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private Weapon _weapon;
    public UnityEvent OnSelected = new UnityEvent();

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    public void Hover()
    {
        _animator.SetBool("Hover",true);
        _button.Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Exit();
    }

    public void Exit()
    {
        _animator.SetBool("Hover",false);
    }

    public void Select()
    {
        Debug.Log($"select {_weapon}");
        
        OnSelected.Invoke();
        _characterManager.CurrentWeapon = _weapon;

        CharacterWeaponState characterWeaponState = 
            new CharacterWeaponState(_characterManager,_characterManager.CurrentStateBase.MonoBehaviourRef,_characterManager.CurrentStateBase.CameraManagerRef);
        _characterManager.SwitchState(characterWeaponState);
        
        CameraCombatState cameraCombatState = new CameraCombatState(_characterManager.CameraManagerRef, _characterManager.CurrentStateBase.MonoBehaviourRef);
        _characterManager.CameraManagerRef.SwitchState(cameraCombatState);
    }
}
