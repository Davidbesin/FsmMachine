# Spark — Player Movement & Input System

Core player-side movement, input, animation, and FSM sThis covers input wiring, character movement/gravity, rotation, animator parameter sync, and the state interfaces the movement FSM is built on.

## Files

| File | Responsibility |
|---|---|
| `InputSystem_Actions.cs` / `.inputactions` | Generated wrapper + asset for Unity's new Input System. Defines the `Player` action map. |
| `PlayerController.cs` | Reads input, drives `CharacterController` movement, gravity, jump. Exposes movement state as public properties. |
| `PlayerRotation.cs` | Applies mouse/stick look input to yaw rotation. |
| `AnimationController.cs` | Singleton that pushes gameplay state into Animator parameters every frame. |
| `IState.cs` | FSM contracts: `IState` base + typed interfaces (`IMovementState`, `IActionState`, `IStatusState`). |
| `PlayerState.cs` | Enums backing the three FSM layers: `MovementStateType`, `ActionStateType`, `StatusStateType`. |

## Input System

`Player` action map (`InputSystem_Actions.inputactions`) defines: `Move`, `Look`, `Attack`, `Interact` (Hold), `Crouch`, `Jump`, `Previous`, `Next`, `Sprint`, `Dodge`.

Currently wired up in code: `Move`, `Look`, `Jump`, `Sprint`. `Attack`, `Interact`, `Crouch`, `Dodge`, `Previous`, `Next` exist in the action map but have no listeners yet 

Both `PlayerController` and `PlayerRotation` instantiate their own `InputSystem_Actions` and enable the `Player` map independently. Each subscribes/unsubscribes its own callbacks in `OnEnable`/`OnDisable` and disposes in `OnDestroy`.

## PlayerController

- Requires `CharacterController`.
- `ReadInput()` currently only accepts forward movement (`move.y` clamped to ≥0 via `Mathf.Max`)
- `HandleMovement()` computes camera-relative move direction and accelerates/decelerates `currentSpeed` toward `walkSpeed`/`runSpeed` using separate accel/decel rates via `MoveTowards`.
- `HandleJumpAndGravity()` handles grounded reset, jump impulse (`sqrt(jumpHeight * -2 * gravity)`), gravity integration, and the final `controller.Move` call.
- `MultiplySpeed()` is a public hook for external systems (dash, hit-stun, etc.) to scale `currentSpeed` without touching the accel/decel pipeline.
- `Update()` calls `MovementFSM.Instance.CurrentState.Tick()` — **`MovementFSM` is not part of this file set**; it's the state machine that's expected to call back into `HandleMovement()` / `HandleJumpAndGravity()` per active state.
- `jumpPressedThisFrame` is a one-frame flag, cleared at the end of every `Update()`, so anything reading it (gravity/jump logic, animation trigger) must do so before the next frame.

## PlayerRotation

Minimal yaw-only rotation: takes `Look.x`, multiplies by `sensitivity`, rotates around world up. 

## AnimationController

Singleton (`Instance`), caches `Animator` on `Awake`. Every `Update()` it pulls state from other singletons and pushes it to Animator params:

- `Speed` ← `PlayerController.Instance.CurrentSpeed`
- `Grounded` ← `PlayerController.Instance.IsGrounded`
- `FreeFall` ← `MovementFSM.Instance.CurrentStateType == MovementStateType.Fall`

`Jump()` is called externally (from `PlayerController.OnJumpPerformed`) to fire the `Jump` trigger. This is a pull-based sync pattern — animator has no awareness of *why* state changed, just polls current values every frame.

## FSM Contracts

`IState` is the shared base (`Enter`, `Tick`, `Exit`, `CanTransition`). Three typed layers run in parallel rather than one monolithic state machine:

- **Movement** (`IMovementState` / `MovementStateType`): `Idle`, `Move`, `Jump`, `Fall`, `Dash`
- **Action** (`IActionState` / `ActionStateType`): `None`, `Attack`, `ChargeAttack`, `Interact`
- **Status** (`IStatusState` / `StatusStateType`): `Normal`, `Hurt`, `Dead`

`CanTransition(IState nextState)` is defined per-state, so transition legality is decided by the state being exited, not a central table.

## Known gaps / not included here
- `Dash` exists as a `MovementStateType` and `Dodge` exists as an input action, but neither has an implementation here yet.
