using MultiplayerLocal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigManager : MonoBehaviour
{
    private List<PlayerConfig> playersConfig;
    [SerializeField] InitLvl _initlvl;

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
        playerSpawn[index].gameObject.SetActive(false);
        if (playersConfig.Count > 0 && playersConfig.Count <= _maxPlayer
            && playersConfig.All(p => p.IsReady == true))
        {
            var players = _playersParent.GetComponentsInChildren<CharacterMultiPlayerManager>();
            foreach (var player in players) 
            {
                player.CharacterManager.SetCanMove(true);
            }
            _initlvl.gameObject.SetActive(false);
            GameManager.Instance.SharkPossessed.GetComponentInParent<SharkWithPathController>().StartRunning = true;
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

            CharacterMultiPlayerManager player = Instantiate(_playerPrefab, playerSpawn[pi.playerIndex].position, playerSpawn[pi.playerIndex].rotation, _playersParent);
            _multipleTargetCamera.Targets.Add(player.Kayak.transform);
            player.GetComponentInChildren<IsInCameraView>().MultipleTargetCamera = _multipleTargetCamera;
            player.InputManager.InitPlayer(playersConfig[pi.playerIndex]);

            playerSpawn[pi.playerIndex].transform.GetChild(0).gameObject.SetActive(false);
            player.ColorPlayer.InitColor(pi.playerIndex);

        }
    }


    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] CharacterMultiPlayerManager _playerPrefab;
    [SerializeField] MultipleTargetCamera _multipleTargetCamera;
    [SerializeField] Transform _playersParent;
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
