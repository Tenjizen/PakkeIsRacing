using Enemies.Shark;

public abstract class SharkBaseState
{
    public abstract void EnterState(SharkManager sharkManager);

    public abstract void UpdateState(SharkManager sharkManager);

    public abstract void FixedUpdate(SharkManager sharkManager);

    public abstract void SwitchState(SharkManager sharkManager);
}
