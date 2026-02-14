using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private Camera m_camera;
    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;

    private void Awake()
    {
        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController=GetComponent<CharacterController>());
        m_input = GetComponent<ReadPlayerInput>();
    }

    private void Update()
    {
        m_motor.Move(m_input.MoveInput, m_camera.transform);
    }
}
