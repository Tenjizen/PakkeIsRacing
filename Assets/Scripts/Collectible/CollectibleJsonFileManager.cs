using System.Collections.Generic;
using System.Linq;
using Json;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.Events;

namespace Collectible
{
    [System.Serializable]
    public class CollectedItemData
    {
        public string ItemName;
        public bool IsCollected;
        public Collectible CollectibleGameObject;
    }

    public class CollectibleJsonFileManager : MonoBehaviour
    {
        [field:SerializeField, Header("Collectibles")] public List<CollectedItemData> CollectedItems { get; set; }
        [SerializeField] private bool _setCollectiblesFromJsonFileAtStart = false;
        [SerializeField] private string _JsonFileID;
        
        private JsonFileManager<CollectedItemData> _fileManager;

        public UnityEvent OnNewCollectibleGet = new UnityEvent(); 

        protected void Awake()
        {
            _fileManager = new JsonFileManager<CollectedItemData>(_JsonFileID);
        }

        private void Start()
        {
            if (_setCollectiblesFromJsonFileAtStart)
            {
                CollectedItems = _fileManager.GetDataList();
                foreach (CollectedItemData item in CollectedItems)
                {
                    item.CollectibleGameObject.SetCollectedAtStart();
                    item.IsCollected = true;
                }
            }
            
            WriteJsonFile();
        }

        private void WriteJsonFile()
        {
            _fileManager.ClearDataList();
            
            foreach (CollectedItemData item in CollectedItems)
            {
                _fileManager.AddToDataList(item);
            }

            _fileManager.SaveToJsonFile();
        }

        public void SetCollectibleCollected(Collectible collectible)
        {
            CollectedItems = _fileManager.GetDataList();

            CollectedItems.Find(item => item.CollectibleGameObject == collectible).IsCollected = true;
            //CollectedDialogs.Add(new CollectedItemData(){CollectibleGameObject = collectible, IsCollected = true, ItemName = collectible.Data.Name});
            
            WriteJsonFile();
            
            OnNewCollectibleGet.Invoke();
        }
    }
}