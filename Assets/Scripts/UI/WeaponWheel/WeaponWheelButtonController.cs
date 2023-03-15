using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponWheelButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Button _button;

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
}
