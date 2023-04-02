using System.Collections.Generic;
using Fight;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : ScriptableObject
    {
        [FormerlySerializedAs("balanceLerpTo0Value")]
        [Header("Balance")]
        [SerializeField, Range(0, 1), Tooltip("The lerp value that reset the balance to 0 over time")]
        public float BalanceLerpTo0Value = 0.01f;
        [Range(0, 40), Tooltip("The limit over which the player will go in unbalance state")]
        public float BalanceLimit = 10f;
        [Range(0, 40), Tooltip("The limit over which the player will die")]
        public float BalanceDeathLimit = 15f;
        [Range(0, 40), Tooltip("The angle the player has to reach when unbalanced to get back balanced")]
        public float RebalanceAngle = 8f;
        [Range(0, 10), Tooltip("Minimum Time/Balance the player has to react when unbalanced")]
        public float MinimumTimeUnbalanced = 2f;
        [Range(0, 20), Tooltip("The multiplier applied to the paddle force to the balance")]
        public float RotationToBalanceMultiplier = 10f;
        [Range(0, 10), Tooltip("The multiplier to the floaters' level difference added to the balance value")]
        public float FloatersLevelDifferenceToBalanceMultiplier = 1f;
        
        [Header("Unbalanced")]
        [Tooltip("The number of times to press the button")]
        public int NumberPressButton;
        [Tooltip("Conversion of a 'balance' unit to time")]
        public float UnitBalanceToTimer;
        [Tooltip("The more balance you have the less time you have (formula: (unitbalance * balance) - ((balance - balanceLimit) * ReductionForce))")]
        public float ReductionForce;

        [Header("Weapon Mode")]
        [Range(0, 1f), Tooltip("The lerp applied to the boat following camera direction when aiming")]
        public float BoatFollowAimLerp = 0.05f;
        [field:SerializeField] public Projectile HarpoonPrefab { get; set; }
        [field:SerializeField] public Projectile NetPrefab { get; set; }
    }
}