using UnityEngine;

namespace SharkWithPath.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharkWithPathData", order = 1)]
    public class SharkWithPathData : ScriptableObject
    {
        [field: SerializeField, Header("Speed")]
        public float MovingValue { get; private set; } = 15;
        [field: SerializeField]
        public float SlowMovingValue { get; private set; } = 10;


        [field: SerializeField]
        public float RotationSpeedValue { get; private set; } = 3;


        [field: SerializeField]
        public float MaxDistBetweenSharkAndClosestPlayer { get; private set; } = 50f;


    }
}