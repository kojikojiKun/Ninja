using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private ViewTargetProfile m_viewProfile;

    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;
    private PlayerCombat m_combat;
    private SoundRangeController m_soundRangeController;
    private Animator m_animator;
    private PlayerAnimation m_playerAnimation;
    private AssassinationRange m_assasinateRange;
    private ViewTarget m_viewTarget;
    private bool m_isRunPressing;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_animator = GetComponent<Animator>();
        m_assasinateRange = GetComponentInChildren<AssassinationRange>();

        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController);
        m_combat = new PlayerCombat();
        m_playerAnimation = new PlayerAnimation(m_animator);
        m_viewTarget = new ViewTarget(m_viewProfile);
    }

    public void Initialize(Camera camera)
    {
        m_motor.SetCamera(camera.transform);
        m_assasinateRange.SetPlayerPos(this.transform);
    }

    private void OnEnable()
    {
        m_input.OnJumpPressed += HandleJump;
        m_input.OnRunPressing += HandleRun;
        m_input.OnCrouchPressed += HandleCrouch;
        m_input.OnAttackPressed += HandleAttack;
        m_input.OnAssassinated += HandleAssassinate;
    }

    private void OnDisable()
    {
        m_input.OnJumpPressed -= HandleJump;
        m_input.OnRunPressing -= HandleRun;
        m_input.OnCrouchPressed -= HandleCrouch;
        m_input.OnAttackPressed -= HandleAttack;
        m_input.OnAssassinated -= HandleAssassinate;
    }

    void HandleRun(bool pressing)
    {
        m_isRunPressing = pressing;
    }

    void HandleCrouch()
    {
        m_motor.ToggleCrouch();
    }

    void HandleJump()
    {
        m_motor.Jump();
    }

    void HandleAttack()
    {
        
    }

    void HandleAssassinate()
    {
        IAssassinateable closest = m_assasinateRange.GetClosest();
        if (closest == null)
            return;

        m_viewTarget.SetTarget(closest.OriginTransform);
        m_combat.Assassinate(closest, m_viewTarget.IsSeeTarget());
    }

    public void TakeDamage(int value)
    {
        //無敵時間やアニメーションは後で.
        m_core.TakeDamage(value);
    }

    private void Update()
    {
        if (m_core.IsDead() == true)
            return;

        PlayerMoveState state = m_motor.GetState();
        Vector2 moveInput = m_input.GetMoveInput();
        bool run = m_isRunPressing;

        m_motor.SetState(moveInput, run);
        m_motor.Move(moveInput);
        m_soundRangeController.ApplyNoiseRange(state);
    }

    private void LateUpdate()
    {
        //アニメーションはここで指示.
    }
}
