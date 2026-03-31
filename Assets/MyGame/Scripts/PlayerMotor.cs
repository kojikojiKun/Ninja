using UnityEngine;
public class PlayerMotor : IControllable
{
    private PlayableEntityStatus m_status;
    private CharacterController m_controller;
    private Timer m_holdTimer;
    private Timer m_turnTimer;

    private Vector2 m_input;
    private Vector2 m_lastInput;
    private Vector3 m_lastHorizonal;
    private Vector3 m_horizonal;
    private float m_timeToHold;
    private float m_turnDuration;
    private float m_targetSpeed;
    private const float GRAVITY = -9.81f;
    private float m_velocity_Y = 0f;

    public bool IsStartTurn { get; private set; }
    public float CurrentSpeed { get; private set; }
    public PlayerMotor(PlayableEntityStatus status, CharacterController controller, float hold, float Duration)
    {
        m_status = status;
        m_controller = controller;
        m_timeToHold = hold;
        m_turnDuration = Duration;
        m_holdTimer = new Timer();
        m_turnTimer = new Timer();
    }

    public float SpeedRatio()
    {
        float ratio = CurrentSpeed / m_targetSpeed;
        if (ratio > 1)
            ratio = 1;
        return ratio;
    }

    //移動速度から状態を決定する.
    public PlayerMoveState CurrentState(bool isCrouching)
    {
        float speed = CurrentSpeed;
        if (speed > m_status.WalkSpeed)
        {
            return  PlayerMoveState.Run;
        }
        else if (speed > m_status.CrouchWalkSpeed && speed <= m_status.WalkSpeed)
        {
            return PlayerMoveState.Walk;
        }
        else if (isCrouching)
        {
            return PlayerMoveState.Crouch;
        }
        else if (!isCrouching && speed < m_status.CrouchWalkSpeed)
        {
            return PlayerMoveState.Stop;
        }

        return PlayerMoveState.Stop;
    }

    //変数を保持.
    public void Hold()
    {
        //スピードが目標の値まで達していて、入力ベクトルが保持しているベクトルに対して逆方向になったとき.
        if (CurrentSpeed == m_targetSpeed && Vector2.Dot(m_input, m_lastInput) < 0f)
        {
            //ターンの検知は入力値基準.
            IsStartTurn = true;
        }

        if (IsStartTurn)
        {
            if (m_turnTimer.IsOutOfDuration(m_turnDuration))
            {
                IsStartTurn = false;
                m_turnTimer.Reset();
            }
        }

        //timeToHold秒前の入力ベクトル,移動方向を保持.
        if (m_holdTimer.IsOutOfDuration(m_timeToHold))
        {
            m_lastInput = m_input;
            m_lastHorizonal = m_horizonal;
            m_holdTimer.Reset();
        }
    }

    public void Acceleration()
    {
        CurrentSpeed = Mathf.Lerp(
            CurrentSpeed,
            m_targetSpeed,
            (1 - Mathf.Exp(-m_status.SharpnessToTargetSpeed * Time.deltaTime))
            );

        if (m_targetSpeed - CurrentSpeed <= 0.01f)
            CurrentSpeed = m_targetSpeed;
    }

    public void Deceleraiton()
    {
        float diff = CurrentSpeed;

        float decel = diff / 0.5f;

        CurrentSpeed = Mathf.Lerp(
            CurrentSpeed,
            0f,
            decel * Time.deltaTime
            );

        if (m_targetSpeed - CurrentSpeed <= 0.01f)
            CurrentSpeed = m_targetSpeed;
    }

    void Rotate(Vector3 dir)
    {
        //徐々に正面に向ける.
        Vector3 desierdForward = Vector3.RotateTowards(
            m_controller.transform.forward,
            dir,
            m_status.TurnSpeed * Time.deltaTime,
            0f
            );

        if (desierdForward.sqrMagnitude > 0.01f)
            m_controller.transform.rotation = Quaternion.LookRotation(desierdForward);
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

        //移動ベクトル計算.
        m_horizonal = dir * CurrentSpeed * Time.deltaTime;

        if (IsStartTurn)
            m_horizonal = m_lastHorizonal;

        m_controller.Move(m_horizonal + FreeFall());
    }

    public void SetTargetSpeed(float speed) { m_targetSpeed = speed; }

    public void Jump() { m_velocity_Y = m_status.JumpForce; }
}
