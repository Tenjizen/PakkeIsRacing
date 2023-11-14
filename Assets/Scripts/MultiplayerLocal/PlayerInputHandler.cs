using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerConfig _pc;
    //[HideInInspector] public PlayerController Mover;

    InputManagement _inputActions;
    void Awake()
    {
        //Mover = GetComponent<PlayerController>();
        _inputActions = new InputManagement();
    }

    public void InitPlayer(PlayerConfig pc)
    {
        _pc = pc;
        //_pc.Input.onActionTriggered += Input_onActionTriggered;
        //Mover.PlayerConfig = _pc;
        //GameCore.Instance.Players.Add(this);
    }

}
