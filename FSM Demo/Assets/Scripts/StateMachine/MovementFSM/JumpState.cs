public class JumpState : IMovementState
{
    public MovementStateType Type => MovementStateType.Jump;

    public void Enter()
    {
    }

    public void Tick()
    {
        var pc = PlayerController.Instance;

        pc.HandleMovement();
        pc.HandleJumpAndGravity();

        if (pc.VerticalVelocity <= 0f)
        {
            MovementFSM.Instance.ChangeState(MovementFSM.Instance.Fall);
        }
    }

    public void Exit()
    {
    }

    public bool CanTransition(IState nextState)
    {
        if (nextState is DashState)
            return true;

        if (nextState is IdleState || nextState is MoveState)
            return false;

        return !(nextState is JumpState);
    }
}