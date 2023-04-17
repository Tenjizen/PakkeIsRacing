using Collectible.Data;
using UnityEngine;

namespace UI.Menu
{
    public class CollectibleUIObject : MenuUIObject
    {
        public CollectibleData Data;

        public override void Set(bool isActive)
        {
            base.Set(isActive);
            IconImage.sprite = Data == null ? IconImage.sprite : Data.Image;
        }

        public override string GetName()
        {
            return Data == null ? "Mysterious collectible" : Data.Name;
        }
        
        public override string GetDescription()
        {
            return Data == null ? "Collectible not found yet." : Data.Description;
        }
    }
}