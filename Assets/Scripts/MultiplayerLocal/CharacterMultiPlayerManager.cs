using Character;
using Kayak;
using UnityEngine;

public class CharacterMultiPlayerManager : MonoBehaviour
{
    public CharacterManager CharacterManager;
    public InputManagement InputManager;
    public KayakController Kayak;

    public bool InSharkZone = false;

    private int _points;
    public int Points => _points;

    private float _timerInTrigger = 0;
    public float TimerInTrigger => _timerInTrigger;

}
