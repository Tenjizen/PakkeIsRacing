using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    


    void Awake()
    {
        //Mover = GetComponent<PlayerController>();


    }

    public void InitPlayer(PlayerConfig pc)
    {
        //_pc = pc;


        //Mover.PlayerConfig = _pc;


        //_pc.Input.onActionTriggered += Input_onActionTriggered;
    }
    void Input_onActionTriggered(CallbackContext obj)
    {
        //if (obj.action.name == _gameplayInputs.)
        //{

        //}
    }
}
