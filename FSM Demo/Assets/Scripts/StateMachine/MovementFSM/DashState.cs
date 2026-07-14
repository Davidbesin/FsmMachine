using UnityEngine;

public class DashState : IMovementState
{
    [SerializeField] private float dashSpeed = 12f;

    public MovementStateType Type => MovementStateType.Dash;

    public void Enter()
    {
        AnimationController.Instance.Slide();
    }

    public void Tick()
    {
        PlayerController.Instance.Dash(dashSpeed);
        
        PlayerController.Instance.HandleJumpAndGravity();

        var animator = AnimationController.Instance.Animator;
        var state = AnimationController.Instance.CurrentStateInfo;

        bool inDash = state.IsName("Dash");
        bool exitingIntoDash = animator.IsInTransition(0) &&
                                animator.GetNextAnimatorStateInfo(0).IsName("Dash");

        if (inDash && state.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            MovementFSM.Instance.ChangeState(
                PlayerController.Instance.IsMoving ? MovementFSM.Instance.Move : MovementFSM.Instance.Idle);
        }
    }

    public void Exit()
    {
        
    }

    public bool CanTransition(IState nextState)
    {
        if (nextState is DashState)
            return false;

        var pc = PlayerController.Instance;

        if (nextState is FallState)
            return !pc.IsGrounded;

        return pc.IsGrounded;
    }
}