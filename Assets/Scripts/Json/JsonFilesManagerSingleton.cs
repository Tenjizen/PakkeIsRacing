using Collectible;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Json
{
    public class JsonFilesManagerSingleton : Singleton<JsonFilesManagerSingleton>
    {
        [field:SerializeField]
        public CollectibleJsonFileManager CollectibleJsonFileManagerProperty { get; private set; }
    }
}