using Character;
using System.Collections;
using System.Collections.Generic;
using MultiplayerLocal;
using UnityEngine;

public class InitLvl : MonoBehaviour
{
    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] CharacterMultiPlayerManager _playerPrefab;
    [SerializeField] MultipleTargetCamera _multipleTargetCamera ;

    public void Init()
    {
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            CharacterMultiPlayerManager player = Instantiate(_playerPrefab, playerSpawn[i].position, playerSpawn[i].rotation, gameObject.transform);
            _multipleTargetCamera.Targets.Add(player.Kayak.transform);
            player.GetComponentInChildren<IsInCameraView>().MultipleTargetCamera = _multipleTargetCamera;
            player.InputManager.InitPlayer(playerConfigs[i]);
        }
    }

}
