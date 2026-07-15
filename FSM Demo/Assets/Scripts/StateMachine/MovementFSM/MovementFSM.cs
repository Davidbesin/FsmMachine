using UnityEngine;
using System;
using TMPro;

public class MovementFSM : MonoBehaviour
{
    public static MovementFSM Instance { get; private set; }

    public IMovementState CurrentState { get; private set; }

    [Header("Debug")]
    public MovementStateType CurrentStateType;
    public TextMeshProUGUI stateText;

    // Cached state instances - created once, reused for every transition
    // instead of "new"-ing a fresh object each time we swap states.
    public IdleState Idle { get; private set; }
    public MoveState Move { get; private set; }
    public JumpState Jump { get; private set; }
    public FallState Fall { get; private set; }
    public DashState Dash { get; private set; }

    public event Action<MovementStateType> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Idle = new IdleState();
        Move = new MoveState();
        Jump = new JumpState();
        Fall = new FallState();
        Dash = new DashState();

        ChangeState(Idle);
    }

    public bool ChangeState(IMovementState nextState)
    {
        if (nextState == null)
            return false;

        if (CurrentState != null &&
            !CurrentState.CanTransition(nextState))
            return false;

        CurrentState?.Exit();

        CurrentState = nextState;

        CurrentState.Enter();

        CurrentStateType = CurrentState.Type;

        OnStateChanged?.Invoke(CurrentStateType);

        if (stateText != null)
            stateText.text = CurrentStateType.ToString();

        return true;
    }
}