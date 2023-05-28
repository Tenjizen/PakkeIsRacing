using UnityEngine;

namespace Collectible.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CollectibleData", order = 1)]
    public class CollectibleData : ScriptableObject
    {
        public string ID;
        public string NameFR;
        public string NameEN;
        [TextArea] public string DescriptionFR;
        [TextArea] public string DescriptionEN;
        public Sprite Image;
    }
}