using Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class ControllerScreenManager : MonoBehaviour
    {
        [SerializeField] private GameObject _controlScreen;
        private void Awake()
        {
            _controlScreen.SetActive(false);
        }

        private void Update()
        {
            _controlScreen.SetActive(CharacterManager.Instance.InputManagementProperty.Inputs.DisplayControlScreen);
        }
    }
}
