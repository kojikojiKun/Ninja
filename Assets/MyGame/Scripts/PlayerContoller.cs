using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
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

    private void Awake()
    {
        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController = GetComponent<CharacterController>());
        m_input = GetComponent<ReadPlayerInput>();
        m_input.OnJumpPressed += HandleJump;
        m_input.OnCrouchPressed += HandleCrouch;
    }

    void HandleJump()
    {
        if (m_characterController.isGrounded == true)
            m_motor.Jump();
    }

    void HandleCrouch() { }

    private void Update()
    {
        if (m_core.IsDead == false)
        {
            m_motor.Move(m_input.MoveInput, m_camera.transform);

            if (m_input.IsRunPressed == true)
                m_motor.StartRun();
            else
                m_motor.StopRun();
        }
    }
}
