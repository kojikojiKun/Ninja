using UnityEngine;
public class PlayerMotor : IControllable
{
    private Vector2 m_input;
    private float m_currentSpeed;
    private float m_targetSpeed;
    private float m_jumpForce;
    private const float GRAVITY = -9.81f;
    private float m_velocity_Y = 0f;
    private PlayableEntityStatus m_status;
    private CharacterController m_controller;

    public PlayerMotor(PlayableEntityStatus status, CharacterController controller)
    {
        m_status = status;
        m_controller = controller;
    }

    void Acceleration()
    {
        m_currentSpeed = Mathf.MoveTowards(
            m_currentSpeed,
            m_targetSpeed,
            m_status.Acceleration * Time.deltaTime
            );
    }

    void Deceleraiton()
    {
        m_currentSpeed = Mathf.MoveTowards(
            m_currentSpeed,
            m_targetSpeed,
            m_status.Deceleration * Time.deltaTime
            );
    }

    void Rotate(Vector3 dir)
    {
        Vector3 desierdForward = Vector3.RotateTowards(
            m_controller.transform.forward,
            dir,
            m_status.TurnSpeed * Time.deltaTime,
            0f
            );
    }

    void FreeFall()
    {
        m_velocity_Y += GRAVITY * Time.deltaTime;
        m_controller.Move(Vector3.up * m_velocity_Y * Time.deltaTime);

        if (m_controller.isGrounded && m_velocity_Y < 0)
        {
            m_velocity_Y = -2f;
        }
    }

    public void Move(Vector2 input, Transform cam)
    {
        FreeFall();

        m_input = input;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * m_input.y + right * m_input.x;

        if (dir.magnitude > 1)
        {
            dir.Normalize();
            Acceleration();
            Rotate(dir);
        }
        else
        {
            Deceleraiton();
        }
        m_controller.Move(dir);
    }

    public void StartRun() { }

    public void StopRun() { }

    public void Jump() { }
}
