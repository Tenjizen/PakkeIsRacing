using UnityEngine;

namespace SharkWithPath.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharkWithPathData", order = 1)]
    public class SharkWithPathData : ScriptableObject
    {
        [field: SerializeField, Header("Speed"), Range(0, 0.1f)]
        public float MovingValue { get; private set; } = 0.05f;

        [field: SerializeField, Range(0, 1f)]
        public float SpeedLerpToMovingValue { get; private set; } = 0.1f;
    }
}