public class FallState : IMovementState
{
    public MovementStateType Type => MovementStateType.Fall;

    public void Enter()
    {
    }

    public void Tick()
    {
        var pc = PlayerController.Instance;

        pc.HandleMovement();
        pc.HandleJumpAndGravity();

        if (pc.IsGrounded)
        {
            if (pc.IsMoving)
                MovementFSM.Instance.ChangeState(MovementFSM.Instance.Move);
            else
                MovementFSM.Instance.ChangeState(MovementFSM.Instance.Idle);
        }
    }

    public void Exit()
    {
        PlayerController.Instance.MultiplySpeed(0.5f);
        AnimationController.Instance.Land();
    }

    public bool CanTransition(IState nextState)
    {
        if (nextState is DashState)
            return true;

        if (nextState is JumpState)
            return false;

        return !(nextState is FallState);
    }
}