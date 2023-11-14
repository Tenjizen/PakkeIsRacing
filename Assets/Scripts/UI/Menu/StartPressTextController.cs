using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Menu
{
    public class StartPressTextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void Update()
        {

            //if (CharacterManager.Instance.IsGameLaunched == false)
            //{
            //    return;
            //}
        
            //if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            //{
            //    DisableMenuStartUI();
            //}
        }


        private void DisableMenuStartUI()
        {
            _text.DOFade(0, 0.15f);
        }
    }
}
