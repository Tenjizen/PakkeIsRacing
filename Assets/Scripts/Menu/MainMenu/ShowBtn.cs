using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowBtn : MonoBehaviour
{
    [SerializeField] GameObject _content;

    public GameObject ButtonGameObject;

    public void Update()
    {
        // Compare selected gameObject with referenced Button gameObject
        if (EventSystem.current.currentSelectedGameObject == ButtonGameObject)
        {
            _content.SetActive(true);
        }
        else
        {
            _content.SetActive(false);
        }
    }
}
