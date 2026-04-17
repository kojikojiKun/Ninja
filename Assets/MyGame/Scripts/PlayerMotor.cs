using UnityEngine;
public class PlayerMotor
{
    private PlayableEntityStatus m_status;
    private CharacterController m_characterController;
    private Transform m_cameraPos;
    private PlayerMoveState m_currentState;
    private bool m_isCrouching;

    private Vector3 m_horizonal;
    private float m_targetSpeed;
    private const float GRAVITY = -9.81f;
    private float m_velocity_Y = 0f;
    private float m_currentSpeed;
    private Transform m_playerPos;

    public PlayerMotor(PlayableEntityStatus status, CharacterController controller,Transform playerPos)
    {
        m_status = status;
        m_characterController = controller;
        m_playerPos = playerPos;
    }

    public void SetCamera(Transform camera)
    {
        m_cameraPos = camera;
    }

    public Transform GetSnapPoint(AssassinateContext context)
    {
        foreach (var dict in context.DataMap)
        {
            if (dict.Value.Direction == context.Direction)
            {
                return dict.Value.SnapPoint;
            }
        }

        return null;
    }

    public void MoveToAssassinatePosition(Transform target,bool isSuccess)
    {
        if (target == null || !isSuccess)
            return;

        Debug.Log($"{m_playerPos}...{target}");
        m_playerPos.position = target.position;
    }

    public float SpeedRatio()
    {
        float ratio = m_currentSpeed / m_targetSpeed;
        if (ratio > 1)
            ratio = 1;
        return ratio;
    }

    public void Acceleration()
    {
        //最大速度に向けて加速.
        m_currentSpeed = Mathf.Lerp(
            m_currentSpeed,
            m_targetSpeed,
            (1 - Mathf.Exp(-m_status.SharpnessToTargetSpeed * Time.deltaTime))
            );

        if (m_targetSpeed - m_currentSpeed <= 0.1f)
            m_currentSpeed = m_targetSpeed;
    }

    public void Deceleraiton()
    {
        //一定時間でスピードを0にするための係数を計算.
        float diff = m_currentSpeed;
        float decel = diff / 0.5f;

        //0に向けて減速.
        m_currentSpeed = Mathf.Lerp(
            m_currentSpeed,
            0f,
            decel * Time.deltaTime
            );

        if (m_currentSpeed <= 0.5f)
            m_currentSpeed = 0;
    }

    public float GetCurrentSpeed()
    {
        return m_currentSpeed;
    }

    void Rotate(Vector3 dir)
    {
        //徐々に正面に向ける.
        Vector3 desierdForward = Vector3.RotateTowards(
            m_characterController.transform.forward,
            dir,
            m_status.TurnSpeed * Time.deltaTime,
            0f
            );

        if (desierdForward.sqrMagnitude > 0.01f)
            m_characterController.transform.rotation = Quaternion.LookRotation(desierdForward);
    }

    Vector3 FreeFall()
    {
        //接地中に地面に押し付ける.
        if (m_characterController.isGrounded && m_velocity_Y < 0)
            m_velocity_Y = -2f;

        m_velocity_Y += GRAVITY * Time.deltaTime;

        return Vector3.up * m_velocity_Y * Time.deltaTime;
    }

    Vector3 CalcDirection(Vector2 input)
    {
        //カメラを基準にプレイヤーを移動.
        Vector3 forward = m_cameraPos.forward;
        Vector3 right = m_cameraPos.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * input.y + right * input.x;
        return dir;
    }

    public void Move(Vector2 input)
    {
        Vector3 dir = CalcDirection(input);

        if (dir.sqrMagnitude > 1)
            dir.Normalize();

        if (dir.sqrMagnitude > 0.01f)
        {
            Rotate(dir);
            Acceleration();
        }
        else
        {
            Deceleraiton();
        }

        //移動ベクトル計算.
        m_horizonal = dir * m_currentSpeed * Time.deltaTime;

        m_characterController.Move(m_horizonal + FreeFall());
    }

    public void ToggleCrouch()
    {
        //しゃがみ切り替え.
        m_isCrouching = !m_isCrouching;
    }

    public void SetState(Vector2 input, bool IsRunPressing)
    {
        //走る入力をしゃがみ入力より優先する.
        if (IsRunPressing && m_isCrouching)
            m_isCrouching = false;

        //入力状況で移動状態を決定する.
        if (input.sqrMagnitude < 0.1f)
        {
            m_currentState = m_isCrouching
                ? PlayerMoveState.CrouchIdle
                : PlayerMoveState.Idle;
        }
        else if (IsRunPressing)
        {
            m_currentState = PlayerMoveState.Run;
        }
        else
        {
            m_currentState = m_isCrouching
                ? PlayerMoveState.CrouchWalk
                : PlayerMoveState.Walk;
        }

        SetTargetSpeed();
    }

    public PlayerMoveState GetState()
    {
        return m_currentState;
    }

    public void SetTargetSpeed()
    {
        //stateに応じた移動速度に設定.
        switch (m_currentState)
        {
            case PlayerMoveState.Walk:
                m_targetSpeed = m_status.WalkSpeed;
                break;
            case PlayerMoveState.Run:
                m_targetSpeed = m_status.RunSpeed;
                break;
            case PlayerMoveState.CrouchWalk:
                m_targetSpeed = m_status.CrouchWalkSpeed;
                break;
            default:
                break;
        }
    }

    public void Jump()
    {
        if (CanJamp())
            m_velocity_Y = m_status.JumpForce;
    }

    bool CanJamp()
    {
        return m_characterController.isGrounded;
    }
}
