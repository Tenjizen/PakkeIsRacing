using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class OptionMenuButtonUIObject : MenuUIObject
    {
        [SerializeField] private OptionMenuController _optionMenuController;

        protected override void Activate(InputAction.CallbackContext context)
        {
            if (IsSelected == false || _optionMenuController.IsUsable == false)
            {
                return;
            }

            base.Activate(context);
        }
    }
}
