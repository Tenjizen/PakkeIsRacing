using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuLeaveUIObject : MenuUIObject
{
    [SerializeField] MenuLeave _menuLeave;

    public override void Set(bool isActive)
    {
        base.Set(isActive);
    }

    protected override void Activate(InputAction.CallbackContext context)
    {
        if (IsSelected == false)
        {
            return;
        }

        base.Activate(context);
    }
}
