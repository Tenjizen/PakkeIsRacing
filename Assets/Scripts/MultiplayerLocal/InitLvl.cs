using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLvl : MonoBehaviour
{
    [SerializeField] List<Transform> playerSpawn;
    [SerializeField] GameObject _playerPrefab;


    public void Init()
    {
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigs().ToArray();
        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(_playerPrefab, playerSpawn[i].position, playerSpawn[i].rotation, gameObject.transform);

            //player.GetComponent<CharacterMultiPlayerManager>();
            player.GetComponent<CharacterMultiPlayerManager>().InputManager.InitPlayer(playerConfigs[i]);
            //player.GetComponent<PlayerController>().SetTeam(i % 2);
            //player.GetComponent<PlayerController>().ID = i;
            //playerSpawn.Remove(playerSpawn[i]);
            //GameCore.Instance.Players.Add(player.GetComponent<PlayerInputHandler>());
        }
    }

}
