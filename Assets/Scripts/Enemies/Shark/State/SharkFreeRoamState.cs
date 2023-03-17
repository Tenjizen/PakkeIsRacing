using Kayak;
using UnityEngine;

public class SharkFreeRoamState : SharkBaseState
{
    public override void EnterState(SharkManager sharkManager) 
    {
        Debug.Log("free");
    }

    public override void UpdateState(SharkManager sharkManager) 
    {
        SwitchState(sharkManager);
    }

    public override void FixedUpdate(SharkManager sharkManager) 
    { 

    }

    public override void SwitchState(SharkManager sharkManager)
    {

    }
}
