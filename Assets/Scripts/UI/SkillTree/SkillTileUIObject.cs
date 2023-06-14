using System;
using Character;
using DG.Tweening;
using UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SkillTree
{
    [Serializable]
    public enum SkillCategory
    {
        Navigation,
        Combat
    }
    
    public class SkillTileUIObject : MenuUIObject
    {
        [SerializeField] private Image _colorImage, _lockIcon, _unlockIcon;
        [SerializeField] private string _title_EN, _title_FR;
        [SerializeField, TextArea] private string _description_EN, _description_FR;

        protected override void Start()
        {
            base.Start();
            
            OverlayImage.DOKill();
            OverlayImage.DOFade(0f, 0f).SetUpdate(true);
        }

        public void SetSkillTile(Color color)
        {
            _colorImage.color = color;
        }

        public string GetTitle()
        {
            return CharacterManager.Instance.Parameters.Language ? _title_EN : _title_FR;
        }
        
        public string GetDescription()
        {
            return CharacterManager.Instance.Parameters.Language ? _description_EN : _description_FR;
        }

        public override void Set(bool isActive)
        {
            base.Set(isActive);
        }
    }
}
