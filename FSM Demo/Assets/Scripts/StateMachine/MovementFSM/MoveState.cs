public class MoveState : IMovementState
{
    public MovementStateType Type => MovementStateType.Move;

    public void Enter()
    {
    }

    public void Tick()
    {
        var pc = PlayerController.Instance;

        pc.HandleMovement();
        pc.HandleJumpAndGravity();

        if (!pc.IsGrounded)
        {
            if (pc.VerticalVelocity > 0f)
                MovementFSM.Instance.ChangeState(MovementFSM.Instance.Jump);
            else
                MovementFSM.Instance.ChangeState(MovementFSM.Instance.Fall);
            return;
        }

        if (!pc.IsMoving)
        {
            MovementFSM.Instance.ChangeState(MovementFSM.Instance.Idle);
        }
    }

    public void Exit()
    {
    }

    public bool CanTransition(IState nextState)
    {
        if (nextState is DashState)
            return true;

        return !(nextState is MoveState);
    }
}