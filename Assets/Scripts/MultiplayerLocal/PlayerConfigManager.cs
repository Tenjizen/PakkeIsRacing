using MultiplayerLocal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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

    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] List<GameObject> playerPlaceHolder;
    [SerializeField] List<GameObject> playerBtns;
    [SerializeField] List<SpriteRenderer> PreviewColor;
    [SerializeField] List<Image> Jauge;
    [SerializeField] CharacterMultiPlayerManager _playerPrefab;
    public MultipleTargetCamera MultipleTargetCamera;
    public Transform PlayersParent;
    [SerializeField] float TimeFadeOut;
    [SerializeField] float TimeFadeIn;

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log(pi.playerIndex);

        if (!PlayersConfig.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            PlayersConfig.Add(new PlayerConfig(pi));

            CharacterMultiPlayerManager player = Instantiate(_playerPrefab, playerSpawn[pi.playerIndex].position, playerSpawn[pi.playerIndex].rotation, PlayersParent);
            //player.CharacterManager.SetCanMove(true);
            var _pos = new Vector3(playerPlaceHolder[pi.playerIndex].gameObject.transform.GetComponentInChildren<Transform>().position.x, playerPlaceHolder[pi.playerIndex].gameObject.GetComponentInChildren<Transform>().position.y, playerPlaceHolder[pi.playerIndex].gameObject.transform.GetComponentInChildren<Transform>().position.z+23.5f);
            //player.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //player.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            player.transform.DOMove(_pos, 4).OnComplete(() => {
                MultipleTargetCamera.Targets.Add(player.Kayak.transform);
                player.GetComponentInChildren<IsInCameraView>().MultipleTargetCamera = MultipleTargetCamera;
            });
            player.InputManager.InitPlayer(PlayersConfig[pi.playerIndex]);
            //Disparition
            playerPlaceHolder[pi.playerIndex].gameObject.transform.GetComponentInChildren<SpriteRenderer>().DOFade(0, TimeFadeOut).OnComplete(
                () => playerPlaceHolder[pi.playerIndex].gameObject.SetActive(false));
            playerPlaceHolder[pi.playerIndex].gameObject.transform.GetChild(1).gameObject.transform.GetComponentInChildren<SpriteRenderer>().DOFade(0, TimeFadeOut);
            playerPlaceHolder[pi.playerIndex].gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetComponent<TextMeshPro>().DOFade(0, TimeFadeOut);
            //Apparition
            playerBtns[pi.playerIndex].gameObject.SetActive(true);
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(0).gameObject.transform.GetComponent<TextMeshPro>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(0).gameObject.transform.GetComponent<TextMeshPro>().DOFade(1, TimeFadeOut));
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(1).gameObject.transform.GetComponent<TextMeshPro>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(1).gameObject.transform.GetComponent<TextMeshPro>().DOFade(1, TimeFadeOut));
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(2).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(2).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(1, TimeFadeIn));
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(3).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(3).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(1, TimeFadeIn));
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(4).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(4).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(1, TimeFadeIn));
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(5).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(0, 0).OnComplete(() =>
            playerBtns[pi.playerIndex].gameObject.transform.GetChild(5).gameObject.transform.GetComponent<SpriteRenderer>().DOFade(1, TimeFadeIn));
            player.PreviewColor = PreviewColor[pi.playerIndex];
            player.Jauge.color = Jauge[pi.playerIndex].color;
            player.ColorPlayerRef.InitColor(pi.playerIndex);
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
