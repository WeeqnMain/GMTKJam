using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats")]
public class PlayerStats : ScriptableObject
{
    // Setup
    [Header("Setup")]
    public LayerMask PlayerLayer;
    public LayerMask CollisionLayers;
    public CharacterSize CharacterSize;

    // Controller Setup
    [Header("Controller Setup"), Space]
    public float VerticalDeadZoneThreshold = 0.3f;
    public double HorizontalDeadZoneThreshold = 0.1f;
    [Tooltip("Velocity = smoother, but can be occasionally unreliable on jagged terrain. Immediate = Occasionally jittery, but stable")]
    public PositionCorrectionMode PositionCorrectionMode = PositionCorrectionMode.Velocity;

    // Movement
    [Header("Movement"), Space]
    public float BaseSpeed = 9;
    public float Acceleration = 50;
    public float Friction = 30;
    public float AirFrictionMultiplier = 0.5f;
    public float DirectionCorrectionMultiplier = 3f;
    public float MaxWalkableSlope = 50;
    public float RigidbodyMass;

    // Jump
    [Header("Jump"), Space]
    public float ExtraConstantGravity = 40;
    public float BufferedJumpTime = 0.15f;
    public float CoyoteTime = 0.15f;
    public float JumpPower = 20;
    public float EndJumpEarlyExtraForceMultiplier = 3;
    public int MaxAirJumps = 1;

    //Glide
    [Header("Glide"), Space]
    public float MaxVelocityOnFalling;

    // Walls
    [Header("Walls"), Space]
    public bool AllowWalls;
    public LayerMask ClimbableLayer;
    public float WallJumpTotalInputLossTime = 0.2f;
    public float WallJumpInputLossReturnTime = 0.5f;
    public bool RequireInputPush;
    public Vector2 WallJumpPower = new(25, 15);
    public Vector2 WallPushPower = new(15, 10);
    public float WallClimbSpeed = 5;
    public float WallFallAcceleration = 20;
    public float WallPopForce = 10;
    public float WallCoyoteTime = 0.3f;
    public float WallDetectorRange = 0.1f;

    private void OnValidate()
    {
        var potentialPlayer = FindObjectsOfType<PlayerController>();
        foreach (var player in potentialPlayer)
        {
            player.OnValidate();
        }
    }
}

[Serializable]
public class CharacterSize
{
    public const float STEP_BUFFER = 0.05f;
    public const float COLLIDER_EDGE_RADIUS = 0.05f;

    [Range(0.1f, 10), Tooltip("How tall you are. This includes a collider and your step height.")]
    public float Height = 1.8f;

    [Range(0.1f, 10), Tooltip("The width of your collider")]
    public float Width = 0.6f;

    [Range(STEP_BUFFER, 15), Tooltip("Step height allows you to step over rough terrain like steps and rocks.")]
    public float StepHeight = 0.5f;

    [Range(0.01f, 0.2f), Tooltip("The outer buffer distance of the grounder rays. Reducing this too much can cause problems on slopes, too big and you can get stuck on the sides of drops.")]
    public float RayInset = 0.1f;

    public GeneratedCharacterSize GenerateCharacterSize()
    {
        ValidateHeights();

        var s = new GeneratedCharacterSize
        {
            Height = Height,
            Width = Width,
            StepHeight = StepHeight,
            RayInset = RayInset
        };

        s.StandingColliderSize = new Vector2(s.Width - COLLIDER_EDGE_RADIUS * 2, s.Height - s.StepHeight - COLLIDER_EDGE_RADIUS * 2);
        s.StandingColliderCenter = new Vector2(0, s.Height - s.StandingColliderSize.y / 2 - COLLIDER_EDGE_RADIUS);

        return s;
    }

    private static double _lastDebugLogTime;
    private const double TIME_BETWEEN_LOGS = 1f;

    private void ValidateHeights()
    {
#if UNITY_EDITOR
        var maxStepHeight = Height - STEP_BUFFER;
        if (StepHeight > maxStepHeight)
        {
            StepHeight = maxStepHeight;
            Log("Step height cannot be larger than height");
        }

        var minCrouchHeight = StepHeight + STEP_BUFFER;

        void Log(string text)
        {
            var time = EditorApplication.timeSinceStartup;
            if (_lastDebugLogTime + TIME_BETWEEN_LOGS > time) return;
            _lastDebugLogTime = time;
            Debug.LogWarning(text);
        }
#endif
    }
}

public struct GeneratedCharacterSize
{
    public float Height;
    public float Width;
    public float StepHeight;
    public float RayInset;
    public Vector2 StandingColliderSize;
    public Vector2 StandingColliderCenter;
}

[Serializable]
public enum PositionCorrectionMode
{
    Velocity,
    Immediate
}