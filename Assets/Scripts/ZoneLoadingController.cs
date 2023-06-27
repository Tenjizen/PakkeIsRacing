using System;
using System.Collections.Generic;
using GPEs;
using UnityEngine;

public class ZoneLoadingController : PlayerTriggerManager
{
    [SerializeField] private List<GameObject> _gameObjectsToSetInactive;
    [SerializeField] private List<GameObject> _gameObjectsToSetActive;

    protected override void Awake()
    {
        base.Awake();
        OnPlayerEntered.AddListener(SetObjects);
    }

    private void OnDisable()
    {
        OnPlayerEntered.RemoveListener(SetObjects);
    }

    private void SetObjects()
    {
        _gameObjectsToSetInactive.ForEach(x => x.SetActive(false));
        _gameObjectsToSetActive.ForEach(x => x.SetActive(true));
    }
}