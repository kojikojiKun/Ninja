using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private ViewTargetProfile m_viewProfile;

    private Camera m_camera;
    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;
    private SoundRangeController m_soundRangeController;
    private Animator m_animator;
    private PlayerAnimation m_playerAnimation;
    private ViewTarget m_viewTarget;
    private AssassinationRange m_assasinateRange;

    private PlayerMoveState m_currentMoveState;
    private bool m_isRunPressing;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_animator = GetComponent<Animator>();
        m_assasinateRange = GetComponentInChildren<AssassinationRange>();

        m_viewTarget = new ViewTarget(m_viewProfile);
        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController);
        m_viewTarget = new ViewTarget(m_viewProfile);
        m_playerAnimation = new PlayerAnimation(m_animator);

        
    }

    public void Initialize(Camera camera)
    {
        m_camera = camera;
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

        m_motor.Assassinate();
    }

    public void TakeDamage(int value)
    {
        m_core.TakeDamage(value);
    }

    private void Update()
    {
        if (m_core.IsDead() == true || m_camera == null)
            return;

        m_currentMoveState = m_motor.GetState();
        m_motor.SetState(m_input.GetMoveInput(), m_isRunPressing);
        m_soundRangeController.ApplyNoiseRange(m_currentMoveState);       
        m_motor.Move(m_input.GetMoveInput());
    }

    private void LateUpdate()
    {
        Vector2 input = m_input.GetMoveInput();
        float currensSpeed = m_motor.GetCurrentSpeed();

        if (input.magnitude > 0.1f)
        {
            m_playerAnimation.SetMultiplier(m_motor.SpeedRatio());
        }
        else
        {
            m_playerAnimation.SetMultiplier(1f);
        }


        m_playerAnimation.MoveAnimation(input,
            m_currentMoveState,
            currensSpeed
            );
    }
}
