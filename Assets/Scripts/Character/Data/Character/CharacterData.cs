using Fight;
using UnityEngine;

namespace Character.Data.Character
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : ScriptableObject
    {
        [field:SerializeField, Header("Balance"), Range(0, 1), Tooltip("The lerp value that reset the balance to 0 over time")] 
        public float BalanceLerpTo0Value { get; private set; } = 0.1f;
        
        
        [field:SerializeField, Range(0, 40), Tooltip("The limit over which the player will go in unbalance state")]
        public float BalanceLimit { get; private set; } = 10f;
        
        
        [field:SerializeField, Range(0, 40), Tooltip("The limit over which the player will die")]
        public float BalanceDeathLimit { get; private set; } = 15f;
        
        
        [field:SerializeField, Range(0, 40), Tooltip("The angle the player has to reach when unbalanced to get back balanced")]
        public float RebalanceAngle { get; private set; } = 8f;
        
        
        [field:SerializeField, Range(0, 10), Tooltip("Minimum Time/Balance the player has to react when unbalanced")]
        public float MinimumTimeUnbalanced { get; private set; } = 2f;
        
        
        [field:SerializeField, Range(0, 20), Tooltip("The multiplier applied to the paddle force to the balance")]
        public float RotationToBalanceMultiplier { get; private set; } = 10f;
        
        
        [field:SerializeField, Range(0, 10), Tooltip("The multiplier to the floaters' level difference added to the balance value")]
        public float FloatersLevelDifferenceToBalanceMultiplier { get; private set; } = 1f;
        
        [field:SerializeField, Range(0, 10)]
        public float InvincibleTimeAfterUnbalance { get; private set; } = 3f;
        
        
        [field:SerializeField, Header("Unbalanced"), Tooltip("The number of times to press the button")]
        public int NumberPressButton { get; private set; }
        
        
        [field:SerializeField, Range(0.25f,4f),Tooltip("Conversion of a 'balance' unit to time")]
        public float UnitBalanceToTimer { get; private set; }
        
        
        [field:SerializeField, Tooltip("The more balance you have the less time you have (formula: (unitbalance * balance) - ((balance - balanceLimit) * ReductionForce))")]
        public float ReductionForce { get; private set; }

        
        [field:SerializeField, Header("Weapon Mode"), Range(0, 1f), Tooltip("The lerp applied to the boat following camera direction when aiming")]
        public float BoatFollowAimLerp { get; private set; } = 0.05f;
        
        
        [field:SerializeField] 
        public Projectile HarpoonPrefab { get; private set; }
        
        
        [field:SerializeField] 
        public Projectile NetPrefab { get; private set; }
        
        
        [field:SerializeField, Header("Auto-Aim"), Tooltip("The width of the circle in within the auto aim detect an enemy")]
        public float AutoAimSize { get; private set; }
        
        [field:SerializeField, Tooltip("The time needed in the inner circle of auto-aim to activate auto-aim at launch")]
        public float TimeToAutoAim { get; private set; }
        
        [field:SerializeField]
        public int AutoAimNumberOfCastStep { get; private set; }
        
        [field:SerializeField]
        public float AutoAimDistanceToSweepEachStep { get; private set; }
        
        [field:SerializeField]
        public float AutoAimDistanceBetweenEachStep { get; private set; }
    }
}