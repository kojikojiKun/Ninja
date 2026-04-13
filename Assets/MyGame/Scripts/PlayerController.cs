using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    [SerializeField] private PlayableEntityData m_data;
    [SerializeField] private ViewProfile m_viewProfile;
    [SerializeField] private Transform m_viewOrigin;

    private CharacterController m_characterController;
    private PlayableEntityStatus m_status;
    private PlayerCore m_core;
    private ReadPlayerInput m_input;
    private PlayerMotor m_motor;
    private PlayerCombat m_combat;
    private SoundRangeController m_soundRangeController;
    private Animator m_animator;
    private PlayerAnimator m_playerAnimator;
    private AssassinationRange m_assasinateRange;
    private ViewTarget m_viewTarget;
    private CalcTargetToSelfDirection m_calcDirection;
    private bool m_isRunPressing;

    public Transform OriginTransform { get; private set; }

    private void Awake()
    {
        OriginTransform = this.transform;

        m_characterController = GetComponent<CharacterController>();
        m_soundRangeController = GetComponent<SoundRangeController>();
        m_input = GetComponent<ReadPlayerInput>();
        m_animator = GetComponent<Animator>();
        m_assasinateRange = GetComponentInChildren<AssassinationRange>();

        m_status = new PlayableEntityStatus(m_data);
        m_core = new PlayerCore(m_status);
        m_motor = new PlayerMotor(m_status, m_characterController,OriginTransform);
        m_combat = new PlayerCombat();
        m_playerAnimator = new PlayerAnimator(m_animator);
        m_viewTarget = new ViewTarget(m_viewProfile,m_viewOrigin);
        m_calcDirection = new CalcTargetToSelfDirection();
    }

    public void Initialize(Camera camera)
    {
        m_motor.SetCamera(camera.transform);
    }

    private void OnEnable()
    {
        m_input.OnJumpPressed += HandleJump;
        m_input.OnRunPressing += HandleRun;
        m_input.OnCrouchPressed += HandleCrouch;
        m_input.OnAttackPressed += HandleAttack;
        m_input.OnAssassinated += HandleAssassinate;
    }

    private void OnDisable()
    {
        m_input.OnJumpPressed -= HandleJump;
        m_input.OnRunPressing -= HandleRun;
        m_input.OnCrouchPressed -= HandleCrouch;
        m_input.OnAttackPressed -= HandleAttack;
        m_input.OnAssassinated -= HandleAssassinate;
    }

    void HandleRun(bool pressing)
    {
        m_isRunPressing = pressing;
    }

    void HandleCrouch()
    {
        m_motor.ToggleCrouch();
    }

    void HandleJump()
    {
        m_motor.Jump();
    }

    void HandleAttack()
    {
        
    }

    void HandleAssassinate()
    {
        //最も近い暗殺可能対象を取得.
        IAssassinateable closest = m_assasinateRange.GetClosest(this.transform);
        if (closest == null)
            return;

        AssassinateContext context = new AssassinateContext
        {
            Target = closest,
            CanSee = m_viewTarget.IsSeeTarget(closest.OriginTransform),
            Direction = m_calcDirection.GetTargetToSelfDirection(closest.OriginTransform, OriginTransform),
            DataMap = closest.DataMap
        };

        m_combat.TryAssassinate(context);
        bool isSuccess = m_combat.IsSuccess();
        Transform snapPoint = m_combat.GetSnapPoint(context);

        m_motor.MoveToTargetPosition(snapPoint,isSuccess);
        m_playerAnimator.Assassinate(context.Direction, isSuccess);
    }

    public void TakeDamage(int value)
    {
        //無敵時間やアニメーションは後で.
        m_core.TakeDamage(value);
    }

    private void Update()
    {
        if (m_core.IsDead() == true)
            return;

        //プレイヤーの移動状況を決定する.
        PlayerMoveState state = m_motor.GetState();
        Vector2 moveInput = m_input.GetMoveInput();
        bool run = m_isRunPressing;

        m_motor.SetState(moveInput, run);
        m_motor.Move(moveInput);
        m_soundRangeController.ApplyNoiseRange(state);
    }

    private void LateUpdate()
    {
        //アニメーションはここで指示.
    }
}
