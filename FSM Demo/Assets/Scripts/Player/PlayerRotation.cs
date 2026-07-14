using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.2f;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (MovementFSM.Instance.CurrentStateType == MovementStateType.Dash)
        return;

        Vector2 look = context.ReadValue<Vector2>();

        transform.Rotate(Vector3.up, look.x * sensitivity, Space.World);
    }
}