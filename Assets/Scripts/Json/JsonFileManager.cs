using System.Collections.Generic;
using UnityEngine;

namespace Json
{
    [System.Serializable]
    public class JsonWrapper<T>
    {
        public List<T> DataList;

        public JsonWrapper()
        {
            DataList = new List<T>();
        }
    }
    
    [System.Serializable]
    public class JsonFileManager<T>
    {
        public JsonWrapper<T> Wrapper;
        private string _id;

        public JsonFileManager(string id)
        {
            Wrapper = new JsonWrapper<T>();
            _id = id;
        }

        public List<T> GetDataList()
        {
            string path = Application.streamingAssetsPath;
            string filePath = path + $"/{typeof(T)}{_id}.json";
            string json = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<JsonWrapper<T>>(json).DataList;
        }

        public void AddToDataList(T t)
        {
            Wrapper.DataList.Add(t);
        }

        public void SaveToJsonFile()
        {
            string path = Application.streamingAssetsPath;
            string json = JsonUtility.ToJson(Wrapper, true);
            string filePath = path + $"/{typeof(T)}{_id}.json";
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log("Data saved to: " + filePath);
        }

        public void ClearDataList()
        {
            Wrapper.DataList.Clear();
        }
    }
}