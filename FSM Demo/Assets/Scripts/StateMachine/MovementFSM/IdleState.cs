public class IdleState : IMovementState
{
    public MovementStateType Type => MovementStateType.Idle;

    public void Enter()
    {
        AnimationController.Instance.IdleAndWhatNot();
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

        if (pc.IsMoving)
        {
            MovementFSM.Instance.ChangeState(MovementFSM.Instance.Move);
        }
    }

    public void Exit()
    {
    }

    public bool CanTransition(IState nextState)
    {
        // Dash is a hard override - always allowed in, from any state.
        if (nextState is DashState)
            return true;

        return !(nextState is IdleState);
    }
}
