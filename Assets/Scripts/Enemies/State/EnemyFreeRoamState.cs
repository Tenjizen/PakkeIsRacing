using Kayak;
using UnityEngine;

public class EnemyFreeRoamState : EnemyBaseState
{
    public override void EnterState(EnemyManager enemyManager) 
    {
        Debug.Log("free");
    }

    public override void UpdateState(EnemyManager enemyManager) 
    {
        SwitchState(enemyManager);
    }

    public override void FixedUpdate(EnemyManager enemyManager) 
    { 

    }

    public override void SwitchState(EnemyManager enemyManager)
    {

    }
}
