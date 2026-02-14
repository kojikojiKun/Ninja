using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerMoveState
{
    Stop, //静止中.
    CrouchWalk, //しゃがみ歩き.
    Walk, //歩き.
    Run //走り.
}

[RequireComponent(typeof(CharacterController))]
public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private PlayerData m_data;
    [SerializeField] private Camera m_mainCamera;
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_deceleration;
    private float m_targetSpeed;
    private const float GRAVITY = -9.8f;
    private int m_hp;
    private float m_currentSpeed;
    private float m_walkSpeed;
    private float m_runSpeed;
    private float m_crouchWalkSpeed;
    private float m_jumpForce;
    private PlayerMoveState m_currentMoveState;
    private SneakSystem m_sneakSystem;

    private Vector2 m_moveInput;
    private Vector3 m_currentDir;
    private float m_verticalVelocity = 0f;
    private bool m_hasInput;
    private bool m_isCrouching = false;
    private bool m_isJumpPressed;
    private CharacterController m_characterCtrl;

    //移動入力を読み取り.
    public void OnMove(InputAction.CallbackContext context)
    {
        m_moveInput = context.ReadValue<Vector2>();
    }

    //走るボタンの入力を読み取り.
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            //しゃがんでいるならしゃがみ解除.
            if (m_isCrouching == true)
                m_isCrouching = false;

            m_targetSpeed = m_runSpeed;
        }
        else if (context.canceled)
        {
            m_targetSpeed = m_walkSpeed;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_isCrouching = !m_isCrouching;
        }

        if(m_isCrouching == true)
        {
            //しゃがんでいるときは移動速度変更する.
            m_targetSpeed = m_crouchWalkSpeed;
        }
        else
        {
            m_targetSpeed = m_walkSpeed;
        }
    }

    //ジャンプ入力を読み取り.
    public void OnJump(InputAction.CallbackContext context)
    {
        //ジャンプボタンが押された瞬間だけジャンプさせる.
        if (context.started)
        {
            m_isJumpPressed = true;
            JumpPlayer();
        }
        else if (context.canceled)
        {
            m_isJumpPressed = false;
        }
    }

    //キャラクターの正面を進行方向に向ける.
    void RotatePlayer()
    {
        Vector3 camForward = m_mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = m_mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 inputDir =
            camForward * m_moveInput.y +
            camRight * m_moveInput.x;

        //入力がないならreturn
        if (inputDir.sqrMagnitude < 0.01f)
            return;

        inputDir.Normalize();

        float radian = m_turnSpeed * Mathf.Deg2Rad * Time.deltaTime;

        m_currentDir = Vector3.RotateTowards(
            m_currentDir,
            inputDir,
            radian,
            0f
            );

        //回転方向を見る.
        transform.rotation = Quaternion.LookRotation(m_currentDir);
    }

    //移動速度の加減速を行う.
    void SpeedControl()
    {
        m_hasInput = m_moveInput.sqrMagnitude >= 0.01f;

        if (m_hasInput == true)
        {
            //加速.
            m_currentSpeed = Mathf.MoveTowards(
                m_currentSpeed,
                m_targetSpeed,
                m_acceleration * Time.deltaTime
                );
        }
        else
        {
            //減速.
            m_currentSpeed = Mathf.MoveTowards(
                m_currentSpeed,
                0f,
                m_deceleration * Time.deltaTime
                );
        }

        ChangeState();
    }

    //移動状況に応じて発生させる騒音の範囲を変更する.
    void ChangeState()
    {
        PlayerMoveState changedState = m_currentMoveState;

        //走っているとき.
        if (m_currentSpeed > m_walkSpeed && m_currentSpeed <= m_runSpeed)
        {
            m_currentMoveState = PlayerMoveState.Run;
        }
        //歩いているとき.
        else if (m_currentSpeed > m_crouchWalkSpeed && m_currentSpeed <= m_walkSpeed)
        {
            m_currentMoveState = PlayerMoveState.Walk;
        }
        //しゃがみ歩きしているとき.
        else if (m_currentSpeed <= m_crouchWalkSpeed && m_hasInput == true)
        {
            m_currentMoveState = PlayerMoveState.CrouchWalk;
        }
        //止まっているとき.
        else if (m_hasInput == false)
        {
            m_currentMoveState = PlayerMoveState.Stop;
        }

        //移動状況が変化したとき.
        if (m_currentMoveState != changedState)
        {
            //SneakSystemに移動状況を渡す.
            m_sneakSystem.GenerateNoise(m_currentMoveState);
        }

    }

    //移動入力に応じてキャラクターを動かす.
    void MovePlayer()
    {
        //地面にいるとき.
        if (m_characterCtrl.isGrounded == true && m_verticalVelocity < 0)
        {
            //地面に吸い付ける.
            m_verticalVelocity = -2f;
        }

        //重力をかける.
        m_verticalVelocity += GRAVITY * Time.deltaTime;

        RotatePlayer();
        SpeedControl();

        Vector3 moveDir = m_currentDir * m_currentSpeed;
        Vector3 move = moveDir + Vector3.up * m_verticalVelocity;

        //キャラクターを移動させる.
        m_characterCtrl.Move(move * Time.deltaTime);
    }

    //キャラクターをジャンプさせる.
    void JumpPlayer()
    {
        if (m_characterCtrl.isGrounded == true && m_isJumpPressed == true)
        {
            Debug.Log("jump");
            m_verticalVelocity = Mathf.Sqrt(m_jumpForce * -2f * GRAVITY);
        }
    }

    private void Awake()
    {
        //キャラクターのステータスを代入.
        m_hp = m_data.HP;
        m_walkSpeed = m_data.WalkSpeed;
        m_runSpeed = m_data.RunSpeed;
        m_crouchWalkSpeed = m_data.CrouchWalkSpeed;
        m_jumpForce = m_data.JumpForce;

        m_characterCtrl = GetComponent<CharacterController>();
        m_sneakSystem = GetComponent<SneakSystem>();
    }

    private void Start()
    {
        m_currentDir = this.gameObject.transform.forward;
        m_targetSpeed = m_walkSpeed;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        JumpPlayer();
    }
}
