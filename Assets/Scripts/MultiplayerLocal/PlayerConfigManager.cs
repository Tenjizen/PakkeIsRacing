using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigManager : MonoBehaviour
{
    private List<PlayerConfig> playersConfig;
    [SerializeField] InitLvl _initlvl;
    [SerializeField] GameObject _playerPanel;
    [SerializeField] int _maxPlayer = 4;
    public static PlayerConfigManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Instance not null");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            playersConfig = new List<PlayerConfig>();
        }
    }

    public void ReadyPlayer(int index)
    {
        playersConfig[index].IsReady = true;
        if (playersConfig.Count > 0 && playersConfig.Count <= _maxPlayer
            && playersConfig.All(p => p.IsReady == true))
        {
            _playerPanel.SetActive(false);
            _initlvl.Init();
        }
    }

    public List<PlayerConfig> GetPlayerConfigs()
    {
        return playersConfig;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log(pi.playerIndex);

        if (!playersConfig.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playersConfig.Add(new PlayerConfig(pi));
        }
    }

}
public class PlayerConfig
{
    public PlayerConfig(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }

}
