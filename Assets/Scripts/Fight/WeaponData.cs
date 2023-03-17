using UnityEngine;

namespace Fight
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public string WeaponName;
        public float MaxSpeed;
        public AnimationCurve SpeedCurve;
        public float Cooldown;
    }
}