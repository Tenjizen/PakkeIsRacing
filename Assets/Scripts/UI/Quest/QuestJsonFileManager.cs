using System.Collections.Generic;
using Json;
using UI.Dialog;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Quest
{
    [System.Serializable]
    public class CollectedQuestData
    {
        [ReadOnly] public bool IsCollected;
        [ReadOnly] public bool IsDone;
        public QuestCreator QuestCreatorGameObject;
    }

    public class QuestJsonFileManager : MonoBehaviour
    {
        [field: SerializeField, Header("Quests")] 
        public List<CollectedQuestData> CollectedQuests { get; set; } = new List<CollectedQuestData>();
        
        [SerializeField] private string _jsonFileID;
        
        private JsonFileManager<CollectedQuestData> _fileManager;

        public UnityEvent OnNewQuestGet = new UnityEvent();

        protected void Awake()
        {
            _fileManager = new JsonFileManager<CollectedQuestData>(_jsonFileID);
        }

        private void Start()
        {
            WriteJsonFile();
        }

        private void WriteJsonFile()
        {
            _fileManager.ClearDataList();
            
            foreach (CollectedQuestData item in CollectedQuests)
            {
                _fileManager.AddToDataList(item);
            }

            _fileManager.SaveToJsonFile();
        }

        public void SetQuestCollected(QuestCreator questCreator, bool isDone)
        {
            CollectedQuests = _fileManager.GetDataList();

            CollectedQuestData quest = CollectedQuests.Find(item => item.QuestCreatorGameObject == questCreator);
            if (quest != null)
            {
                quest.IsCollected = true;
                quest.IsDone = isDone;
            }
            
            WriteJsonFile();
            
            OnNewQuestGet.Invoke();
        }
    }
}