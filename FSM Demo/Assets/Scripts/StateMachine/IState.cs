public interface IState
{
    void Enter();
    void Tick();
    void Exit();

    bool CanTransition(IState nextState);
}

public interface IMovementState : IState
{
    MovementStateType Type { get; }
}

public interface IActionState : IState
{
    ActionStateType Type { get; }
}

public interface IStatusState : IState
{
    StatusStateType Type { get; }
}