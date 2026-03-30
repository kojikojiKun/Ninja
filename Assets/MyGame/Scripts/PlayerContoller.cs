using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SoundRangeController))]
[RequireComponent(typeof(ReadPlayerInput))]
public class PlayerContoller : MonoBehaviour, IAmbientLightReader
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private float m_timeToHoldInput;
    [SerializeField] private float m_turnDuraiton;
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
    private bool m_isCrouching;
    private float m_targetSpeed;
    private float m_prevTargetSpeed;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_animator = GetComponent<Animator>();

        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController,m_timeToHoldInput);
        m_playerAnimation = new PlayerAnimation(m_animator);

        if (GameManager.s_Instance.MainCamera != null)
            m_camera = GameManager.s_Instance.MainCamera.transform;
    }

    private void OnEnable()
    {
        m_input.OnJumpPressed += HandleJump;
        m_input.OnCrouchPressed += HandleCrouch;
    }

    private void OnDisable()
    {
        m_input.OnJumpPressed -= HandleJump;
        m_input.OnCrouchPressed -= HandleCrouch;
    }

    void HandleJump()
    {
        if (m_characterController.isGrounded == true)
            m_motor.Jump();
    }

    void HandleCrouch()
    {
        m_isCrouching = !m_isCrouching;
    }

    void SetMoveSpeed()
    {
        bool run = m_input.IsRunPressed;

        //ˆÚ“®ó‘Ô‚É‰ž‚¶‚ÄˆÚ“®‘¬“x‚ð•ÏX.
        if (run)
        {
            m_isCrouching = false;
            m_targetSpeed = m_status.RunSpeed;
        }
        else if (m_isCrouching)
            m_targetSpeed = m_status.CrouchWalkSpeed;
        else
            m_targetSpeed = m_status.WalkSpeed;
    }

    void SetCurrentMoveState()
    {
        float speed = m_motor.CurrentSpeed;
        if (speed >= m_status.RunSpeed)
        {
            m_currentMoveState = PlayerMoveState.Run;
        }
        else if (speed > m_status.CrouchWalkSpeed && speed <= m_status.WalkSpeed)
        {
            m_currentMoveState = PlayerMoveState.Walk;
        }
        else if (m_isCrouching)
        {
            m_currentMoveState = PlayerMoveState.Crouch;
        }
        else if (!m_isCrouching && speed < m_status.CrouchWalkSpeed)
        {
            m_currentMoveState = PlayerMoveState.Stop;
        }

        m_soundRangeController.ApplyNoiseRange(m_currentMoveState);
    }

    void GiveSpeedToMotor()
    {
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

        SetMoveSpeed();
        SetCurrentMoveState();
        GiveSpeedToMotor();

        if (m_camera != null)
            m_motor.Move(m_input.MoveInput, m_camera.transform);
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

        m_playerAnimation.SetMoveParameters(m_input.MoveInput, 
            m_currentMoveState, 
            m_motor.CurrentSpeed
            );
    }
}
