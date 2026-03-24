using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_sliderPivot;
    [SerializeField] private GameObject m_sliders;
    [SerializeField] private Slider m_sliderLeft;
    [SerializeField] private Slider m_sliderRight;
    private SearchGauge m_gauge;
    private Transform m_camera;

    private void Awake()
    {
        m_gauge = new SearchGauge(m_sliderPivot);

        if (GameManager.s_Instance.MainCamera != null)
            m_camera = GameManager.s_Instance.MainCamera.transform;
    }

    void ControlFillValue()
    {
        //スライダーの増え方が違うため、左右別で計算.
        m_sliderLeft.value = m_gauge.CalcLeftFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
        m_sliderRight.value = m_gauge.CalcRightFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
    }

    private void Update()
    {
        if (m_camera == null)
            m_camera = GameManager.s_Instance.MainCamera.transform;

        if (GameManager.s_Instance.Player != null)
            m_gauge.GetPlayer(GameManager.s_Instance.Player.transform);

        if (GameManager.s_Instance.HighestScoreEnemy == null || GameManager.s_Instance.Player == null)
            return;

        ControlFillValue();
        m_gauge.TurnToEnemy(
            GameManager.s_Instance.HighestScoreEnemy.transform,
            GameManager.s_Instance.MainCamera
            );

        if (GameManager.s_Instance.HighestScoreEnemy.TotalScore <= 0)
            m_sliders.SetActive(false);
        else
            m_sliders.SetActive(true);
    }
}
