using System.Collections;
using System.Collections.Generic;
using Tools.SingletonClassBase;
using UI;
using UnityEngine;
using WaterAndFloating;

public class GameManager : Singleton<GameManager>
{
    //[field: SerializeField] public OptionMenuManager OptionMenuManager { get; private set; }
    //[field: SerializeField] public UIEnemyManager EnemyUIManager { get; private set; }
    [field: SerializeField] public Waves WavesRef { get; private set; }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
