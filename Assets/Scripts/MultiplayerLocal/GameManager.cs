using System.Collections;
using System.Collections.Generic;
using Tools.SingletonClassBase;
using UI;
using UnityEngine;
using WaterAndFloating;
using MultiplayerLocal;
public class GameManager : Singleton<GameManager>
{
    [field: SerializeField] public Waves WavesRef { get; private set; }
    [field: SerializeField] public MultipleTargetCamera MultiTargetRef { get; private set; }


    [field: SerializeField] public float TimerInTriggerShark { get; private set; }
    [field: SerializeField] public int PointsWin { get; private set; }
    [field: SerializeField] public int MaxPointToUnlockButton { get; private set; }


    [SerializeField] public bool EnnemyPossessed = true;
    [field: SerializeField] public GameObject SharkPossessed { get; private set; }


    public Dictionary<Material, string> CurrentColorPlayer = new Dictionary<Material, string>();


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
