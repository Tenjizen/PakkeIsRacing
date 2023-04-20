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
        public string ItemName;
        public bool IsCollected;
        public DialogCreator DialogCreatorGameObject;
    }

    public class DialogsJsonFileManager : MonoBehaviour
    {
        [field: SerializeField, Header("Dialogs")] 
        public List<CollectedDialogsData> CollectedDialogs { get; set; } = new List<CollectedDialogsData>();
        
        [SerializeField] private string _JsonFileID;
        
        private JsonFileManager<CollectedDialogsData> _fileManager;

        public UnityEvent OnNewDialogGet = new UnityEvent(); 

        protected void Awake()
        {
            _fileManager = new JsonFileManager<CollectedDialogsData>(_JsonFileID);
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

            CollectedDialogs.Find(item => item.DialogCreatorGameObject == dialogCreator).IsCollected = true;
            //CollectedDialogs.Add(new CollectedItemData(){CollectibleGameObject = collectible, IsCollected = true, ItemName = collectible.Data.Name});
            
            WriteJsonFile();
            
            OnNewDialogGet.Invoke();
        }
    }
}