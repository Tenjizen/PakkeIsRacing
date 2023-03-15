public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyManager camera);

    public abstract void UpdateState(EnemyManager camera);

    public abstract void FixedUpdate(EnemyManager camera);

    public abstract void SwitchState(EnemyManager camera);
}
