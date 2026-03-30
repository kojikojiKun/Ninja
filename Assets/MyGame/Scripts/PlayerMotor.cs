using UnityEngine;
public class PlayerMotor : IControllable
{
    private PlayableEntityStatus m_status;
    private CharacterController m_controller;

    private Vector2 m_input;
    private Vector2 m_lastInput;
    private float m_timeToHoldInput;
    private float m_timer;
    private float m_timer_1;
    private bool m_isStartTurn;

    public float CurrentSpeed { get; private set; }
    private float m_targetSpeed;
    private const float GRAVITY = -9.81f;
    private float m_velocity_Y = 0f;
    
    public PlayerMotor(PlayableEntityStatus status, CharacterController controller,float timeToHoldInput)
    {
        m_status = status;
        m_controller = controller;
        m_timeToHoldInput = timeToHoldInput;
    }

    public float SpeedRatio()
    {
        float ratio = CurrentSpeed / m_targetSpeed;
        if (ratio > 1)
            ratio = 1;
        return ratio;
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
        Vector2 currentInput = m_input;
        m_timer += Time.deltaTime;

        if (Vector2.Dot(currentInput, m_lastInput) < 0f)
        {
            m_isStartTurn = true;
        }

        if (m_isStartTurn)
        {
            m_timer_1 += Time.deltaTime;
            if (m_timer_1 >= 0.5f)
            {
                m_isStartTurn = false;
                m_timer_1 = 0;
            }
        }

        //徐々に正面に向ける.
        Vector3 desierdForward = Vector3.RotateTowards(
            m_controller.transform.forward,
            dir,
            m_status.TurnSpeed * Time.deltaTime,
            0f
            );

        if (desierdForward.sqrMagnitude > 0.01f)
            m_controller.transform.rotation = Quaternion.LookRotation(desierdForward);

        //入力を保持.
        if (m_timer >= m_timeToHoldInput)
        {
            m_lastInput = currentInput;
            m_timer = 0;
        }
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
        //カメラを基準にプレイヤーを移動.
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

        if (dir.sqrMagnitude > 0.01f)
            Rotate(dir);

        //加減速実行.
        bool hasInput = input.sqrMagnitude > 0;
        if (hasInput)
        {
            Acceleration();
        }
        else
        {
            Deceleraiton();
        }

        //移動ベクトル計算.
        Vector3 horizonal = dir * CurrentSpeed * Time.deltaTime;
        m_controller.Move(horizonal + FreeFall());
    }

    public void SetTargetSpeed(float speed) { m_targetSpeed = speed; }

    public void Jump() { m_velocity_Y = m_status.JumpForce; }
}
