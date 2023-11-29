using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGaem : MonoBehaviour
{
    public GameObject Start;
    private void OnTriggerEnter(Collider other)
    {
        var shark = other.GetComponent<SharkWithPathController>();
        if (shark != null)
        {
            Start.SetActive(true);
            shark.Shpere.SetActive(true);

            var players = GameManager.Instance.PlayerConfigManagerRef.PlayersParent.GetComponentsInChildren<CharacterMultiPlayerManager>();
            foreach (var player in players)
            {
                player.CharacterManager.SetCanMove(true);
            }

            GameManager.Instance.PlayerConfigManagerRef.MultipleTargetCamera.SetFirstTarget();

        }
    }
}
