using System;
using System.Collections.Generic;
using Character.Data;
using Character.Data.Experience;
using Fight.Data;
using UI;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class ExperienceManager : MonoBehaviour
    {
        [field:SerializeField] public ExperienceUIManager ExperienceUIManagerProperty { get; private set; }
        public ExperienceData Data;

        [Header("Levels"),ReadOnly, SerializeField] private Level _currentLevelData;
        [ReadOnly, SerializeField] private int _currentLevel;
        [ReadOnly, SerializeField] private float _currentExperience;
        [ReadOnly, SerializeField] private float _currentCombatExperience;
        [ReadOnly, SerializeField] private float _currentNavigationExperience;

        [Header("Character Values multiplier")]
        //navigation
        [ReadOnly] public float BreakingDistanceMultiplier = 1;
        [ReadOnly] public float MaximumDistanceMultiplier = 1;
        [ReadOnly] public float RotatingSpeedMultiplier = 1;
        [ReadOnly] public float BalanceLimitMultiplier = 1;
        //combat
        [ReadOnly] public float ProjectileSpeedMultiplier = 1;

        [Header("Events")] 
        public List<UnityEvent> EventAtEachLevel = new List<UnityEvent>();
        
        private void Start()
        {
            _currentLevel = 0;
            _currentExperience = 0f;
            _currentLevelData = Data.Levels[_currentLevel];

            ExperienceUIManagerProperty.SetMaxLevel(Data.Levels.Count);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Combat,_currentExperience,_currentLevelData.ExperienceNeededToComplete, 0f);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Navigation,_currentExperience,_currentLevelData.ExperienceNeededToComplete, 0f);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience,_currentExperience,_currentLevelData.ExperienceNeededToComplete, 0f);
        }

        public void AddExperience(float value)
        {
            _currentExperience += value;
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience, _currentExperience, _currentLevelData.ExperienceNeededToComplete, 2f);
            
            while (_currentExperience >= _currentLevelData.ExperienceNeededToComplete && _currentLevel <= Data.Levels.Count)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentExperience -= _currentLevelData.ExperienceNeededToComplete;

            _currentCombatExperience += _currentLevelData.CombatExperienceGained;
            _currentNavigationExperience += _currentLevelData.NavigationExperienceGained;
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Combat, _currentCombatExperience, Data.CombatGaugeMax, 2f);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Navigation, _currentNavigationExperience, Data.NavigationGaugeMax, 2f);
                
            _currentLevel++;
            _currentLevelData = Data.Levels[_currentLevel];
            
            //values
            BreakingDistanceMultiplier = SetMultiplierFromPercentageAndValue(Data.BreakingDistanceMultiplier);
            MaximumDistanceMultiplier = SetMultiplierFromPercentageAndValue(Data.MaximumDistanceMultiplier);
            RotatingSpeedMultiplier = SetMultiplierFromPercentageAndValue(Data.RotatingSpeedMultiplier);
            BalanceLimitMultiplier = SetMultiplierFromPercentageAndValue(Data.BalanceLimitMultiplier);
            ProjectileSpeedMultiplier = SetMultiplierFromPercentageAndValue(Data.ProjectileSpeedMultiplier);
            
            //events
            if (_currentLevel < EventAtEachLevel.Count)
            {
                EventAtEachLevel[_currentLevel].Invoke();
            }
            
            //weapons
            for (int i = 0; i < Data.WeaponLevels.Count; i++)
            {
                if (Data.WeaponLevels[i].Level > _currentLevel)
                {
                    continue;
                }

                WheelButton button = CharacterManager.Instance.WeaponUIManagerProperty.Buttons.Find(x => x.ButtonController.Type == Data.WeaponLevels[i].Type);
                button.ButtonController.SetCanBeUnlocked(true);
            }
        }

        private float SetMultiplierFromPercentageAndValue(Value value)
        {
            float percentage = 0;
            switch (value.ValueType)
            {
                case Value.Type.Navigation:
                    percentage = _currentNavigationExperience / Data.NavigationGaugeMax;
                    break;
                case Value.Type.Combat:
                    percentage = _currentCombatExperience / Data.CombatGaugeMax;
                    break;
            }
            return 1 + value.MaxValue * value.IncreaseCurve.Evaluate(percentage);
        }
    }
}