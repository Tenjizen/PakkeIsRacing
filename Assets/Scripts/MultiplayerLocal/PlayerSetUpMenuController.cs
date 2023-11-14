using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSetUpMenuController : MonoBehaviour
{

    private int PlayerIndex;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button readyButton;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled = false;

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        title.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }
    private void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetColor()
    {
        if (!inputEnabled) return;
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }
    public void ReadyPlayer()
    {
        if (!inputEnabled) return;
        PlayerConfigManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
    }

}
