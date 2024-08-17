using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _actions;
    private InputAction _move, _jump;

    private void Awake()
    {
        _actions = new PlayerInputActions();
        _move = _actions.Player.Move;
        _jump = _actions.Player.Jump;
    }

    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    public FrameInput Gather()
    {
        return new FrameInput
        {
            JumpDown = _jump.WasPressedThisFrame(),
            JumpHeld = _jump.IsPressed(),
            Move = _move.ReadValue<Vector2>()
        };
    }

    public struct FrameInput
    {
        public Vector2 Move;
        public bool JumpDown;
        public bool JumpHeld;
        public bool DashDown;
    }
}