using System;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    #region References

    [SerializeField] private bool _drawGizmos = true;

    private BoxCollider2D _collider;
    private CapsuleCollider2D _airborneCollider;
    private ConstantForce2D _constantForce;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;

    #endregion

    #region Interface

    [field: SerializeField] public PlayerStats Stats { get; private set; }

    public event Action<JumpType> Jumped;
    public event Action Landed;
    public event Action GlidingBegun;
    public event Action GlidingEnded;
    public event Action<bool, float> GroundedChanged;
    public event Action<bool> WallGrabChanged;
    public event Action<Vector2> Repositioned;
    public event Action<bool> ToggledPlayer;

    public bool Active { get; private set; } = true;
    public Vector2 GroundNormal { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsGliding { get; private set; }
    public Vector2 Velocity { get; private set; }
    public int WallDirection { get; private set; }

    public void AddFrameForce(Vector2 force, bool resetVelocity = false)
    {
        if (resetVelocity) SetVelocity(Vector2.zero);
        _forceToApplyThisFrame += force;
    }

    public void RepositionImmediately(Vector2 position, bool resetVelocity = false)
    {
        _rigidbody.position = position;
        if (resetVelocity) SetVelocity(Vector2.zero);
        Repositioned?.Invoke(position);
    }

    public void TogglePlayer(bool on)
    {
        Active = on;

        _rigidbody.isKinematic = !on;
        ToggledPlayer?.Invoke(on);
    }

    #endregion

    #region Loop

    private void Awake()
    {
        SetupCharacter();
    }

    public void OnValidate() => SetupCharacter();

    public void Update()
    {
        GatherInput();
    }

    public void FixedUpdate()
    {
        if (!Active) return;

        RemoveTransientVelocity();

        SetFrameData();

        CalculateCollisions();
        CalculateDirection();

        CalculateWalls();
        CalculateJump();

        TraceGround();
        Move();
        Glide();

        CleanFrameData();
    }

    #endregion

    #region Setup

    private bool _cachedQueryMode;
    private GeneratedCharacterSize _character;
    private const float GRAVITY_SCALE = 1;

    private void SetupCharacter()
    {
        _playerInput = GetComponent<PlayerInput>();
        _constantForce = GetComponent<ConstantForce2D>();

        _character = Stats.CharacterSize.GenerateCharacterSize();
        _cachedQueryMode = Physics2D.queriesStartInColliders;

        _wallDetectionBounds = new Bounds(
            new Vector3(0, _character.Height / 2),
            new Vector3(_character.StandingColliderSize.x + CharacterSize.COLLIDER_EDGE_RADIUS * 2 + Stats.WallDetectorRange, _character.Height - 0.1f));

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.mass = Stats.RigidbodyMass;

        // Primary collider
        _collider = GetComponent<BoxCollider2D>();
        _collider.edgeRadius = CharacterSize.COLLIDER_EDGE_RADIUS;
        _collider.sharedMaterial = _rigidbody.sharedMaterial;
        _collider.enabled = true;

        // Airborne collider
        _airborneCollider = GetComponent<CapsuleCollider2D>();
        _airborneCollider.size = new Vector2(_character.Width - SKIN_WIDTH * 2, _character.Height - SKIN_WIDTH * 2);
        _airborneCollider.offset = new Vector2(0, _character.Height / 2);
        _airborneCollider.sharedMaterial = _rigidbody.sharedMaterial;

        _playerInput.hideFlags = HideFlags.NotEditable;
        _constantForce.hideFlags = HideFlags.NotEditable;
        _collider.hideFlags = HideFlags.NotEditable;
        _airborneCollider.hideFlags = HideFlags.NotEditable;
        _rigidbody.hideFlags = HideFlags.NotEditable;

        SetColliderMode(ColliderMode.Airborne);
    }

    #endregion

    #region Input

    private PlayerInput.FrameInput _frameInput;

    private void GatherInput()
    {
        _frameInput = _playerInput.Gather();

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = Time.time;
        }

        bool lastGlidingState = IsGliding;
        if (_frameInput.JumpDown && !IsGrounded)
        {
            IsGliding = true;
        }
        if (_frameInput.JumpReleased || IsGrounded)
        {
            IsGliding = false;
        }

        if (lastGlidingState != IsGliding)
        {
            if (IsGliding) GlidingBegun?.Invoke();
            else GlidingEnded?.Invoke();
        } 
    }

    #endregion

    #region Frame Data

    private bool _hasInputThisFrame;
    private Vector2 _trimmedFrameVelocity;
    private Vector2 _framePosition;
    private Bounds _wallDetectionBounds;

    private void SetFrameData()
    {
        _framePosition = _rigidbody.position;

        _hasInputThisFrame = _frameInput.Move.x != 0;

        Velocity = _rigidbody.velocity;
        _trimmedFrameVelocity = new Vector2(Velocity.x, 0);
    }

    private void RemoveTransientVelocity()
    {
        var currentVelocity = _rigidbody.velocity;
        var velocityBeforeReduction = currentVelocity;

        currentVelocity -= _totalTransientVelocityAppliedLastFrame;
        SetVelocity(currentVelocity);

        _frameTransientVelocity = Vector2.zero;
        _totalTransientVelocityAppliedLastFrame = Vector2.zero;

        var decay = Stats.Friction * Stats.AirFrictionMultiplier;
        if ((velocityBeforeReduction.x < 0 && _decayingTransientVelocity.x < velocityBeforeReduction.x) ||
            (velocityBeforeReduction.x > 0 && _decayingTransientVelocity.x > velocityBeforeReduction.x) ||
            (velocityBeforeReduction.y < 0 && _decayingTransientVelocity.y < velocityBeforeReduction.y) ||
            (velocityBeforeReduction.y > 0 && _decayingTransientVelocity.y > velocityBeforeReduction.y)) decay *= 5;

        _decayingTransientVelocity = Vector2.MoveTowards(_decayingTransientVelocity, Vector2.zero, decay * Time.fixedDeltaTime);

        _immediateMove = Vector2.zero;
    }

    private void CleanFrameData()
    {
        _jumpToConsume = false;
        _forceToApplyThisFrame = Vector2.zero;
        _lastFrameY = Velocity.y;
    }

    #endregion

    #region Collisions

    private const float SKIN_WIDTH = 0.02f;
    private const int RAY_SIDE_COUNT = 5;
    private RaycastHit2D _groundHit;
    private float _currentStepDownLength;

    private float GrounderLength => _character.StepHeight + SKIN_WIDTH;

    private Vector2 RayPoint => _framePosition + Vector2.up * (_character.StepHeight + SKIN_WIDTH);

    private void CalculateCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Is the middle ray good?
        var isGroundedThisFrame = PerformRay(RayPoint);

        // If not, zigzag rays from the center outward until we find a hit
        if (!isGroundedThisFrame)
        {
            foreach (var offset in GenerateRayOffsets())
            {
                isGroundedThisFrame = PerformRay(RayPoint + Vector2.right * offset) || PerformRay(RayPoint - Vector2.right * offset);
                if (isGroundedThisFrame) break;
            }
        }

        if (isGroundedThisFrame && !IsGrounded)
        {
            ToggleGrounded(true);
            Landed?.Invoke();
        }
        else if (!isGroundedThisFrame && IsGrounded) ToggleGrounded(false);

        Physics2D.queriesStartInColliders = _cachedQueryMode;

        bool PerformRay(Vector2 point)
        {
            _groundHit = Physics2D.Raycast(point, -Vector2.up, GrounderLength + _currentStepDownLength, Stats.CollisionLayers);
            if (!_groundHit) return false;

            if (Vector2.Angle(_groundHit.normal, Vector2.up) > Stats.MaxWalkableSlope)
            {
                return false;
            }

            return true;
        }
    }

    private IEnumerable<float> GenerateRayOffsets()
    {
        var extent = _character.StandingColliderSize.x / 2 - _character.RayInset;
        var offsetAmount = extent / RAY_SIDE_COUNT;
        for (var i = 1; i < RAY_SIDE_COUNT + 1; i++)
        {
            yield return offsetAmount * i;
        }
    }

    private void ToggleGrounded(bool grounded)
    {
        IsGrounded = grounded;
        if (grounded)
        {
            GroundedChanged?.Invoke(true, _lastFrameY);
            _rigidbody.gravityScale = 0;
            SetVelocity(_trimmedFrameVelocity);
            _constantForce.force = Vector2.zero;
            _currentStepDownLength = _character.StepHeight;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            ResetAirJumps();
            SetColliderMode(ColliderMode.Standard);
        }
        else
        {
            GroundedChanged?.Invoke(false, 0);
            _timeLeftGrounded = Time.time;
            _rigidbody.gravityScale = GRAVITY_SCALE;
            SetColliderMode(ColliderMode.Airborne);
        }
    }

    private void SetColliderMode(ColliderMode mode)
    {
        //_airborneCollider.enabled = mode == ColliderMode.Airborne;

        switch (mode)
        {
            case ColliderMode.Standard:
                _collider.size = _character.StandingColliderSize;
                _collider.offset = _character.StandingColliderCenter;
                break;
            case ColliderMode.Airborne:
                break;
        }
    }

    private enum ColliderMode
    {
        Standard,
        Airborne
    }

    #endregion

    #region Direction

    private Vector2 _frameDirection;

    private void CalculateDirection()
    {
        _frameDirection = new Vector2(_frameInput.Move.x, 0);

        if (IsGrounded)
        {
            GroundNormal = _groundHit.normal;
            var angle = Vector2.Angle(GroundNormal, Vector2.up);
            if (angle < Stats.MaxWalkableSlope) _frameDirection.y = _frameDirection.x * -GroundNormal.x / GroundNormal.y;
        }

        _frameDirection = _frameDirection.normalized;
    }

    #endregion

    #region Walls

    private const float WALL_REATTACH_COOLDOWN = 0.2f;

    private bool _isOnWall;
    private float _canGrabWallAfter;
    private int _wallDirThisFrame;

    private bool HorizontalInputPressed => Mathf.Abs(_frameInput.Move.x) > Stats.HorizontalDeadZoneThreshold;
    private bool IsPushingAgainstWall => HorizontalInputPressed && (int)Mathf.Sign(_frameDirection.x) == _wallDirThisFrame;

    private void CalculateWalls()
    {
        if (!Stats.AllowWalls) return;

        var rayDir = _isOnWall ? WallDirection : _frameDirection.x;
        var hasHitWall = DetectWallCast(rayDir);

        _wallDirThisFrame = hasHitWall ? (int)rayDir : 0;

        if (!_isOnWall && ShouldStickToWall() && Time.time > _canGrabWallAfter && Velocity.y < 0) ToggleOnWall(true);
        else if (_isOnWall && !ShouldStickToWall()) ToggleOnWall(false);

        // If we're not grabbing a wall, let's check if we're against one for wall-jumping purposes
        if (!_isOnWall)
        {
            if (DetectWallCast(-1)) _wallDirThisFrame = -1;
            else if (DetectWallCast(1)) _wallDirThisFrame = 1;
        }

        bool ShouldStickToWall()
        {
            if (_wallDirThisFrame == 0 || IsGrounded) return false;

            if (HorizontalInputPressed && !IsPushingAgainstWall) return false; // If pushing away
            return !Stats.RequireInputPush || (IsPushingAgainstWall);
        }
    }

    private bool DetectWallCast(float dir)
    {
        return Physics2D.BoxCast(_framePosition + (Vector2)_wallDetectionBounds.center, new Vector2(_character.StandingColliderSize.x - SKIN_WIDTH, _wallDetectionBounds.size.y), 0, new Vector2(dir, 0), Stats.WallDetectorRange,
            Stats.ClimbableLayer);
    }

    private void ToggleOnWall(bool on)
    {
        _isOnWall = on;

        if (on)
        {
            _decayingTransientVelocity = Vector2.zero;
            _bufferedJumpUsable = true;
            WallDirection = _wallDirThisFrame;
        }
        else
        {
            _canGrabWallAfter = Time.time + WALL_REATTACH_COOLDOWN;
            _rigidbody.gravityScale = GRAVITY_SCALE;
            WallDirection = 0;
            if (Velocity.y > 0)
            {
                AddFrameForce(new Vector2(0, Stats.WallPopForce), true);
            }

            ResetAirJumps(); // so that we can air jump even if we didn't leave via a wall jump
        }

        WallGrabChanged?.Invoke(on);
    }

    #endregion

    #region Jump

    private const float JUMP_CLEARANCE_TIME = 0.25f;
    private bool IsWithinJumpClearance => _lastJumpExecutedTime + JUMP_CLEARANCE_TIME > Time.time;
    private float _lastJumpExecutedTime;
    private bool _bufferedJumpUsable;
    private bool _jumpToConsume;
    private float _timeJumpWasPressed;
    private Vector2 _forceToApplyThisFrame;
    private bool _endedJumpEarly;
    private int _airJumpsRemaining;
    private bool _coyoteUsable;
    private float _timeLeftGrounded;

    private bool HasBufferedJump => _bufferedJumpUsable && Time.time < _timeJumpWasPressed + Stats.BufferedJumpTime && !IsWithinJumpClearance;
    private bool CanUseCoyote => _coyoteUsable && !IsGrounded && Time.time < _timeLeftGrounded + Stats.CoyoteTime;
    private bool CanAirJump => !IsGrounded && _airJumpsRemaining > 0;

    private void CalculateJump()
    {
        if ((_jumpToConsume || HasBufferedJump))
        {
            if (IsGrounded) ExecuteJump(JumpType.Jump);
            else if (CanUseCoyote) ExecuteJump(JumpType.Coyote);
            else if (CanAirJump) ExecuteJump(JumpType.AirJump);
        }

        if ((!_endedJumpEarly && !IsGrounded && !_frameInput.JumpHeld && Velocity.y > 0) || Velocity.y < 0) _endedJumpEarly = true; // Early end detection
    }

    private void ExecuteJump(JumpType jumpType)
    {
        SetVelocity(_trimmedFrameVelocity);
        _endedJumpEarly = false;
        _bufferedJumpUsable = false;
        _lastJumpExecutedTime = Time.time;
        _currentStepDownLength = 0;

        if (jumpType is JumpType.Jump or JumpType.Coyote)
        {
            _coyoteUsable = false;
            AddFrameForce(new Vector2(0, Stats.JumpPower));
        }
        else if (jumpType is JumpType.AirJump)
        {
            _airJumpsRemaining--;
            AddFrameForce(new Vector2(0, Stats.JumpPower));
        }
            
        Jumped?.Invoke(jumpType);
    }

    private void ResetAirJumps() => _airJumpsRemaining = Stats.MaxAirJumps;

    #endregion

    #region Move

    private Vector2 _frameTransientVelocity;
    private Vector2 _immediateMove;
    private Vector2 _decayingTransientVelocity;
    private Vector2 _totalTransientVelocityAppliedLastFrame;
    private Vector2 _currentFrameSpeedModifier = Vector2.one;
    private const float SLOPE_ANGLE_FOR_EXACT_MOVEMENT = 0.7f;
    private float _lastFrameY;

    private void TraceGround()
    {
        if (IsGrounded && !IsWithinJumpClearance)
        {
            // Use transient velocity to keep grounded. Move position is not interpolated
            var distanceFromGround = _character.StepHeight - _groundHit.distance;
            if (distanceFromGround != 0)
            {
                var requiredMove = Vector2.zero;
                requiredMove.y += distanceFromGround;

                if (Stats.PositionCorrectionMode is PositionCorrectionMode.Velocity) _frameTransientVelocity = requiredMove / Time.fixedDeltaTime;
                else _immediateMove = requiredMove;
            }
        }
    }

    private void Move()
    {
        if (_forceToApplyThisFrame != Vector2.zero)
        {
            _rigidbody.velocity += AdditionalFrameVelocities();
            _rigidbody.AddForce(_forceToApplyThisFrame * _rigidbody.mass, ForceMode2D.Impulse);

            // Returning provides the crispest & most accurate jump experience
            // Required for reliable slope jumps
            return;
        }

        if (_isOnWall)
        {
            _constantForce.force = Vector2.zero;

            float wallVelocity;
            if (_frameInput.Move.y < 0) wallVelocity = _frameInput.Move.y * Stats.WallClimbSpeed;
            else wallVelocity = Mathf.MoveTowards(Mathf.Min(Velocity.y, 0), -Stats.WallClimbSpeed, Stats.WallFallAcceleration * Time.fixedDeltaTime);

            SetVelocity(new Vector2(_rigidbody.velocity.x, wallVelocity));
            return;
        }

        var extraForce = new Vector2(0, IsGrounded && !IsGliding ? 0 : -Stats.ExtraConstantGravity * (_endedJumpEarly && Velocity.y > 0 ? Stats.EndJumpEarlyExtraForceMultiplier : 1));
        _constantForce.force = extraForce * _rigidbody.mass;

        var targetSpeed = _hasInputThisFrame ? Stats.BaseSpeed : 0;

        var step = _hasInputThisFrame ? Stats.Acceleration : Stats.Friction;

        var xDir = (_hasInputThisFrame ? _frameDirection : Velocity.normalized);

        // Quicker direction change
        if (Vector3.Dot(_trimmedFrameVelocity, _frameDirection) < 0) step *= Stats.DirectionCorrectionMultiplier;

        Vector2 newVelocity;
        step *= Time.fixedDeltaTime;
        if (IsGrounded)
        {
            var speed = Mathf.MoveTowards(Velocity.magnitude, targetSpeed, step);

            // Blend the two approaches
            var targetVelocity = xDir * speed;

            // Calculate the new speed based on the current and target speeds
            var newSpeed = Mathf.MoveTowards(Velocity.magnitude, targetVelocity.magnitude, step);

            // TODO: Lets actually trace the ground direction automatically instead of direct
            var smoothed = Vector2.MoveTowards(Velocity, targetVelocity, step); // Smooth but potentially inaccurate
            var direct = targetVelocity.normalized * newSpeed; // Accurate but abrupt
            var slopePoint = Mathf.InverseLerp(0, SLOPE_ANGLE_FOR_EXACT_MOVEMENT, Mathf.Abs(_frameDirection.y)); // Blend factor

            // Calculate the blended velocity
            newVelocity = Vector2.Lerp(smoothed, direct, slopePoint);
        }
        else
        {
            step *= Stats.AirFrictionMultiplier;

            var targetX = Mathf.MoveTowards(_trimmedFrameVelocity.x, xDir.x * targetSpeed, step);
            newVelocity = new Vector2(targetX, _rigidbody.velocity.y);
        }

        SetVelocity((newVelocity + AdditionalFrameVelocities()) * _currentFrameSpeedModifier);

        Vector2 AdditionalFrameVelocities()
        {
            if (_immediateMove.sqrMagnitude > SKIN_WIDTH)
            {
                _rigidbody.MovePosition(_framePosition + _immediateMove);
            }

            _totalTransientVelocityAppliedLastFrame = _frameTransientVelocity + _decayingTransientVelocity;
            return _totalTransientVelocityAppliedLastFrame;
        }
    }

    private void SetVelocity(Vector2 newVel)
    {
        _rigidbody.velocity = newVel;
        Velocity = newVel;
    }

    #endregion

    #region Glide

    private void Glide()
    {
        if (IsGliding)
        {
            ExecuteGlide();
        }
    }

    private void ExecuteGlide()
    {
        var clampedVelocity = Mathf.Max(_rigidbody.velocity.y, -Stats.MaxVelocityOnFalling);
        Vector2 newVelocity = new(_rigidbody.velocity.x, clampedVelocity);
        SetVelocity(newVelocity);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        var pos = (Vector2)transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos + Vector2.up * _character.Height / 2, new Vector3(_character.Width, _character.Height));
        Gizmos.color = Color.magenta;

        var rayStart = pos + Vector2.up * _character.StepHeight;
        var rayDir = Vector3.down * _character.StepHeight;
        Gizmos.DrawRay(rayStart, rayDir);
        foreach (var offset in GenerateRayOffsets())
        {
            Gizmos.DrawRay(rayStart + Vector2.right * offset, rayDir);
            Gizmos.DrawRay(rayStart + Vector2.left * offset, rayDir);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos + (Vector2)_wallDetectionBounds.center, _wallDetectionBounds.size);


        Gizmos.color = Color.black;
        Gizmos.DrawRay(RayPoint, Vector3.right);
    }

    #endregion
}

public enum JumpType
{
    Jump,
    Coyote,
    AirJump,
}