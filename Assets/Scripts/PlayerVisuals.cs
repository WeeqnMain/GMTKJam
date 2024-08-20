using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private const string IsGliding = nameof(IsGliding);
    private const string IsFalling = nameof(IsFalling);
    private const string IsWalking = nameof(IsWalking);
    private const string Death = nameof(Death);

    [SerializeField] private PlayerController player;

    [Header("ParticleEffects")]
    [SerializeField] private ParticleSystem footStepParticleEffect;

    private Animator _animator;

    public event Action WingFlapped;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool isPlayerWalking = player.Velocity.x != 0f && player.IsGrounded;
        _animator.SetBool(IsWalking, isPlayerWalking);

        if (player.Velocity.x < 0f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (player.Velocity.x > 0f)
            transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        player.Jumped += OnJump;
        player.Landed += OnLand;
        player.GlidingBegun += OnGlidingBegun;
        player.GlidingEnded += OnGlidingEnded;
    }

    private void OnDisable()
    {
        player.Jumped -= OnJump;
        player.Landed -= OnLand;
        player.GlidingBegun -= OnGlidingBegun;
        player.GlidingEnded -= OnGlidingEnded;
    }

    private void OnGlidingEnded()
    {
        _animator.SetBool(IsGliding, false);

        if (player.IsGrounded == false)
            _animator.SetBool(IsFalling, true);
    }

    private void OnGlidingBegun()
    {
        _animator.SetBool(IsGliding, true);
    }

    private void OnJump(JumpType jumpType)
    {
        _animator.SetBool(IsFalling, true);
    }
    
    private void OnLand()
    {
        _animator.SetBool(IsFalling, false);
    }

    private Action callback;
    public void StartDeathAnimation(Action callback)
    {
        this.callback = callback;
        _animator.SetTrigger(Death);
    }

    public void AnimationEvent_DeathAnimationEnd()
    {
        callback?.Invoke();
        callback = null;
    }

    public void AnimationEvent_FootstepParticleEmmit()
    {
        if (footStepParticleEffect.isPlaying == false)
        {
            footStepParticleEffect.Play();
        }
    }

    public void AnimationEvent_WingFlapped()
    {
        WingFlapped?.Invoke();
    }
}
