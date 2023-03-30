using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameTools
{
    public class ObjectPool : MonoBehaviour
    {
        public List<GameObject> PooledObjects;
        public GameObject ObjectToPool;
        public int AmountToPool;

        void Start()
        {
            PooledObjects = new List<GameObject>();
            GameObject go;
            for(int i = 0; i < AmountToPool; i++)
            {
                go = Instantiate(ObjectToPool, transform, true);
                go.SetActive(false);
                PooledObjects.Add(go);
            }
        }
        
        public GameObject GetPooledObject()
        {
            for(int i = 0; i < AmountToPool; i++)
            {
                if(PooledObjects[i].activeInHierarchy == false)
                {
                    return PooledObjects[i];
                }
            }
            return null;
        }
    }
}