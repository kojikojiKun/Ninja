using UnityEngine;
public class PlayerMotor : IControllable
{
    private Vector2 m_input;
    public float CurrentSpeed { get; private set; }
    private float m_targetSpeed;
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
        CurrentSpeed = Mathf.MoveTowards(
            CurrentSpeed,
            m_targetSpeed,
            m_status.Acceleration * Time.deltaTime
            );
    }

    void Deceleraiton()
    {
        CurrentSpeed = Mathf.MoveTowards(
            CurrentSpeed,
            0f,
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

        if( desierdForward.sqrMagnitude > 0.01f )
            m_controller.transform.rotation=Quaternion.LookRotation( desierdForward );
    }

    Vector3 FreeFall()
    {
        if (m_controller.isGrounded && m_velocity_Y < 0)
            m_velocity_Y = -2f;

        m_velocity_Y += GRAVITY * Time.deltaTime;

        return Vector3.up * m_velocity_Y * Time.deltaTime;
    }

    public void Move(Vector2 input, Transform cam)
    {
        m_input = input;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * m_input.y + right * m_input.x;

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        bool hasInput = input.sqrMagnitude > 0;
        if (hasInput == true)
        {
            Acceleration();
        }
        else
        {
            Deceleraiton();
        }

        Vector3 horizonal = dir * CurrentSpeed * Time.deltaTime;
        m_controller.Move(horizonal + FreeFall());

        if(dir.sqrMagnitude > 0.01f)
            Rotate(dir);
    }

    public void SetTargetSpeed(float speed) { m_targetSpeed = speed; }

    public void Jump() { m_velocity_Y=m_status.JumpForce; }
}
