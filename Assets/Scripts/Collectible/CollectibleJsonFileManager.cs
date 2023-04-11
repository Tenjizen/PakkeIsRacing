using System.Collections.Generic;
using System.Linq;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Collectible
{
    [System.Serializable]
    public class Wrapper
    {
        public List<CollectedItemData> DataList;

        public Wrapper()
        {
            DataList = new List<CollectedItemData>();
        }
    }
    
    [System.Serializable]
    public class CollectedItemData
    {
        public string ItemName;
        public bool IsCollected;
        public Collectible CollectibleGameObject;
    }

    public class CollectibleJsonFileManager : Singleton<CollectibleJsonFileManager>
    {
        [field:SerializeField, Header("Collectibles")] public List<CollectedItemData> CollectedItems { get; set; }

        [SerializeField] private bool SetCollectiblesFromJsonFileAtStart = false;
        
        private void Start()
        {
            if (SetCollectiblesFromJsonFileAtStart)
            {
                CollectedItems = GetCollectibles();
                foreach (CollectedItemData item in CollectedItems)
                {
                    item.CollectibleGameObject.SetCollectedAtStart();
                }
            }
            WriteJsonFile();
        }

        private void WriteJsonFile()
        {
            Wrapper wrapper = new Wrapper();
            foreach (CollectedItemData item in CollectedItems)
            {
                wrapper.DataList.Add(item);
            }

            string json = JsonUtility.ToJson(wrapper, true);
            string filePath = Application.dataPath + "/collectedItemsData.json";
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log("Collected Items Data saved to: " + filePath);
        }

        public void SetCollectibleCollected(Collectible collectible)
        {
            CollectedItems = GetCollectibles();
            for (int i = 0; i < CollectedItems.Count; i++)
            {
                if (CollectedItems[i].CollectibleGameObject == collectible)
                {
                    CollectedItems[i].IsCollected = true;
                }
            }
            WriteJsonFile();
        }

        private List<CollectedItemData> GetCollectibles()
        {
            string filePath = Application.dataPath + "/collectedItemsData.json";
            string json = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<Wrapper>(json).DataList;
        }
    }
}