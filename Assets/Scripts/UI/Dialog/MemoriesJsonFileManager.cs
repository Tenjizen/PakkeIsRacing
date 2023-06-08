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
    public class CollectedDialogsData
    {
        [ReadOnly] public string CategoryID;
        [ReadOnly] public bool IsCollected;
        public DialogCreator DialogCreatorGameObject;
    }

    public class MemoriesJsonFileManager : MonoBehaviour
    {
        [field: SerializeField, Header("Memories")] 
        public List<CollectedDialogsData> CollectedDialogs { get; set; } = new List<CollectedDialogsData>();
        
        [SerializeField] private string _jsonFileID;
        
        private JsonFileManager<CollectedDialogsData> _fileManager;

        public UnityEvent OnNewDialogGet = new UnityEvent(); 
        

        protected void Awake()
        {
            _fileManager = new JsonFileManager<CollectedDialogsData>(_jsonFileID);
        }

        private void Start()
        {
            WriteJsonFile();
        }

        private void WriteJsonFile()
        {
            _fileManager.ClearDataList();
            
            foreach (CollectedDialogsData item in CollectedDialogs)
            {
                _fileManager.AddToDataList(item);
            }

            _fileManager.SaveToJsonFile();
        }

        public void SetDialogCollected(DialogCreator dialogCreator)
        {
            CollectedDialogs = _fileManager.GetDataList();

            CollectedDialogsData dialog = CollectedDialogs.Find(item => item.DialogCreatorGameObject == dialogCreator);
            if (dialog != null)
            {
                dialog.IsCollected = true;
                dialog.CategoryID = dialog.DialogCreatorGameObject.CategoryID;
            }
            
            WriteJsonFile();
            
            OnNewDialogGet.Invoke();
        }
    }
}