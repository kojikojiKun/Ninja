using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraDistance
{
    Close,
    Middle,
    Far
}

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    [SerializeField] private GameObject m_mainCamera;
    [SerializeField] private float m_distanceClose;
    [SerializeField] private float m_distanceMiddle;
    [SerializeField] private float m_distanceFar;
    [SerializeField] private float m_sensiticity;
    [SerializeField] private float m_minPitch;
    [SerializeField] private float m_maxPitch;

    public CameraDistance m_distanceType;
    private float m_distance;
    private float m_yaw;
    private float m_pitch;

    private Vector2 m_lookInput;

    //視点移動の入力を読み取り.
    public void OnLook(InputValue value)
    {
        m_lookInput = value.Get<Vector2>();
    }

    //カメラとプレイヤーとの距離を変更.
    public void ChangeDistance()
    {
        switch (m_distanceType)
        {
            case CameraDistance.Close:
                m_distance = m_distanceClose;
                break;
            case CameraDistance.Middle:
                m_distance = m_distanceMiddle;
                break;
            case CameraDistance.Far:
                m_distance = m_distanceMiddle;
                break;
        }
    }

    //プレイヤーに追従する.
    void FollowingPlayer()
    {
        if (m_target != null)
        {
            Quaternion rot = Quaternion.Euler(m_pitch, m_yaw, 0f);

            //カメラを一定距離離す.
            Vector3 camPos = m_target.transform.position - rot * Vector3.forward * m_distance;

            //カメラ追従.
            this.gameObject.transform.position = camPos;

            //カメラを回転.
            this.transform.rotation = rot;
        }
    }

    private void Start()
    {
        ChangeDistance();
        Vector3 angles = transform.eulerAngles;
        m_yaw = angles.y;
        m_pitch = angles.x;
    }

    private void Update()
    {
        float multiplier = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero
        ? 0.1f   // マウス
        : 1.0f;  // パッド

        m_yaw += m_lookInput.x * m_sensiticity * multiplier;
        m_pitch -= m_lookInput.y * m_sensiticity * multiplier;

        //上下の視点移動の範囲を制限.
        m_pitch = Mathf.Clamp(m_pitch, m_minPitch, m_maxPitch);
    }

    private void LateUpdate()
    {
        FollowingPlayer();
    }
}
