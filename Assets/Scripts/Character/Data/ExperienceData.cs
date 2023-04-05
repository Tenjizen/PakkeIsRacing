using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExperienceData", order = 1)]
    public class ExperienceData : ScriptableObject
    {
        [Header("Experience gained")]
        public float ExperienceGainedAtCheckpoint;
        public float ExperienceGainedAtColletible;
        public float ExperienceGainedAtEnemy;

        [Header("Experience levels")] 
        public List<Level> Levels;
        
        [Header("Combat & Navigation")]
        public float CombatGaugeMax;
        public float NavigationGaugeMax;
        
        [Header("Character Values multipliers")]
        //navigation
        public Value BreakingDistanceMultiplier;
        public Value MaximumDistanceMultiplier;
        public Value RotatingSpeedMultiplier;
        public Value BalanceLimitMultiplier;
        //combat
        public Value ProjectileSpeedMultiplier;
    }

    [Serializable]
    public struct Level
    {
        public float ExperienceNeededToComplete;
        public int CombatExperienceGained;
        public int NavigationExperienceGained;
    }

    [Serializable]
    public struct Value
    {
        public enum Type
        {
            Navigation = 0,
            Combat = 1
        }

        public Type ValueType;
        public float MaxValue;
        public AnimationCurve IncreaseCurve;
    }
}