using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayControlScreen : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private InputAction _displayControlScreen;
    [SerializeField] private GameObject _controlScreen;
    private void Awake()
    {
        _controlScreen.SetActive(false);
        _displayControlScreen = _inputActions.FindAction("DisplayControlScreen");
        _displayControlScreen.performed += OnOptionsButton;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void OnOptionsButton(InputAction.CallbackContext context)
    {
        switch(_controlScreen.activeSelf)
        {
            case true:
                _controlScreen.SetActive(false);
                break;
            case false:
                _controlScreen.SetActive(true);
                break;
        }
    }
}
