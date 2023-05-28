using System;
using Character;
using Collectible.Data;
using UnityEngine;

namespace UI.Menu
{
    public class CollectibleUIObject : MenuUIObject
    {
        public CollectibleData Data;
        public GameObject CollectibleMesh;

        private void Awake()
        {
            if (CollectibleMesh != null)
            {
                CollectibleMesh.SetActive(false);
            }
        }

        public override void Set(bool isActive)
        {
            base.Set(isActive);
            IconImage.sprite = Data == null ? IconImage.sprite : Data.Image;
        }

        public override string GetName()
        {
            bool isInEnglish = CharacterManager.Instance.Parameters.Language;
            return Data == null ? 
                isInEnglish ? "Mysterious collectible" : "Collectible mystère" : 
                isInEnglish ? Data.NameEN : Data.NameFR;
        }
        
        public override string GetDescription()
        {
            bool isInEnglish = CharacterManager.Instance.Parameters.Language;
            return Data == null ? 
                isInEnglish ? "Collectible not found yet" : "Collectible pas encore trouvé" : 
                isInEnglish ? Data.DescriptionEN : Data.DescriptionFR;
        }
    }
}