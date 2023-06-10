using System;
using System.Collections.Generic;
using Collectible;
using Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UI.Dialog
{
    [System.Serializable]
    public class CollectedMemoriesData
    {
        [ReadOnly] public string CategoryID;
        [ReadOnly] public bool IsCollected;
        public DialogCreator DialogCreatorGameObject;
    }

    public class MemoriesJsonFileManager : MonoBehaviour
    {
        [field: SerializeField, Header("Memories")] 
        public List<CollectedMemoriesData> CollectedDialogs { get; set; } = new List<CollectedMemoriesData>();
        
        [SerializeField] private string _jsonFileID;
        
        private JsonFileManager<CollectedMemoriesData> _fileManager;

        public UnityEvent OnNewDialogGet = new UnityEvent(); 
        

        protected void Awake()
        {
            _fileManager = new JsonFileManager<CollectedMemoriesData>(_jsonFileID);
        }

        private void Start()
        {
            WriteJsonFile();
        }

        private void WriteJsonFile()
        {
            _fileManager.ClearDataList();
            
            foreach (CollectedMemoriesData item in CollectedDialogs)
            {
                _fileManager.AddToDataList(item);
            }

            _fileManager.SaveToJsonFile();
        }

        public void SetDialogCollected(DialogCreator dialogCreator)
        {
            CollectedDialogs = _fileManager.GetDataList();

            CollectedMemoriesData memory = CollectedDialogs.Find(item => item.DialogCreatorGameObject == dialogCreator);
            if (memory != null)
            {
                memory.IsCollected = true;
                memory.CategoryID = memory.DialogCreatorGameObject.CategoryID;
            }
            
            WriteJsonFile();
            
            OnNewDialogGet.Invoke();
        }
    }
}