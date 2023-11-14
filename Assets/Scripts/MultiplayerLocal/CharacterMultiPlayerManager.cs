using Character;
using UnityEngine;

public class CharacterMultiPlayerManager : MonoBehaviour
{
    PlayerConfig _pc;
    //[HideInInspector] public PlayerController Mover;

    InputManagement _inputActions;


    public CharacterManager CharacterManager;












    void Awake()
    {
        //Mover = GetComponent<PlayerController>();
        _inputActions = new InputManagement();
    }

    public void InitPlayer(PlayerConfig pc)
    {
        _pc = pc;
    }

}
