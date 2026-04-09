using UnityEngine;
public class PlayerMotor : IControllable
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

    public PlayerMotor(PlayableEntityStatus status, CharacterController controller)
    {
        m_status = status;
        m_characterController = controller;
    }

    public void SetCamera(Transform camera)
    {
        m_cameraPos = camera;
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
        float diff = m_currentSpeed;

        float decel = diff / 0.5f;

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
        m_isCrouching = !m_isCrouching;
    }

    public void SetState(Vector2 input, bool IsRunPressing)
    {
        if (IsRunPressing && m_isCrouching)
            m_isCrouching = false;

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
        if (m_characterController.isGrounded == true)
            m_velocity_Y = m_status.JumpForce;
    }

    public void Attack()
    {

    }

    public void Assassinate()
    {

    }
}
