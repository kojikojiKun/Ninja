using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent (typeof(SoundRangeController))]
[RequireComponent(typeof(ReadPlayerInput))]
public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private Transform m_camera;
    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;
    private SoundRangeController m_soundRangeController;
    private bool m_isCrouching;
    private float m_targetSpeed;
    private float m_prevTargetSpeed;
    private MoveState m_currentMoveState;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController);
        m_input.OnJumpPressed += HandleJump;
        m_input.OnCrouchPressed += HandleCrouch;
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
        if (m_targetSpeed != m_prevTargetSpeed)
        {
            m_motor.SetTargetSpeed(m_targetSpeed);
            m_prevTargetSpeed = m_targetSpeed;
        }
    }

    void SetCurrentMoveState()
    {
        float speed = m_motor.CurrentSpeed;
        if (speed >= m_status.RunSpeed)
        {
            m_currentMoveState = MoveState.Run;
        }
        else if (speed > m_status.CrouchWalkSpeed && speed <= m_status.WalkSpeed)
        {
            m_currentMoveState = MoveState.Walk;
        }
        else if (speed > 0.2f && speed <= m_status.CrouchWalkSpeed)
        {
            m_currentMoveState = MoveState.Crouch;
        }
        else
        {
            m_currentMoveState = MoveState.Stop;
        }

        m_soundRangeController.ApplyNoiseRange(m_currentMoveState);
    }

    private void Update()
    {
        if (m_core.IsDead() == true)
            return;

        bool run = m_input.IsRunPressed;
        
        if (run)
        {
            m_isCrouching = false;
            m_targetSpeed = m_status.RunSpeed;
        }
        else if (m_isCrouching)
            m_targetSpeed = m_status.CrouchWalkSpeed;
        else
            m_targetSpeed = m_status.WalkSpeed;

        SetCurrentMoveState();
        GiveSpeedToMotor();
        m_motor.Move(m_input.MoveInput, m_camera.transform);
    }
}
