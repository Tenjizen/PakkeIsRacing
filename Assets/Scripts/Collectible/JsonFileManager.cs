using System.Collections.Generic;
using UnityEngine;

namespace Collectible
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

        public JsonFileManager()
        {
            Wrapper = new JsonWrapper<T>();
        }

        public List<T> GetDataList()
        {
            string filePath = Application.dataPath + $"/{typeof(T)}.json";
            string json = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<JsonWrapper<T>>(json).DataList;
        }

        public void AddToDataList(T t)
        {
            Wrapper.DataList.Add(t);
        }

        public void SaveToJsonFile()
        {
            string json = JsonUtility.ToJson(Wrapper, true);
            string filePath = Application.dataPath + $"/{typeof(T)}.json";
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log("Data saved to: " + filePath);
        }

        public void ClearDataList()
        {
            Wrapper.DataList.Clear();
        }
    }
}