using UnityEngine;

namespace Collectible.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CollectibleData", order = 1)]
    public class CollectibleData : ScriptableObject
    {
        public string ID;
        public string Name;
        [TextArea] public string Description;
        public Sprite Image;
    }
}