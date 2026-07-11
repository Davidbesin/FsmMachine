public class DashState : IMovementState
{
    public MovementStateType Type => MovementStateType.Dash;

    // TODO: PlayerController has no dash mechanic yet - no input action

    public void Enter()
    {
    }

    public void Tick()
    {
    }

    public void Exit()
    {
    }

    public bool CanTransition(IState nextState)
    {
        // No re-dashing straight out of a dash.
        if (nextState is DashState)
            return false;

        var pc = PlayerController.Instance;

        // Airborne when the dash ends - only Fall makes sense.
        if (nextState is FallState)
            return !pc.IsGrounded;

        // Idle / Move / Jump all require having landed.
        // (Idle = no input, Move = input held, Jump = grounded + jump input,
        // all handled the same way the other states already decide it.)
        return pc.IsGrounded;
    }
}