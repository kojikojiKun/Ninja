using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SoundRangeController))]
[RequireComponent(typeof(ReadPlayerInput))]
public class PlayerContoller : MonoBehaviour, IAmbientLightReader
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private float m_timeToHold;
    [SerializeField] private float m_turnDuration;
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
        m_motor = new PlayerMotor(m_status, m_characterController, m_timeToHold, m_turnDuration);
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

    void GiveSpeedToMotor()
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

        m_currentMoveState = m_motor.CurrentState(m_isCrouching);
        m_soundRangeController.ApplyNoiseRange(m_currentMoveState);
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


        m_playerAnimation.SetMoveParameters(m_input.MoveInput,
            m_currentMoveState,
            m_motor.CurrentSpeed
            );

        m_playerAnimation.Turn(m_motor.IsStartTurn);
    }
}
