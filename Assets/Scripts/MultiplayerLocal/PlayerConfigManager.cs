using MultiplayerLocal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigManager : MonoBehaviour
{
    public List<PlayerConfig> PlayersConfig;

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
            PlayersConfig = new List<PlayerConfig>();
        }
    }

    public void ReadyPlayer(int index)
    {
        PlayersConfig[index].IsReady = true;
        playerSpawn[index].gameObject.SetActive(false);
        if (PlayersConfig.Count > 0 && PlayersConfig.Count <= _maxPlayer
            && PlayersConfig.All(p => p.IsReady == true))
        {
            //var players = PlayersParent.GetComponentsInChildren<CharacterMultiPlayerManager>();
            //foreach (var player in players) 
            //{
            //    player.CharacterManager.SetCanMove(true);
            //}
            // _initlvl.gameObject.SetActive(false);
            GameManager.Instance.SharkPossessed.GetComponentInParent<SharkWithPathController>().StartRunning = true;
        }
    }

    public List<PlayerConfig> GetPlayerConfigs()
    {
        return PlayersConfig;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log(pi.playerIndex);

        if (!PlayersConfig.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            PlayersConfig.Add(new PlayerConfig(pi));

            CharacterMultiPlayerManager player = Instantiate(_playerPrefab, playerSpawn[pi.playerIndex].position, playerSpawn[pi.playerIndex].rotation, PlayersParent);
            MultipleTargetCamera.Targets.Add(player.Kayak.transform);
            player.GetComponentInChildren<IsInCameraView>().MultipleTargetCamera = MultipleTargetCamera;
            player.InputManager.InitPlayer(PlayersConfig[pi.playerIndex]);

            //playerSpawn[pi.playerIndex].transform.GetChild(1).gameObject.SetActive(false);
            playerPlaceHolder[pi.playerIndex].gameObject.SetActive(false);
            playerBtns[pi.playerIndex].gameObject.SetActive(true);
            player.ColorPlayerRef.InitColor(pi.playerIndex);

        }
    }


    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] List<GameObject> playerPlaceHolder;
    [SerializeField] List<GameObject> playerBtns;
    [SerializeField] CharacterMultiPlayerManager _playerPrefab;
    public MultipleTargetCamera MultipleTargetCamera;
    public Transform PlayersParent;
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
