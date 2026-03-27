using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_sliderPivot;
    [SerializeField] private GameObject m_sliders;
    [SerializeField] private Slider m_sliderLeft;
    [SerializeField] private Slider m_sliderRight;
    [SerializeField, Range(0, 100f)] private float m_percentageOfEnableSlider_Max;
    [SerializeField, Range(0, 100f)] private float m_percentageOfEnableSlider_Min;
    private SearchGauge m_gauge;
    private Transform m_camera;

    private void Awake()
    {
        m_gauge = new SearchGauge(m_sliderPivot);

        //パーセントを少数になおす。
        if (m_percentageOfEnableSlider_Max > 0f)
            m_percentageOfEnableSlider_Max /= 100f;
        if (m_percentageOfEnableSlider_Min > 0f)
            m_percentageOfEnableSlider_Min /= 100;

        if (GameManager.s_Instance.MainCamera != null)
            m_camera = GameManager.s_Instance.MainCamera.transform;
    }

    void ControlSearchGaugeFillValue()
    {
        //スライダーの増え方が違うため、左右別で計算.
        m_sliderLeft.value = m_gauge.CalcLeftFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
        m_sliderRight.value = m_gauge.CalcRightFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
    }

    private bool IsReferenceNull()
    {
        if (m_camera == null)
            m_camera = GameManager.s_Instance.MainCamera.transform;

        if (GameManager.s_Instance.Player != null)
            m_gauge.GetPlayer(GameManager.s_Instance.Player.transform);

        return GameManager.s_Instance.HighestScoreEnemy == null || GameManager.s_Instance.Player == null;
    }

    private void Update()
    {
        if (IsReferenceNull())
            return;

        ControlSearchGaugeFillValue();
        m_gauge.TurnToEnemy(
            GameManager.s_Instance.HighestScoreEnemy.transform,
            GameManager.s_Instance.MainCamera
            );

        //スライダーのValueが閾値におさまっている間はスライダーを表示する.
        float leftPercentage = m_sliderLeft.value / m_sliderLeft.maxValue;
        float rightPercentage = m_sliderRight.value / m_sliderRight.maxValue;
        if (
            leftPercentage <= m_percentageOfEnableSlider_Min || leftPercentage >= m_percentageOfEnableSlider_Max ||
            rightPercentage <= m_percentageOfEnableSlider_Min || rightPercentage >= m_percentageOfEnableSlider_Max
            )
            m_sliders.SetActive(false);
        else
            m_sliders.SetActive(true);
    }
}
