using Collectible;
using Tools.SingletonClassBase;
using UI.Dialog;
using UI.Quest;
using UnityEngine;

namespace Json
{
    public class JsonFilesManagerSingleton : Singleton<JsonFilesManagerSingleton>
    {
        [field:SerializeField]
        public CollectibleJsonFileManager CollectibleJsonFileManagerProperty { get; private set; }
        
        [field:SerializeField]
        public MemoriesJsonFileManager MemoriesJsonFileManagerProperty { get; private set; }
        
        [field:SerializeField]
        public QuestJsonFileManager QuestJsonFileManagerProperty { get; private set; }
    }
}