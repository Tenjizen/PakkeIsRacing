using System;
using Character;
using DG.Tweening;
using UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SkillTree
{
    [Serializable]
    public enum CapacityType
    {
        BreakingDistance,
        MaximumSpeed,
        RotationSpeed,
        UnbalanceThreshold,
        Sprint,
        LaunchDistance,
        ChargingTimeReduction,
        MoreExperience,
        SednaRecuperationAfterShoot,
        IcebergDestruction
    }
    
    public class SkillTileUIObject : MenuUIObject
    {
        [SerializeField] private Image _colorImage, _lockIcon, _unlockIcon, _activatedIcon;
        [SerializeField] private string _title_EN, _title_FR;
        [SerializeField, TextArea] private string _description_EN, _description_FR;
        [Space(5), SerializeField] private bool _unlockAtStart;
        [Space(5), SerializeField] private CapacityType _capacity;
        [SerializeField] private float _multiplier = 1;
        
        private bool _isUnlocked;
        private bool _isActivated;
        
        protected override void Start()
        {
            base.Start();
            
            OverlayImage.DOKill();
            OverlayImage.DOFade(0f, 0f).SetUpdate(true);

            SetLock(false);
            _lockIcon.DOComplete();
            
            _activatedIcon.gameObject.SetActive(false);
        }

        public void SetLock(bool isUnlock)
        {
            _isUnlocked = _unlockAtStart || isUnlock;

            _lockIcon.DOFade(_isUnlocked ? 0 : 1, 0.5f);
            _unlockIcon.gameObject.SetActive(_isUnlocked);

            if (_isUnlocked)
            {
                CharacterManager.Instance.UIMenuManagerRef.RemoveFromDictionary(_lockIcon);
            }
        }

        public void SetActivated(bool isActivated)
        {
            _isActivated = isActivated;
            _unlockIcon.gameObject.SetActive(_isActivated == false);

            _activatedIcon.gameObject.SetActive(_isActivated);
            _activatedIcon.gameObject.transform.DOComplete();
            _activatedIcon.gameObject.transform.DOPunchScale(Vector3.one*0.1f, 0.2f);

            SetCapacityEffect();
        }

        private void SetCapacityEffect()
        {
            PlayerStatsMultipliers character = CharacterManager.Instance.PlayerStats;
            switch (_capacity)
            {
                case CapacityType.BreakingDistance:
                    character.BreakingDistanceMultiplier = _multiplier;
                    break;
                case CapacityType.MaximumSpeed:
                    character.MaximumSpeedMultiplier = _multiplier;
                    break;
                case CapacityType.RotationSpeed:
                    character.RotationSpeedMultiplier = _multiplier;
                    break;
                case CapacityType.UnbalanceThreshold:
                    character.UnbalancedThresholdMultiplier = _multiplier;
                    break;
                case CapacityType.Sprint:
                    PlayerAbilities abilities = CharacterManager.Instance.Abilities;
                    abilities.SprintUnlock = true;
                    CharacterManager.Instance.Abilities = abilities;
                    break;
                case CapacityType.LaunchDistance:
                    character.WeaponLaunchDistanceMultiplier = _multiplier;
                    break;
                case CapacityType.ChargingTimeReduction:
                    character.ChargeTimeReducingMultiplier = _multiplier;
                    break;
                case CapacityType.MoreExperience:
                    character.ExperienceGainMultiplier = _multiplier;
                    break;
                case CapacityType.SednaRecuperationAfterShoot:
                    character.WeaponRecallTimeMultiplier = _multiplier;
                    break;
                case CapacityType.IcebergDestruction:
                    PlayerAbilities abilitiesDestroy = CharacterManager.Instance.Abilities;
                    abilities.CanDestroyIceberg = true;
                    CharacterManager.Instance.Abilities = abilitiesDestroy;
                    break;
            }
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
