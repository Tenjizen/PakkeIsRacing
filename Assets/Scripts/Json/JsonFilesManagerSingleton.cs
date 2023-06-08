using Collectible;
using Tools.SingletonClassBase;
using UI.Dialog;
using UnityEngine;

namespace Json
{
    public class JsonFilesManagerSingleton : Singleton<JsonFilesManagerSingleton>
    {
        [field:SerializeField]
        public CollectibleJsonFileManager CollectibleJsonFileManagerProperty { get; private set; }
        
        [field:SerializeField]
        public MemoriesJsonFileManager MemoriesJsonFileManagerProperty { get; private set; }
    }
}