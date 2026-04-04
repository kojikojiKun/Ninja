using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SoundRangeController))]
[RequireComponent(typeof(ReadPlayerInput))]
public class PlayerContoller : MonoBehaviour, IAmbientLightReader
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private float m_timeToHold;
    [SerializeField] private float m_turnDuration;
    [SerializeField] private float m_inputReceptionTime;
    [SerializeField] private float m_inputIgnoreTime;

    private Transform m_camera;
    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;
    private SoundRangeController m_soundRangeController;
    private Animator m_animator;
    private PlayerAnimation m_playerAnimation;

    private PlayerMoveState m_currentMoveState;
    private float m_targetSpeed;
    private float m_prevTargetSpeed;
    private bool m_isStartCrouch;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_animator = GetComponent<Animator>();

        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController);
        m_motor.SetFloat(m_timeToHold, m_turnDuration);
        m_playerAnimation = new PlayerAnimation(m_animator);

        if (GameManager.s_Instance.MainCamera != null)
            m_camera = GameManager.s_Instance.MainCamera.transform;
    }

    private void OnEnable()
    {
        m_input.OnJumpPressed += HandleJump;
        m_input.OnCrouchPressed += HandleCrouch;
        m_input.OnAttackPressed += HandleAttack;
    }

    private void OnDisable()
    {
        m_input.OnJumpPressed -= HandleJump;
        m_input.OnCrouchPressed -= HandleCrouch;
        m_input.OnAttackPressed -= HandleAttack;
    }

    void HandleJump()
    {
        if (m_characterController.isGrounded == true)
            m_motor.Jump();
    }

    void HandleCrouch()
    {
        m_isStartCrouch = !m_isStartCrouch;
    }

    void HandleAttack()
    {
        m_motor.Attack();
    }

    void GiveSpeedToMotor()
    {
        bool isRunPressed = m_input.IsRunPressed;
        if (isRunPressed)
        {
            m_isStartCrouch = false;
            m_targetSpeed = m_status.RunSpeed;
        }
        else if (m_isStartCrouch)
        {
            m_targetSpeed = m_status.CrouchWalkSpeed;
        }
        else if (!isRunPressed && !m_isStartCrouch)
        {
            m_targetSpeed = m_status.WalkSpeed;
        }

        if (m_targetSpeed != m_prevTargetSpeed)
        {
            m_motor.SetTargetSpeed(m_targetSpeed);
            m_prevTargetSpeed = m_targetSpeed;
        }
    }

    private void Update()
    {
        if (m_camera == null)
            m_camera = GameManager.s_Instance.MainCamera.transform;

        if (m_core.IsDead() == true)
            return;

        m_soundRangeController.ApplyNoiseRange(m_currentMoveState);
        m_currentMoveState = m_motor.CurrentState(m_input.IsRunPressed, m_isStartCrouch);
        GiveSpeedToMotor();

        m_motor.Hold();
        m_motor.Move(m_input.MoveInput, m_camera.transform);

        bool isMoving = m_input.MoveInput.sqrMagnitude > 0.01f;
        if (isMoving)
        {
            m_motor.Acceleration();
        }
        else if(!isMoving || m_motor.IsStartTurn)
        {
            m_motor.Deceleraiton();
        }
    }

    private void LateUpdate()
    {
        if (m_input.MoveInput.magnitude > 0.1f)
        {
            m_playerAnimation.SetMultiplier(m_motor.SpeedRatio());
        }
        else
        {
            m_playerAnimation.SetMultiplier(1f);
        }


        m_playerAnimation.MoveAnimation(m_input.MoveInput,
            m_currentMoveState,
            m_motor.CurrentSpeed
            );

        m_playerAnimation.StartTurn(m_motor.IsStartTurn);
    }
}
