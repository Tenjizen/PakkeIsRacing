using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExperienceData", order = 1)]
    public class ExperienceData : ScriptableObject
    {
        [field:SerializeField, Header("Experience gained")]
        public float ExperienceGainedAtCheckpoint { get; private set; } 
        
        
        [field:SerializeField]
        public float ExperienceGainedAtColletible { get; private set; } 
        
        
        [field:SerializeField]
        public float ExperienceGainedAtEnemy { get; private set; } 

        
        [field:SerializeField, Header("Experience levels")] 
        public List<Level> Levels { get; private set; } 
        
        
        [field:SerializeField, Header("Combat & Navigation")]
        public float CombatGaugeMax { get; private set; } 
        
        
        [field:SerializeField]
        public float NavigationGaugeMax { get; private set; } 
        
        
        [field:SerializeField, Header("Character Values multipliers")]
        public Value BreakingDistanceMultiplier { get; private set; } 
        
        
        [field:SerializeField]
        public Value MaximumDistanceMultiplier { get; private set; } 
        
        
        [field:SerializeField]
        public Value RotatingSpeedMultiplier { get; private set; } 
        
        
        [field:SerializeField]
        public Value BalanceLimitMultiplier { get; private set; } 


        [field:SerializeField]
        public Value ProjectileSpeedMultiplier { get; private set; } 
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