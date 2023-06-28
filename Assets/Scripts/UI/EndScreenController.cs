using System;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class EndScreenController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _blackBackground;
        [SerializeField] private TMP_Text _text;
        

        private void Awake()
        {
            _text.DOFade(0,0);
            _blackBackground.DOFade(0,0);
        }
    
        public void SetEndScreen()
        {
            _text.DOFade(1,1);
            _blackBackground.DOFade(1,1);

            CharacterManager.Instance.CurrentStateBaseProperty.CanBeMoved = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanOpenMenus = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMove = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterMakeActions = false;
            CharacterManager.Instance.CurrentStateBaseProperty.CanCharacterOpenWeapons = false;
        }
    }
}
